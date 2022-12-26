using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public abstract class State : MonoBehaviour
    {
        public abstract State Tick(EnemyManager enemyManager, EnemyStats stats, EnemyAnimatorHandler enemyAnimatorHandler);
    }

}