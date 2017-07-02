#include "ofUnityWindow.hpp"


ofUnityWindow::ofUnityWindow() {};

ofUnityWindow::~ofUnityWindow(){};

void ofUnityWindow::setupOpenGL(int w, int h, int screenMode) {}
void ofUnityWindow::initializeWindow() {}
void ofUnityWindow::runAppViaInfiniteLoop(ofBaseApp * appPtr) {}

void ofUnityWindow::hideCursor() {}
void ofUnityWindow::showCursor() {}

void ofUnityWindow::setWindowPosition(int x, int y) {}
void ofUnityWindow::setWindowShape(int w, int h) {
    width = w;
    height = h;
}

int ofUnityWindow::getFrameNum() { return 0; }
float ofUnityWindow::getFrameRate() {return 0; }
double ofUnityWindow::getLastFrameTime(){ return 0.0; }

ofPoint	ofUnityWindow::getWindowPosition() {return ofPoint(); }
ofPoint	ofUnityWindow::getWindowSize(){return ofPoint(width,height); }
ofPoint	ofUnityWindow::getScreenSize(){return ofPoint(width,height); }

void ofUnityWindow::setOrientation(ofOrientation orientation){ }
ofOrientation ofUnityWindow::getOrientation(){ return OF_ORIENTATION_DEFAULT; }
bool ofUnityWindow::doesHWOrientation(){return false;}

//this is used by ofGetWidth and now determines the window width based on orientation
int ofUnityWindow::getWidth(){ return width; }
int	ofUnityWindow::getHeight(){ return width; }

void ofUnityWindow::setFrameRate(float targetRate){}
void ofUnityWindow::setWindowTitle(string title){}

ofWindowMode ofUnityWindow::getWindowMode() {return OF_WINDOW;}

void ofUnityWindow::setFullscreen(bool fullscreen){}
void ofUnityWindow::toggleFullscreen(){}

void ofUnityWindow::enableSetupScreen(){}
void ofUnityWindow::disableSetupScreen(){}
