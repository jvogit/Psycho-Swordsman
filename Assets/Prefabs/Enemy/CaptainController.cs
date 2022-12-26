using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CaptainController : EnemyController
{
    [SerializeField]
    public int CaptainID;
    public float blockDuration = 5f;
    [SerializeField]
    public float blockThreshold = 3f;
    [SerializeField]
    public Transform playerPosCutscene;
    [SerializeField] 
    public Cinemachine.CinemachineVirtualCamera vCamCutscene;


    protected float lastBlockTime = 0f;
    protected bool hasDied = false;

    protected override void OnDeath_anim()
    {
        this.hasDied = true;
        this.block = false;
        this.anim.animator.SetBool("DeathLoop", true);
        this.motor.enabled = false;
        EventManager.INSTANCE.TriggerCaptainDefeat(this);
        vCamCutscene.enabled = true;
    }

    public virtual void CaptainKill()
    {
        EventManager.INSTANCE.TriggerCaptainKill(this);
        Destroy(this.gameObject);
    }

    public virtual void CaptainSpare()
    {
        EventManager.INSTANCE.TriggerCaptainSpare(this);
        vCamCutscene.enabled = false;
        this.enabled = false;
        this.motor.enabled = false;
        this.motor.setTarget(null);
    }

    protected override void onHealthChanged(int previous, int current, int max, GameObject by)
    {
        if (!this.hasDied) base.onHealthChanged(previous, current, max, by);
    }

    protected void Block()
    {
        Reset();
        this.block = true;
        this.anim.animator.SetBool("Block", true);
        this.motor.stop();
    }

    protected void UnBlock()
    {
        Reset();
        this.block = false;
        this.anim.animator.SetBool("Block", false);
        this.lastBlockTime = Time.time;
    }

    protected override bool CanMove()
    {
        return base.CanMove() && !block;
    }

    protected bool CanBlock()
    {
        return !this.block && !this.isTakingDamage && !this.isAttacking && !this.isChargeAttacking && (Time.time - lastBlockTime) >= blockThreshold;
    }

    protected override void OnDamageExit_anim()
    {
        base.OnDamageExit_anim();
        if (!this.block && (Time.time - lastBlockTime) >= blockThreshold && motor.enabled)
        {
            Block();
            Invoke("UnBlock", blockDuration);
        }
    }

    protected override void DoAI()
    {
        if (this.hasDied || this.block)
        {
            // Debug.Log("CaptainController stopped AI " + this.hasDied + " " + this.block);
            return;
        }
        base.DoAI();
    }

    protected void QuickAttack()
    {
        base.StartAttack();
    }

    protected override void StartAttack()
    {
        int r = Random.Range(0, 2);

        if (CanBlock() && r == 0)
        {
            Block();
            Invoke("UnBlock", blockDuration);
        }
        else
        {
            base.StartAttack();
        }
    }
}
