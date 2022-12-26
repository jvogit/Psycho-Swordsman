using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;

public class PlayerClass : MonoBehaviour
{
    public enum UnlockState
    {
        LOCKED,
        SPARE,
        KILL
    }

    // 0 - tut capt
    // 1 - first capt
    // 2 - second capt
    // 3 - warlod
    public UnlockState[] abilities = { UnlockState.LOCKED, UnlockState.LOCKED, UnlockState.LOCKED, UnlockState.LOCKED };
    private Animator playerAnim;
    private PlayerAttackController pac;
    private CharacterStats cs;
    public const int TUT_CAPT = 0;
    public const int FIRST_CAPT = 1;
    public const int SECOND_CAPT = 2;
    public const int WARLORD = 3;


    public int burnDamage = 2;
    public float burnDuration = 3;
    public float burnTick = 0.5f;

    public int stunDuration = 2;
    public DamageNumbersPro.DamageNumber stunPopup;

    private void Start()
    {
        EventManager.INSTANCE.OnCaptainKill += OnCaptainKill;
        EventManager.INSTANCE.OnCaptainSpare += OnCaptainSpare;
        EventManager.INSTANCE.OnPlayerAttackEnemy += OnPlayerAttackEnemy;
        playerAnim = GetComponent<Animator>();
        pac = GetComponent<PlayerAttackController>();
        cs = GetComponent<CharacterStats>();

        pac.OnParry += OnParry;
        pac.OnChargeAttack += OnChargeAttack;
        pac.OnQuickAttack += OnQuickAttack;

        for (int i = 0; i < abilities.Length; ++i) CallOnAbility(i, abilities[i]);
    }

    void OnPlayerAttackEnemy(bool isQuickAttack, Transform enemy)
    {
        if (!isQuickAttack) return;
        Debug.Log("Found quick attack!");
        if (abilities[FIRST_CAPT] == UnlockState.SPARE)
        {
            cs.addHealth((int) (0.10f * (cs.maxHealth - cs.currentHealth)));
        }
        else if (abilities[FIRST_CAPT] == UnlockState.KILL)
        {
            StartCoroutine(BurnEnemy(enemy));
        }
    }

    IEnumerator BurnEnemy(Transform enemy)
    {
        var burnUntil = Time.time + burnDuration;
        while (Time.time < burnUntil && enemy != null)
        {
            CharacterStats cs = enemy.GetComponent<CharacterStats>();
            cs.damage(burnDamage, null);
            yield return new WaitForSeconds(burnTick);
        }
    }

    void OnChargeAttack()
    {
        if (abilities[TUT_CAPT] == UnlockState.KILL)
        {
            pac.pwb.DamageEnemies(10, 5f, transform);
        }
        if (abilities[SECOND_CAPT] == UnlockState.SPARE)
        {
            pac.pwb.DamageEnemies(pac.mitigatedDamage, 5f, transform);
        }
    }

    void OnQuickAttack(int quickAttackPhase)
    {
        if (quickAttackPhase != 2) return;
        if (abilities[WARLORD] == UnlockState.SPARE)
        {
            foreach (Collider other in Physics.OverlapSphere(transform.position, 5f))
            {
                if (!other.CompareTag("Enemy")) continue;
                var facing = (other.transform.position - transform.position);
                facing.y = 0;

                other.transform.position += facing * 1;

                StartCoroutine(StunEnemy(other.transform));
            }
        }
        else if (abilities[WARLORD] == UnlockState.KILL)
        {
            foreach (Collider other in Physics.OverlapSphere(transform.position, 5f))
            {
                if (!other.CompareTag("Enemy")) continue;
                var facing = (other.transform.position - transform.position);
                facing.y = 0;

                other.transform.position += facing * 1;

                StartCoroutine(BurnEnemy(other.transform));
            }
        }
    }

    void OnParry(Transform parried)
    {
        // if we spared the tut captain, then on successful parry add a shield
        if (abilities[TUT_CAPT] == UnlockState.SPARE)
        {
            cs.addShield(10);
        }

        if (abilities[SECOND_CAPT] == UnlockState.KILL)
        {
            Debug.Log("Stunned enemy!");
            StartCoroutine(StunEnemy(parried));
        }
    }

    IEnumerator StunEnemy(Transform enemy)
    {
        var stunUntil = Time.time + stunDuration;
        EnemyMotor motor = enemy.GetComponent<EnemyMotor>();
        while (Time.time < stunUntil && enemy != null)
        {
            motor.enabled = false;
            stunPopup.Spawn(enemy.position);
            yield return new WaitForSeconds(0.5f);
        }
        if (enemy != null) motor.enabled = true;
    }

    void CallOnAbility(int id, UnlockState state)
    {
        if (state == UnlockState.LOCKED) return;
        switch (id)
        {
            case TUT_CAPT:
                OnTutUnlock(state);
                break;
            case FIRST_CAPT:
                OnFirstCaptainUnlock(state);
                break;
            case SECOND_CAPT:
                OnSecondCaptainUnlock(state);
                break;
            case WARLORD:
                OnWarlordUnlock(state);
                break;
        }
    }

    void OnTutUnlock(UnlockState state)
    {
        if (state == UnlockState.SPARE)
        {

        }
        else
        {
            playerAnim.SetFloat("ChargeMultiplier", 2f);
        }

        cs.addMaxHealth(50);
    }

    void OnFirstCaptainUnlock (UnlockState state)
    {
        if (state == UnlockState.SPARE)
        {

        }
        else
        {

        }
        cs.addMaxHealth(50);
    }

    void OnSecondCaptainUnlock(UnlockState state)
    {
        if (state == UnlockState.SPARE)
        {

        }
        else
        {

        }
        cs.addMaxHealth(50);
    }

    void OnWarlordUnlock(UnlockState state)
    {
        if (state == UnlockState.SPARE)
        {

        }
        else
        {

        }
        cs.addMaxHealth(50);
    }

    void OnCaptainKill(CaptainController c)
    {
        if (abilities[c.CaptainID] != UnlockState.LOCKED) return;
        abilities[c.CaptainID] = UnlockState.KILL;
        // FeedbackStatusBehavior.INSTANCE.SetFeedback("You feel a bit unneasy...");
        CallOnAbility(c.CaptainID, UnlockState.KILL);
    }

    void OnCaptainSpare(CaptainController c)
    {
        if (abilities[c.CaptainID] != UnlockState.LOCKED) return;
        abilities[c.CaptainID] = UnlockState.SPARE;
        // FeedbackStatusBehavior.INSTANCE.SetFeedback("You feel determined!");
        CallOnAbility(c.CaptainID, UnlockState.SPARE);
    }

}
