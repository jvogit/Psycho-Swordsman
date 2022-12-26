using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    [CreateAssetMenu(menuName = "AI/Enemy Actons/Attack")]
    public class EnemyAttackAction : EnemyAction
    {
        public bool canCombo = false;
        public EnemyAttackAction comboAttackAction;

        public int attackScore = 3;
        public float recoveryTime = 2f;

        public float maximumAttackAngle = 35f;
        public float minimumAttackAngle = -35f;

        public float minimumDistanceNeededToAttack = 0f;
        public float maximumDistanceNeededToAttack = 3f;
    }
}
