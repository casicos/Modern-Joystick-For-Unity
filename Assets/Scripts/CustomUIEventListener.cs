using UnityEngine;

public abstract class CustomUIEventListener : MonoBehaviour
{
    public bool ListenableForBroadcast;
    public CustomUIEventCaster.UIEventInfo EventInfo;
    
    public bool IsEventMouseInput { get { return EventInfo.Id == CustomUIEventCaster.MouseInputTouchId; } }
    public bool IsEventHoldOn { get { return EventInfo != null; } }

    protected virtual void Awake()
    {
        EventInfo = null;
    }

    protected virtual Vector3 GetEventPosition()
    {
        if (!IsEventHoldOn) return Vector3.zero;

        var rayCastCamera = CustomUIEventCaster.GetInstance.RayCastCamera;

        return IsEventMouseInput
            ? rayCastCamera.ScreenToWorldPoint(Input.mousePosition)
            : rayCastCamera.ScreenToWorldPoint(Input.GetTouch(EventInfo.Id).position);
    }

    protected abstract void OnClickBegan(CustomUIEventCaster.UIEventInfo eventInfo);
    protected abstract void OnClickEnded(CustomUIEventCaster.UIEventInfo eventInfo);  
}