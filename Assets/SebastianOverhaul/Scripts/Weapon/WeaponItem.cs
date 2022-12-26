using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : ScriptableObject
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Attack Animations Base Name")]
        public string OH_Quick_Attack_;
        public string OH_Charge_Attack_;

        [Header("Weapon Type")]
        public bool isMeleeWeapon;
        public bool isShieldWeapon;

        public WeaponParticle weaponParticle;
    }
}
