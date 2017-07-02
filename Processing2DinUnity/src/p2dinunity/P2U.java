package p2dinunity;

import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.RenderingHints;
import java.awt.image.BufferedImage;
import java.awt.image.DataBufferInt;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.Random;

import javax.swing.JFrame;
import processing.awt.PGraphicsJava2D;

public abstract class P2U {

	PGraphicsJava2D g;
	boolean finished = false;
	ByteBuffer buffer1, buffer2;
	int currentByteBuffer = 0;

	int width, height;
	int debugWidth = 256, debugHeight = 256;

	P2U sketch;
	Thread thread;
	boolean paused;
	protected Object pauseObject = new Object();
	int frameCount = 0;
	float frameRate = 0;
	protected float frameRateTarget = 60;
	protected long frameRatePeriod = 1000000000L / 60L;
	protected long frameRateLastNanos = 0;
	
	TestFrame testFrame;

	public void settings(int width, int height) {
		this.width = width;
		this.height = height;
		g = new PGraphicsJava2D();
		g.setSize(width, height);
		g.image = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
		buffer1 = ByteBuffer.allocateDirect(width * height * 4);
		buffer1.order(ByteOrder.nativeOrder());
		buffer2 = ByteBuffer.allocateDirect(width * height * 4);
		buffer2.order(ByteOrder.nativeOrder());

		sketch = this;
	}

	void updateByteBuffer() {
		BufferedImage bi = (BufferedImage) g.image;
		int[] pixels = ((DataBufferInt) bi.getRaster().getDataBuffer()).getData();
		ByteBuffer buffer = currentByteBuffer == 0 ? buffer2 : buffer1;
		buffer.asIntBuffer().put(pixels);
		currentByteBuffer = (currentByteBuffer + 1) % 2;
	}

	public void handleDraw() {
		g.beginDraw();
		long now = System.nanoTime();

		if (frameCount == 0) {
			setup();
		} else { // frameCount > 0, meaning an actual draw()
			// update the current frameRate
			double rate = 1000000.0 / ((now - frameRateLastNanos) / 1000000.0);
			float instantaneousRate = (float) (rate / 1000.0);
			frameRate = (frameRate * 0.9f) + (instantaneousRate * 0.1f);

			draw();
		}
		g.endDraw();
		updateByteBuffer();
		if (testFrame != null) testFrame.repaint(); 
		frameRateLastNanos = now;
		frameCount++;
	}

	public void run() {
		// g.beginDraw();
		// setup();
		// g.endDraw();
		// updateByteBuffer();
		//
		// while (!finished) {
		// g.beginDraw();
		// setup();
		// g.endDraw();
		// }
		startThread();
	}
	
	public ByteBuffer getBuffer1Ptr() {
		return buffer1;
	}
	
	public ByteBuffer getBuffer2Ptr() {
		return buffer1;
	}
	
	public int currentBufferId() {
		return currentByteBuffer;
	}

	public void setup() {
	}

	public void draw() {
	}

	public static void main(String[] args) {
		if (args.length == 0) {
			throw new RuntimeException("no args");
		}
		P2U sketch;
		try {
			Class<?> c = Thread.currentThread().getContextClassLoader().loadClass(args[0]);
			sketch = (P2U) c.newInstance();
		} catch (RuntimeException re) {
			// Don't re-package runtime exceptions
			throw re;
		} catch (Exception e) {
			// Package non-runtime exceptions so we can throw them freely
			throw new RuntimeException(e);
		}
		sketch.settings(sketch.debugWidth, sketch.debugHeight);
		sketch.run();
		TestFrame frame = new TestFrame();
		sketch.testFrame = frame;
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.readImage = (BufferedImage) sketch.g.image;

		frame.setBounds(0, 0, sketch.debugWidth, sketch.debugHeight);
		frame.setVisible(true);
	}

	public static class TestFrame extends JFrame {

		BufferedImage readImage;

		@Override
		public void paint(Graphics g) {
			Graphics2D g2 = (Graphics2D) g;
			g2.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
			if (readImage != null) {
				g2.drawImage(readImage, 0, 0, this);
			}
		}
	}

	public Thread createThread() {
		return new AnimationThread();
	}

	public void startThread() {
		if (thread == null) {
			thread = createThread();
			thread.start();
		} else {
			throw new IllegalStateException("Thread already started in " + getClass().getSimpleName());
		}
	}

	public boolean stopThread() {
		if (thread == null) {
			return false;
		}
		thread = null;
		return true;
	}

	public boolean isStopped() {
		return thread == null;
	}

	// sets a flag to pause the thread when ready
	public void pauseThread() {
		// PApplet.debug("PApplet.run() paused, calling object wait...");
		paused = true;
	}

	// halts the animation thread if the pause flag is set
	protected void checkPause() {
		if (paused) {
			synchronized (pauseObject) {
				try {
					pauseObject.wait();
					// PApplet.debug("out of wait");
				} catch (InterruptedException e) {
					// waiting for this interrupt on a start() (resume) call
				}
			}
		}
		// PApplet.debug("done with pause");
	}

	public void resumeThread() {
		paused = false;
		synchronized (pauseObject) {
			pauseObject.notifyAll(); // wake up the animation thread
		}
	}

	public void setFrameRate(float fps) {
		frameRateTarget = fps;
		frameRatePeriod = (long) (1000000000.0 / frameRateTarget);
		// g.setFrameRate(fps);
	}

	public class AnimationThread extends Thread {

		public AnimationThread() {
			super("Animation Thread");
		}

		// broken out so it can be overridden by Danger et al
		public void callDraw() {
			sketch.handleDraw();
		}

		@Override
		public void run() { // not good to make this synchronized, locks things
							// up
			long beforeTime = System.nanoTime();
			long overSleepTime = 0L;

			int noDelays = 0;
			// Number of frames with a delay of 0 ms before the
			// animation thread yields to other running threads.
			final int NO_DELAYS_PER_YIELD = 15;

			// sketch.setup();

			while (!sketch.finished) {
				checkPause();

				callDraw();

				long afterTime = System.nanoTime();
				long timeDiff = afterTime - beforeTime;
				// System.out.println("time diff is " + timeDiff);
				long sleepTime = (frameRatePeriod - timeDiff) - overSleepTime;

				if (sleepTime > 0) { // some time left in this cycle
					try {
						Thread.sleep(sleepTime / 1000000L, (int) (sleepTime % 1000000L));
						noDelays = 0; // Got some sleep, not delaying anymore
					} catch (InterruptedException ex) {
					}

					overSleepTime = (System.nanoTime() - afterTime) - sleepTime;

				} else { // sleepTime <= 0; the frame took longer than the
							// period
					overSleepTime = 0L;
					noDelays++;

					if (noDelays > NO_DELAYS_PER_YIELD) {
						Thread.yield(); // give another thread a chance to run
						noDelays = 0;
					}
				}

				beforeTime = System.nanoTime();
			}
		}
	}

	Random internalRandom;

	public final float random(float high) {
		// avoid an infinite loop when 0 or NaN are passed in
		if (high == 0 || high != high) {
			return 0;
		}

		if (internalRandom == null) {
			internalRandom = new Random();
		}

		// for some reason (rounding error?) Math.random() * 3
		// can sometimes return '3' (once in ~30 million tries)
		// so a check was added to avoid the inclusion of 'howbig'
		float value = 0;
		do {
			value = internalRandom.nextFloat() * high;
		} while (value == high);
		return value;
	}

}
