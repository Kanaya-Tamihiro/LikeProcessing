// Example low level rendering Unity plugin

#include "PlatformBase.h"
#include "RenderingPlugin.h"
#include "Processing.h"
#include "ProcessingTexture.h"
#include <assert.h>
#include <math.h>
#include <vector>
#include <jni.h>
//#include "ofMain.h"
#include "ofUnityWindow.hpp"
#include "ofUnityLoggerChannel.hpp"

JNIEnv *env;
JavaVM *jvm;
jclass cls;
jmethodID mid;
int *buf;

// --------------------------------------------------------------------------
// SetTimeFromUnity, an example function we export which is called by one of the scripts.

static float g_Time;

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetTimeFromUnity (float t) { g_Time = t; }



// --------------------------------------------------------------------------
// SetTextureFromUnity, an example function we export which is called by one of the scripts.

static void* g_TextureHandle = NULL;
static int   g_TextureWidth  = 0;
static int   g_TextureHeight = 0;

//static ofUnityWindow* g_ofWindow;
shared_ptr<ofUnityWindow> unityWindow = NULL;
shared_ptr<ofUnityLoggerChannel> unityLoggerChannel = NULL;

static RenderAPI* s_CurrentAPI = NULL;
static UnityGfxRenderer s_DeviceType = kUnityGfxRendererNull;


extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetTextureFromUnity(void* textureHandle, int w, int h)
{
	// A script calls this at initialization time; just remember the texture pointer here.
	// Will update texture pixels each frame from the plugin rendering event (texture update
	// needs to happen on the rendering thread).
	g_TextureHandle = textureHandle;
	g_TextureWidth = w;
	g_TextureHeight = h;
}


// --------------------------------------------------------------------------
// SetMeshBuffersFromUnity, an example function we export which is called by one of the scripts.

static void* g_VertexBufferHandle = NULL;
static int g_VertexBufferVertexCount;

struct MeshVertex
{
	float pos[3];
	float normal[3];
	float uv[2];
};
static std::vector<MeshVertex> g_VertexSource;


extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetMeshBuffersFromUnity(void* vertexBufferHandle, int vertexCount, float* sourceVertices, float* sourceNormals, float* sourceUV)
{
	// A script calls this at initialization time; just remember the pointer here.
	// Will update buffer data each frame from the plugin rendering event (buffer update
	// needs to happen on the rendering thread).
	g_VertexBufferHandle = vertexBufferHandle;
	g_VertexBufferVertexCount = vertexCount;

	// The script also passes original source mesh data. The reason is that the vertex buffer we'll be modifying
	// will be marked as "dynamic", and on many platforms this means we can only write into it, but not read its previous
	// contents. In this example we're not creating meshes from scratch, but are just altering original mesh data --
	// so remember it. The script just passes pointers to regular C# array contents.
	g_VertexSource.resize(vertexCount);
	for (int i = 0; i < vertexCount; ++i)
	{
		MeshVertex& v = g_VertexSource[i];
		v.pos[0] = sourceVertices[0];
		v.pos[1] = sourceVertices[1];
		v.pos[2] = sourceVertices[2];
		v.normal[0] = sourceNormals[0];
		v.normal[1] = sourceNormals[1];
		v.normal[2] = sourceNormals[2];
		v.uv[0] = sourceUV[0];
		v.uv[1] = sourceUV[1];
		sourceVertices += 3;
		sourceNormals += 3;
		sourceUV += 2;
	}
}

void initJavaVM() {
	JavaVMInitArgs vm_args;
	JavaVMOption options[4];
	printf("beginning execution...?n");
	
	/*
	 * /opt/blackdown-jdk-1.4.2/がJavaのルートディレクトリの場合
	 */
	options[0].optionString = (char *)  "-Djava.class.path=.:/Library/Java/JavaVirtualMachines/jdk1.8.0_111.jdk/Contents/Home/jre/lib/rt.jar:/Applications/Processing.app/Contents/Java/core/library/core.jar:/Users/tamichan/projects/java/called_from_c:/Users/tamichan/projects/Unity/LikeProcessing/Processing2DinUnity/bin";
	options[1].optionString = (char *)  "-Djava.compiler=NONE";
	options[2].optionString = (char *)  "-Djava.awt.headless=true";
	vm_args.version = JNI_VERSION_1_6;
	vm_args.options = options;
	vm_args.nOptions = 3;
	vm_args.ignoreUnrecognized = JNI_FALSE;
	
	JNI_CreateJavaVM(&jvm,(void **)&env,&vm_args);
	
	//    cls = env->FindClass("Test");
	//    if (cls == 0) {
	//        printf("cannot found Test?n");
	//        exit(1);
	//    }
	//
	//    mid = env->GetStaticMethodID(cls, "nakid2d", "()Ljava/nio/ByteBuffer;");
	//    if (mid == 0) {
	//        printf("Could not locate method texture with signature ()Ljava/nio/ByteBuffer;");
	//        exit(1);
	//    }
	//
	//    jobject buffer = (jobject)env->CallStaticObjectMethod(cls, mid, NULL);
	//    buf = (int *)env->GetDirectBufferAddress(buffer);
}

void initOpenFrameworks() {
//	ofSetLogLevel(OF_LOG_VERBOSE);
	unityLoggerChannel = shared_ptr<ofUnityLoggerChannel>(new ofUnityLoggerChannel());
	ofLog::setChannel(unityLoggerChannel);
	unityWindow = shared_ptr<ofUnityWindow>(new ofUnityWindow());
	ofInit();
	auto settings = unityWindow->getSettings();
	settings.width = 512;
	settings.height = 512;
	int major;
	int minor;
	s_CurrentAPI->GetGlVersion(&major, &minor);
	settings.setGLVersion(major, minor);
	settings.windowMode = OF_WINDOW;
	ofGetMainLoop()->addWindow(unityWindow);
	unityWindow->setup(settings);
	
	//	ofBackground(ofFloatColor::fromHsb(179/359.0, 77/100.0, 99/100.0));
	ofSetBackgroundAuto(false);
	ofSetLineWidth(2);
}


// --------------------------------------------------------------------------
// UnitySetInterfaces

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType);

static IUnityInterfaces* s_UnityInterfaces = NULL;
static IUnityGraphics* s_Graphics = NULL;

extern "C" void	UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	s_UnityInterfaces = unityInterfaces;
	s_Graphics = s_UnityInterfaces->Get<IUnityGraphics>();
	s_Graphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);
	
	// Run OnGraphicsDeviceEvent(initialize) manually on plugin load
	OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
    
	initJavaVM();
	initOpenFrameworks();
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
	s_Graphics->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
    jvm->DestroyJavaVM();
}

#if UNITY_WEBGL
typedef void	(UNITY_INTERFACE_API * PluginLoadFunc)(IUnityInterfaces* unityInterfaces);
typedef void	(UNITY_INTERFACE_API * PluginUnloadFunc)();

extern "C" void	UnityRegisterRenderingPlugin(PluginLoadFunc loadPlugin, PluginUnloadFunc unloadPlugin);

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API RegisterPlugin()
{
	UnityRegisterRenderingPlugin(UnityPluginLoad, UnityPluginUnload);
}
#endif

// --------------------------------------------------------------------------
// GraphicsDeviceEvent




RenderAPI* getCurrentAPI() {
    return s_CurrentAPI;
}

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType)
{
	// Create graphics API implementation upon initialization
	if (eventType == kUnityGfxDeviceEventInitialize)
	{
		assert(s_CurrentAPI == NULL);
		s_DeviceType = s_Graphics->GetRenderer();
		s_CurrentAPI = CreateRenderAPI(s_DeviceType);
	}

	// Let the implementation process the device related events
	if (s_CurrentAPI)
	{
		s_CurrentAPI->ProcessDeviceEvent(eventType, s_UnityInterfaces);
	}

	// Cleanup graphics API implementation upon shutdown
	if (eventType == kUnityGfxDeviceEventShutdown)
	{
		delete s_CurrentAPI;
		s_CurrentAPI = NULL;
		s_DeviceType = kUnityGfxRendererNull;
	}
}



// --------------------------------------------------------------------------
// OnRenderEvent
// This will be called for GL.IssuePluginEvent script calls; eventID will
// be the integer passed to IssuePluginEvent. In this example, we just ignore
// that value.


static void DrawColoredTriangle()
{
	// Draw a colored triangle. Note that colors will come out differently
	// in D3D and OpenGL, for example, since they expect color bytes
	// in different ordering.
	struct MyVertex
	{
		float x, y, z;
		unsigned int color;
	};
	MyVertex verts[3] =
	{
		{ -0.5f, -0.25f,  0, 0xFFff0000 },
		{ 0.5f, -0.25f,  0, 0xFF00ff00 },
		{ 0,     0.5f ,  0, 0xFF0000ff },
	};

	// Transformation matrix: rotate around Z axis based on time.
	float phi = g_Time; // time set externally from Unity script
	float cosPhi = cosf(phi);
	float sinPhi = sinf(phi);
	float depth = 0.7f;
	float finalDepth = s_CurrentAPI->GetUsesReverseZ() ? 1.0f - depth : depth;
	float worldMatrix[16] = {
		cosPhi,-sinPhi,0,0,
		sinPhi,cosPhi,0,0,
		0,0,1,0,
		0,0,finalDepth,1,
	};

	s_CurrentAPI->DrawSimpleTriangles(worldMatrix, 1, verts);
}


static void ModifyTexturePixels()
{
	void* textureHandle = g_TextureHandle;
	int width = g_TextureWidth;
	int height = g_TextureHeight;
	if (!textureHandle)
		return;

	int textureRowPitch;
	void* textureDataPtr = s_CurrentAPI->BeginModifyTexture(textureHandle, width, height, &textureRowPitch);
	if (!textureDataPtr)
		return;

	const float t = g_Time * 4.0f;

	unsigned char* dst = (unsigned char*)textureDataPtr;
	for (int y = 0; y < height; ++y)
	{
		unsigned char* ptr = dst;
		for (int x = 0; x < width; ++x)
		{
			// Simple "plasma effect": several combined sine waves
			int vv = int(
				(127.0f + (127.0f * sinf(x / 7.0f + t))) +
				(127.0f + (127.0f * sinf(y / 5.0f - t))) +
				(127.0f + (127.0f * sinf((x + y) / 6.0f - t))) +
				(127.0f + (127.0f * sinf(sqrtf(float(x*x + y*y)) / 4.0f - t)))
				) / 4;

			// Write the texture pixel
			ptr[0] = vv;
			ptr[1] = vv;
			ptr[2] = vv;
			ptr[3] = vv;

			// To next pixel (our pixels are 4 bpp)
			ptr += 4;
		}

		// To next image row
		dst += textureRowPitch;
	}

	s_CurrentAPI->EndModifyTexture(textureHandle, width, height, textureRowPitch, textureDataPtr);
}


static void ModifyVertexBuffer()
{
	void* bufferHandle = g_VertexBufferHandle;
	int vertexCount = g_VertexBufferVertexCount;
	if (!bufferHandle)
		return;

	size_t bufferSize;
	void* bufferDataPtr = s_CurrentAPI->BeginModifyVertexBuffer(bufferHandle, &bufferSize);
	if (!bufferDataPtr)
		return;
	int vertexStride = int(bufferSize / vertexCount);

	const float t = g_Time * 3.0f;

	char* bufferPtr = (char*)bufferDataPtr;
	// modify vertex Y position with several scrolling sine waves,
	// copy the rest of the source data unmodified
	for (int i = 0; i < vertexCount; ++i)
	{
		const MeshVertex& src = g_VertexSource[i];
		MeshVertex& dst = *(MeshVertex*)bufferPtr;
		dst.pos[0] = src.pos[0];
		dst.pos[1] = src.pos[1] + sinf(src.pos[0] * 1.1f + t) * 0.4f + sinf(src.pos[2] * 0.9f - t) * 0.3f;
		dst.pos[2] = src.pos[2];
		dst.normal[0] = src.normal[0];
		dst.normal[1] = src.normal[1];
		dst.normal[2] = src.normal[2];
		dst.uv[0] = src.uv[0];
		dst.uv[1] = src.uv[1];
		bufferPtr += vertexStride;
	}

	s_CurrentAPI->EndModifyVertexBuffer(bufferHandle);
}

ProcessingTexture* ptexture = nullptr;
std::vector<ProcessingTexture*>  processingTextures;

static void UNITY_INTERFACE_API OnRenderEvent(int eventID)
{
	// Unknown / unsupported graphics device type? Do nothing
	if (s_CurrentAPI == NULL)
		return;
//    ofDrawEllipse(0, 0, 100, 100);
//	DrawColoredTriangle();
	//ModifyTexturePixels();
//    ModifyTexturePixelsProcessing();
//	ModifyVertexBuffer();
	
	if (unityWindow != NULL) {
		unityWindow->draw();
	}
	
    for(auto it = processingTextures.begin(); it != processingTextures.end(); ++it) {
        (*it)->updateTexturePixels();
    }
    processingTextures.clear();
//    }
}


// --------------------------------------------------------------------------
// GetRenderEventFunc, an example function we export which is used to get a rendering event callback function.

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc()
{
	return OnRenderEvent;
}

extern "C" Processing* UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetProcessingObject()
{
    return new Processing();
}

extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API CallProcessingHoge(void* pObj)
{
    return ((Processing*) pObj)->hoge();
}

extern "C" ProcessingTexture* UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API CreateProcessingTexture(void* textureHandle, char* fullyQualifiedClassName, int width, int height)
{
    jclass cls = env->FindClass(fullyQualifiedClassName);
    if (cls == 0) {
        return nullptr;
    }
    return new ProcessingTexture(textureHandle, width, height, env, cls);
}



extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API ProcessingTextureUpdate (void* _processingTexture)
{
    ProcessingTexture* processingTexture = (ProcessingTexture *) _processingTexture;
    processingTexture->updateCurrentBufferId();
    processingTextures.push_back(processingTexture);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetScreenSizeToMyPlugin (int width, int height)
{
	unityWindow->setWindowShape(width, height);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UpdateCameraPosition (float posX, float posY, float posZ)
{
	unityWindow->updateCameraPosition(posX, posY, posZ);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UpdateCameraRotMat (float m00, float m01, float m02, float m03,
																				 float m10, float m11, float m12, float m13,
																				 float m20, float m21, float m22, float m23,
																				 float m30, float m31, float m32, float m33)
{
	unityWindow->updateCameraRotMat(m00, m01, m02, m03,
									  m10, m11, m22, m33,
									  m20, m21, m22, m33,
									  m30, m31, m22, m33);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UpdateCameraRotQuat (float qx, float qy, float qz, float qw)
{
	unityWindow->updateCameraRotQuat(qx, qy, qz, qw);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetDebugLogFunc (void* func)
{
	unityLoggerChannel->setDebugLogFunc(func);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API FreeDebugLogStrPtr (void* ptr)
{
	char* arr = (char*) ptr;
	free(arr);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API RegistOfRenderMethod (void* func)
{
	unityWindow->setRenderFunc(func);
}

//extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API RegistOfApp (void* ptr)
//{
//	unityWindow->registOfApp(ptr);
//}




