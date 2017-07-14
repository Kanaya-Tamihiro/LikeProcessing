#include "ofUnityLoggerChannel.hpp"

void ofUnityLoggerChannel::log(ofLogLevel level, const string &module, const string &message) {
	if (unityDebugLogFunc != NULL) {
		string str = "[" + ofGetLogLevelName(level, true)  + "] ";
		if(module != ""){
			str += module + ": ";
		}
		str += message;
		char* cstr = (char*)malloc(sizeof(char) * (str.length()+1));
		strcpy(cstr, str.c_str());
		unityDebugLogFunc(cstr);
	}
}

void ofUnityLoggerChannel::log(ofLogLevel level, const string &module, const char *format, ...) {
	if (unityDebugLogFunc != NULL) {
		va_list args;
		va_start(args, format);
		log(level, module, format, args);
		va_end(args);
	}
}

void ofUnityLoggerChannel::log(ofLogLevel level, const string &module, const char *format, va_list args) {
	if (unityDebugLogFunc != NULL) {
		auto temp = std::vector<char> {};
		string str = "[" + ofGetLogLevelName(level, true) + "]";
		if(module != ""){
			str += module + ": ";
		}
		vsnprintf(temp.data(), 1024, format, args);
		str += string(temp.begin(), temp.end()) + "\n";
		char* cstr = (char*)malloc(sizeof(char) * (str.length()+1));
		strcpy(cstr, str.c_str());
		unityDebugLogFunc(cstr);
	}

}

void ofUnityLoggerChannel::setDebugLogFunc(void* func) {
	unityDebugLogFunc = (UnityDebugLog) func;
}
