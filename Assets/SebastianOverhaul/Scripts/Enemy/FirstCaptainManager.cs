using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class FirstCaptainManager : CaptainManager
    {
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
    }
}
