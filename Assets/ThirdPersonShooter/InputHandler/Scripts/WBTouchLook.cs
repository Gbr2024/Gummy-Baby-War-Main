using UnityEngine;
using UnityEngine.EventSystems;
using WeirdBrothers.ThirdPersonController;

public class WBTouchLook : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float speed;
    public WBPlayerContext context;

    private static Vector2 touchDist;
    public static Vector2 TouchDist { get { return touchDist; } }

    private float Sensitivity = 0.75f;


    //public void OnDrag(PointerEventData eventData)
    //{
    //    touchDist.x = eventData.delta.x;
    //    touchDist.y = eventData.delta.y;
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    touchDist = Vector2.zero;
    //}
    private void Start()
    {
        Sensitivity = PlayerPrefs.GetFloat("Aim",.75f);
    }

    void Update()
    {
        if (!WBUIActions.isPlayerActive)
        {
            touchDist = Vector2.zero;
            return;
        }
        // Check if there are any touches
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.position.x > Screen.width / 2)
                {
                    // Check if the touch phase is began
                    touchDist.x = touch.deltaPosition.x * Sensitivity;
                    touchDist.y = touch.deltaPosition.y * Sensitivity;
                    if(context!=null)
                    {
                        if(context.isScopeOn)
                        {
                            touchDist *= context.ScopeOnRatio;
                        }
                    }
                    
                }
               
            }
        }
    }
}
