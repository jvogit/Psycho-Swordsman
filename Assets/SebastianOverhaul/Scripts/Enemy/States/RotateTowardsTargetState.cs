using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class RotateTowardsTargetState : State
    {
        public override State Tick(EnemyManager enemyManager, EnemyStats stats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            return this;
        }
    }
}
