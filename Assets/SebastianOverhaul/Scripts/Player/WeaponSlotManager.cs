using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class WeaponSlotManager : MonoBehaviour
    {
        public LayerMask collideWith;
        private CharacterManager playerManager;
        private WeaponHolderSlot leftSlot;
        private WeaponHolderSlot rightSlot;

        public System.Action<CharacterManager> OnRightDamage;

        private DamageCollider leftDamageCollider;
        private DamageCollider rightDamageCollider;

        private void Awake()
        {
            foreach (WeaponHolderSlot slot in GetComponentsInChildren<WeaponHolderSlot>())
            {
                if (slot.isLeftHandSlot)
                {
                    leftSlot = slot;
                }
                if (slot.isRightHandSlot)
                {
                    rightSlot = slot;
                }
            }
        }

        private void Start()
        {
            playerManager = GetComponentInParent<CharacterManager>();
        }

        public void LoadWeaponSlot(WeaponItem item, bool isRight)
        {
            if (isRight)
            {
                rightSlot.LoadWeaponModel(item);
                rightDamageCollider = rightSlot.GetComponentInChildren<DamageCollider>();
                if (rightDamageCollider) rightDamageCollider.collideWith = collideWith;
                rightDamageCollider.onDamage += Trigger_OnRight;
            }
            else
            {
                leftSlot.LoadWeaponModel(item);
                leftDamageCollider = leftSlot.GetComponentInChildren<DamageCollider>();
                if (leftDamageCollider) leftDamageCollider.collideWith = collideWith;
            }
        }

        void Trigger_OnRight(CharacterManager cm)
        {
            if (OnRightDamage != null) OnRightDamage(cm);
        }

        public void EnableDamageCollider()
        {
            if (playerManager.isUsingLeftHand)
            {
                EnableLeftDamageCollider();
            }
            else
            {
                EnableRightDamageCollider();
            }
        }

        public void DisableDamageCollider()
        {
            DisableLeftDamageCollider();
            DisableRightDamageCollider();
        }

        private void EnableLeftDamageCollider()
        {
            leftDamageCollider.EnableDamageCollider();
        }

        private void EnableRightDamageCollider()
        {
            rightDamageCollider.EnableDamageCollider();
        }

        private void DisableLeftDamageCollider()
        {
            if (leftDamageCollider) leftDamageCollider.DisableDamageCollider();
        }

        private void DisableRightDamageCollider()
        {
            if (rightDamageCollider) rightDamageCollider.DisableDamageCollider();
        }
    }
}
