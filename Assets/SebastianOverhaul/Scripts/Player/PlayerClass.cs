using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
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
        private CharacterStats cs;
        private PlayerManager pm;
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
            pm = GetComponent<PlayerManager>();
            cs = GetComponent<PlayerStats>();
            SpareKillUIManager.INSTANCE.OnCaptainKill += OnCaptainKill;
            SpareKillUIManager.INSTANCE.OnCaptainSpare += OnCaptainSpare;
            pm.OnDamage += OnDamage;
            pm.OnParry += OnParry;
            pm.OnQuickAttack3 += OnQuickAttack3;
            pm.OnChargeAttack += OnChargeAttack;
            for (int i = 0; i < abilities.Length; ++i) CallOnAbility(i, abilities[i]);
        }

        
        void OnDamage(CharacterManager enemy)
        {
            if (abilities[FIRST_CAPT] == UnlockState.SPARE)
            {
                cs.addHealth((int)(0.10f * (cs.maxHealth - cs.currentHealth)));
            }
            else if (abilities[FIRST_CAPT] == UnlockState.KILL)
            {
                StartCoroutine(BurnEnemy(enemy));
            }
        }

        void OnChargeAttack()
        {
            if (abilities[TUT_CAPT] == UnlockState.KILL)
            {
                // damage by 10
                DamageEnemies(10, 5f, transform);
            }
            if (abilities[SECOND_CAPT] == UnlockState.SPARE)
            {
                // mitigated damage
                Debug.Log("Released " + pm.mitigatedDamage);
                DamageEnemies(pm.mitigatedDamage, 5f, transform);
                pm.mitigatedDamage = 0;
            }
        }

        public void DamageEnemies(int damage, float radius, Transform at)
        {
            foreach (Collider other in Physics.OverlapSphere(at.position, radius))
            {
                EnemyStats es = other.GetComponent<EnemyStats>();
                if (!es) continue;

                es.damage(damage, this.gameObject);
            }
        }

        void OnQuickAttack3()
        {
            if (abilities[WARLORD] == UnlockState.SPARE)
            {
                foreach (Collider other in Physics.OverlapSphere(transform.position, 5f))
                {
                    CharacterManager cm = other.GetComponent<EnemyManager>();
                    if (!cm || cm.characterStats.isDead) continue;

                    // other.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(other.transform.position - transform.position) * 5f, ForceMode.Impulse);

                    StartCoroutine(StunEnemy(cm));
                }
            }
            else if (abilities[WARLORD] == UnlockState.KILL)
            {
                foreach (Collider other in Physics.OverlapSphere(transform.position, 5f))
                {
                    CharacterManager cm = other.GetComponent<EnemyManager>();
                    if (!cm || cm.characterStats.isDead) continue;

                    // other.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(other.transform.position - transform.position) * 5f, ForceMode.Impulse);

                    StartCoroutine(BurnEnemy(cm));
                }
            }
        }

        void OnParry(CharacterManager parried)
        {
            if (abilities[SECOND_CAPT] == UnlockState.KILL)
            {
                StartCoroutine(StunEnemy(parried));
            }
        }

        IEnumerator BurnEnemy(CharacterManager enemy)
        {
            var burnUntil = Time.time + burnDuration;
            while (Time.time < burnUntil && !enemy.characterStats.isDead)
            {
                CharacterStats cs = enemy.GetComponent<CharacterStats>();
                cs.damage(burnDamage, null);
                yield return new WaitForSeconds(burnTick);
            }
        }

        IEnumerator StunEnemy(CharacterManager enemy)
        {
            var stunUntil = Time.time + stunDuration;
            enemy.animatorHandler.PlayTargetAnimation("Parried", true);
            enemy.animatorHandler.anim.SetFloat("stunMultiplier", 0.2f);
            while (Time.time < stunUntil && !enemy.characterStats.isDead)
            {
                stunPopup.Spawn(enemy.transform.position);
                yield return new WaitForSeconds(0.5f);
            }

            enemy.animatorHandler.anim.SetFloat("stunMultiplier", 1f);
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

                FeedbackStatusBehavior.INSTANCE.SetFeedback("When blocking attacks (Hold Right), gain a shield!");
            }
            else
            {

                FeedbackStatusBehavior.INSTANCE.SetFeedback("Your Charge Attack (Hold Left) now deals damage around you!");
            }

            cs.addMaxHealth(10);
        }

        void OnFirstCaptainUnlock(UnlockState state)
        {
            if (state == UnlockState.SPARE)
            {

                FeedbackStatusBehavior.INSTANCE.SetFeedback("Your Quick Attack (Left Click) now gives lifesteal!");
            }
            else
            {

                FeedbackStatusBehavior.INSTANCE.SetFeedback("Your Quick Attack (Left Click) now has burn damage!");
            }
            cs.addMaxHealth(10);
        }

        void OnSecondCaptainUnlock(UnlockState state)
        {
            if (state == UnlockState.SPARE)
            {

                FeedbackStatusBehavior.INSTANCE.SetFeedback("Your Charge Attack (Right Click) now absorbs your damage and deals back to the enemies!");
            }
            else
            {

                FeedbackStatusBehavior.INSTANCE.SetFeedback("Your Parry (Right Click) now stuns the enemy longer!");
            }
            cs.addMaxHealth(25);
        }

        void OnWarlordUnlock(UnlockState state)
        {
            if (state == UnlockState.SPARE)
            {

                FeedbackStatusBehavior.INSTANCE.SetFeedback("Every Third Quick Attack (Right Click) now stuns enemies in an area!");
            }
            else
            {

                FeedbackStatusBehavior.INSTANCE.SetFeedback("Every Third Quick Attack (Right Click) now has burn damage!");
            }
            cs.addMaxHealth(25);
        }

        private void OnCaptainKill(CaptainManager c)
        {
            if (abilities[c.CaptainID] != UnlockState.LOCKED) return;
            abilities[c.CaptainID] = UnlockState.KILL;
            CallOnAbility(c.CaptainID, UnlockState.KILL);
        }

        private void OnCaptainSpare(CaptainManager c)
        {
            if (abilities[c.CaptainID] != UnlockState.LOCKED) return;
            abilities[c.CaptainID] = UnlockState.SPARE;
            CallOnAbility(c.CaptainID, UnlockState.SPARE);
        }
    }
}
