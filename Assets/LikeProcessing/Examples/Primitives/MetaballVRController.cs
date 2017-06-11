using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LikeProcessing;
using LikeProcessing.PMetaball;
using UnityEngine.VR;

namespace LikeProcessig.Examples
{

    public class MetaballVRController : MonoBehaviour {

        Core dragCore;

        Primitives primitives;

        public VRNode leftOrRight;

    // Use this for initialization
        void Start() {
            primitives = GameObject.Find("PSketch").GetComponent<Primitives>();
        }

        // Update is called once per frame
        void Update() {
            SteamVR_TrackedObject trackedObject = GetComponent<SteamVR_TrackedObject>();
            var device = SteamVR_Controller.Input((int)trackedObject.index);

            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                RaycastHit hit;
                Ray ray = new Ray(InputTracking.GetLocalPosition(leftOrRight), InputTracking.GetLocalRotation(leftOrRight) * Vector3.forward);
                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    dragCore = hit.collider.gameObject.GetComponent<Core.CoreMono>().core;
                }
            }

            if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                dragCore = null;
            }

            if (dragCore != null)
            {
                primitives.pmetaball.MoveCore(dragCore, dragCore.gameObject.transform.localPosition + new Vector3(0.1f, 0, 0));
            }

            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {

                RaycastHit hit;
                Ray ray = new Ray(InputTracking.GetLocalPosition(leftOrRight), InputTracking.GetLocalRotation(leftOrRight) * Vector3.forward);

                if (Physics.Raycast(ray, out hit))
                {
                    primitives.pmetaball.AddCore(new Core(primitives.pmetaball, hit.point));
                }
                else
                {
                    Vector3 v3 = InputTracking.GetLocalPosition(leftOrRight) + InputTracking.GetLocalRotation(leftOrRight) * Vector3.forward * 4.0f;
                    //v3.z = 7.0f;
                    primitives.pmetaball.AddCore(new Core(primitives.pmetaball, v3));
                }
            }
        }
    }
}