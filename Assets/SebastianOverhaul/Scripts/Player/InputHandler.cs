using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler INSTANCE;
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool rollFlag = false;
        public bool roll_Input = false;

        public bool sprintFlag = false;

        public bool right_Attack_Input = false;
        public bool right_Charge_Input = false;
        public bool right_Charge_release_Input = false;
        public bool left_Attack_Input = false;
        public bool left_Charge_Input = false;
        public bool left_Charge_release_Input = false;
        public float leftPressedAttackAt = 0f;
        public float rightPressedAttackAt = 0f;
        public float attackHoldThreshold = 0.5f;

        public bool lockOn_Input = false;
        public bool lockOnLeft_Input = false;
        public bool lockOnRight_Input = false;

        PlayerManager playerManager;
        PlayerAttacker playerAttacker;
        WeaponInventory playerInventory;
        PlayerControls inputActions;
        CameraHandler cameraHandler;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            if (INSTANCE)
            {
                Destroy(INSTANCE);
            }

            INSTANCE = this;
        }

        private void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
                inputActions.PlayerActions.LeftAttack.started += ctx =>
                {
                    leftPressedAttackAt = Time.time;
                };
                inputActions.PlayerActions.LeftAttack.canceled += ctx =>
                {
                    if (playerManager.isRightChargingUp) return;
                    left_Attack_Input = Time.time - leftPressedAttackAt < attackHoldThreshold;
                    left_Charge_release_Input = !left_Attack_Input && playerManager.isLeftChargingUp;
                    Debug.Log("Release left attack " + left_Attack_Input + " " + left_Charge_release_Input);
                };
                inputActions.PlayerActions.RightAttack.started += ctx =>
                {
                    rightPressedAttackAt = Time.time;
                };
                inputActions.PlayerActions.RightAttack.canceled += ctx =>
                {
                    if (playerManager.isLeftChargingUp) return;
                    right_Attack_Input = Time.time - rightPressedAttackAt < attackHoldThreshold;
                    right_Charge_release_Input = !right_Attack_Input && playerManager.isRightChargingUp;
                };
                inputActions.PlayerActions.Roll.performed += ctx => roll_Input = true;
                inputActions.PlayerActions.LockOn.performed += ctx => lockOn_Input = true;
                inputActions.PlayerActions.LockOnLeft.performed += ctx => lockOnLeft_Input = true;
                inputActions.PlayerActions.LockOnRight.performed += ctx => lockOnRight_Input = true;
            }
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        private void Start()
        {
            cameraHandler = CameraHandler.INSTANCE;
            playerManager = GetComponent<PlayerManager>();
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<WeaponInventory>();
        }

        public void TickInput(float delta)
        {
            HandleChargeInput(delta);
            HandleSprintInput(delta);
            MoveInput(delta);
            HandleRollInput(delta);
            HandleAttackInput(delta);
            HandleLockOnInput(delta);
            HandleLockOnLeftInput(delta);
            HandleLockOnRightInput(delta);
        }

        private void HandleSprintInput(float delta)
        {
            this.sprintFlag = inputActions.PlayerMovement.Sprint.IsPressed();
        }

        private void HandleChargeInput(float delta)
        {
            // detect if left attack button is held down
            if (inputActions.PlayerActions.LeftAttack.IsPressed())
            {
                // if pass the threshold and not chargin pulse the charge input
                if (Time.time - leftPressedAttackAt >= attackHoldThreshold && !playerManager.isLeftChargingUp)
                {
                    left_Charge_Input = true;
                }
            }
            else if (inputActions.PlayerActions.RightAttack.IsPressed())
            {
                if (Time.time - rightPressedAttackAt >= attackHoldThreshold && !playerManager.isRightChargingUp)
                {
                    right_Charge_Input = true;
                }
            }
        }

        private void MoveInput(float delta)
        {
            if (!this.enabled)
            {
                moveAmount = 0;
                return;
            }
            horizontal = movementInput.x;
            vertical = movementInput.y;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleRollInput(float delta)
        {
            if (roll_Input)
            {
                rollFlag = true;
            }
        }

        private void HandleAttackInput(float delta)
        {
            if (left_Attack_Input || right_Attack_Input)
            {
                // intentionally flipped: left mouse button -> right hand item of player
                var isLeft = !left_Attack_Input;
                var weaponItem = isLeft ? playerInventory.leftItem : playerInventory.rightItem;
                if (!weaponItem) return;
                // !left_Attack_Input means isLeft hand will carry out!
                if (weaponItem.isMeleeWeapon) playerAttacker.HandleQuickAttack(weaponItem, isLeft);
                if (weaponItem.isShieldWeapon && !playerManager.isInteracting) playerAttacker.HandleParry(weaponItem, isLeft);
            }
            else if (left_Charge_Input)
            {
                playerAttacker.HandleChargeAttackChargeUp(playerInventory.rightItem, true);
            }
            else if (left_Charge_release_Input)
            {
                playerAttacker.HandleChargeAttackRelease(playerInventory.rightItem, true);
            }
            else if (right_Charge_Input)
            {
                playerAttacker.HandleChargeAttackChargeUp(playerInventory.leftItem, false);
            }
            else if(right_Charge_release_Input)
            {
                playerAttacker.HandleChargeAttackRelease(playerInventory.leftItem, false);
            }
        }

        private void HandleLockOnInput(float delta)
        {
            if (!lockOn_Input) return;

            lockOn_Input = false;

            if (cameraHandler.lockOn)
            {
                cameraHandler.lockOn = false;
            }
            else
            {
                // enable lock on only if a lock on target is found
                if (cameraHandler.FindLockOnTargets()) cameraHandler.lockOn = true;
            }
        }

        private void HandleLockOnLeftInput(float delta)
        {
            if (!lockOnLeft_Input) return;

            lockOnLeft_Input = false;

            if (cameraHandler.lockOn)
            {
                cameraHandler.FindLockOnTargets();
                cameraHandler.SwitchToLeftLockOnTarget();
            }
        }

        private void HandleLockOnRightInput(float delta)
        {
            if (!lockOnRight_Input) return;

            lockOnRight_Input = false;

            if (cameraHandler.lockOn)
            {
                cameraHandler.FindLockOnTargets();
                cameraHandler.SwitchToRightLockOnTarget();
            }
        }
    }
}