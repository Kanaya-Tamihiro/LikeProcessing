using System;
using UnityEngine;
using System.Collections.Generic;

namespace LikeProcessing.PMetaball
{
	public class Core {

		public class CoreMono : MonoBehaviour {
			public Core core;
		}

		public GameObject gameObject;
        public Vector3 coreLocalPosition;
		public HashSet<Lattice> affectLattices = new HashSet<Lattice> ();
		float colliderRadius;
		float colliderRadiusSqrt;

		public Core (PMetaball metaball) {
			gameObject = new GameObject ("core");
			gameObject.tag = PMetaball.CoreTag;
			gameObject.transform.SetParent (metaball.gameObject.transform);
			Rigidbody rigidbody = gameObject.AddComponent<Rigidbody> ();
			rigidbody.isKinematic = true;
			colliderRadius = 1.0f;
			colliderRadiusSqrt = colliderRadius * colliderRadius;
			AddCollider ();
			CoreMono coreMono = gameObject.AddComponent<CoreMono> ();
			coreMono.core = this;
		}

		public Core(PMetaball metaball, Vector3 _position) : this(metaball) {
            SetLocalPosition(_position);
		}

		virtual public void AddCollider () {
			SphereCollider collider = gameObject.AddComponent<SphereCollider> ();
			collider.center = Vector3.zero;
			collider.isTrigger = true;
			collider.radius = colliderRadius;
		}

        public void SetLocalPosition (Vector3 localPosition) {
            gameObject.transform.localPosition = localPosition;
            coreLocalPosition = localPosition;
        }

		virtual public float[] CulcIsoValueAndNormal(Point p, float isoPower) {
			float[] result = new float[4];
			Vector3 position = gameObject.transform.localPosition;
			float sqrMagnitude = (p.loc - position).sqrMagnitude;
			float magnitude = (p.loc - position).magnitude;
			//				return isoPower / (1.0f + sqrMagnitude);
			//result [0] = isoPower / (1.0f + sqrMagnitude);	//1.0f / sqrMagnitude;
			result [0] = Mathf.Max (1.0f - magnitude, 0);
			Vector3 normal = Vector3.zero;
			if (result[0] != 0) {
				float k = 1.0f / magnitude;
				normal = k * (p.loc - position);	
			}
//			Vector3 normal = (1.0f / (sqrMagnitude * sqrMagnitude)) * ((p.loc - position) * 2);
			result [1] = normal.x;
			result [2] = normal.y;
			result [3] = normal.z;
			return result;
		}
//		virtual public float[] CulcIsoValueAndNormal(Point p, float isoPower) {
//			float[] result = new float[4];
//			Vector3 position = coreLocalPosition;
//			float sqrMagnitude = (p.loc - position).sqrMagnitude;
//			result[0] = Mathf.Max(1.0f - (sqrMagnitude / colliderRadiusSqrt), 0);
////			Debug.Log (result[0]);
//			Vector3 normal = Vector3.zero;
//			if (result[0] != 0) {
//				normal = (2.0f * 1.0f / colliderRadiusSqrt) * (p.loc - position);	
//			}
//			result [1] = normal.x;
//			result [2] = normal.y;
//			result [3] = normal.z;
//			return result;
//		}

	}

	public class CoreLine : Core
	{
		Vector3 p_org, p_end;
		Vector3 delta;

		public CoreLine (PMetaball metaball, Vector3 from, Vector3 to) : base(metaball)
		{
			p_org = from;
			p_end = to;
			delta = to - from;
		}

		override public float[] CulcIsoValueAndNormal(Point p, float isoPower) {
			float[] result = new float[4];
			Vector3 q = p.loc - p_org;
			Vector3 corePosition;
			float t = Vector3.Dot(delta, q) / delta.sqrMagnitude;
			float sqrMagnitude;
			if (t > 1) {
				corePosition = p_end;
				sqrMagnitude = (p.loc - p_end).sqrMagnitude;
			} else if (t < 0) {
				corePosition = p_org;
				sqrMagnitude = (p.loc - p_org).sqrMagnitude;
			} else {
				corePosition =p_org + t * delta; 
				sqrMagnitude = (q - (t * delta)).sqrMagnitude;
			}
			//				result[0] = isoPower / (1.0f + sqrMagnitude);
			result[0] = 1.0f / sqrMagnitude;
			Vector3 normal = (1.0f / (sqrMagnitude * sqrMagnitude)) * ((p.loc - corePosition) * 2);
			result [1] = normal.x;
			result [2] = normal.y;
			result [3] = normal.z;
			return result;
		}
	}
}

