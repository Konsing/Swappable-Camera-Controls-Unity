using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obscura
{
    public class TargetFocusCameraController : AbstractCameraController
    {
        [SerializeField] private float idleDuration;
        [SerializeField] private float returnSpeed;
        [SerializeField] private float leadSpeedMultiplier;
        [SerializeField] private float leadMaxDistance;

        private Camera managedCamera;
        private LineRenderer cameraLineRenderer;

        private PlayerController playerController;

        private float timeSinceLastMove;

        private void Awake()
        {
            managedCamera = gameObject.GetComponent<Camera>();
            cameraLineRenderer = gameObject.GetComponent<LineRenderer>();
            playerController = this.Target.GetComponent<PlayerController>();
        }

        void Start()
        {
            // Directly set the camera's position components
            managedCamera.transform.position = new Vector3(
                this.Target.transform.position.x, 
                this.Target.transform.position.y, 
                managedCamera.transform.position.z);
        }

        void LateUpdate()
        {
            // Directly access the position components of target and camera
            float targetX = this.Target.transform.position.x;
            float targetY = this.Target.transform.position.y;
            float targetZ = this.Target.transform.position.z;
            float cameraX = managedCamera.transform.position.x;
            float cameraY = managedCamera.transform.position.y;
            float cameraZ = managedCamera.transform.position.z;
            
            // Get target's speed and movement direction components
            float targetSpeed = playerController.GetCurrentSpeed();
            float moveDirX = playerController.GetMovementDirection().x;
            float moveDirY = playerController.GetMovementDirection().y;
            float moveDirZ = playerController.GetMovementDirection().z;

            // Calculate leading position components
            float leadPosX = targetX + moveDirX * targetSpeed * leadSpeedMultiplier;
            float leadPosY = targetY + moveDirY * targetSpeed * leadSpeedMultiplier;
            float leadPosZ = targetZ + moveDirZ * targetSpeed * leadSpeedMultiplier;

            float cameraMoveSpeed = targetSpeed;
            float distanceToLeadingPosition = Mathf.Sqrt(
                Mathf.Pow(leadPosX - cameraX, 2) + 
                Mathf.Pow(leadPosY - cameraY, 2) + 
                Mathf.Pow(leadPosZ - cameraZ, 2));

            if (moveDirX == 0 && moveDirY == 0 && moveDirZ == 0) // When target is not moving
            {
                timeSinceLastMove += Time.deltaTime;
                if (timeSinceLastMove > idleDuration)
                {
                    // Move camera towards target position
                    managedCamera.transform.position = new Vector3(
                        Mathf.MoveTowards(cameraX, targetX, Time.deltaTime * returnSpeed),
                        Mathf.MoveTowards(cameraY, targetY, Time.deltaTime * returnSpeed),
                        cameraZ);
                }
            }
            else // When target is moving
            {
                timeSinceLastMove = 0;
                if (distanceToLeadingPosition >= leadMaxDistance)
                {
                    // Adjust leading position to max distance
                    leadPosX = targetX + moveDirX * leadMaxDistance;
                    leadPosY = targetY + moveDirY * leadMaxDistance;
                    leadPosZ = targetZ + moveDirZ * leadMaxDistance;
                }

                if (moveDirX != 0 && moveDirY != 0)
                {
                    cameraMoveSpeed *= Mathf.Sqrt(moveDirX * moveDirX + moveDirY * moveDirY) * leadMaxDistance;
                }

                // Move camera towards leading position
                managedCamera.transform.position = new Vector3(
                    Mathf.MoveTowards(cameraX, leadPosX, Time.deltaTime * cameraMoveSpeed),
                    Mathf.MoveTowards(cameraY, leadPosY, Time.deltaTime * cameraMoveSpeed),
                    cameraZ);
            }

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
            float z = this.Target.transform.position.z - managedCamera.transform.position.z;

            cameraLineRenderer.positionCount = 9;
            cameraLineRenderer.useWorldSpace = false;

            // Set line renderer positions
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