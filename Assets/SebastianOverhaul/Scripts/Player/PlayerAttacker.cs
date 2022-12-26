using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class PlayerAttacker : MonoBehaviour
    {
        private AnimatorHandler animatorHandler;
        private PlayerManager playerManager;
        private int attackPhase = 0;

        private void Start()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
        }

        public void HandleQuickAttack(WeaponItem item, bool isLeft = false)
        {
            if (playerManager.isInteracting && !playerManager.canDoCombo) return;
            if (playerManager.canDoCombo)
            {
                animatorHandler.anim.SetBool("canDoCombo", false);
                attackPhase = (attackPhase + 1) % 3;
            }
            else
            {
                attackPhase = 0;
            }
            animatorHandler.anim.SetBool("isUsingLeftHand", isLeft);
            animatorHandler.PlayTargetAnimation(item.OH_Quick_Attack_ + attackPhase, true);
        }

        public void HandleChargeAttackChargeUp(WeaponItem item, bool isLeft = false)
        {
            if (playerManager.isInteracting) return;
            animatorHandler.anim.SetBool("is" + (isLeft ? "Left" : "Right") + "ChargingUp", true);
            animatorHandler.PlayTargetAnimation(item.OH_Charge_Attack_ + "Charge_Up", true);
        }

        public void HandleChargeAttackRelease(WeaponItem item, bool isLeft = false)
        {
            animatorHandler.PlayTargetAnimation(item.OH_Charge_Attack_ + "Release", true);
        }

        public void HandleParry(WeaponItem item, bool isLeft = false)
        {
            animatorHandler.anim.SetBool("isUsingLeftHand", isLeft);
            animatorHandler.PlayTargetAnimation(item.OH_Quick_Attack_, true);
        }
    }
}
