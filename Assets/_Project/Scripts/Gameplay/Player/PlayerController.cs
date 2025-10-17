using UnityEngine;
using ZombieWar.Data;
using ZombieWar.Gameplay.Weapons;

namespace ZombieWar.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController), typeof(WeaponController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Configuration")]
        public CharacterStats playerStats;

        [Header("Input")]
        public FloatingJoystick joystick;
        public KeyCode runKey = KeyCode.LeftShift;

        [Header("Movement Settings")]
        public float runSpeedMultiplier = 1.5f;
        
        [Header("Components")]
        public Animator animator;
        
        [Header("Weapon System")]
        public WeaponController weaponController;
        
        private CharacterController characterController;
        private Vector3 moveDirection;
        private bool isRunning;
        
        // Properties
        public bool IsMoving => moveDirection.magnitude > 0.1f;
        public bool IsRunning => isRunning;
        public float CurrentSpeed => isRunning ? MoveSpeed * runSpeedMultiplier : MoveSpeed;
        public WeaponController WeaponController => weaponController;
        
        private float MoveSpeed => playerStats ? playerStats.moveSpeed : 5f;
        
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            
            if (weaponController == null)
            {
                weaponController = GetComponent<WeaponController>();
            }
        }
        
        private void Update()
        {
            HandleInput();
            HandleMovement();
            HandleRotation();
        }
        
        private void HandleInput()
        {
            float horizontal = joystick.Horizontal;
            float vertical = joystick.Vertical;

            moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
            isRunning = Input.GetKey(runKey) && IsMoving;
        }
        
        private void HandleMovement()
        {
            Vector3 move = moveDirection * CurrentSpeed;
            characterController.Move(move * Time.deltaTime);

            animator.SetFloat("Velocity", moveDirection.magnitude);
        }
        
        private void HandleRotation()
        {
            if (IsMoving)
            {
                Vector3 lookDirection = moveDirection;
                lookDirection.y = 0f;
                
                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    float rotationSpeed = playerStats ? playerStats.rotationSpeed : 180f;
                    
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
        
        public void SetMoveDirection(Vector3 direction)
        {
            moveDirection = direction.normalized;
        }
        
        /// <summary>
        /// Equip a weapon through the player controller
        /// </summary>
        public void EquipWeapon(WeaponData weapon)
        {
            if (weaponController != null)
            {
                weaponController.EquipWeapon(weapon);
            }
        }
        
        /// <summary>
        /// Get current weapon info for UI
        /// </summary>
        public WeaponData GetCurrentWeapon()
        {
            return weaponController?.CurrentWeapon;
        }
    }
}