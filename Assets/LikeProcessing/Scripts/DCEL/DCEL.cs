using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LikeProcessing
{

    public class HalfEdge {
        public Vertex Origin;
        public HalfEdge Twin;
        public HalfEdge Next;
        public HalfEdge Prev;
        public Face IncidentFace;

    }

    public class Vertex {
        public Vector3 position;
        public HalfEdge IncidentHalfEdge;
    }

    public class Face {
        public HalfEdge IncidentHalfEdge;
    }

    public class DCEL
    {
		public Vector3[] vertices;
		public int[] indeces;
		public List<HalfEdge> halfEdgeList = new List<HalfEdge>();

		public DCEL (Vector3[] _vertices, int[] _indeces) {
			for (int i=0; i<_indeces.Length; i+=3) {
				
			}
		}
        
    }
}