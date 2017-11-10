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

        
    }
}