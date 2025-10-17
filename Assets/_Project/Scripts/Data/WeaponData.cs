using UnityEngine;

namespace ZombieWar.Data
{
    public enum WeaponType
    {
        Melee,
        Ranged
    }

    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "ZombieWar/Data/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        public string weaponName;
        public WeaponType weaponType = WeaponType.Ranged;
        public Sprite weaponIcon;
        public GameObject weaponPrefab;
        
        [Header("Stats")]
        public float damage = 25f;
        public float fireRate = 1f;
        public float range = 10f;
        public int ammoCapacity = 30;
        public float reloadTime = 2f;
        
        [Header("Ammo System")]
        public int currentAmmo = 30;
        
        [Header("Audio")]
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public AudioClip emptyClipSound;
        
        [Header("Effects")]
        public GameObject muzzleFlashEffect;
        public GameObject impactEffect;
    }
}