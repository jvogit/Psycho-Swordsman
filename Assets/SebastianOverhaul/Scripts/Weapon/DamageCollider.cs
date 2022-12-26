using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class DamageCollider : MonoBehaviour
    {
        public int damage = 10;
        private Collider damageCollider;
        public LayerMask collideWith;
        [HideInInspector]
        public CharacterManager owner;

        [Header("Audio")]
        public AudioClip[] swordhit;
        public AudioClip swordHitShield;
        public AudioClip parried;
        public DamageNumbersPro.DamageNumber damagePopup;

        [Header("Particle")]
        public WeaponParticle weaponParticle;
        public Transform playParticleAt;

        public System.Action<CharacterManager> onDamage;

        // Start is called before the first frame update
        void Start()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
            CheckForOwner();
        }

        private void CheckForOwner()
        {
            CharacterManager cs = GetComponentInParent<CharacterManager>();
            if (cs) owner = cs;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Contains(collideWith, other.gameObject.layer)) return;
            var cs = other.GetComponent<CharacterStats>();
            var cm = other.GetComponent<CharacterManager>();
            if (cs && cm)
            {
                if (cm.isParrying)
                {
                    if (cm.OnParry != null) cm.OnParry(owner);
                    owner.animatorHandler.PlayTargetAnimation("Parried", true);
                    AudioSource.PlayClipAtPoint(parried, transform.position);
                }
                else if (!cs.isDead)
                {
                    PlayParticle(weaponParticle.attackParticle);
                    var dmg = owner && owner.isChargeAttacking ? damage + 15 : damage;

                    // cheat code for player
                    if (owner && owner is PlayerManager && CheatCode.INSTANCE.unlocked)
                    {
                        dmg = 1_000_000;
                    }

                    if (cm && cm is PlayerManager && CheatCode.INSTANCE.unlocked)
                    {
                        dmg = 1;
                    }

                    cs.damage(dmg, owner.gameObject);

                    if (cm.isInvulnerable)
                    {
                        AudioSource.PlayClipAtPoint(swordHitShield, transform.position);
                    }
                    else
                    {
                        if (onDamage != null) onDamage(cm);
                        AudioSource.PlayClipAtPoint(swordhit[Random.Range(0, swordhit.Length)], transform.position);
                        damagePopup.Spawn(cm.transform.position, dmg);
                    }
                }
            }
            else if (cs)
            {
                cs.damage(10, owner.gameObject);
                AudioSource.PlayClipAtPoint(swordhit[Random.Range(0, swordhit.Length)], transform.position);
                PlayParticle(weaponParticle.attackParticle);
            }
        }

        private void PlayParticle(GameObject particle)
        {
            var model = Instantiate(particle);
            model.transform.parent = playParticleAt;

            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }

        private bool Contains(LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
    }
}
