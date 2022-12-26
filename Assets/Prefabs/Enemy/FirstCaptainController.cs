using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class FirstCaptainController : CaptainController
    {
        [SerializeField]
        public float stunTime = 2f;

        private bool inQuickAttackMode = false;
        private bool isChargingAttack = false;

        private int phase = 0;

        protected override bool CanMove()
        {
            return base.CanMove() && !isChargingAttack;
        }

        public override void OnParry(Transform player)
        {
            ImpactPlayer(player, 75 * -player.forward + 50 * Vector3.up, 50f);
            Invoke("UnBlock", 0.75f);
            StartCoroutine(StunPlayer(player));
        }

        void ImpactPlayer(Transform player, Vector3 dir, float force)
        {
            ImpactReceiver ir = player.GetComponent<ImpactReceiver>();
            ir.AddImpact(dir, force);
        }

        IEnumerator StunPlayer(Transform player)
        {
            var tpc = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            tpc.DeactivateInput();
            FeedbackStatusBehavior.INSTANCE.SetFeedback("STUNNED!!", stunTime);
            yield return new WaitForSeconds(stunTime);
            tpc.ActivateInput();
        }

        protected override void StartAttack()
        {
            if (this.block || this.isChargingAttack) return;
            int r = Random.Range(0, 3);

            if (this.inQuickAttackMode)
            {
                QuickAttack();
                if (phase == 2)
                {
                    this.inQuickAttackMode = false;
                }

                this.phase = (this.phase + 1) % 3;
                return;
            }

            if (!this.block && (Time.time - this.lastBlockTime) >= this.blockThreshold && r == 0)
            {
                Block();
                Invoke("UnBlock", blockDuration);
            }
            else if (r == 1)
            {
                this.motor.stop();
                base.anim.animator.SetTrigger("ChargeAttack");
            }
            else
            {
                QuickAttack();
                inQuickAttackMode = true;
            }
        }

        protected override void Reset()
        {
            this.isChargeAttacking = false;
            base.Reset();
        }
    }
}
