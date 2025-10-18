using UnityEngine;
using ZombieWar.Data;
using ZombieWar.Core.Events;
using System.Collections;
using ZombieWar.Gameplay.Player;

namespace ZombieWar.Gameplay.Weapons
{
    [RequireComponent(typeof(TargetingSystem))]
    public class WeaponController : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private Animator animator;

        [Header("Weapon Slots")]
        [SerializeField] private WeaponData currentWeapon;
        [SerializeField] private WeaponData secondaryWeapon;
        
        [Header("Transform References")]
        public Transform firePoint;
        public Transform weaponModel;
        
        [Header("Weapon States")]
        [SerializeField] private bool isReloading;
        [SerializeField] private bool canFire = true;
        [SerializeField] private bool isMeleeAttacking;
        
        private float lastFireTime;
        private float lastMeleeTime;
        private Camera playerCamera;
        private TargetingSystem targetingSystem;

        // Properties
        public WeaponData CurrentWeapon => currentWeapon;
        public WeaponData SecondaryWeapon => secondaryWeapon;
        public bool IsReloading => isReloading;
        public bool CanFire => canFire && !isReloading && !isMeleeAttacking;
        public bool CanMelee => !isMeleeAttacking && !isReloading && currentWeapon != null && currentWeapon.weaponType == WeaponType.Melee;
        public WeaponType CurrentWeaponType => currentWeapon.weaponType;

        private void Start()
        {
            InitializeWeapons();

            playerCamera = Camera.main;
            targetingSystem = GetComponent<TargetingSystem>();

            UpdateWeaponDisplay();
        }

        private void OnEnable()
        {
            GameEvent.OnSwapWeaponRequested += SwapWeapons;
            GameEvent.OnAttack += Attack;
            // GameEvent.OnDeath +=  
        }

        private void OnDisable()
        {
            GameEvent.OnSwapWeaponRequested -= SwapWeapons;
            GameEvent.OnAttack -= Attack;
        }

        private void Update()
        {
            HandleInput();
        }
        
        private void InitializeWeapons()
        {
            // Start with the primary weapon if available
        }
        
        private void HandleInput()
        {
        }

        #region Weapon Switching

        private void SwapWeapons()
        {
            (secondaryWeapon, currentWeapon) = (currentWeapon, secondaryWeapon);
            UpdateWeaponDisplay();

            AudioManager.Instance.PlaySFX(currentWeapon.reloadSound);

            GameEvent.OnWeaponSwitched?.Invoke();
        }
        
        #endregion
        
        #region Ranged Combat
        
        private void Attack()
        {
            if (currentWeapon.weaponType == WeaponType.Ranged)
            {
                TryFireRanged();

                if (targetingSystem.CurrentTarget != null)
                {
                    targetingSystem.CurrentTarget.GetComponent<IDamageable>()?.TakeDamage(currentWeapon.damage);
                }
            }
            else if (currentWeapon.weaponType == WeaponType.Melee)
            {
                TryMeleeAttack();
            }
        }
        
        private bool CanFireRanged()
        {
            return CanFire && currentWeapon != null && currentWeapon.weaponType == WeaponType.Ranged;
        }
        
        private void TryFireRanged()
        {
            if (currentWeapon == null || currentWeapon.weaponType != WeaponType.Ranged) return;

            if (Time.time - lastFireTime < (1f / currentWeapon.fireRate))
                return;
                
            FireRanged();
            lastFireTime = Time.time;
        }
        
        private void FireRanged()
        {
            if (currentWeapon == null || currentWeapon.weaponType != WeaponType.Ranged) return;
            
            // Perform raycast from camera center
            if (playerCamera != null)
            {
                Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
                
                if (Physics.Raycast(ray, out RaycastHit hit, currentWeapon.range))
                {
                    ProcessRangedHit(hit, currentWeapon);
                }
            }
            
            // Play effects
            PlayFireEffects(currentWeapon);
            
            // Trigger events
            // OnWeaponFired?.Invoke();
            
            // Auto reload if empty
            if (currentWeapon.currentAmmo <= 0)
            {
                TryReload();
            }
        }
        
        private void ProcessRangedHit(RaycastHit hit, WeaponData weapon)
        {
            // Check if we hit an enemy
            var enemy = hit.collider.GetComponent<IDamageable>();
            if (enemy != null)
            {
                enemy.TakeDamage(weapon.damage);
                Debug.Log($"Hit {hit.collider.name} for {weapon.damage} damage with {weapon.weaponName}!");
            }
            
            // Spawn impact effect
            if (weapon.impactEffect != null)
            {
                GameObject impact = Instantiate(weapon.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }
        }
        
        private void TryReload()
        {
            if (currentWeapon == null || currentWeapon.weaponType != WeaponType.Ranged || isReloading) return;
            
            // Check if we need to reload
            if (currentWeapon.currentAmmo >= currentWeapon.ammoCapacity) return;
            
            StartCoroutine(ReloadCoroutine());
        }
        
        private IEnumerator ReloadCoroutine()
        {
            if (currentWeapon == null || currentWeapon.weaponType != WeaponType.Ranged) yield break;

            isReloading = true;
            // OnReloadStarted?.Invoke();
            
            // Play reload sound
            if (currentWeapon.reloadSound != null)
            {
                AudioSource.PlayClipAtPoint(currentWeapon.reloadSound, transform.position);
            }
            
            yield return new WaitForSeconds(currentWeapon.reloadTime);
            
            isReloading = false;
            // OnReloadCompleted?.Invoke();
        }
        
        #endregion
        
        #region Melee Combat
        
        private void TryMeleeAttack()
        {
            if (currentWeapon.weaponType != WeaponType.Melee) return;
            
            if (Time.time - lastMeleeTime < (1f / currentWeapon.fireRate))
                return;
                
            StartCoroutine(MeleeAttackCoroutine());
            lastMeleeTime = Time.time;
        }
        
        private IEnumerator MeleeAttackCoroutine()
        {
            isMeleeAttacking = true;
            // OnMeleeAttack?.Invoke();
            
            // Play melee sound
            if (currentWeapon.fireSound != null)
            {
                AudioSource.PlayClipAtPoint(currentWeapon.fireSound, transform.position);
            }
            
            // Perform melee raycast/sphere check
            PerformMeleeAttack();
            
            // Melee attack duration
            yield return new WaitForSeconds(0.3f);
            
            isMeleeAttacking = false;
        }
        
        private void PerformMeleeAttack()
        {
            // Use sphere cast for melee attacks
            Vector3 attackDirection = transform.forward;
            Vector3 attackPosition = transform.position + Vector3.up * 1.5f + attackDirection * 0.5f;
            
            Collider[] hitColliders = Physics.OverlapSphere(attackPosition, currentWeapon.range);
            
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == gameObject) continue; // Don't hit self
                
                var enemy = hitCollider.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    enemy.TakeDamage(currentWeapon.damage);
                    Debug.Log($"Melee hit {hitCollider.name} for {currentWeapon.damage} damage!");
                }
            }
            
            // Debug visualization - using Gizmos instead of Debug.DrawWireSphere
            #if UNITY_EDITOR
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(attackPosition, Vector3.up, currentWeapon.range);
            #endif
        }
        
        #endregion
        
        #region Helper Methods
        
        private void PlayFireEffects(WeaponData weapon)
        {
            // Play fire sound
            if (weapon.fireSound != null)
            {
                AudioManager.Instance.PlaySFX(weapon.fireSound);
            }

            // Spawn muzzle flash
            if (weapon.muzzleFlashEffect != null && firePoint != null)
            {
                GameObject muzzleFlash = Instantiate(weapon.muzzleFlashEffect, firePoint.position, firePoint.rotation);
                Destroy(muzzleFlash, 0.1f);
            }

            // animator.SetTrigger("Fire");
            animator.Play("Shoot", 1, 0f);
        }
        
        private void UpdateWeaponDisplay()
        {
            if (weaponModel == null) return;

            foreach (Transform child in weaponModel)
            {
                Destroy(child.gameObject);
            }
            
            if (currentWeapon != null && currentWeapon.weaponPrefab != null)
            {
                GameObject weaponVisual = Instantiate(currentWeapon.weaponPrefab, weaponModel);
            }
        }

        #endregion

        #region Public API
        public void EquipWeapon(WeaponData newWeapon)
        {
            if (newWeapon == null) return;

            if (currentWeapon == null)
            {
                currentWeapon = newWeapon;
            }
            else if (secondaryWeapon == null)
            {
                secondaryWeapon = newWeapon;
            }
            else
            {
                currentWeapon = newWeapon;

                // TODO: drop the old weapon
            }

            UpdateWeaponDisplay();
            GameEvent.OnWeaponSwitched?.Invoke();
        }
        #endregion
    }
}