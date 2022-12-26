using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class EnemyAnimatorHandler : AnimatorHandler
    {
        EnemyManager enemyManager;

        protected override void Start()
        {
            base.Start();
            enemyManager = GetComponentInParent<EnemyManager>();
        }
        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            enemyManager.rb.drag = 0;
            Vector3 deltaPostion = anim.deltaPosition;
            deltaPostion.y = 0;

            Vector3 vel = deltaPostion / delta;
            enemyManager.rb.velocity = vel;
        }
    }
}
