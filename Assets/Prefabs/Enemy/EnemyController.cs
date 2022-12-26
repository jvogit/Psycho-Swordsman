using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour
{
    // enemy controller params
    public float patrolRadius = 15f;
    public float agroRadius = 20f;
    public float startAttackThreshold = 20f;
    public float swingTime = 1.5f;

    // popups
    public DamageNumbersPro.DamageNumber dmgPopup;

    // protected fields and states
    public EnemyAnimator anim { get; private set; }
    protected EnemyMotor motor;
    protected EnemyWeaponBehavior ewb;

    protected bool isAgro = false;
    protected bool isPatrolling = false;
    protected bool isTakingDamage = false;
    public bool isAttacking { get; protected set; } = false;
    public bool isChargeAttacking { get; protected set; } = false;
    protected CharacterStats cs;
    protected float lastSwing = 0f;
    public bool block { get; protected set; } = false;

    private float lastDodge = 0f;

    public void SetTarget(GameObject obj)
    {
        isAgro = true;
        motor.setTarget(obj.transform);
    }

    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<EnemyAnimator>();
        motor = GetComponent<EnemyMotor>();
        cs = GetComponent<CharacterStats>();
        ewb = GetComponentInChildren<EnemyWeaponBehavior>();
        GetComponent<CharacterStats>().onHealthChanged += onHealthChanged;
        GetComponent<CharacterStats>().onDeath += onDeath;
        StartCoroutine(patrolRoutine());
    }

    public virtual void OnParry(Transform player)
    {
        Debug.Log("Parried");
    }

    protected virtual void OnAttackEnter_anim()
    {
        isAttacking = true;
        ewb.DamagePlayer(10);
        lastDodge = Time.time;
        motor.move(motor.transform.position + 5 * motor.transform.forward);
    }

    protected virtual void OnAttackExit_anim()
    {
        isAttacking = false;
    }

    protected virtual void OnChargeAttackEnter_anim()
    {
        Reset();
        this.isChargeAttacking = true;
        ewb.DamagePlayer(20);
        lastDodge = Time.time;
    }

    protected virtual void OnChargeAttackExit_anim()
    {
        Reset();
    }

    // set null to ignore TakeDamage
    protected virtual void onHealthChanged(int previous, int current, int max, GameObject by)
    {
        Reset();
        if (current > 0 && by != null)
        {
            anim.setTrigger("TakeDamage");
            motor.stop();
            isTakingDamage = true;
        }
        if (by != null) motor.setTarget(by.transform);
        isAgro = true;
        dmgPopup.Spawn(transform.position, previous - current);
    }

    protected virtual void Reset()
    {
        Debug.Log("Reset!");
        block = false;
        isAttacking = false;
        isTakingDamage = false;
        isChargeAttacking = false;
    }

    protected virtual void onDeath()
    {
        anim.setTrigger("Death");
    }

    protected virtual void OnDamageExit_anim()
    {
        Reset();
        lastSwing = Time.time;
    }

    protected virtual void OnDeath_anim()
    {
        Destroy(this.gameObject);
    }

    protected void checkAgro()
    {
        foreach (Collider col in Physics.OverlapSphere(transform.position, agroRadius))
        {
            if (col.transform.CompareTag("Player"))
            {
                motor.setTarget(col.transform);
                isAgro = true;
                isPatrolling = false;
                return;
            }
        }

        /*motor.setTarget(null);
        isAgro = false;
        isPatrolling = true;*/

        /* RaycastHit hit;

        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), transform.forward, out hit,agroRadius) && hit.collider.CompareTag("Player"))
        {
            Debug.DrawRay(transform.position + (Vector3.up * 0.5f), transform.forward * agroRadius, Color.green, 1f);
            isAgro = true;
            isPatrolling = false;
            motor.setTarget(hit.collider.transform);
        }
        else
        {
            isAgro = false;
            isPatrolling = true;
        } */
    }

    protected bool InAttackRange()
    {
        return Vector3.Magnitude(transform.position - motor.getTarget().transform.position) <= startAttackThreshold;
    }

    protected bool ReadyAttack()
    {
        return InAttackRange() && (Time.time - lastSwing) >= swingTime;
    }

    protected virtual void StartAttack()
    {
        motor.stop();
        anim.setTrigger("Attack" + Random.Range(0, 3));
        lastSwing = Time.time;
    }

    protected virtual bool CanMove()
    {
        return !isAttacking && !isTakingDamage && !isChargeAttacking && anim.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Idle";
    }

    protected IEnumerator patrolRoutine()
    {
        while (true)
        {
            if (isPatrolling)
            {
                Vector3 offset = Random.insideUnitSphere * patrolRadius;
                offset.y = 0;
                offset += transform.position;
                motor.move(offset);
            }
            yield return new WaitForSeconds(Random.Range(5, 10));
        }
    }

    protected virtual void DoAI()
    {
        if (isAttacking || isChargeAttacking || isTakingDamage || !motor.enabled)
        {
            // Debug.Log("Enemy Controller stopped AI " + this.isAttacking + " " + this.isChargeAttacking + " " + this.isTakingDamage);
            return;
        }
        if (isAgro)
        {
            if (ReadyAttack())
            {
                StartAttack();
            }
            else if (!InAttackRange() && CanMove())
            {
                motor.move(motor.getTarget().position);
            }
        }
        else
        {
            checkAgro();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        DoAI();
    }
}
