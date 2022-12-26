using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform cameraTargetRoot;
        public static CameraHandler INSTANCE;
        public float pivotSpeed = 5f;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;
        public bool cursorLocked = true;

        private void Awake()
        {
            if (INSTANCE)
            {
                Destroy(INSTANCE);
            }

            INSTANCE = this;

            Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        #region Camera Rotation

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _threshold = 0.01f;

        public void HandleCameraRotation(float delta, float mouseX, float mouseY)
        {
            if (lockOn && nearestLockOn)
            {
                // only if lockOn enabled and nearestLockOn is available
                cameraTargetRoot.rotation = Quaternion.LookRotation(nearestLockOn.position - cameraTargetRoot.position + Vector3.down * LockOnHeightOffset);
            }
            else
            {
                // if there is an input and camera position is not fixed
                var _input = new Vector2(mouseX, mouseY);
                if (_input.sqrMagnitude >= _threshold)
                {
                    _cinemachineTargetYaw += _input.x * delta;
                    _cinemachineTargetPitch += _input.y * delta;
                }

                // clamp our rotations so our values are limited 360 degrees
                _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Cinemachine will follow this target
                cameraTargetRoot.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);

                // cannot be in lockOn
                _lockOn = false;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        #endregion

        #region Lock On

        public float maximumLockOnDistance = 30;
        public Transform nearestLockOn;
        public Transform nearestLeftLockOn;
        public Transform nearestRightLockOn;
        private bool _lockOn;
        public bool lockOn 
        { 
            get => _lockOn; 
            set 
            { 
                _cinemachineTargetPitch = 0;
                var q = cameraTargetRoot.GetComponentInParent<PlayerManager>().transform.rotation;
                _cinemachineTargetYaw = Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z);
                this._lockOn = value; 
            } 
        }
        public float LockOnHeightOffset = 5f;
        public LayerMask environmentLayerMask;

        public bool FindLockOnTargets()
        {
            CharacterManager shortestDistanceTarget = null;
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceLeft = -Mathf.Infinity;
            float shortestDistanceRight = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(cameraTargetRoot.position, 26);
            List<CharacterManager> availableTargets = new List<CharacterManager>();

            foreach (Collider other in colliders)
            {
                CharacterManager cm = other.GetComponent<CharacterManager>();
                if (!cm || !(cm is EnemyManager) || cm.characterStats.isDead) continue;

                Vector3 lockTargetDir = cm.transform.position - cameraTargetRoot.position;
                float distanceFromTarget = Vector3.Distance(cameraTargetRoot.position, cm.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDir, Camera.main.transform.forward);

                if (viewableAngle > -50 && viewableAngle < 50 && distanceFromTarget <= maximumLockOnDistance)
                {
                    if (Physics.Linecast(cameraTargetRoot.position, cm.lockonTransform.position, environmentLayerMask))
                    {
                        Debug.DrawLine(cameraTargetRoot.position, cm.lockonTransform.position);
                        continue;
                    }
                    availableTargets.Add(cm);
                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistanceTarget = cm;
                        shortestDistance = distanceFromTarget;
                    }
                }
            }

            if (!lockOn) nearestLockOn = shortestDistanceTarget ? shortestDistanceTarget.lockonTransform : null;

            // do not check left and right if nearestLockOn cannot be found
            if (!nearestLockOn) return false;

            // determine left and right targets
            foreach (CharacterManager available in availableTargets)
            {
                Vector3 relativeEnemyPosition = cameraTargetRoot.InverseTransformPoint(available.transform.position);
                var distanceFromLeftOfTarget = relativeEnemyPosition.x;
                var distanceFromRightOfTarget = relativeEnemyPosition.x;

                if (relativeEnemyPosition.x <= 0.00f && distanceFromLeftOfTarget > shortestDistanceLeft)
                {
                    nearestLeftLockOn = available.lockonTransform;
                    shortestDistanceLeft = distanceFromLeftOfTarget;
                }
                else if (relativeEnemyPosition.x >= 0.00f && distanceFromRightOfTarget < shortestDistanceRight)
                {
                    nearestRightLockOn = available.lockonTransform;
                    shortestDistanceRight = distanceFromRightOfTarget;
                }
            }

            return true;
        }

        public void SwitchToLeftLockOnTarget()
        {
            if (nearestLeftLockOn) nearestLockOn = nearestLeftLockOn;
        }

        public void SwitchToRightLockOnTarget()
        {
            if (nearestRightLockOn) nearestLockOn = nearestRightLockOn;
        }

        #endregion
    }
}
