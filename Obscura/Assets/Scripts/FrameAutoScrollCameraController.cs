using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obscura
{
    public class FrameAutoScrollCameraController : AbstractCameraController
    {
        [SerializeField] private Vector2 topLeft;
        [SerializeField] private Vector2 bottomRight;
        [SerializeField] private float autoScrollSpeed;

        private Camera managedCamera;
        private LineRenderer cameraLineRenderer;

        private void Awake()
        {
            managedCamera = gameObject.GetComponent<Camera>();
            cameraLineRenderer = gameObject.GetComponent<LineRenderer>();
        }
        void Start()
        {
            Vector3 targetPosition = this.Target.transform.position;
            Vector3 cameraPosition = new Vector3(targetPosition.x, targetPosition.y, managedCamera.transform.position.z);
            managedCamera.transform.position = cameraPosition;
        }
        void LateUpdate()
        {
            // Access the position components of camera and target directly
            float cameraX = managedCamera.transform.position.x;
            float cameraY = managedCamera.transform.position.y;
            float cameraZ = managedCamera.transform.position.z;
            float targetX = this.Target.transform.position.x;
            float targetY = this.Target.transform.position.y;

            // Compute new camera X position
            float newXPosition = cameraX + autoScrollSpeed * Time.deltaTime;

            // Check and adjust target's position within specified boundaries
            if (targetY > cameraY + topLeft.y) targetY = cameraY + topLeft.y;
            if (targetY < cameraY + bottomRight.y) targetY = cameraY + bottomRight.y;
            if (targetX > newXPosition + bottomRight.x) targetX = newXPosition + bottomRight.x;
            if (targetX < newXPosition + topLeft.x) targetX = newXPosition + topLeft.x;

            // Update the target's position
            this.Target.transform.position = new Vector3(targetX, targetY, this.Target.transform.position.z);

            // Update the camera's position
            managedCamera.transform.position = new Vector3(newXPosition, cameraY, cameraZ);

            // Draw logic check
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
            var z = this.Target.transform.position.z - this.managedCamera.transform.position.z;

            cameraLineRenderer.positionCount = 5;
            cameraLineRenderer.useWorldSpace = false;
            cameraLineRenderer.SetPosition(0, new Vector3(topLeft.x, topLeft.y, z));
            cameraLineRenderer.SetPosition(1, new Vector3(bottomRight.x, topLeft.y, z));
            cameraLineRenderer.SetPosition(2, new Vector3(bottomRight.x, bottomRight.y, z));
            cameraLineRenderer.SetPosition(3, new Vector3(topLeft.x, bottomRight.y, z));
            cameraLineRenderer.SetPosition(4, new Vector3(topLeft.x, topLeft.y, z));
        }
    }
}