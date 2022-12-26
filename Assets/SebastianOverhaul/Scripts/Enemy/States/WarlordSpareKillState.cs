using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class WarlordSpareKillState : SpareKillState
    {
        public bool hasDecided = false;

        private void Start()
        {
            SpareKillUIManager.INSTANCE.OnCaptainKill += OnCaptainKill;
            SpareKillUIManager.INSTANCE.OnCaptainSpare += OnCaptainSpare;
        }

        public override State Tick(EnemyManager enemyManager, EnemyStats stats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            if (hasDecided)
            {
                SpareKillUIManager.INSTANCE.OnCaptainKill -= OnCaptainKill;
                SpareKillUIManager.INSTANCE.OnCaptainSpare -= OnCaptainSpare;
                return null;
            }

            return this;
        }

        void OnCaptainKill(CaptainManager cm)
        {
            if (cm.CaptainID != PlayerClass.WARLORD) return;
            // PauseMenu.INSTANCE.Win();
            ((WarlordManager)cm).warlordEndgame.Activate();
            hasDecided = true;
        }

        void OnCaptainSpare(CaptainManager cm)
        {
            if (cm.CaptainID != PlayerClass.WARLORD) return;
            cm.animatorHandler.PlayTargetAnimation("Empty", false);
            cm.characterStats.addMaxHealth(100);
            cm.SetCollidersState(true);
            cm.currentState = cm.GetComponentInChildren<IdleState>();
            hasDecided = true;

        }
    }
}
