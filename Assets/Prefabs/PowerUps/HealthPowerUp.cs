using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class HealthPowerUp : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            other.GetComponent<CharacterStats>().addHealth(20);
            Destroy(this.gameObject);
        }
    }
}
