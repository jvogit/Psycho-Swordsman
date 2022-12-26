using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponBehavior : MonoBehaviour
{
    
    public PlayerAttackController pac;
    public float radius = 2f;
    public AudioClip[] hit;
    public AudioClip[] blockHit;

    public void DamageOnly(GameObject other, int damage, bool isQuickAttack = true)
    {
        CharacterStats enemyCS = other.GetComponent<CharacterStats>();
        EnemyController ec = other.GetComponent<EnemyController>();

        if (!ec.block)
        {
            enemyCS.damage(damage, this.gameObject);
            EventManager.INSTANCE.TriggerOnPlayerAttackEnemy(isQuickAttack, other.transform);
            AudioSource.PlayClipAtPoint(hit[Random.Range(0, hit.Length)], this.transform.position);
        }
        else
        {
            pac.GetComponent<Animator>().SetTrigger("TakeDamage");
            ec.OnParry(pac.gameObject.transform);
            AudioSource.PlayClipAtPoint(blockHit[Random.Range(0, blockHit.Length)], this.transform.position);
        }
    }

    public void DamageEnemies(int damage, float radius, Transform at)
    {
        foreach (Collider other in Physics.OverlapSphere(at.position, radius))
        {
            if (!other.CompareTag("Enemy")) continue;

            DamageOnly(other.gameObject, damage, false);
        }
    }
}
