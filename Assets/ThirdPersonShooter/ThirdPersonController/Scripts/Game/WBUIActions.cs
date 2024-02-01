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
    }
}