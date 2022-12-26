using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PS
{
    [RequireComponent(typeof(EnemyLocoMotion))]
    public class EnemyManager : CharacterManager
    {
        public State currentState;
        public Rigidbody rb;

        [Header("AI Settings")]
        public CharacterManager currentTarget;
        public LayerMask detectionLayer;
        public bool isPerformingAction;
        public float agroRadius = 20;
        public float detectionRadius = 5f;
        public float chaseSpeed = 2f;
        public float minimumDetectionAngle = -50;
        public float maximumDetectionAngle = 50;
        public NavMeshAgent navMeshAgent;
        public float rotationSpeed = 5f;
        public bool allowToDoCombos = true;
        public float comboLikelihood = 75f;

        public float recoveryTimer = 0f;

        protected EnemyLocoMotion enemyLocoMotion;

        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            animatorHandler = GetComponentInChildren<EnemyAnimatorHandler>();
            enemyLocoMotion = GetComponent<EnemyLocoMotion>();
            characterStats = GetComponent<EnemyStats>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            navMeshAgent.enabled = false;


            // set character stats listeners
            characterStats.onDamage += onDamage;
            characterStats.onDeath += onDeath;
        }

        protected virtual void onDamage(int prev, int curr, int max, GameObject by)
        {
            if (by != null) animatorHandler.PlayTargetAnimation("Take_Damage_1", true);
            if (curr > 0) currentState = this.GetComponentInChildren<CombatStanceState>();
        }

        protected virtual void onDeath()
        {
            // turn off lock on if it is on
            if (CameraHandler.INSTANCE.lockOn) CameraHandler.INSTANCE.lockOn = false;
            animatorHandler.PlayTargetAnimation("Death", true);
            this.currentState = null;
            this.SetCollidersState(false);
        }

        public void SetCollidersState(bool state)
        {
            foreach (Collider col in this.GetComponentsInChildren<Collider>())
            {
                if (LayerMask.LayerToName(col.gameObject.layer) != "CharacterCollisionBlocker") continue;
                col.enabled = state;
            }
        }

        protected override void Update()
        {
            if (characterStats.isDead) return;
            base.Update();
            HandleRecoveryTimer();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            HandleState();
        }

        public void HandleState()
        {
            if (currentState != null) currentState = currentState.Tick(this, characterStats as EnemyStats, animatorHandler as EnemyAnimatorHandler);
        }
        private void HandleRecoveryTimer()
        {
            if (recoveryTimer > 0) recoveryTimer -= Time.deltaTime;
            else if (isPerformingAction) isPerformingAction = false;
        }
    }
}
