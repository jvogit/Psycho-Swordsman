using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    [CreateAssetMenu(menuName = "Particles/Weapon Particle")]
    public class WeaponParticle : ScriptableObject
    {
        public GameObject attackParticle;
        public GameObject burnParticle;
        public GameObject chargeParticle;
        public GameObject idleParticle;
    }
}
