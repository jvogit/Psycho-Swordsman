using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    [CreateAssetMenu(menuName = "Particles/CharacterManagerParticles")]
    public class CharacterManagerParticles : ScriptableObject
    {
        public GameObject chargeParticle;
        public GameObject blockParticle;
    }
}
