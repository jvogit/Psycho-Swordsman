using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class WarlordManager : CaptainManager
    {
        public WarlordEndgame warlordEndgame;

        protected override void onDamage(int prev, int curr, int max, GameObject by)
        {
            base.onDamage(prev, curr, max, by);
            if (curr > 0)
            {
                var blockStance = GetComponentInChildren<BlockState>();
                var parryStanceState = GetComponentInChildren<ParryCombatStanceState>();
                parryStanceState.parryUntil = Time.time + parryStanceState.parryDuration + blockStance.blockDuration;
                currentState = blockStance;
            }
        }

        protected override void onDeath()
        {
            base.onDeath();
            if (GetComponentInChildren<WarlordSpareKillState>().hasDecided) warlordEndgame.Activate();
        }
    }
}
