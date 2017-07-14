#include "Unity/IUnityGraphics.h"
#include "ofMain.h"

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API uofDrawBox (float size)
{
	ofDrawBox(size);
}


extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API uofDrawLine (float x1, float y1, float z1, float x2, float y2, float z2)
{
	ofDrawLine (x1, y1, z1, x2, y2, z2);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API uofFill ()
{
	ofFill();
}


extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API uofNoFill ()
{
	ofNoFill();
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API uofSetColor(int r, int g, int b, int a)
{
	ofSetColor(r, g, b, a);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API uofSetLineWidth (float lineWidth)
{
	ofSetLineWidth (lineWidth);
}
