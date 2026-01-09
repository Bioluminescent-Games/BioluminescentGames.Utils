using System;
using UnityEngine;

namespace BioluminescentGames.Utils.Components
{
    public class LookAtCamera : MonoBehaviour
    {
        private enum Mode
        {
            LookAt,
            LookAtInverted,
            CameraForward,
            CameraForwardInverted,
        }
        
        [SerializeField] private Mode mode;

        private void LateUpdate()
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeCameraMain
            Camera cam = Camera.main;
            
            if (!cam) return;
            
            switch (mode)
            {
                case Mode.LookAt:
                    transform.LookAt(cam.transform);
                    break;
                case Mode.LookAtInverted:
                    Vector3 dirFromCamera = transform.position - cam.transform.position;
                    transform.LookAt(transform.position + dirFromCamera);
                    break;
                case Mode.CameraForward:
                    transform.forward = cam.transform.forward;
                    break;
                case Mode.CameraForwardInverted:
                    transform.forward = -cam.transform.forward;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
