using UnityEngine;
using UnityEngine.EventSystems;

public class WBTouchLook : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private static Vector2 touchDist;
    public static Vector2 TouchDist { get { return touchDist; } }

    public void OnDrag(PointerEventData eventData)
    {
        touchDist.x = eventData.delta.x;
        touchDist.y = eventData.delta.y;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touchDist = Vector2.zero;
    }
}
