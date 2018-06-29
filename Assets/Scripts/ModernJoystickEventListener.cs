public class ModernJoystickEventListener : CustomUIEventListener
{         
    protected override void OnClickBegan(CustomUIEventCaster.UIEventInfo eventInfo)
    {
        if (eventInfo.Collider == null && EventInfo == null)
        {
            EventInfo = eventInfo;
        }
    }

    protected override void OnClickEnded(CustomUIEventCaster.UIEventInfo eventInfo)
    {
        if (EventInfo == null) return;

        if (EventInfo.Id == eventInfo.Id)
            EventInfo = null;
    }
}