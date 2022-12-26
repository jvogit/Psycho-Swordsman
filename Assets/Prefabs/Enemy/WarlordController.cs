using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class WarlordController : CaptainController
    {
        [SerializeField]
        public float ChargingAttackFor = 1f;
        public float stunTime = 3f;
        public float chargeStunRadius = 5f;

        private bool inQuickAttackMode = false;
        private bool inChargeAttackMode = false;

        private int quickAttackPhase = 0;
        private int chargeAttackPhase = 0;

        public bool hasDiedFirst { get; private set; } = false;

        public override void OnParry(Transform player)
        {
            ImpactPlayer(player, 75 * -player.forward + 50 * Vector3.up, 50f);
            Invoke("UnBlock", 1f);
            StartCoroutine(StunPlayer(player));
        }

        protected override void OnChargeAttackEnter_anim()
        {
            base.OnChargeAttackEnter_anim();
            checkAndStunPlayerRadius();
        }

        public override void CaptainKill()
        {
            base.CaptainKill();

        }

        public override void CaptainSpare()
        {

            if (hasDiedFirst)
            {
                base.CaptainSpare();
                return;
            }

            EventManager.INSTANCE.TriggerCaptainSpare(this);
            anim.animator.SetBool("DeathLoop", false);
            vCamCutscene.enabled = false;
            this.hasDied = false;
            this.cs.addMaxHealth(100);
            hasDiedFirst = true;
        }

        protected void checkAndStunPlayerRadius()
        {
            foreach (Collider other in Physics.OverlapSphere(transform.position, chargeStunRadius))
            {
                if (!other.CompareTag("Player")) continue;

                var pac = other.GetComponent<PlayerAttackController>();

                ImpactPlayer(other.transform, 75 * -other.transform.forward + 50 * Vector3.up, 50f);

                StartCoroutine(StunPlayer(other.transform));

                break;
            }
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
            FeedbackStatusBehavior.INSTANCE.SetFeedback("STUNNED!!");
            yield return new WaitForSeconds(stunTime);
            tpc.ActivateInput();
        }

        protected override void OnAttackEnter_anim()
        {
            base.OnAttackEnter_anim();
            if (this.quickAttackPhase == 2)
            {
                Debug.Log("Third attack!");
                checkAndStunPlayerRadius();
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
                this.motor.stop();
                base.anim.animator.SetTrigger("ChargeAttack");
            }
            else
            {
                this.inQuickAttackMode = true;
            }
        }
    }
}
