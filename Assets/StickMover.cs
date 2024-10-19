using UnityEngine;
using UnityEngine.EventSystems;

public class StickMover : MonoBehaviour, IPointerDownHandler,IDragHandler, IPointerUpHandler
{
    [SerializeField]RectTransform joystick; // Assign the joystick RectTransform in the inspector
    private RectTransform canvasRect; // Reference to the parent canvas

    Joystick Joystick;
    private void Start()
    {
        joystick = transform.GetChild(0).GetComponent<RectTransform>();
        // Find the parent canvas, assuming the joystick is part of a UI Canvas
        canvasRect = GetComponent<RectTransform>();
        Joystick = joystick.GetComponent<Joystick>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Convert the screen position to canvas-local position
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out localPoint);

        // Set the joystick's position to the pointer position
        joystick.anchoredPosition = localPoint;
        Joystick.OnPointerDown(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Joystick.OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Joystick.OnPointerUp(eventData);
    }
}
