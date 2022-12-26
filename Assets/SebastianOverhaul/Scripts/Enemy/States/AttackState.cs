using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class AttackState : State
    {
        public CombatStanceState combatStanceState;
        public PursueTargetState pursueTargetState;
        public EnemyAttackAction currentAttackMove;

        public bool willDoComboOnNextAttack = false;
        public bool hasPerformedAttack = false;

        public override State Tick(EnemyManager enemyManager, EnemyStats stats, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            PursueTargetState.HandleRotateToTarget(enemyManager);

            if (enemyManager.isInteracting)
            {
                enemyAnimatorHandler.UpdateAnimatorValues(0, 0);
                hasPerformedAttack = false;
                return this;
            }

            if (willDoComboOnNextAttack && enemyManager.canDoCombo)
            {
                AttackTargetWithCombo(enemyAnimatorHandler);
            }

            if (!hasPerformedAttack)
            {
                AttackTarget(enemyManager, enemyAnimatorHandler);
                RollForComboChance(enemyManager);
            }

            if (willDoComboOnNextAttack && hasPerformedAttack)
            {
                return this;
            }

            return combatStanceState;
        }

        private void AttackTarget(EnemyManager enemyManager, EnemyAnimatorHandler enemyAnimatorHandler)
        {
            enemyAnimatorHandler.PlayTargetAnimation(currentAttackMove.actionAnimation, true);
            enemyManager.recoveryTimer = currentAttackMove.recoveryTime;
            hasPerformedAttack = true;
        }

        private void AttackTargetWithCombo(EnemyAnimatorHandler enemyAnimatorHandler)
        {
            willDoComboOnNextAttack = false;
            enemyAnimatorHandler.PlayTargetAnimation(currentAttackMove.actionAnimation, true);
            currentAttackMove = null;
        }

        private void RollForComboChance(EnemyManager enemyManager)
        {
            float comboChance = Random.Range(0, 100);

            if (enemyManager.allowToDoCombos && comboChance <= enemyManager.comboLikelihood)
            {
                if (currentAttackMove.comboAttackAction != null)
                {
                    willDoComboOnNextAttack = true;
                    currentAttackMove = currentAttackMove.comboAttackAction;
                }
                else
                {
                    willDoComboOnNextAttack = false;
                    currentAttackMove = null;
                }
            }
        }
    }
}
