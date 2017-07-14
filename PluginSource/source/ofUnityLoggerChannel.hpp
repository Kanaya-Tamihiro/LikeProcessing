#pragma once

#include <ofLog.h>

typedef void (*UnityDebugLog)(char*);

class ofUnityLoggerChannel : public ofBaseLoggerChannel{
public:
	void log(ofLogLevel level, const string & module, const string & message);
	
	void log(ofLogLevel level, const string & module, const char* format, ...);
	
	void log(ofLogLevel level, const string & module, const char* format, va_list args);
	
	void setDebugLogFunc(void* func);
	
private:
	UnityDebugLog unityDebugLogFunc;
};

