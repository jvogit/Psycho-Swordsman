using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PS
{
    public class EnemyLocoMotion : MonoBehaviour
    {
        private EnemyManager enemyManager;
        private EnemyAnimatorHandler enemyAnimatorHandler;

        private void Start()
        {
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorHandler = GetComponentInChildren<EnemyAnimatorHandler>();
        }
    }
}
