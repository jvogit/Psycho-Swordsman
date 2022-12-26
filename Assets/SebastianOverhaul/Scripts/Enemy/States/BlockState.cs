using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class BlockState : State
    {
        public float blockDuration = 4f;
        public float blockRecoveryDuration = 5f;
        public float blockRecoverUntil = 5f;
        public float blockUntil = 0f;
        public State nextState;
        
        public override State Tick(EnemyManager enemyManager, EnemyStats stats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            if (Time.time < blockRecoverUntil)
            {
                return nextState;
            }
            enemyAnimatorHandler.UpdateAnimatorValues(0, 0);
            if (!enemyManager.isInvulnerable && !enemyManager.isInteracting)
            {
                enemyAnimatorHandler.PlayTargetAnimation("Block_Charge_Up", true);
                blockUntil = Time.time + blockDuration;
            }
            else if (enemyManager.isInvulnerable && Time.time > blockUntil)
            {
                enemyAnimatorHandler.PlayTargetAnimation("Block_Release", true);
                blockRecoverUntil = Time.time + blockRecoveryDuration;
                return nextState;
            }

            return this;
        }
    }
}
