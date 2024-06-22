using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obscura
{
    public class PositionFollowCameraController : AbstractCameraController
    {
        [SerializeField] private float followSpeedFactor = 0.6f;
        [SerializeField] private float leashDistance = 16;
        [SerializeField] private float catchUpSpeed = 6f;

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
            // Directly access and set the position components of the camera
            managedCamera.transform.position = new Vector3(
                this.Target.transform.position.x,
                this.Target.transform.position.y,
                managedCamera.transform.position.z
            );
        }

        void LateUpdate()
        {
            // Directly access the position components of target and camera
            float targetX = this.Target.transform.position.x;
            float targetY = this.Target.transform.position.y;
            float cameraX = managedCamera.transform.position.x;
            float cameraY = managedCamera.transform.position.y;
            float cameraZ = managedCamera.transform.position.z;

            // Calculate distance in X and Y
            float distanceX = targetX - cameraX;
            float distanceY = targetY - cameraY;
            float distance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

            // Get target's speed and movement direction
            float targetSpeed = playerController.GetCurrentSpeed();
            Vector3 movementDirection = playerController.GetMovementDirection();

            // Define camera move speed
            float cameraMoveSpeed;

            if (movementDirection == Vector3.zero)
            {
                cameraMoveSpeed = catchUpSpeed;
            }
            else
            {
                cameraMoveSpeed = targetSpeed * followSpeedFactor;
                if (distance >= leashDistance)
                {
                    cameraMoveSpeed = targetSpeed;
                }
                if (movementDirection.x != 0 && movementDirection.y != 0)
                {
                    cameraMoveSpeed *= movementDirection.magnitude;
                }
            }

            // Move the camera towards the target
            float newX = Mathf.MoveTowards(cameraX, targetX, cameraMoveSpeed * Time.deltaTime);
            float newY = Mathf.MoveTowards(cameraY, targetY, cameraMoveSpeed * Time.deltaTime);
            managedCamera.transform.position = new Vector3(newX, newY, cameraZ);

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
            float halfLine = 5f;
            float z = this.Target.transform.position.z - this.managedCamera.transform.position.z;

            cameraLineRenderer.positionCount = 9;
            cameraLineRenderer.useWorldSpace = false;

            // Define line positions
            cameraLineRenderer.SetPosition(0, new Vector3(0, 0, z));
            cameraLineRenderer.SetPosition(1, new Vector3(-halfLine, 0, z));
            cameraLineRenderer.SetPosition(2, new Vector3(0, 0, z));
            cameraLineRenderer.SetPosition(3, new Vector3(halfLine, 0, z));
            cameraLineRenderer.SetPosition(4, new Vector3(0, 0, z));
            cameraLineRenderer.SetPosition(5, new Vector3(0, halfLine, z));
            cameraLineRenderer.SetPosition(6, new Vector3(0, 0, z));
            cameraLineRenderer.SetPosition(7, new Vector3(0, -halfLine, z));
            cameraLineRenderer.SetPosition(8, new Vector3(0, 0, z));
        }
    }
}