using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;

        public GameObject currentWeaponModel;

        public void UnloadWeapon()
        {
            if (currentWeaponModel)
            {
                currentWeaponModel.SetActive(false);
            }
        }

        public void UnloadWeaponAndDestroy()
        {
            if (currentWeaponModel)
            {
                Destroy(currentWeaponModel);
            }
        }

        public void LoadWeaponModel(WeaponItem item)
        {

            UnloadWeaponAndDestroy();

            if (item == null)
            {
                UnloadWeapon();
                return;
            }

            GameObject model = Instantiate(item.modelPrefab);
            if (parentOverride) model.transform.parent = parentOverride;
            else model.transform.parent = transform;

            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            currentWeaponModel = model;
        }
    }
}
