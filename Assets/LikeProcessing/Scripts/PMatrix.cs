using System;
using UnityEngine;

namespace LikeProcessing
{
	public class PMatrix
	{
		public Matrix4x4 m;

		public PMatrix() {
			m = Matrix4x4.identity;
		}

		public static PMatrix identity {
			get { 
				return new PMatrix ();
			}	
		}

		public void translate(Vector3 v) {
			m[0,3] += v.x*m[0,0] + v.y*m[0,1] + v.z*m[0,2];
			m[1,3] += v.x*m[1,0] + v.y*m[1,1] + v.z*m[1,2];
			m[2,3] += v.x*m[2,0] + v.y*m[2,1] + v.z*m[2,2];
			m[3,3] += v.x*m[3,0] + v.y*m[3,1] + v.z*m[3,2];
		}

		public void rotate(float angle) {
			rotateZ(angle);
		}


		public void rotateX(float angle) {
			float c = Mathf.Cos(angle);
			float s = Mathf.Sin(angle);
			apply(1, 0, 0, 0,  0, c, -s, 0,  0, s, c, 0,  0, 0, 0, 1);
		}


		public void rotateY(float angle) {
			float c = Mathf.Cos(angle);
			float s = Mathf.Sin(angle);
			apply(c, 0, s, 0,  0, 1, 0, 0,  -s, 0, c, 0,  0, 0, 0, 1);
		}


		public void rotateZ(float angle) {
			float c = Mathf.Cos(angle);
			float s = Mathf.Sin(angle);
			apply(c, -s, 0, 0,  s, c, 0, 0,  0, 0, 1, 0,  0, 0, 0, 1);
		}

		public void rotate(float angle, Vector3 v) {
			float norm2 = v.x * v.x + v.y * v.y + v.z * v.z;
			if (norm2 < PConstants.EPSILON) {
				// The vector is zero, cannot apply rotation.
				return;
			}

			if (Mathf.Abs(norm2 - 1) > PConstants.EPSILON) {
				// The rotation vector is not normalized.
				v.Normalize();
			}

			float c = Mathf.Cos(angle);
			float s = Mathf.Sin(angle);
			float t = 1.0f - c;

			float v0 = v.x, v1 = v.y, v2 = v.z;

			apply((t*v0*v0) + c, (t*v0*v1) - (s*v2), (t*v0*v2) + (s*v1), 0,
				(t*v0*v1) + (s*v2), (t*v1*v1) + c, (t*v1*v2) - (s*v0), 0,
				(t*v0*v2) - (s*v1), (t*v1*v2) + (s*v0), (t*v2*v2) + c, 0,
				0, 0, 0, 1);
		}

		public void apply(
			float n00, float n01, float n02, float n03,
			float n10, float n11, float n12, float n13,
			float n20, float n21, float n22, float n23,
			float n30, float n31, float n32, float n33)
		{
			float r00 = m[0,0]*n00 + m[0,1]*n10 + m[0,2]*n20 + m[0,3]*n30;
			float r01 = m[0,0]*n01 + m[0,1]*n11 + m[0,2]*n21 + m[0,3]*n31;
			float r02 = m[0,0]*n02 + m[0,1]*n12 + m[0,2]*n22 + m[0,3]*n32;
			float r03 = m[0,0]*n03 + m[0,1]*n13 + m[0,2]*n23 + m[0,3]*n33;

			float r10 = m[1,0]*n00 + m[1,1]*n10 + m[1,2]*n20 + m[1,3]*n30;
			float r11 = m[1,0]*n01 + m[1,1]*n11 + m[1,2]*n21 + m[1,3]*n31;
			float r12 = m[1,0]*n02 + m[1,1]*n12 + m[1,2]*n22 + m[1,3]*n32;
			float r13 = m[1,0]*n03 + m[1,1]*n13 + m[1,2]*n23 + m[1,3]*n33;

			float r20 = m[2,0]*n00 + m[2,1]*n10 + m[2,2]*n20 + m[2,3]*n30;
			float r21 = m[2,0]*n01 + m[2,1]*n11 + m[2,2]*n21 + m[2,3]*n31;
			float r22 = m[2,0]*n02 + m[2,1]*n12 + m[2,2]*n22 + m[2,3]*n32;
			float r23 = m[2,0]*n03 + m[2,1]*n13 + m[2,2]*n23 + m[2,3]*n33;

			float r30 = m[3,0]*n00 + m[3,1]*n10 + m[3,2]*n20 + m[3,3]*n30;
			float r31 = m[3,0]*n01 + m[3,1]*n11 + m[3,2]*n21 + m[3,3]*n31;
			float r32 = m[3,0]*n02 + m[3,1]*n12 + m[3,2]*n22 + m[3,3]*n32;
			float r33 = m[3,0]*n03 + m[3,1]*n13 + m[3,2]*n23 + m[3,3]*n33;

			m[0,0] = r00; m[0,1] = r01; m[0,2] = r02; m[0,3] = r03;
			m[1,0] = r10; m[1,1] = r11; m[1,2] = r12; m[1,3] = r13;
			m[2,0] = r20; m[2,1] = r21; m[2,2] = r22; m[2,3] = r23;
			m[3,0] = r30; m[3,1] = r31; m[3,2] = r32; m[3,3] = r33;
		}

		public PMatrix copy() {
			PMatrix outgoing = new PMatrix ();
			for (int i=0; i<16; i++) {
				outgoing.m [i] = m [i];
			}
			return outgoing;
		}

	}
}

