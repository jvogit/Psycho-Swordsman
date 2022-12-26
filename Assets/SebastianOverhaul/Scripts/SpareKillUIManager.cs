using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class SpareKillUIManager : MonoBehaviour
    {
        public static SpareKillUIManager INSTANCE;
        public GameObject ButtonUI;
        private PlayerManager playerManager;
        private CaptainManager currentCaptain;

        public System.Action<CaptainManager> OnCaptainSpare, OnCaptainKill;

        private void Awake()
        {
            if (INSTANCE)
            {
                Destroy(INSTANCE);
            }

            INSTANCE = this;
        }

        private void Start()
        {
            this.ButtonUI.SetActive(false);
        }

        public void EnableUI(CaptainManager cm, PlayerManager pm)
        {
            this.ButtonUI.SetActive(true);
            this.currentCaptain = cm;
            this.playerManager = pm;
            Cursor.lockState = CursorLockMode.None;
        }

        public void SpareCaptain()
        {
            if (OnCaptainSpare != null) OnCaptainSpare(this.currentCaptain);
            this.ButtonUI.SetActive(false);
            currentCaptain.cutSceneVCam.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerManager.animatorHandler.PlayTargetAnimation("Empty", false);
            InputHandler.INSTANCE.enabled = true;
        }

        public void KillCaptain()
        {
            if (OnCaptainKill != null) OnCaptainKill(this.currentCaptain);
            this.ButtonUI.SetActive(false);
            currentCaptain.cutSceneVCam.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerManager.animatorHandler.PlayTargetAnimation("OH_Charge_Attack_Release", true);
            InputHandler.INSTANCE.enabled = true;
        }
    }
}
