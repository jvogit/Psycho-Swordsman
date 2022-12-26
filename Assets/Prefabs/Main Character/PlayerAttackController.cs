using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerAttackController : MonoBehaviour
{
    public float chargeThreshold = 1f;
    public bool block { get; private set; } = false;
    public PlayerInput playerInput;
    public int quickAttackDmg { get; set; } = 10;
    public int chargeAttackDmg { get; set; } = 25;

    public bool isAttacking { get; private set; } = false;
    private int quickAttackPhase = 0;

    public bool isChargeAttacking { get; private set; } = false;
    private bool isChargingAttack = false;
    private CaptainController captain;

    private InputAction attackAction;
    private InputAction blockAction;
    private InputAction moveAction;
    private float attackTime = 0;
    private Animator anim;
    private CharacterStats cs;
    private PlayerClass pc;
    public PlayerWeaponBehavior pwb { get; private set; }

    public event System.Action<Transform> OnParry;
    public event System.Action<int> OnQuickAttack;
    public event System.Action OnChargeAttack;

    public int mitigatedDamage { get; set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        cs = GetComponent<CharacterStats>();
        pc = GetComponent<PlayerClass>();
        pwb = GetComponentInChildren<PlayerWeaponBehavior>();
        attackAction = playerInput.actions["Attack"];
        blockAction = playerInput.actions["Block"];
        moveAction = playerInput.actions["Move"];
        attackAction.started += AttackStarted;
        attackAction.canceled += AttackCancelled;
        blockAction.started += BlockStarted;
        blockAction.canceled += BlockCancelled;
        cs.onShieldChange += onShieldChange;
        cs.onHealthChanged += onHealthChanged;
        cs.onDeath += onDeath;

        EventManager.INSTANCE.OnCaptainDefeat += OnCaptainDefeat;
    }

    private void OnDestroy()
    {
        attackAction.started -= AttackStarted;
        attackAction.canceled -= AttackCancelled;
        blockAction.started -= BlockStarted;
        blockAction.canceled -= BlockCancelled;
        cs.onHealthChanged -= onHealthChanged;
    }

    // character stats events
    void onDeath()
    {
        SceneManager.LoadScene(0);
    }

    void onShieldChange(int previous, int shieldHealth, int maxShield, GameObject by)
    {
        onHealthChanged(previous, shieldHealth, maxShield, by);
    }

    void onHealthChanged(int previous, int current, int max, GameObject by)
    {
        Reset();
        if (current >= previous) return;
        var hasSparedSecondCaptain = pc.abilities[PlayerClass.SECOND_CAPT] == PlayerClass.UnlockState.SPARE;
        if (isChargingAttack && hasSparedSecondCaptain)
        {
            // Debug.Log("Mitigated " + 0.5f * (previous - current));
            this.mitigatedDamage = (int) Mathf.Min(this.mitigatedDamage + 0.5f * (previous - current), 50);
        }
        else if (by != null)
        {
            moveAction.Disable();
            anim.SetTrigger("TakeDamage");
        }
    }

    // pac events trigger
    // trigger the OnParry event on successful parry
    public void TriggerOnParry(Transform parried)
    {
        if (this.OnParry != null) this.OnParry(parried); 
    }

    public void TriggerOnChargeAttack()
    {
        if (this.OnChargeAttack != null) this.OnChargeAttack();
    }


    // input system unity events
    public void BlockStarted(InputAction.CallbackContext ctx)
    {
        Reset();
        block = true;
        anim.SetBool("Block", block);
        moveAction.Disable();
    }

    public void BlockCancelled(InputAction.CallbackContext ctx)
    {
        Reset();
        block = false;
        anim.SetBool("Block", block);
        moveAction.Enable();
    }

    public void AttackStarted(InputAction.CallbackContext ctx)
    {
        if (block) return;
        attackTime = Time.time;
        moveAction.Disable();
    }


    public void AttackCancelled(InputAction.CallbackContext ctx)
    {
        if (block) return;
        if (isChargingAttack)
        {
            isChargingAttack = false;
            anim.SetBool("ChargeAttack", false);
        }
        else
        {
            anim.SetTrigger("Attack" + quickAttackPhase);
        }
    }

    public void Reset()
    {
        isAttacking = false;
        isChargeAttacking = false;
    }

    void Update()
    {
        if (attackAction.IsPressed() && (Time.time - attackTime) >= chargeThreshold && !isChargingAttack && !block)
        {
            isChargingAttack = true;
            mitigatedDamage = 0;
            anim.SetBool("ChargeAttack", true);
        }
    }

    public Quaternion getAutoTargetRotation()
    {
        Transform closest = getClosestEnemy();
        if (closest)
        {
            return Quaternion.LookRotation(closest.position - transform.position);
        }

        return transform.rotation;
    }

    public Transform getClosestEnemy()
    {
        Transform closest = null;
        foreach (Collider c in Physics.OverlapSphere(transform.position, 2f))
        {
            if (c.CompareTag("Enemy"))
            {
                if (!closest || Vector3.Distance(transform.position, c.transform.position) <= Vector3.Distance(transform.position, closest.position))
                {
                    closest = c.transform;
                }
            }
        }

        return closest;
    }

    // captain events
    public void OnCaptainDefeat(CaptainController c)
    {
        this.gameObject.transform.position = c.playerPosCutscene.position;
        this.gameObject.transform.rotation = c.playerPosCutscene.rotation;
        this.playerInput.DeactivateInput();
        this.captain = c;
    }

    // to be triggered by UI button
    public void KillCaptain()
    {
        this.anim.SetTrigger("Attack0");
        this.playerInput.ActivateInput();
        this.captain.CaptainKill();
        this.captain = null;
    }

    public void SpareCaptain()
    {
        this.playerInput.ActivateInput();
        this.captain.CaptainSpare();
        this.captain = null;
    }

    // animation
    // attack damage frames
    public void OnAttackEnter_anim()
    {
        isAttacking = true;

        var target = getClosestEnemy();
        transform.rotation = getAutoTargetRotation();
        this.OnQuickAttack(this.quickAttackPhase);
        if (target) pwb.DamageOnly(getClosestEnemy().gameObject, quickAttackDmg);
    }

    public void OnAttackExit_anim()
    {
        Reset();
        moveAction.Enable();
        this.quickAttackPhase = (this.quickAttackPhase + 1) % 3;
    }

    public void OnChargeAttackEnter_anim()
    {
        isChargeAttacking = true;

        this.TriggerOnChargeAttack();
        var target = getClosestEnemy();
        transform.rotation = getAutoTargetRotation();
        if (target) pwb.DamageOnly(getClosestEnemy().gameObject, chargeAttackDmg);
    }

    public void OnChargeAttackExit_anim()
    {
        Reset();
        moveAction.Enable();
    }
    
    // on take damage
    public void OnDamageExit_anim()
    {
        moveAction.Enable();
    }
}
