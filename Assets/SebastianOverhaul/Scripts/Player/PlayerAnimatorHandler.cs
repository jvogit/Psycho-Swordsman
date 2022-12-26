using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class PlayerAnimatorHandler : AnimatorHandler
    {
        PlayerManager playerManager;
        PlayerLocoMotion playerLocoMotion;

        protected override void Start()
        {
            base.Start();
            playerManager = GetComponentInParent<PlayerManager>();
            playerLocoMotion = GetComponentInParent<PlayerLocoMotion>();
        }

        private void OnAnimatorMove()
        {
            if (!playerManager.isInteracting) return;

            float delta = Time.deltaTime;

            playerLocoMotion.rb.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;

            Vector3 velocity = deltaPosition / delta;
            velocity.y = playerLocoMotion.rb.velocity.y;
            playerLocoMotion.rb.velocity = velocity;
        }
    }
}
