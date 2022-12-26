using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponBehavior : MonoBehaviour
{
    
    private EnemyController ec;
    public float radius = 2f;
    public AudioClip[] hit;
    public AudioClip[] blockHit;

    public void Start()
    {
        ec = GetComponentInParent<EnemyController>();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void DamagePlayer(int damage)
    {

        foreach(Collider other in Physics.OverlapSphere(transform.position, radius))
        {
            if (!other.CompareTag("Player")) continue;

            PlayerAttackController pac = other.GetComponent<PlayerAttackController>();
            if (!pac.block)
            {
                CharacterStats enemy = other.GetComponent<CharacterStats>();
                enemy.damage(10, this.gameObject);
                AudioSource.PlayClipAtPoint(hit[Random.Range(0, hit.Length)], this.transform.position);
            }
            else
            {
                ec.anim.setTrigger("TakeDamage");
                pac.TriggerOnParry(ec.gameObject.transform);
                AudioSource.PlayClipAtPoint(blockHit[Random.Range(0, blockHit.Length)], this.transform.position);
            }
            break;
        }
    }
}
