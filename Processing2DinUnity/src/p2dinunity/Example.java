package p2dinunity;

public class Example extends P2U {

	public Example () {
		this.debugWidth = 512;
		this.debugHeight = 512;
	}
	
	@Override public void setup () {
		g.background(30, 170, 200, 0);
	    g.stroke(100, 200, 20,2);
	    g.strokeWeight(10);
//	    g.line(256, 0, 0, 256);
	}
	
	@Override public void draw() {
//		g.background(127);
		g.strokeWeight(2);
		g.fill(random(255),random(255),random(255), 11);
		g.ellipse(random(width), random(height), 20, 20);
	}
}
