using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class PlayerManager : CharacterManager
    {
        InputHandler inputHandler;
        PlayerLocoMotion playerLocoMotion;
        CameraHandler cameraHandler;
        WeaponSlotManager weaponSlotManager;
        public PlayerClass playerClass;

        public bool isGrounded = true;
        public bool isInAir = false;

        public int mitigatedDamage = 1;

        protected override void Start()
        {
            base.Start();
            cameraHandler = CameraHandler.INSTANCE;
            inputHandler = GetComponent<InputHandler>();
            playerLocoMotion = GetComponent<PlayerLocoMotion>();
            animatorHandler = GetComponentInChildren<PlayerAnimatorHandler>();
            characterStats = GetComponent<PlayerStats>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            playerClass = GetComponent<PlayerClass>();
            weaponSlotManager.OnRightDamage += Trigger_Damage;
            characterStats.onDamage += (prev, curr, max, by) =>
            {
                if (this.isLeftChargingUp && this.playerClass.abilities[PlayerClass.SECOND_CAPT] == PlayerClass.UnlockState.SPARE)
                {
                    this.mitigatedDamage += (prev - curr);
                    return;
                }
                if (by != null) animatorHandler.PlayTargetAnimation("Take_Damage_1", true);
            };
            characterStats.onDeath += () =>
            {
                PauseMenu.INSTANCE.Lose();
                animatorHandler.PlayTargetAnimation("Death", true);
            };
            this.OnParry += by => { characterStats.addHealth(15); };
        }

        protected override void Update()
        {
            if (characterStats.isDead) return;
            if (PauseMenu.INSTANCE.IsPaused()) return;

            base.Update();

            float delta = Time.deltaTime;

            inputHandler.TickInput(delta);
            playerLocoMotion.HandleMovement(delta);
            playerLocoMotion.HandleRollAndSprint(delta);
            playerLocoMotion.HandleMovement(delta);
            playerLocoMotion.HandleRotation(delta);
        }

        protected override void FixedUpdate()
        {
            if (PauseMenu.INSTANCE.IsPaused()) return;
            base.FixedUpdate();
            playerLocoMotion.HandleFalling(Time.fixedDeltaTime);
        }

        protected override void LateUpdate()
        {
            if (PauseMenu.INSTANCE.IsPaused()) return;
            base.LateUpdate();
            // handle camera
            cameraHandler.HandleCameraRotation(Time.fixedDeltaTime, inputHandler.mouseX, inputHandler.mouseY);

            // handle input resets
            inputHandler.roll_Input = false;
            inputHandler.rollFlag = false;
            inputHandler.left_Attack_Input = false;
            inputHandler.left_Charge_Input = false;
            inputHandler.left_Charge_release_Input = false;
            inputHandler.right_Attack_Input = false;
            inputHandler.right_Charge_Input = false;
            inputHandler.right_Charge_release_Input = false;

            // keep track of in air timer
            if (isInAir)
            {
                playerLocoMotion.inAirTimer += Time.deltaTime;
            }
        }
    }
}
