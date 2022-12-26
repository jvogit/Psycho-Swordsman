using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class FootstepsBehavior : MonoBehaviour
    {
        private PlayerManager playerManager;
        private AudioSource aS;

        private void Start()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            aS = GetComponent<AudioSource>();
        }

        private void Update()
        {
            bool isMoving = playerManager.IsMoving() && InputHandler.INSTANCE.moveAmount > 0;
            if (isMoving && !aS.isPlaying)
            {
                aS.Play();
            }
            else if (!isMoving)
            {
                aS.Stop();
            }
        }
    }
}
