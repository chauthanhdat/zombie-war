using UnityEngine;

namespace ZombieWar.Utilities
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected State currentState;
        
        public virtual void ChangeState(State newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }
            
            currentState = newState;
            
            if (currentState != null)
            {
                currentState.Enter();
            }
        }
        
        private void Update()
        {
            currentState?.Update();
        }
        
        private void FixedUpdate()
        {
            currentState?.FixedUpdate();
        }
    }
    
    public abstract class State
    {
        protected StateMachine stateMachine;
        
        protected State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
        
        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
    }
}