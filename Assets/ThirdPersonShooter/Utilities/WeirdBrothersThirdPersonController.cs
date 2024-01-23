using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace WeirdBrothers.ThirdPersonController
{
    [Serializable]
    public class WBItemUI
    {
        public GameObject UIPanel;
        public TMP_Text ItemText;
        public Image ItemImage;
    }

    [Serializable]
    public class WBWeaponPositionData
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }

    [System.Serializable]
    public class CrossHairSettings
    {        
        public RectTransform CrossHair;
        [HideInInspector] public float CrossHairSpread;
        public float MinSpread;
        public float MaxSpread;
    }

    [Serializable]
    public class WBItem
    {
        public string ItemName;
        public WBItemType ItemType;
        public int ItemAmount;
    }
    
    public class WBWeaponSlots
    {
        public Transform RightHandReference;
        public Transform PrimarySlot1;
        public Transform PrimarySlot2;
        public Transform SecondarySlot;
        public Transform MeleeSlot;
    }

    public enum FireType
    {
        None,
        Auto,
        Semi
    }

    public enum WBItemType
    {
        Bullet
    }

    public enum WBWeaponType
    {
        Primary,
        Secondary,
        Melee
    }

    public enum PlayerState
    {
        Shooter,
        None
    }

    public interface IItemImage
    {
        Sprite GetItemImage();
    }

    public interface IItemName
    {
        string GetItemName();
    }

    public interface IState
    {
        void Execute();
    }

    public static class ThirdPersonControllerHelper
    {
        public static void Schedule(this object obj)
        {
            try
            {
                IState state = (IState)obj;
                if (state != null)
                {
                    state.Execute();
                }
            }
            catch (InvalidCastException e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        public static Sprite GetItemImage(this GameObject gameObject)
        {
            IItemImage image = gameObject.GetComponent<IItemImage>();
            if (image != null)
            {
                return image.GetItemImage();
            }
            return null;
        }

        public static string GetItemName(this GameObject gameObject)
        {
            IItemName name = gameObject.GetComponent<IItemName>();
            if (name != null)
            {
                return name.GetItemName();
            }
            return null;
        }
    }
}