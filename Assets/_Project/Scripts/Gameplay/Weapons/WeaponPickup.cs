using UnityEngine;
using ZombieWar.Data;
using ZombieWar.Gameplay.Player;

namespace ZombieWar.Gameplay.Weapons
{
    public class WeaponPickup : MonoBehaviour
    {
        [Header("Pickup Configuration")]
        public WeaponData weaponData;
        public bool addAmmo = true;
        public int ammoAmount = 60;
        
        [Header("Visual Effects")]
        public float rotationSpeed = 45f;
        public float bobSpeed = 2f;
        public float bobHeight = 0.5f;
        
        [Header("Audio")]
        public AudioClip pickupSound;
        
        private Vector3 startPosition;
        private bool hasBeenPickedUp = false;
        
        private void Start()
        {
            startPosition = transform.position;
        }
        
        private void Update()
        {
            if (hasBeenPickedUp) return;
            
            // Rotate the pickup
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            
            // Bob up and down
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (hasBeenPickedUp) return;
            
            var playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                PickupWeapon(playerController);
            }
        }
        
        private void PickupWeapon(PlayerController player)
        {
            if (weaponData == null) return;
            
            hasBeenPickedUp = true;
            
            player.EquipWeapon(weaponData);
            
            // Add ammo if it's a ranged weapon
            if (addAmmo && weaponData.weaponType == WeaponType.Ranged)
            {
                var weaponController = player.WeaponController;
                if (weaponController != null)
                {
                    // weaponController.AddAmmo(ammoAmount);
                }
            }
            
            // Play pickup sound
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            
            // Show pickup message
            Debug.Log($"Picked up {weaponData.weaponName}!");
            
            // Destroy the pickup
            Destroy(gameObject);
        }
        
        private void OnDrawGizmos()
        {
            // Draw pickup range
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}