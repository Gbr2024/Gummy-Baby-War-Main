using UnityEngine;
using System.Collections;
using Cinemachine;
using Unity.Netcode; // Import Unity.NetCode for NetworkBehaviour
using UnityStandardAssets.CrossPlatformInput;

// Add the GenerateAuthoringComponent attribute to indicate that this component will be generated for each networked entity
public class WBInputHandler : NetworkBehaviour
{
    // Enum to define the types of input (PC or Mobile)
    private enum InputType
    {
        PC,
        Mobile
    }

    [SerializeField] private InputType _inputType;

    [Space]
    [SerializeField] private GameObject _mobileInput;
    [SerializeField] private float _touchSensitivity;
    [SerializeField] private bool HasControl = false; 

    private FixedJoystick _joystick;

    private IEnumerator Start()
    {
        // Check if the client has control before setting up input
        yield return new WaitForSeconds(0.25f);
        if(GetComponent<NetworkObject>()!=null)
            HasControl = GetComponent<NetworkObject>().IsOwner; 

        if (!HasControl)
            yield break;

        if (_inputType == InputType.Mobile)
        {
            _mobileInput?.SetActive(true);
            CinemachineCore.GetInputAxis = HandleAxisInputDelegate;
            _joystick = FindObjectOfType<FixedJoystick>();
        }
        else
        {
            _mobileInput?.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Method to set the mobile input game object
    

    internal void SetInput(GameObject go)
    {
        _mobileInput = go;
    }



    // Property to get the horizontal input
    public float Horizontal
    {
        get
        {
            // Check if the client has control before returning input
            if (HasControl && _inputType == InputType.PC)
                return Input.GetAxis("Horizontal");
            else if (HasControl && _inputType == InputType.Mobile)
                return _joystick.Horizontal;
            return 0;
        }
    }

    // Property to get the vertical input
    public float Vertical
    {
        get
        {
            // Check if the client has control before returning input
            if (HasControl && _inputType == InputType.PC)
                return Input.GetAxis("Vertical");
            else if (HasControl && _inputType == InputType.Mobile)
                return _joystick.Vertical;
            return 0;
        }
    }

    // Method to get a button state
    public bool GetButton(string buttonName)
    {
        // Check if the client has control before returning input
        if (HasControl && _inputType == InputType.PC)
        {
            return Input.GetButton(buttonName);
        }
        else if (HasControl && _inputType == InputType.Mobile)
        {
            return CrossPlatformInputManager.GetButton(buttonName);
        }
        return false;
    }

    // Method to check if a button was pressed
    public bool GetButtonDown(string buttonName)
    {
        // Check if the client has control before returning input
        if (HasControl && _inputType == InputType.PC)
        {
            return Input.GetButtonDown(buttonName);
        }
        else if (HasControl && _inputType == InputType.Mobile)
        {
            return CrossPlatformInputManager.GetButtonDown(buttonName);
        }
        return false;
    }

    // Method to check if a button was released
    public bool GetButtonUp(string buttonName)
    {
        // Check if the client has control before returning input
        if (HasControl && _inputType == InputType.PC)
        {
            return Input.GetButtonUp(buttonName);
        }
        else if (HasControl && _inputType == InputType.Mobile)
        {
            return CrossPlatformInputManager.GetButtonUp(buttonName);
        }
        return false;
    }

    // Delegate method to handle axis input (e.g., mouse or touch)
    private float HandleAxisInputDelegate(string axisName)
    {
        switch (axisName)
        {
            case "Mouse X":
                if (Input.touchCount > 0)
                {
                    return WBTouchLook.TouchDist.x * _touchSensitivity;
                }
                else
                {
                    return HasControl ? Input.GetAxis(axisName) : 0f;
                }

            case "Mouse Y":
                if (Input.touchCount > 0)
                {
                    return WBTouchLook.TouchDist.y * _touchSensitivity;
                }
                else
                {
                    return HasControl ? Input.GetAxis(axisName) : 0f;
                }

            default:
                Debug.LogError("Input <" + axisName + "> not recognized.", this);
                break;
        }

        return 0f;
    }

    // Property to check if the client has control (authority) or if it's on the server
    

    // Additional restrictions or modifications can be added here based on your specific requirements
}
