using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class IdleState : State
    {
        public PursueTargetState pursueTargetState;
        public override State Tick(EnemyManager enemyManager, EnemyStats stats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            if (enemyManager.isInteracting) return this;

            Collider[] colliders = Physics.OverlapSphere(enemyManager.transform.position, enemyManager.detectionRadius, enemyManager.detectionLayer);
            foreach (Collider other in colliders)
            {
                CharacterManager cs = other.GetComponent<CharacterManager>();
                if (!cs) continue;

                Vector3 targetDir = cs.transform.position - enemyManager.transform.position;
                float viewableAngle = Vector3.Angle(targetDir, enemyManager.transform.forward);

                if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                {
                    enemyManager.currentTarget = cs;
                }
            }

            return enemyManager.currentTarget ? pursueTargetState as State : this as State;
        }
    }
}
