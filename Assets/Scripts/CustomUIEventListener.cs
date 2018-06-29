using UnityEngine;

public abstract class CustomUIEventListener : MonoBehaviour
{
    public bool ListenableForBroadcast;
    public CustomUIEventCaster.UIEventInfo EventInfo;

    protected virtual void Awake()
    {
        EventInfo = null;
    }

    protected abstract void OnClickBegan(CustomUIEventCaster.UIEventInfo eventInfo);
    protected abstract void OnClickEnded(CustomUIEventCaster.UIEventInfo eventInfo);
}