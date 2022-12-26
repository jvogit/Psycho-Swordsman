using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{

    public event Action<bool, Transform> OnPlayerAttackEnemy;
    public event Action<CaptainController> OnCaptainDefeat;
    public event Action<CaptainController> OnCaptainKill;
    public event Action<CaptainController> OnCaptainSpare;
    public static EventManager INSTANCE;

    private void Awake()
    {
        if (INSTANCE)
        {
            Destroy(INSTANCE);
        }

        INSTANCE = this;
    }

    public void TriggerCaptainDefeat(CaptainController c)
    {
        if (OnCaptainDefeat != null) OnCaptainDefeat(c); 
    }

    public void TriggerCaptainKill(CaptainController c)
    {
        if (OnCaptainKill != null) OnCaptainKill(c);
    }

    public void TriggerCaptainSpare(CaptainController c)
    {
        if (OnCaptainSpare != null) OnCaptainSpare(c);
    }

    public void TriggerOnPlayerAttackEnemy(bool isQuickAttcl, Transform enemy)
    {
        if (OnPlayerAttackEnemy != null) OnPlayerAttackEnemy(isQuickAttcl, enemy);
    }

}
