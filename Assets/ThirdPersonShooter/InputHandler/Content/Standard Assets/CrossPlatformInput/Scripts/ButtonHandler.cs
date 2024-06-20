using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class ButtonHandler : MonoBehaviour
    {

        public string Name;

        void OnEnable()
        {

        }

        public void SetDownState()
        {
            Debug.LogError("Down " + name);
            CrossPlatformInputManager.SetButtonDown(Name);
        }


        public void SetUpState()
        {
            Debug.LogError("up " + name);
            CrossPlatformInputManager.SetButtonUp(Name);
        }


        public void SetAxisPositiveState()
        {
            CrossPlatformInputManager.SetAxisPositive(Name);
        }


        public void SetAxisNeutralState()
        {
            CrossPlatformInputManager.SetAxisZero(Name);
        }


        public void SetAxisNegativeState()
        {
            CrossPlatformInputManager.SetAxisNegative(Name);
        }

        private void OnDisable()
        {
            CrossPlatformInputManager.SetButtonUp(Name);
        }

    }
}
