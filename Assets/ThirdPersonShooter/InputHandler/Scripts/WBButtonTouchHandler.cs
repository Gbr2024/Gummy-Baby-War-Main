using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class WBButtonTouchHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private ButtonHandler _handler;

    private bool _isPressed = false;

    private void Start()
    {
        _handler = GetComponent<ButtonHandler>();
    }

    private void Update()
    {        
        if (_handler != null)
        {            
            if (CrossPlatformInputManager.GetButtonDown(_handler.Name))
            {
                _isPressed = true;         
            }
            else if (CrossPlatformInputManager.GetButtonUp(_handler.Name))
            {
                _isPressed = false;                
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isPressed)
            return;        

       
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isPressed)
            return;
       
    }
}
