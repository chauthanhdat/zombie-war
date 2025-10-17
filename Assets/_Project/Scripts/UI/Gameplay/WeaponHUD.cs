using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ZombieWar.Gameplay.Weapons;
using ZombieWar.Gameplay.Player;
using ZombieWar.Data;
using ZombieWar.Core.Events;

namespace ZombieWar.UI.Gameplay
{
    public class WeaponHUD : MonoBehaviour
    {
        [Header("UI References")]
        public Image currentWeaponIcon;
        public Image secondaryWeaponIcon;
        public Button attackButton;
        public Button swapWeaponButton;
        
        [Header("Weapon Slot Indicators")]
        public Image weapon1Indicator;
        public Image weapon2Indicator;
        public Image meleeIndicator;
        public Color activeWeaponColor = Color.white;
        public Color inactiveWeaponColor = Color.gray;
        
        [Header("Weapon Slot Info")]
        public TextMeshProUGUI weapon1NameText;
        public TextMeshProUGUI weapon2NameText;
        public TextMeshProUGUI meleeNameText;
        
        private WeaponController weaponController;
        private PlayerController playerController;

        private void Start()
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                weaponController = playerController.WeaponController;
            }

            UpdateDisplay();
        }

        private void OnEnable()
        {
            swapWeaponButton.onClick.AddListener(OnSwapWeaponButtonClicked);
            attackButton.onClick.AddListener(OnAttackButtonClicked);
        }

        private void OnDisable()
        {
            swapWeaponButton.onClick.RemoveListener(OnSwapWeaponButtonClicked);
            attackButton.onClick.RemoveListener(OnAttackButtonClicked);
        }

        private void Update()
        {
            UpdateDisplay();
        }

        private void OnSwapWeaponButtonClicked()
        {
            GameEvent.OnSwapWeaponRequested?.Invoke();
        }

        private void OnAttackButtonClicked()
        {
            GameEvent.OnAttack?.Invoke();
        }
        
        private void UpdateDisplay()
        {
            if (weaponController == null) return;
            
            UpdateWeaponDisplay();
        }
        private void UpdateWeaponDisplay()
        {
            if (weaponController == null) return;
            
            var currentWeapon = weaponController.CurrentWeapon;
            if (currentWeapon != null)
            {
                if (currentWeaponIcon != null)
                {
                    currentWeaponIcon.sprite = currentWeapon.weaponIcon;
                    currentWeaponIcon.gameObject.SetActive(currentWeapon.weaponIcon != null);
                }
            }
            else
            {
                if (currentWeaponIcon != null)
                {
                    currentWeaponIcon.gameObject.SetActive(false);
                }
            }
            
            var secondaryWeapon = weaponController.SecondaryWeapon;
            if (secondaryWeapon != null)
            {
                if (secondaryWeaponIcon != null)
                {
                    secondaryWeaponIcon.sprite = secondaryWeapon.weaponIcon;
                    secondaryWeaponIcon.gameObject.SetActive(secondaryWeapon.weaponIcon != null);
                }
            }
            else
            {
                if (secondaryWeaponIcon != null)
                {
                    secondaryWeaponIcon.gameObject.SetActive(false);
                }
            }
        }
        
        private void UpdateSlotInfo(TextMeshProUGUI nameText, WeaponData weapon)
        {
            if (nameText != null)
            {
                nameText.text = weapon != null ? weapon.weaponName : "Empty";
            }
        }
        
        private void UpdateSlotName(TextMeshProUGUI textComponent, WeaponData weapon)
        {
            if (textComponent != null)
            {
                textComponent.text = weapon != null ? weapon.weaponName : "Empty";
            }
        }
        
        private void OnDestroy()
        {
            // Events cleanup if needed in the future
        }
    }
}