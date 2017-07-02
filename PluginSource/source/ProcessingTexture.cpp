#include "ProcessingTexture.h"
#include "RenderingPlugin.h"

ProcessingTexture::ProcessingTexture(void* _textureHandle, int _width, int _height, JNIEnv* _env, jclass _cls) {
    textureHandle = _textureHandle;
    width = _width;
    height = _height;
    env = _env;
    cls = _cls;
    currentBufferId = 0;
    
    jmethodID constructorMethodId = env->GetMethodID(cls, "<init>", "()V");
    jmethodID runMethodId = env->GetMethodID(cls, "run", "()V");
    jmethodID settingsMethodId = env->GetMethodID(cls, "settings", "(II)V");
    jmethodID getBuffer1PtrMethodId = env->GetMethodID(cls, "getBuffer1Ptr", "()Ljava/nio/ByteBuffer;");
    jmethodID getBuffer2PtrMethodId = env->GetMethodID(cls, "getBuffer2Ptr", "()Ljava/nio/ByteBuffer;");
    currentBufferMethodID = env->GetMethodID(cls, "currentBufferId", "()I");
    javaObj = (jobject) env->NewObject(cls, constructorMethodId);
    env->CallVoidMethod(javaObj, settingsMethodId, width, height);
    jobject jbuffer1 = (jobject) env->CallObjectMethod(javaObj, getBuffer1PtrMethodId);
    buffer1 = (int *) env->GetDirectBufferAddress(jbuffer1);
    jobject jbuffer2 = (jobject) env->CallObjectMethod(javaObj, getBuffer2PtrMethodId);
    buffer2 = (int *)env->GetDirectBufferAddress(jbuffer2);
    env->CallVoidMethod(javaObj, runMethodId);
}

int ProcessingTexture::updateCurrentBufferId () {
    currentBufferId = (int) env->CallIntMethod(javaObj, currentBufferMethodID);
    return currentBufferId;
}

void ProcessingTexture::updateTexturePixels()
{
    if (!textureHandle)
        return;
    int* buffer = currentBufferId == 0 ? buffer1 : buffer2;
    
    int textureRowPitch;
    void* textureDataPtr = getCurrentAPI()->BeginModifyTexture(textureHandle, width, height, &textureRowPitch);
    if (!textureDataPtr)
        return;
    
    unsigned char* dst = (unsigned char*)textureDataPtr;
    for (int y = 0; y < height; ++y)
    {
        unsigned char* ptr = dst;
        for (int x = 0; x < width; ++x)
        {
            // ARGB -> RGBA
            // A -> 8 * 3
            // R -> 8 * 2
            // G -> 8 * 1
            // B -> 8 * 0
            int argb = buffer[y*height+x];
            ptr[0] = (unsigned char) (argb >> 8 * 2);
            ptr[1] = (unsigned char) (argb >> 8 * 1);
            ptr[2] = (unsigned char) (argb >> 8 * 0);
            ptr[3] = (unsigned char) (argb >> 8 * 3);
//            ptr[0] = (unsigned char)255;
//            ptr[1] = (unsigned char)255;
//            ptr[2] = (unsigned char)26;
//            ptr[3] = (unsigned char)255;
            ptr += 4;
        }
        
        // To next image row
        dst += textureRowPitch;
    }
    
    getCurrentAPI()->EndModifyTexture(textureHandle, width, height, textureRowPitch, textureDataPtr);
}


