using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class CharacterManager : MonoBehaviour
    {
        public Transform lockonTransform;
        public AnimatorHandler animatorHandler;
        public CharacterStats characterStats;
        public bool isUsingLeftHand = false;
        public bool isInteracting = false;
        public bool isParrying = false;
        public bool isLeftChargingUp = false;
        public bool isRightChargingUp = false;
        public bool canDoCombo = false;
        public bool isInvulnerable = false;
        public bool isParried = false;
        public bool canRotate = false;
        public bool isChargeAttacking = false;

        public System.Action<CharacterManager> OnDamage;
        public System.Action<CharacterManager> OnParry;
        public System.Action OnQuickAttack;
        public System.Action OnQuickAttack3;
        public System.Action OnChargeAttack;

        protected virtual void Start()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            characterStats = GetComponent<CharacterStats>();
        }

        protected virtual void Update()
        {
            isInteracting = animatorHandler.anim.GetBool("isInteracting");
            isLeftChargingUp = animatorHandler.anim.GetBool("isLeftChargingUp");
            isRightChargingUp = animatorHandler.anim.GetBool("isRightChargingUp");
            canDoCombo = animatorHandler.anim.GetBool("canDoCombo");
            isInvulnerable = animatorHandler.anim.GetBool("isInvulnerable");
            canRotate = animatorHandler.anim.GetBool("canRotate");
            isUsingLeftHand = animatorHandler.anim.GetBool("isUsingLeftHand");
            isParrying = animatorHandler.anim.GetBool("isParrying");
            isChargeAttacking = animatorHandler.anim.GetBool("isChargeAttacking");
        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void LateUpdate()
        {

        }

        public bool IsMoving()
        {
            return (Mathf.Abs(animatorHandler.anim.GetFloat("Vertical")) > 0f || Mathf.Abs(animatorHandler.anim.GetFloat("Horizontal")) > 0f) && !isInteracting;
        }

        public void Trigger_OnQuickAttack()
        {
            if (OnQuickAttack != null) OnQuickAttack();
        }

        public void Trigger_OnQuickAttack3()
        {
            if (OnQuickAttack3 != null) OnQuickAttack3();
        }

        public void Trigger_OnChargeAttack()
        {
            if (OnChargeAttack != null) OnChargeAttack();
        }

        public void Trigger_Damage(CharacterManager cm)
        {
            if (OnDamage != null) OnDamage(cm);
        }
    }
}
