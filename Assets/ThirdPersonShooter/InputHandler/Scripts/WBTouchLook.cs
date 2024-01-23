using UnityEngine;
using UnityEngine.EventSystems;

public class WBTouchLook : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public static Vector2 TouchDist;

    [SerializeField] private Vector2 _pointerOld;
    [SerializeField] private int _pointerId;
    [SerializeField] private bool _pressed;
    [SerializeField] private float smoothnessFactor=.5f;

    private void Update()
    {
        if (_pressed)
        {
            if (_pointerId >= 0 && _pointerId < Input.touches.Length)
            {
                Vector2 currentTouchPosition = Input.touches[_pointerId].position;
                TouchDist = Vector2.Lerp(TouchDist, currentTouchPosition - _pointerOld, Time.deltaTime * smoothnessFactor);
                _pointerOld = currentTouchPosition;
            }
            else
            {
                Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                TouchDist = Vector2.Lerp(TouchDist, currentMousePosition - _pointerOld, Time.deltaTime * smoothnessFactor);
                _pointerOld = currentMousePosition;
            }
        }
        else
        {
            TouchDist = new Vector2();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        _pointerId = eventData.pointerId;
        _pointerOld = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
    }
}
