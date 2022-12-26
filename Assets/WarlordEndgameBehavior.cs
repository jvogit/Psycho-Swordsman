using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class WarlordEndgameBehavior : MonoBehaviour
    {
        [SerializeField]
        private PlayerClass pc;
        public GameObject[] captains = { };
        public Transform[] spawnPoint;

        // Start is called before the first frame update
        void Start()
        {
            EventManager.INSTANCE.OnCaptainKill += OnCaptainKill;
            EventManager.INSTANCE.OnCaptainSpare += OnCaptainSpare;
            this.gameObject.SetActive(false);
        }

        void OnCaptainKill(CaptainController cc)
        {
            if (cc.CaptainID != PlayerClass.WARLORD) return;
            if (((WarlordController)cc).hasDiedFirst) return;
            this.gameObject.SetActive(true);
            this.SpawnCaptains();
        }

        void OnCaptainSpare(CaptainController cc)
        {
            if (cc.CaptainID != PlayerClass.WARLORD) return;
            if (((WarlordController)cc).hasDiedFirst) return;
            this.gameObject.SetActive(true);
        }

        void SpawnCaptains()
        {
            for (int i = 0; i < 3; ++i)
            {
                if (pc.abilities[i] != PlayerClass.UnlockState.SPARE) continue;
                GameObject captain = Instantiate(captains[i], spawnPoint[i].position, Quaternion.identity);
            }
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
