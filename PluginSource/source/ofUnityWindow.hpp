#pragma once

#include "ofMain.h"
#include "ofAppBaseWindow.h"

class ofBaseApp;
class ofUnityWindow : public ofAppBaseWindow {
private:
    float width;
    float height;
public:
    
    ofUnityWindow();
    ~ofUnityWindow();
    
    static void loop(){};
    static bool doesLoop(){ return false; }
    static bool allowsMultiWindow(){ return true; }
    static bool needsPolling(){ return true; }
    static void pollEvents(){}
    
    void setup(const ofWindowSettings & settings){};
    void update(){};
    void draw(){};
    
    void setupOpenGL(int w, int h, int screenMode);
    void initializeWindow();
    void runAppViaInfiniteLoop(ofBaseApp * appPtr);
    
    void hideCursor();
    void showCursor();
    
    void setWindowPosition(int x, int y);
    void setWindowShape(int w, int h);
    
    int	getFrameNum();
    float getFrameRate();
    double getLastFrameTime();
    
    ofPoint	getWindowPosition();
    ofPoint	getWindowSize();
    ofPoint	getScreenSize();
    
    void setOrientation(ofOrientation orientation);
    ofOrientation getOrientation();
    bool doesHWOrientation();
    
    int	getWidth();
    int	getHeight();
    
    void setFrameRate(float targetRate);
    void setWindowTitle(string title);
    
    ofWindowMode getWindowMode();
    
    void setFullscreen(bool fullscreen);
    void toggleFullscreen();
    
    void enableSetupScreen();
    void disableSetupScreen();
    

};

