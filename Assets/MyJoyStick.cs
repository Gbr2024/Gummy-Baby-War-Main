using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyJoystick : Joystick
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        handle.anchoredPosition = Vector2.zero;
        input = Vector2.zero;
        // Check for touches on the left side of the screen
        //if (Input.touchCount > 0)
        //{
        //    for (int i = 0; i < Input.touchCount; i++)
        //    {
        //        Touch touch = Input.GetTouch(i);

        //        // Check if the touch is on the left side of the screen
        //        if (touch.position.x < Screen.width / 2)
        //        {
        //            MoveJoystick(touch.position);
        //            break; // Only handle the first valid touch on the left side
        //        }
        //    }
        //}
    }

    private void MoveJoystick(Vector2 touchPosition)
    {
        RectTransform parentRect = background.parent as RectTransform;

        // Convert the touch position to local point within the parent RectTransform
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, touchPosition, canvas.worldCamera, out localPoint);

        // Get the anchor point of the joystick
        Vector2 anchorOffset = new Vector2(
            parentRect.rect.width * background.anchorMin.x,  // Left offset
            parentRect.rect.height * background.anchorMin.y  // Bottom offset
        );

        // Adjust local point to the joystick's anchor position
        localPoint -= anchorOffset;

        // Get half the size of the joystick background to account for its width/height
        Vector2 halfBackgroundSize = background.sizeDelta / 2;

        // Clamp the joystick's position within the parent bounds, considering the anchor
        float clampedX = Mathf.Clamp(localPoint.x, -parentRect.rect.width / 2 + halfBackgroundSize.x, parentRect.rect.width / 2 - halfBackgroundSize.x);
        float clampedY = Mathf.Clamp(localPoint.y, -parentRect.rect.height / 2 + halfBackgroundSize.y, parentRect.rect.height / 2 - halfBackgroundSize.y);

        // Set the new joystick background position (ensure it stays within bounds)
        background.anchoredPosition = new Vector2(clampedX, clampedY);

        // Keep the handle centered
        handle.anchoredPosition = Vector2.zero;
        input = Vector2.zero;
    }
}
