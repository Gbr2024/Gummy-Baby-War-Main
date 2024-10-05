using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using DG.Tweening;

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
        [SerializeField] GameObject Tick,SecShootButton;
        [SerializeField] TMP_Text tickTimer;
        [SerializeField] TMP_Text MyKills,killstreaklabel;
        [SerializeField] GameObject BlackPanel,GrenadeButton,KillstreakButton,Map,BrokenScreen;
        [SerializeField] Sprite GunSprite, GrenadeSprite;
        [SerializeField] Image ShootImage;
        [SerializeField] WBTouchLook DisableTouch;

        [Header("Weapon Icons")]
        [SerializeField] private GameObject _weaponPanels;


        [Header("Speacial Kill Button")]
        [SerializeField] GameObject SpecialKillButton;
        [SerializeField] GameObject SkillList;
        [SerializeField] Transform SpecialKillMaster;
        [SerializeField] Button SpecialKillEffect;
        [SerializeField] Button SpecialKillSetBtn;
        [SerializeField] Image SpecialKillCover;

        private void OnEnable()
        {
           // if (!IsOwner) return;
            WBUIActions.ShowItemPickUp += ShowItemPickUp;
            WBUIActions.SetPrimaryWeaponUI += SetPrimaryWeaponUI;
            WBUIActions.SetWeaponUI += SetWeaponUI;
            WBUIActions.UpdateHealth += UpdateHealth;
            WBUIActions.UpdatelocalScore += UpdateMykills;
            WBUIActions.EnableBlackPanel += EnableBlackPanel;
            WBUIActions.EnableBrokenScreen += EnableBrokenScreen;
            WBUIActions.EnableGrenadeTime += StartTick;
            WBUIActions.EnableGrenadeButton += EnableGrenadeButton;
            WBUIActions.ChangeFireIcon += SetButtonIcon;
            WBUIActions.EnableKillstreakButton += SetKillStreakButton;
            WBUIActions.EnableMap += EnableMap;
            WBUIActions.EnableTouch += EnableTouch;
            WBUIActions.ChangeKillstreak += SetStreakLabel;
            WBUIActions.EnableSecShoot += SetSecShootbtn;
            WBUIActions.SetSpecialKill += SetSpecialKill;
        }

        private void SetSpecialKill(List<string> obj)
        {
            while (SpecialKillMaster.childCount>0)
            {
                DestroyImmediate(SpecialKillMaster.GetChild(0).gameObject);
            }
            foreach (var item in obj)
            {
                if (item == "Granny" && !KillStreakSystem.Instance.getSpawnGranny)
                    continue;

                var g=Instantiate(SpecialKillEffect, SpecialKillMaster);
                var kill = item;
                Debug.LogError(g);
                g.onClick.AddListener(() => { OnSkillInvoked(kill); });
                g.transform.GetComponentInChildren<TMP_Text>().text = kill;
            }
            SpecialKillButton.SetActive(true);
        }

        private void OnSkillInvoked(string kill)
        {
            WBUIActions.OnKillInvoked?.Invoke(kill);
            SpecialKillSetBtn.interactable = false;
            SpecialKillCover.fillAmount = 1f;
            SpecialKillCover.gameObject.SetActive(true);
            SpecialKillCover.DOFillAmount(0, 90f).OnComplete(() =>
            {
                SpecialKillCover.gameObject.SetActive(false);
                SpecialKillSetBtn.interactable = true;
            });
            SkillList.SetActive(false);
        }

        public void OpenSkillList()
        {
            SkillList.SetActive(!SkillList.activeSelf);
        }

        private void EnableBrokenScreen(bool obj)
        {
            BrokenScreen.SetActive(obj);
        }

        private void SetSecShootbtn(bool obj)
        {
            SecShootButton.SetActive(obj);
        }

        private void SetStreakLabel(string obj)
        {
            killstreaklabel.text = obj;
        }

        private void EnableTouch(bool obj)
        {
            DisableTouch.enabled = obj;
        }

        private void EnableMap(bool obj)
        {
            Map.SetActive(obj);
        }

        

        private void EnableBlackPanel(bool b)
        {
            BlackPanel.SetActive(b);
        }

        private void OnDisable()
        {
           
            WBUIActions.ShowItemPickUp -= ShowItemPickUp;
            WBUIActions.SetPrimaryWeaponUI -= SetPrimaryWeaponUI;
            WBUIActions.SetWeaponUI -= SetWeaponUI;
            WBUIActions.UpdateHealth -= UpdateHealth;
            WBUIActions.UpdatelocalScore -= UpdateMykills;
            WBUIActions.EnableBlackPanel -= EnableBlackPanel;
            WBUIActions.EnableBrokenScreen -= EnableBrokenScreen;
            WBUIActions.EnableGrenadeButton -= EnableGrenadeButton;
            WBUIActions.EnableGrenadeTime -= StartTick;
            WBUIActions.ChangeFireIcon -= SetButtonIcon;
            WBUIActions.EnableKillstreakButton -= SetKillStreakButton;
            WBUIActions.EnableMap -= EnableMap;
            WBUIActions.EnableTouch -= EnableTouch;
            WBUIActions.ChangeKillstreak -= SetStreakLabel;
            WBUIActions.EnableSecShoot -= SetSecShootbtn;
            WBUIActions.SetSpecialKill -= SetSpecialKill;
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

        public void LoadMenu()
        {
            Loader.Load(1);
        }

        void StartTick(bool enable)
        {
            StopCoroutine(StartTimer());
            Tick.SetActive(enable);
            if(Tick.activeSelf)
                StartCoroutine(StartTimer());
            else
                ShootImage.sprite = GunSprite;
        }

        void EnableGrenadeButton(bool act)
        {
            GrenadeButton.SetActive(act);
        }

        void SetButtonIcon(string id="Gun")
        {
            switch(id)
            {
                case "Grenade":
                    ShootImage.sprite = GrenadeSprite;
                    break;
                default:
                    ShootImage.sprite = GunSprite;
                    break;
            }
        }

        IEnumerator StartTimer()
        {
            tickTimer.text = "6";
            for (int i = 6; i >= 0; i--)
            {
                yield return new WaitForSeconds(1f);
                tickTimer.text = i.ToString();
            }
            Tick.SetActive(false);
        }

        private void SetKillStreakButton(bool obj)
        {
            KillstreakButton.SetActive(obj);
        }
    }

}