using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;
    // Start is called before the first frame update
    [SerializeField] CharactersData characterData;
    [SerializeField] WeaponsData weaponData;
    [SerializeField] ColorReference ColorData;
    [SerializeField]Transform CharacterMaster, WeaponMaster;
    [SerializeField] Material mat;
    [SerializeField] GameObject ColorPanel;
    [SerializeField] Button[] Controls;
    [SerializeField] bool moving = false;
    int CharacterIndex, WeaponIndex,ColorIndex;



    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CharacterIndex = PlayerPrefs.GetInt("CharacterIndex", 0);
        WeaponIndex = PlayerPrefs.GetInt("WeaponIndex", 0);
        ColorIndex = PlayerPrefs.GetInt("ColorIndex", 0);
        SetColorData();
    }

    private void SetColorData()
    {
        mat.SetColor("_BaseColor", ColorData.CharacterColors[ColorIndex].color);
    }

    public void SetCharacterOnShow()
    {
        DestroyinShow();
        GameObject go=Instantiate(characterData.CharactersOnShow[CharacterIndex], CharacterMaster);
    }
    public void SetWeaponOnShow()
    {
        DestroyinShow();
        GameObject go=Instantiate(weaponData.WeaponsOnShow[WeaponIndex], WeaponMaster);
    }

    public void Save()
    {
        PlayerPrefs.SetInt("CharacterIndex", CharacterIndex);
        PlayerPrefs.SetInt("WeaponIndex", WeaponIndex);
        PlayerPrefs.SetInt("ColorIndex", ColorIndex);
    }

    public void SeNextPrevColor(string id)
    {
        for (int i = 0; i < ColorData.CharacterColors.Length; i++)
        {
            if (id == ColorData.CharacterColors[i].id)
            { 
                ColorIndex = i;
                SetColorData();
            }
        }
    }

    public void SetNextPrevCharacter(int i)
    {
        if (moving) return;
        moving = true;
        CharacterIndex += i;
        if (CharacterIndex < 0)
            CharacterIndex = 0;
        else if (CharacterIndex > characterData.CharactersOnShow.Length - 1)
            CharacterIndex = characterData.CharactersOnShow.Length - 1;
        SetCharacterOnShow();
        CharacterMaster.localPosition = new Vector3(5f * i, CharacterMaster.localPosition.y, 0);
        CharacterMaster.DOLocalMoveX(0 , 1f).OnComplete(() => { moving = false; });
    }
    
    public void SetNextPrevWeapon(int i)
    {
        if (moving) return;
        moving = true;
        WeaponIndex += i;
        if (WeaponIndex < 0)
            WeaponIndex = 0;
        else if (WeaponIndex > weaponData.WeaponsOnShow.Length - 1)
            WeaponIndex = weaponData.WeaponsOnShow.Length - 1;
        SetWeaponOnShow();
        WeaponMaster.localPosition = new Vector3(5f * i, WeaponMaster.localPosition.y, 0);
        WeaponMaster.DOLocalMoveX(0, 1f).OnComplete(()=> { moving = false; });
    }
   
    public void ShowColorPanel()
    {
        if (moving) return;
        moving = true;
        DestroyinShow();
        foreach (var item in Controls)
        {
            item.gameObject.SetActive(false);
        }
        SetCharacterOnShow();
        CharacterMaster.DOMoveX(-2f, 1f).OnComplete(() => {
            moving = false;
            ColorPanel.SetActive(true); });
    }

    public void ShowCharacterPanel()
    {
        
        if (moving) return;
        moving = true;
        Controls[0].onClick.RemoveAllListeners();
        Controls[1].onClick.RemoveAllListeners();
        Controls[0].onClick.AddListener(() => {
            SetNextPrevCharacter(-1);
        });
        Controls[1].onClick.AddListener(() => {
            SetNextPrevCharacter(1);
        });
        foreach (var item in Controls)
        {
            item.gameObject.SetActive(true);
        }
        ColorPanel.SetActive(false);
        DestroyinShow();
        SetCharacterOnShow();
        CharacterMaster.DOMoveX(0, 1f).OnComplete(() => {
            moving = false;
        });
    }
    public void ShowWeaponPanel()
    {
        if (moving) return;
        moving = true;
        Controls[0].onClick.RemoveAllListeners();
        Controls[1].onClick.RemoveAllListeners();
        Controls[0].onClick.AddListener(() => {
            SetNextPrevWeapon(-1);
        });
        Controls[1].onClick.AddListener(() => {
            SetNextPrevWeapon(1);
        });
        foreach (var item in Controls)
        {
            item.gameObject.SetActive(true);
        }
        ColorPanel.SetActive(false);
        DestroyinShow();
        SetWeaponOnShow();
        CharacterMaster.DOMoveX(0, 1f).OnComplete(() => {
            moving = false;
        });
    }

    public void DestroyinShow()
    {
        foreach (Transform item in CharacterMaster)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in WeaponMaster)
        {
            Destroy(item.gameObject);
        }
    }
}
