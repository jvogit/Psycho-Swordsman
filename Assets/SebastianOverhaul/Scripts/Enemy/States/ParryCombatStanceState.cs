using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class ParryCombatStanceState : CombatStanceState
    {
        public EnemyAttackAction parryMove;
        public float parryUntil = 0f;
        public float parryDuration = 5f;

        public override State Tick(EnemyManager enemyManager, EnemyStats stats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            var nextState = base.Tick(enemyManager, stats, enemyAnimatorHandler);

            // if in parry and the next state is attack override with parry
            if (Time.time < parryUntil && nextState is AttackState) {
                this.attackState.currentAttackMove = parryMove;
            }

            return nextState;
        }
    }
}
