#ifndef ProcessingTexture_h
#define ProcessingTexture_h

#include "Unity/IUnityGraphics.h"
#include <jni.h>
#include <stdio.h>

class ProcessingTexture
{
private:
    int width;
    int height;
    void* textureHandle;
    JNIEnv* env;
    jclass cls;
    jmethodID currentBufferMethodID;
    int currentBufferId;
    int* buffer1;
    int* buffer2;
    jobject javaObj;
public:
    ProcessingTexture(void* textureHandle, int width, int height, JNIEnv* env, jclass cls);
    int updateCurrentBufferId();
    void updateTexturePixels();
};

#endif /* ProcessingTexture_h */
