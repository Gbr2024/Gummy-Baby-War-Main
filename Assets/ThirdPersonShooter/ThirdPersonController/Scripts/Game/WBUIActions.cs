using System;
using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public static class WBUIActions
    {
        public static Action<bool, Sprite, string> ShowItemPickUp;
        public static Action<int, Sprite, int, int> SetPrimaryWeaponUI;
        public static Action<bool> SetWeaponUI;
        public static Action<float> UpdateHealth;
        public static Action<int> UpdatelocalScore;
        public static Action<bool> EnableBlackPanel;
        public static Action<bool> EnableGrenadeTime;
        public static Action<bool> EnableGrenadeButton;
        public static Action<bool> EnableKillstreakButton;
        public static Action<bool> EnableMapButton;
        public static Action<bool> EnableMap;
        public static Action<bool> EnableTouch;
        public static Action<string> ChangeFireIcon;
        public static Action<string> ChangeKillstreak;
       

        internal static bool isPlayerActive = true;
    }
}