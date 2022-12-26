using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class PlayerStats : CharacterStats
    {
        PlayerManager playerManager;
        PlayerClass playerClass;

        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            playerClass = GetComponent<PlayerClass>();
        }

        public override void damage(int damage, GameObject by)
        {
            // cancel the damage
            if (playerManager.isInvulnerable)
            {
                // blocking and unlocked TUT_CAPT spare ability
                if (playerManager.isRightChargingUp && playerClass.abilities[PlayerClass.TUT_CAPT] == PlayerClass.UnlockState.SPARE)
                {
                    this.addShield(10);
                }

                return;
            }

            base.damage(damage, by);
        }
    }
}
