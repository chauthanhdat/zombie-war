using UnityEngine;
using UnityEngine.AI;
using ZombieWar.Utilities;
using ZombieWar.Data;
using ZombieWar.Gameplay.Combat;

namespace ZombieWar.Gameplay.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(IDamageable))]
    public class ZombieAI : StateMachine
    {
        public Transform player;
        
        [Header("AI Settings")]
        public float detectionRange = 10f;
        public float attackRange = 2f;
        public LayerMask playerLayer = 1;
        
        private NavMeshAgent agent;
        private CharacterHealth health;
        
        // States
        private IdleState idleState;
        private ChaseState chaseState;
        private AttackState attackState;
        private DeadState deadState;
        
        public Transform Player => player;
        public NavMeshAgent Agent => agent;
        public CharacterHealth Health => health;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<CharacterHealth>();
            
            InitializeStates();
            InitializeAgent();
        }
        
        private void Start()
        {
            ChangeState(idleState);
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
        
        public bool CanSeePlayer()
        {
            if (player == null) return false;
            
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            return distanceToPlayer <= detectionRange;
        }
        
        public bool IsInAttackRange()
        {
            if (player == null) return false;
            
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            return distanceToPlayer <= attackRange;
        }
        
        public void OnDeath()
        {
            ChangeState(deadState);
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
            if (zombie.Health.IsDead)
            {
                zombie.OnDeath();
                return;
            }
            
            if (zombie.CanSeePlayer())
            {
                zombie.ChangeState(new ChaseState(zombie));
            }
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
            if (zombie.Health.IsDead)
            {
                zombie.OnDeath();
                return;
            }
            
            if (!zombie.CanSeePlayer())
            {
                zombie.ChangeState(new IdleState(zombie));
                return;
            }
            
            if (zombie.IsInAttackRange())
            {
                zombie.ChangeState(new AttackState(zombie));
                return;
            }
            
            zombie.Agent.SetDestination(zombie.Player.position);
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
            if (zombie.Health.IsDead)
            {
                zombie.OnDeath();
                return;
            }
            
            if (!zombie.IsInAttackRange())
            {
                zombie.ChangeState(new ChaseState(zombie));
                return;
            }
            
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
            var playerHealth = zombie.Player.GetComponent<IDamageable>();
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