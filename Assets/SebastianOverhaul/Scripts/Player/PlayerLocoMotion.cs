using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class PlayerLocoMotion : MonoBehaviour
    {
        [Header("Ground & Air Stats")]
        public float groundDetectionRayStartPoint = 0.5f;
        public float minimumDistanceNeededToFall = 1f;
        public float groundDirectionRayDistance = 0.2f;
        public LayerMask ignoreGroundCheckLayers;
        public float gravity = 9.8f;

        [Header("Movement Stats")]
        public float movementSpeed = 5f;
        public float sprintSpeed = 10f;
        public float rotationSpeed = 10f;


        InputHandler inputHandler;
        AnimatorHandler animatorHandler;
        PlayerManager playerManager;
        CameraHandler cameraHandler;

        [HideInInspector]
        public Rigidbody rb;
        private Transform cam;
        private Vector3 moveDirection;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
            inputHandler = GetComponent<InputHandler>();
            cameraHandler = CameraHandler.INSTANCE;
            cam = Camera.main.transform;
        }

        #region Movement

        private Vector3 targetPosition;

        public void HandleRotation(float delta)
        {
            if (!playerManager.canRotate) return;

            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            if (cameraHandler.lockOn && cameraHandler.nearestLockOn)
            {
                targetDir = cameraHandler.nearestLockOn.position - transform.position;
            }
            else
            {
                targetDir = cam.forward * inputHandler.vertical;
                targetDir += cam.right * inputHandler.horizontal;
            }

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);
        }

        public void HandleMovement(float delta)
        {

            if (playerManager.isInteracting) return;

            moveDirection = cam.forward * inputHandler.vertical;
            moveDirection += cam.right * inputHandler.horizontal;
            moveDirection.y = rb.velocity.y;
            moveDirection.Normalize();

            moveDirection *= inputHandler.sprintFlag ? sprintSpeed : movementSpeed;

            rb.velocity = Vector3.ProjectOnPlane(moveDirection, normalVector);

            if (cameraHandler.lockOn)
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal);
            }
            else
            {
                var multiplier = inputHandler.sprintFlag ? 2 : 1;

                animatorHandler.UpdateAnimatorValues(multiplier * inputHandler.moveAmount, 0);
            }
        }

        public void HandleRollAndSprint(float delta)
        {
            if (playerManager.isInteracting) return;

            if (inputHandler.rollFlag)
            {
                moveDirection = cam.forward * inputHandler.vertical;
                moveDirection += cam.right * inputHandler.horizontal;

                if (inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Roll", true);
                    moveDirection.y = rb.velocity.y;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = rollRotation;
                }
            }
        }


        private Vector3 normalVector;
        public float inAirTimer = 0f;

        public void HandleFalling(float delta)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = transform.position;
            origin.y += groundDetectionRayStartPoint;

            if (Physics.Raycast(origin, transform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }

            if (playerManager.isInAir)
            {
                rb.AddForce(Vector3.down);
            }

            Vector3 dir = -moveDirection;
            dir.Normalize();
            origin += dir * groundDirectionRayDistance;

            targetPosition = transform.position;

            Debug.DrawRay(origin, Vector3.down * minimumDistanceNeededToFall, Color.red, 0.1f, false);

            // hit ground
            if (Physics.Raycast(origin, Vector3.down, out hit, minimumDistanceNeededToFall, ~ignoreGroundCheckLayers))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;

                if (playerManager.isInAir)
                {
                    if (inAirTimer > 0.7f)
                    {
                        animatorHandler.PlayTargetAnimation("Land", true);
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Empty", false);
                    }

                    inAirTimer = 0;
                    playerManager.isInAir = false;
                }
            }

            else
            {
                if (playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }

                if (!playerManager.isInAir)
                {
                    if (!playerManager.isInteracting) animatorHandler.PlayTargetAnimation("Fall", true);
                    rb.velocity = rb.velocity.normalized;
                    playerManager.isInAir = true;
                }
            }
        }

        #endregion
    }
}