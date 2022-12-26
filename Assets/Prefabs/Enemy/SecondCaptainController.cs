using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class SecondCaptainController : CaptainController
    {
        [SerializeField]
        public float ChargingAttackFor = 1f;
        public int burnDamage = 1;
        public float burnTick = 1f;
        public float burnDuration = 5f;
        public float burnRadius = 2f;

        private bool inQuickAttackMode = false;
        private bool inChargeAttackMode = false;

        private int quickAttackPhase = 0;
        private int chargeAttackPhase = 0;

        protected override void OnChargeAttackEnter_anim()
        {
            base.OnChargeAttackEnter_anim();
            foreach (Collider other in Physics.OverlapSphere(transform.position, burnRadius))
            {
                if (!other.CompareTag("Player")) continue;

                StartCoroutine(Burn_Coroutine(other.GetComponent<CharacterStats>()));

                break;
            }
        }

        protected override void OnChargeAttackExit_anim()
        {
            this.block = false;
            this.lastBlockTime = Time.time;
            base.OnChargeAttackExit_anim();
        }

        protected override void OnDamageExit_anim()
        {
            base.OnDamageExit_anim();
            this.inQuickAttackMode = false;
            this.inChargeAttackMode = true;
        }

        IEnumerator Burn_Coroutine(CharacterStats playerCs)
        {
            var burnTimeUntil = Time.time + burnDuration;

            while (Time.time < burnTimeUntil)
            {
                playerCs.damage(burnDamage, this.gameObject);
                FeedbackStatusBehavior.INSTANCE.SetFeedback("BURNING!!");
                yield return new WaitForSeconds(burnTick);
            }
        }

        protected override void StartAttack()
        {
            if (this.inQuickAttackMode)
            {
                QuickAttack();
                if (this.quickAttackPhase == 2)
                {
                    this.inQuickAttackMode = false;
                    this.inChargeAttackMode = true;
                }

                this.quickAttackPhase = (this.quickAttackPhase + 1) % 3;
            }
            else if (this.inChargeAttackMode)
            {
                this.inChargeAttackMode = true;
                if (this.chargeAttackPhase == 2)
                {
                    this.inChargeAttackMode = false;
                    this.inQuickAttackMode = true;
                }
                this.chargeAttackPhase = (this.chargeAttackPhase + 1) % 3;
                // isChargingAttack = true;
                this.block = true;
                this.motor.stop();
                base.anim.animator.SetTrigger("ChargeAttack");
                // Invoke("ReleaseChargeAttack", ChargingAttackFor);
            }
            else
            {
                QuickAttack();
                this.inQuickAttackMode = true;
            }
        }
    }
}
