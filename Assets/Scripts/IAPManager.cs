using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using TMPro;
using LitJson;
using System.IO;
using UnityEngine.SceneManagement;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    [SerializeField] Button PurchaseButton,SelectionButton,SelectedButton;
    [SerializeField] GameObject PurchasePanel,PopupMessage;
    [SerializeField] Image panelImage;
    [SerializeField] TMP_Text PurchaseButtonLabel,NameLabel;
    [SerializeField] Buyable[] buyables;
    UnlockedWeapons SetWeapons;

    private IStoreController storeController;
    private IExtensionProvider extensionProvider;

    // Start is called before the first frame update
    void Start()
    {
        setupBuilder();
        LoadResources();
    }

    private void Update()
    {
        if (PurchasePanel.activeSelf) return;
        if (Input.touchCount > 0)
        {
            // Get the first touch
            Touch touch = Input.GetTouch(0);

            // Check if the touch has just begun
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                   if(hit.transform.TryGetComponent(out ShopItems shopitem))
                    {
                        foreach (var item in buyables)
                        {
                            if(shopitem.id==item.Name)
                            {
                                panelImage.sprite = item.image;
                                PurchasePanel.SetActive(true);
                                NameLabel.text = item.Name;
                                SelectionButton.onClick.RemoveAllListeners();
                                SelectionButton.onClick.AddListener(() => { SelectWeapon(item.index); });
                                if (SetWeapons.unlockedWeapons.Contains(item.index))
                                {

                                    if (PlayerPrefs.GetInt("WeaponIndex")==item.index)
                                    {
                                        PurchaseButton.gameObject.SetActive(false);
                                        SelectionButton.gameObject.SetActive(false);
                                        SelectedButton.gameObject.SetActive(true);
                                        return;
                                    }
                                    PurchaseButton.gameObject.SetActive(false);
                                    SelectionButton.gameObject.SetActive(true);
                                    SelectedButton.gameObject.SetActive(false);
                                }

                                else
                                {
                                    PurchaseButton.gameObject.SetActive(true);
                                    SelectionButton.gameObject.SetActive(false);
                                    SelectedButton.gameObject.SetActive(false);
                                    PurchaseButtonLabel.text = item.Price + "$";
                                    PurchaseButton.onClick.RemoveAllListeners();
                                    PurchaseButton.onClick.AddListener(() => { SetPurchaseItem(item.Id); });
                                    
                                }
                               
                            }
                        }
                    }
                }
            }
        }
    }

    private void SelectWeapon(int index)
    {
        PlayerPrefs.SetInt("WeaponIndex", index);
        PlayerPrefs.SetInt("WeaponSelected", 1);
        PurchaseButton.gameObject.SetActive(false);
        SelectionButton.gameObject.SetActive(false);
        SelectedButton.gameObject.SetActive(true);
    }
    public void UnSelectWeapon()
    {
        PlayerPrefs.SetInt("WeaponSelected", 0);
        PurchaseButton.gameObject.SetActive(false);
        SelectionButton.gameObject.SetActive(true);
        SelectedButton.gameObject.SetActive(false);
    }

    private void setupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var item in buyables)
        {
            builder.AddProduct(item.Id, ProductType.NonConsumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public void SetPurchaseItem(string id)
    {
        try
        {
            storeController.InitiatePurchase(id);
        }
        catch
        {
            PopupMessage.SetActive(true);
        }
        
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"Purchase of product {product.definition.id} failed due to {failureDescription.reason}");
        // Implement your logic for handling failed purchases here
        PopupMessage.SetActive(true);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"Initialization failed due to {error}");
        // Implement your logic for handling initialization failures here
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"Initialization failed due to {error}: {message}");
        // Implement your logic for handling initialization failures here
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log($"Purchase successful: {purchaseEvent.purchasedProduct.definition.id}");
        // Implement your logic for handling successful purchases here
        foreach (var item in buyables)
        {
            if (item.Id == purchaseEvent.purchasedProduct.definition.id)
            { 
                SetWeapon(item.index);
                PurchaseButton.gameObject.SetActive(false);
                SelectionButton.gameObject.SetActive(true);
                SelectedButton.gameObject.SetActive(false);
            }



        }

        

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase of product {product.definition.id} failed due to {failureReason}");
        // Implement your logic for handling failed purchases here
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP initialization successful");
        storeController = controller;
        extensionProvider = extensions;
        // Implement any additional initialization logic here
    }



    public void SetWeapon(int index)
    {
        if(!SetWeapons.unlockedWeapons.Contains(index))
        {
            SetWeapons.unlockedWeapons.Add(index);
        }
        SaveData();
    }

    private void SaveData()
    {
        string jsonData = JsonMapper.ToJson(SetWeapons);
        File.WriteAllText(Application.persistentDataPath + "/weaponData.json", jsonData);
        LoadResources();
    }

    private void LoadResources()
    {
        if (File.Exists(Application.persistentDataPath + "/weaponData.json"))
        {
            string jsonData = File.ReadAllText(Application.persistentDataPath + "/weaponData.json");
            SetWeapons = JsonMapper.ToObject<UnlockedWeapons>(jsonData);
        }
        else
        {
            SetWeapons = new();
            SetWeapons.unlockedWeapons = new();
        }
    }

    public void LoadMenu()
    {
        AdmobAds.Instance.ShowInterstitialAd(Loadmenu);
    }

    public void Loadmenu()
    {
        SceneManager.LoadScene(1);
    }

}

[System.Serializable]
public class Buyable
{
    public string Name;
    public string Id;
    public float Price;
    public int index;
    public Sprite image;
}

public class UnlockedWeapons
{
    public List<int> unlockedWeapons { get; set; }

}
