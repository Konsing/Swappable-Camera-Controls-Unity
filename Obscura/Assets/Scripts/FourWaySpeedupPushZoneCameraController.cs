using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obscura
{
    public class FourWaySpeedupPushZoneCameraController : AbstractCameraController
    {
        [SerializeField] private float pushRatio;
        [SerializeField] private Vector2 topLeft;
        [SerializeField] private Vector2 bottomRight;

        private Camera managedCamera;
        private LineRenderer cameraLineRenderer;

        private PlayerController playerController;

        private void Awake()
        {
            managedCamera = gameObject.GetComponent<Camera>();
            cameraLineRenderer = gameObject.GetComponent<LineRenderer>();
            playerController = this.Target.GetComponent<PlayerController>();
        }

        void Start()
        {
            // Directly set camera's position components
            managedCamera.transform.position = new Vector3(
                this.Target.transform.position.x, 
                this.Target.transform.position.y, 
                managedCamera.transform.position.z);
        }

        void LateUpdate()
        {
            // Access camera's and target's position components
            float cameraX = managedCamera.transform.position.x;
            float cameraY = managedCamera.transform.position.y;
            float cameraZ = managedCamera.transform.position.z;
            float targetX = this.Target.transform.position.x;
            float targetY = this.Target.transform.position.y;

            // Get target's speed
            float targetSpeed = playerController.GetCurrentSpeed();
            // Get movement direction components
            float moveDirX = playerController.GetMovementDirection().x;
            float moveDirY = playerController.GetMovementDirection().y;

            // Edge touch conditions
            bool touchingLeftEdge = targetX <= cameraX + topLeft.x;
            bool touchingRightEdge = targetX >= cameraX + bottomRight.x;
            bool touchingTopEdge = targetY >= cameraY + topLeft.y;
            bool touchingBottomEdge = targetY <= cameraY + bottomRight.y;

            bool touchingHorizontalEdge = touchingLeftEdge || touchingRightEdge;
            bool touchingVerticalEdge = touchingTopEdge || touchingBottomEdge;

            float xSpeed = touchingHorizontalEdge ? targetSpeed : targetSpeed * pushRatio;
            float ySpeed = touchingVerticalEdge ? targetSpeed : targetSpeed * pushRatio;

            // Calculate new position components
            float newX = cameraX;
            float newY = cameraY;

            if (moveDirX != 0 && touchingHorizontalEdge)
            {
                newX += moveDirX * xSpeed * Time.deltaTime;
            }

            if (moveDirY != 0 && touchingVerticalEdge)
            {
                newY += moveDirY * ySpeed * Time.deltaTime;
            }

            // Update camera's position
            managedCamera.transform.position = new Vector3(newX, newY, cameraZ);

            // Drawing logic
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
            float z = this.Target.transform.position.z - managedCamera.transform.position.z;

            cameraLineRenderer.positionCount = 5;
            cameraLineRenderer.useWorldSpace = false;

            // Set line renderer positions using component-wise positions
            cameraLineRenderer.SetPosition(0, new Vector3(topLeft.x, topLeft.y, z));
            cameraLineRenderer.SetPosition(1, new Vector3(bottomRight.x, topLeft.y, z));
            cameraLineRenderer.SetPosition(2, new Vector3(bottomRight.x, bottomRight.y, z));
            cameraLineRenderer.SetPosition(3, new Vector3(topLeft.x, bottomRight.y, z));
            cameraLineRenderer.SetPosition(4, new Vector3(topLeft.x, topLeft.y, z));
        }
    }
}