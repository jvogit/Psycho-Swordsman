using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class CombatStanceState : State
    {
        public EnemyAttackAction[] enemyAttacks;
        public AttackState attackState;
        public PursueTargetState pursueTargetState;
        public float circlingRadius = 3f;
        public float minRadius = 1.5f;

        bool randomDestinationSet = false;
        float verticalMovementValue;
        float horizontalMovementValue;

        public override State Tick(EnemyManager enemyManager, EnemyStats stats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            if (enemyManager.isInteracting)
            {
                enemyAnimatorHandler.UpdateAnimatorValues(0, 0);
                return this;
            }

            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            if (distanceFromTarget < minRadius)
            {
                verticalMovementValue = 0;
            }

            enemyAnimatorHandler.anim.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime);
            enemyAnimatorHandler.anim.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);

            attackState.hasPerformedAttack = false;

            if (distanceFromTarget > enemyManager.agroRadius)
            {
                return pursueTargetState;
            }

            if (!randomDestinationSet || distanceFromTarget > circlingRadius)
            {
                DecideCirclingAction(enemyManager, enemyAnimatorHandler);
            }

            PursueTargetState.HandleRotateToTarget(enemyManager);

            if (enemyManager.recoveryTimer <= 0 && attackState.currentAttackMove != null)
            {
                randomDestinationSet = false;
                return attackState;
            }
            else
            {
                GetNewAttack(enemyManager);
            }

            return this;
        }

        private void DecideCirclingAction(EnemyManager enemyManager, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            if (distanceFromTarget < circlingRadius) WalkAroundTarget(enemyManager, enemyAnimatorHandler);
            else WalkToTarget(enemyManager);
        }

        private void WalkToTarget(EnemyManager manager)
        {
            verticalMovementValue = manager.chaseSpeed;
            horizontalMovementValue = 0f;
        }

        private void WalkAroundTarget(EnemyManager enemyManager, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            verticalMovementValue = 0.5f;

            horizontalMovementValue = Random.Range(-1, 1);

            if (horizontalMovementValue <= 1 && horizontalMovementValue >= 0)
            {
                horizontalMovementValue = 0.7f;
            }
            else if (horizontalMovementValue >= -1 && horizontalMovementValue < 0)
            {
                horizontalMovementValue = -0.7f;
            }

            randomDestinationSet = true;
        }

        private void GetNewAttack(EnemyManager enemyManager)
        {
            Vector3 targetDir = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float viewableAngle = Vector3.Angle(targetDir, enemyManager.transform.forward);
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            int maxScore = 0;
            foreach (EnemyAttackAction attackAction in enemyAttacks)
            {
                if (distanceFromTarget >= attackAction.minimumDistanceNeededToAttack
                    && distanceFromTarget <= attackAction.maximumDistanceNeededToAttack
                    && viewableAngle >= attackAction.minimumAttackAngle
                    && viewableAngle <= attackAction.maximumAttackAngle)
                {
                    maxScore += attackAction.attackScore;
                }
            }

            int randomAttackScore = Random.Range(0, maxScore + 1);
            int currScore = 0;
            foreach (EnemyAttackAction attackAction in enemyAttacks)
            {
                if (distanceFromTarget >= attackAction.minimumDistanceNeededToAttack
                    && distanceFromTarget <= attackAction.maximumDistanceNeededToAttack
                    && viewableAngle >= attackAction.minimumAttackAngle
                    && viewableAngle <= attackAction.maximumAttackAngle)
                {
                    currScore += attackAction.attackScore;
                    if (currScore > randomAttackScore)
                    {
                        attackState.currentAttackMove = attackAction;
                        return;
                    }
                }
            }
        }
    }
}
