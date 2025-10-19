using UnityEngine;
using UnityEngine.AI;
using ZombieWar.Utilities;
using ZombieWar.Data;
using ZombieWar.Gameplay.Combat;
using System.Collections;
using ZombieWar.Core.Events;

namespace ZombieWar.Gameplay.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(IDamageable))]
    public class ZombieAI : StateMachine
    {
        public Transform target;
        public Animator animator;

        public AudioClip hurtSound;
        public AudioClip deathSound;

        [Header("AI Settings")]
        public float attackRange = 2f;
        public float attackSpeed = 2f;
        public LayerMask playerLayer = 1;

        private NavMeshAgent agent;
        private EnemyHealth health;
        private CharacterHealth targetHealth;
        
        // States
        private IdleState idleState;
        private ChaseState chaseState;
        private AttackState attackState;
        private DeadState deadState;

        private float lastAttackTime;
        private float lastTimeHurt;
        
        public Transform Target => target;
        public NavMeshAgent Agent => agent;
        public CharacterHealth TargetHealth => targetHealth;
        public float AttackRange => attackRange;
        
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<EnemyHealth>();
            health.OnHealthChanged += OnHealthChanged;
            health.OnDeath += OnDeath;

            if (target != null)
            {
                targetHealth = target.GetComponent<CharacterHealth>();
            }
            
            // InitializeStates();
            // InitializeAgent();
        }

        private void Start()
        {
            ChangeState(idleState);
        }

        private void Update()
        {
            if (health.IsDead || target == null) return;

            if (IsInAttackRange())
            {
                agent.isStopped = true;

                if (Time.time >= lastAttackTime + attackSpeed)
                {
                    Debug.LogError("Start Attack");
                    animator.SetTrigger("Attack");
                    lastAttackTime = Time.time;
                    StartCoroutine(DelayAttack());
                }
            }
            else
            {
                if (Time.time >= lastTimeHurt + 1f)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                }
            }

            animator.SetFloat("Speed", agent.velocity.magnitude);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                targetHealth = target.GetComponent<CharacterHealth>();
            }
        }
        
        private void InitializeStates()
        {
            idleState = new IdleState(this);
            chaseState = new ChaseState(this);
            attackState = new AttackState(this);
            deadState = new DeadState(this);
        }
        
        private void InitializeAgent()
        {
            // if (zombieStats != null)
            // {
            //     agent.speed = zombieStats.moveSpeed;
            // }
        }

        public bool IsInAttackRange()
        {
            if (target == null) return false;

            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            return distanceToTarget <= attackRange;
        }

        public IEnumerator DelayAttack()
        {
            yield return new WaitForSeconds(1f);

            if (IsInAttackRange() && targetHealth != null)
            {
                Debug.LogError("Attack!");
                targetHealth.TakeDamage(10);
            }
        }
        
        public void OnHealthChanged(float currentHealth, float maxHealth)
        {
            agent.isStopped = true;
            lastTimeHurt = Time.time;

            // animator.SetTrigger("Hurt");
            animator.Play("Hurt", 1, 0f);
            AudioManager.Instance.PlaySFX(hurtSound);
        }
        
        public void OnDeath()
        {
            // animator.SetTrigger("Die");
            animator.Play("Die", 1, 0f);
            AudioManager.Instance.PlaySFX(deathSound);

            StopAllCoroutines();
            
            GameEvent.OnZombieKilled?.Invoke();
        }
    }
    
    // Zombie States
    public class IdleState : State
    {
        private ZombieAI zombie;
        
        public IdleState(StateMachine stateMachine) : base(stateMachine)
        {
            zombie = stateMachine as ZombieAI;
        }
        
        public override void Enter()
        {
            zombie.Agent.isStopped = true;
        }
        
        public override void Update()
        {
            // if (zombie.Health.IsDead)
            // {
            //     zombie.OnDeath();
            //     return;
            // }
        }
    }
    
    public class ChaseState : State
    {
        private ZombieAI zombie;
        
        public ChaseState(StateMachine stateMachine) : base(stateMachine)
        {
            zombie = stateMachine as ZombieAI;
        }
        
        public override void Enter()
        {
            zombie.Agent.isStopped = false;
        }
        
        public override void Update()
        {
            // if (zombie.Health.IsDead)
            // {
            //     zombie.OnDeath();
            //     return;
            // }
            
            if (zombie.IsInAttackRange())
            {
                zombie.ChangeState(new AttackState(zombie));
                return;
            }
            
            // zombie.Agent.SetDestination(zombie.Player.position);
        }
    }
    
    public class AttackState : State
    {
        private ZombieAI zombie;
        private float lastAttackTime;
        
        public AttackState(StateMachine stateMachine) : base(stateMachine)
        {
            zombie = stateMachine as ZombieAI;
        }
        
        public override void Enter()
        {
            zombie.Agent.isStopped = true;
        }
        
        public override void Update()
        {
            // if (zombie.Health.IsDead)
            // {
            //     zombie.OnDeath();
            //     return;
            // }
            
            // if (!zombie.IsInAttackRange())
            // {
            //     zombie.ChangeState(new ChaseState(zombie));
            //     return;
            // }
            
            // Attack logic
            // float attackCooldown = zombie.zombieStats ? zombie.zombieStats.attackCooldown : 1f;
            // if (Time.time - lastAttackTime >= attackCooldown)
            // {
            //     PerformAttack();
            //     lastAttackTime = Time.time;
            // }
        }
        
        private void PerformAttack()
        {
            // Implement attack logic here
            Debug.Log($"{zombie.name} attacks player!");
            
            // Try to damage player
            // var playerHealth = zombie.Player.GetComponent<IDamageable>();
            // if (playerHealth != null && zombie.zombieStats != null)
            // {
            //     playerHealth.TakeDamage(zombie.zombieStats.attackDamage);
            // }
        }
    }
    
    public class DeadState : State
    {
        private ZombieAI zombie;
        
        public DeadState(StateMachine stateMachine) : base(stateMachine)
        {
            zombie = stateMachine as ZombieAI;
        }
        
        public override void Enter()
        {
            zombie.Agent.isStopped = true;
            zombie.Agent.enabled = false;
            
            // Disable colliders, play death animation, etc.
            Debug.Log($"{zombie.name} is dead");
        }
    }
}