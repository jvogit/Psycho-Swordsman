using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class CaptainManager : EnemyManager
    {
        public int CaptainID = 0;
        public Transform spareKillPosition;
        public Cinemachine.CinemachineVirtualCamera cutSceneVCam;
        public float blockCoolDownUntil = 0f;

        protected override void Start()
        {
            base.Start();
        }

        protected bool CanBlock()
        {
            return Time.time > blockCoolDownUntil;
        }

        protected override void onDamage(int prev, int curr, int max, GameObject by)
        {
            base.onDamage(prev, curr, max, by);
            if (curr > 0 && CanBlock())
            {
                this.currentState = this.GetComponentInChildren<BlockState>();
            }
            if (curr <= 0)
            {
                PlayerManager cm = GameObject.FindObjectOfType<PlayerManager>();
                if (cm && cm.playerClass.abilities[CaptainID] == PlayerClass.UnlockState.LOCKED)
                {
                    cm.animatorHandler.PlayTargetAnimation("OH_Charge_Attack_Charge_Up", true);
                    cm.gameObject.transform.position = spareKillPosition.position;
                    cm.gameObject.transform.rotation = spareKillPosition.rotation;
                    InputHandler.INSTANCE.enabled = false;
                    SpareKillUIManager.INSTANCE.EnableUI(this, cm);
                    cutSceneVCam.enabled = true;
                }
            }
        }

        protected override void onDeath()
        {
            string[] randomQuotes = { "Please spare me!", "You don't have the heart to!", "Do what you must", "No, please no!" };
            base.onDeath();
            FeedbackStatusBehavior.INSTANCE.SetFeedback(randomQuotes[Random.Range(0, randomQuotes.Length)]);
            this.currentState = this.GetComponentInChildren<SpareKillState>();
        }
    }
}
