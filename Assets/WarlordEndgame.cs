using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class WarlordEndgame : MonoBehaviour
    {
        public int neededToKill = 20;
        public GameObject[] toActivate;

        public int score = 0;
        public bool isActivated = false;
        public void Start()
        {
            foreach (GameObject obj in toActivate)
            {
                obj.SetActive(false);
            }
        }

        public void Activate()
        {
            if (isActivated) return;
            isActivated = true;
            Invoke("SetFeedback", 5);
            foreach (GameObject obj in toActivate)
            {
                obj.SetActive(true);
            }

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (obj.GetComponent<EnemyManager>()) obj.GetComponent<CharacterStats>().onDeath += onDeath;
            }
        }

        public void SetFeedback()
        {

            FeedbackStatusBehavior.INSTANCE.SetFeedback("You have to kill " + (neededToKill - score) + " more enemies!", 60);
        }

        public void onDeath()
        {
            if (score >= neededToKill) return;
            score += 1;
            SetFeedback();
            if (score >= neededToKill) PauseMenu.INSTANCE.Win();
        }
    }
}
