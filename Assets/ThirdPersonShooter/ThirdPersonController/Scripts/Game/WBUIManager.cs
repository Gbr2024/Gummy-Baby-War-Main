using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace WeirdBrothers.ThirdPersonController
{
    public class WBUIManager : MonoBehaviour
    {
        [Header("Item Pickup")]
        [SerializeField] private WBItemUI _itemPickUpUI;

        [Header("Weapon UI")]
        [SerializeField] private WBItemUI _primaryWeaponUI1;
        [SerializeField] private WBItemUI _primaryWeaponUI2;
        [SerializeField] private WBItemUI _secondaryWeaponUI;
        [SerializeField] private WBItemUI _meleeWeaponUI;
        [SerializeField] private Slider HealthBar;
        [SerializeField] TMP_Text MyKills;

        [Header("Weapon Icons")]
        [SerializeField] private GameObject _weaponPanels;

        private void OnEnable()
        {
           // if (!IsOwner) return;
            WBUIActions.ShowItemPickUp += ShowItemPickUp;
            WBUIActions.SetPrimaryWeaponUI += SetPrimaryWeaponUI;
            WBUIActions.SetWeaponUI += SetWeaponUI;
            WBUIActions.UpdateHealth += UpdateHealth;
            WBUIActions.UpdatelocalScore += UpdateMykills;
        }

        private void OnDisable()
        {
           
            WBUIActions.ShowItemPickUp -= ShowItemPickUp;
            WBUIActions.SetPrimaryWeaponUI -= SetPrimaryWeaponUI;
            WBUIActions.SetWeaponUI -= SetWeaponUI;
            WBUIActions.UpdateHealth -= UpdateHealth;
            WBUIActions.UpdatelocalScore -= UpdateMykills;
        }

        private void Start()
        {
           
            _itemPickUpUI.UIPanel.SetActive(false);
            _primaryWeaponUI1.UIPanel.SetActive(false);
            _primaryWeaponUI2.UIPanel.SetActive(false);
            _secondaryWeaponUI.UIPanel.SetActive(false);
            _meleeWeaponUI.UIPanel.SetActive(false);
            SetWeaponUI(false);
        }

        private void ShowItemPickUp(bool state, Sprite itemSprite, string itemName)
        {
           
            _itemPickUpUI.UIPanel.SetActive(state);
            _itemPickUpUI.ItemImage.sprite = itemSprite;
            _itemPickUpUI.ItemText.text = itemName;
        }

        private void SetPrimaryWeaponUI(int index, Sprite weaponImage, int currentAmmo, int totalAmmo)
        {
           
            if (index == 1)
            {
                if (!_primaryWeaponUI1.UIPanel.activeSelf)
                {
                    _primaryWeaponUI1.UIPanel.SetActive(true);
                }
                _primaryWeaponUI1.ItemImage.sprite = weaponImage;
                _primaryWeaponUI1.ItemText.text = (currentAmmo).ToString() + "/" + totalAmmo.ToString();
            }
            else if (index == 2)
            {
                if (!_primaryWeaponUI2.UIPanel.activeSelf)
                {
                    _primaryWeaponUI2.UIPanel.SetActive(true);
                }
                _primaryWeaponUI2.ItemImage.sprite = weaponImage;
                _primaryWeaponUI2.ItemText.text = (currentAmmo).ToString() + "/" + totalAmmo.ToString();
            }
            else if (index == 3)
            {
                if (!_secondaryWeaponUI.UIPanel.activeSelf)
                {
                    _secondaryWeaponUI.UIPanel.SetActive(true);
                }
                _secondaryWeaponUI.ItemImage.sprite = weaponImage;
                _secondaryWeaponUI.ItemText.text = (currentAmmo).ToString() + "/" + totalAmmo.ToString();
            }
            else if (index == 4)
            {
                if (!_meleeWeaponUI.UIPanel.activeSelf)
                {
                    _meleeWeaponUI.UIPanel.SetActive(true);
                }
                _meleeWeaponUI.ItemImage.sprite = weaponImage;
            }

        }

        private void UpdateHealth(float val)
        {
            HealthBar.value = val;
        }

        private void UpdateMykills(int Val)
        {
            MyKills.text = Val.ToString("00");
        }

        private void SetWeaponUI(bool state)
        {
            //if (!IsOwner) return;
            _weaponPanels.SetActive(state);
        }
    }
}