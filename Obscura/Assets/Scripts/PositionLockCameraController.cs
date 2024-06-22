using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obscura
{
    public class PositionLockCameraController : AbstractCameraController
    {
        private Camera managedCamera;
        private LineRenderer cameraLineRenderer;

        private void Awake()
        {
            managedCamera = gameObject.GetComponent<Camera>();
            cameraLineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        void LateUpdate()
    {
        // Retrieve the position of the target GameObject
        Transform targetTransform = this.Target.transform;
        Transform cameraTransform = managedCamera.transform;

        // Adjust the camera's position to match the target's x and y, while maintaining its own z position
        cameraTransform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, cameraTransform.position.z);

        if (this.DrawLogic)
        {
            cameraLineRenderer.enabled = true;
            DrawCameraLogic();
        }
        else
        {
            cameraLineRenderer.enabled = false;
        }
    }

        public override void DrawCameraLogic()
        {
            float halfLineLength = 5f;
            Transform targetTransform = this.Target.transform;
            Transform cameraTransform = this.managedCamera.transform;

            var z = targetTransform.position.z - cameraTransform.position.z;

            cameraLineRenderer.positionCount = 9;
            cameraLineRenderer.useWorldSpace = false;
            cameraLineRenderer.SetPosition(0, new Vector3(0, 0, z));
            cameraLineRenderer.SetPosition(1, new Vector3(-halfLineLength, 0, z));
            cameraLineRenderer.SetPosition(2, new Vector3(0, 0, z));
            cameraLineRenderer.SetPosition(3, new Vector3(halfLineLength, 0, z));
            cameraLineRenderer.SetPosition(4, new Vector3(0, 0, z));
            cameraLineRenderer.SetPosition(5, new Vector3(0, halfLineLength, z));
            cameraLineRenderer.SetPosition(6, new Vector3(0, 0, z));
            cameraLineRenderer.SetPosition(7, new Vector3(0, -halfLineLength, z));
            cameraLineRenderer.SetPosition(8, new Vector3(0, 0, z));
        }
    }
}
