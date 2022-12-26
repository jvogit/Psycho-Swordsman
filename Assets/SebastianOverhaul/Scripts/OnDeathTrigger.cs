using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    [RequireComponent(typeof(CharacterStats))]
    public class OnDeathTrigger : MonoBehaviour
    {
        public GameObject triggerObject;
        CharacterStats cs;

        void Start()
        {
            cs = GetComponent<CharacterStats>();
            cs.onDeath += onDeath;
        }

        void onDeath()
        {
            if (triggerObject) triggerObject.SetActive(false);
        }
    }
}
