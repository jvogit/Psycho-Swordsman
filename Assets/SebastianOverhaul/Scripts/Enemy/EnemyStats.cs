using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class EnemyStats : CharacterStats
    {
        EnemyManager enemyManager;

        private void Start()
        {
            enemyManager = GetComponent<EnemyManager>();
        }

        public override void damage(int damage, GameObject by)
        {
            // cancel the damage
            if (enemyManager.isInvulnerable) return;
            base.damage(damage, by);
        }
    }
}
