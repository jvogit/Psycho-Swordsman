using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class WeaponInventory : MonoBehaviour
    {
        private WeaponSlotManager weaponSlotManager;

        public WeaponItem leftItem;
        public WeaponItem rightItem;

        private void Start()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            weaponSlotManager.LoadWeaponSlot(leftItem, false);
            weaponSlotManager.LoadWeaponSlot(rightItem, true);
        }
    }

}