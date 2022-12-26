using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpareKillUIBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
        EventManager.INSTANCE.OnCaptainDefeat += OnCaptainDefeat;
        EventManager.INSTANCE.OnCaptainKill += DisableUI;
        EventManager.INSTANCE.OnCaptainSpare += DisableUI;
    }

    private void DisableUI(CaptainController c)
    {
        this.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnCaptainDefeat(CaptainController at)
    {
        this.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
