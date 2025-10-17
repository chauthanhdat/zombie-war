using UnityEngine;
using ZombieWar.Data;
using ZombieWar.Gameplay.Weapons;

namespace ZombieWar.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController), typeof(WeaponController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Input")]
        public FloatingJoystick joystick;
        public KeyCode runKey = KeyCode.LeftShift;

        [Header("Movement Settings")]
        public float runSpeedMultiplier = 1.5f;
        
        [Header("Gravity Settings")]
        public float gravity = -9.81f;
        public float groundedGravity = -0.05f;
        
        [Header("Components")]
        public Animator animator;
        
        [Header("Weapon System")]
        public WeaponController weaponController;
        
        private CharacterController characterController;
        private Vector3 moveDirection;
        private Vector3 velocity;
        private bool isRunning;
        
        // Properties
        public bool IsMoving => moveDirection.magnitude > 0.1f;
        public bool IsRunning => isRunning;
        public float CurrentSpeed => isRunning ? MoveSpeed * runSpeedMultiplier : MoveSpeed;
        public bool IsGrounded => characterController.isGrounded;
        public WeaponController WeaponController => weaponController;
        
        private float MoveSpeed => 5f;

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
            HandleGravity();
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
        
        private void HandleGravity()
        {
            if (IsGrounded)
            {
                // Apply small downward force to keep player grounded
                velocity.y = groundedGravity;
            }
            else
            {
                // Apply gravity when not grounded (falling)
                velocity.y += gravity * Time.deltaTime;
            }
            
            // Clamp falling velocity to prevent excessive speed
            velocity.y = Mathf.Max(velocity.y, gravity * 2f);
        }
        
        private void HandleMovement()
        {
            // Horizontal movement
            Vector3 move = moveDirection * CurrentSpeed;
            
            // Combine horizontal movement with vertical velocity (gravity)
            Vector3 finalMovement = new Vector3(move.x, velocity.y, move.z);
            
            characterController.Move(finalMovement * Time.deltaTime);

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
                    float rotationSpeed = 180f;

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
        
        /// <summary>
        /// Reset vertical velocity (useful for teleporting or respawning)
        /// </summary>
        public void ResetVerticalVelocity()
        {
            velocity.y = 0f;
        }
        
        /// <summary>
        /// Apply an upward force (for jumping or knockback effects)
        /// </summary>
        public void ApplyVerticalForce(float force)
        {
            velocity.y = force;
        }
        
        /// <summary>
        /// Get current vertical velocity
        /// </summary>
        public float GetVerticalVelocity()
        {
            return velocity.y;
        }
    }
}