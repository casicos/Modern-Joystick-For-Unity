using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomUIEventCaster : Singleton<CustomUIEventCaster> {

    #region <Consts>
	
    public const int MouseInputTouchId = MaxTouchCount;
    
    private const int MaxTouchCount = 10;

    #endregion
	
    #region <Fields>

    public Camera RayCastCamera;
    public List<CustomUIEventListener> EventListenerGroup;

    #endregion </Fields>
	
    #region <Unity/Callbacks>

    /// <summary>
    /// ListenEvent per update of the frame.
    /// </summary>
    private void Update () {
        ListenEvent();
    }
	
    #endregion </Unity/Callbacks>

    #region <Methods>

    /// <summary>
    /// Called by update per frame to listen something to event.
    /// </summary>
    private void ListenEvent()
    {
        // Verifying the mouse event
        var isLeftMouseButtonDown = Input.GetMouseButtonDown(0);
        var isLeftMouseButtonUp = Input.GetMouseButtonUp(0);
		
        // Touch count > 0 means there is something to event 
        if (Input.touchCount > 0 || isLeftMouseButtonDown || isLeftMouseButtonUp)
        {
            UserInputEvent(Input.touchCount > 0);
        }
    }

    private void UserInputEvent(bool isTouched = false)
    {
        // Mobile Input.
        if (isTouched)
        {
            // Iterate for the checking of each touch events.
            for (var touchId = 0; 
                touchId < Input.touchCount && touchId < MaxTouchCount; ++touchId)
            {
                var touch = Input.GetTouch(touchId);
                var touchPhase = touch.phase;
				
                switch (touchPhase)
                {
                    case TouchPhase.Began:
                        InputProcess(touch.position, onClick: true, eventTriggerId: touchId);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        InputProcess(touch.position, onClick: false, eventTriggerId: touchId);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        // PC Input.
        else
        {
            InputProcess(Input.mousePosition, onClick: Input.GetMouseButtonDown(0));
        }
    }

    private void InputProcess(Vector2 inputPosition, bool onClick, int eventTriggerId = MouseInputTouchId)
    {
        // Event has been started.
        if (onClick)
        {
            var rayCastCollider = GetReceiverColliderOnRayCast(inputPosition);					            
			
            BroadcastEvent("OnClickBegan", new UIEventInfo(eventTriggerId, rayCastCollider));
        }
        // Event has been terminated.
        else
        {
            BroadcastEvent("OnClickEnded", eventTriggerId);
        }
    }
	
    private Collider2D GetReceiverColliderOnRayCast(Vector3 rayCastPoint)
    {
        var ray = RayCastCamera.ScreenPointToRay(rayCastPoint);
        var rayCastHit = Physics2D.Raycast(ray.origin, ray.direction);

        return rayCastHit.collider;
    }
    
    private void BroadcastEvent(string eventMessage, int id)
    {
        BroadcastEvent(eventMessage, new UIEventInfo(id));
    }
        
    private void BroadcastEvent(string eventMessage, UIEventInfo eventInfo)
    {
        // Filtered broadcast
        EventListenerGroup.Where(eventListener => eventListener.ListenableForBroadcast
            || eventListener.gameObject == eventInfo.Collider
            || eventListener.EventInfo.Id == eventInfo.Id)
            .ToList().ForEach(eventListener => 
                eventListener.SendMessage(eventMessage, eventInfo));
    }

    #endregion

    #region <Class>

    public class UIEventInfo
    {
        public readonly Collider2D Collider;
        public readonly int Id;
        
        public UIEventInfo(int id)
        {
            Collider = null;
            Id = id;
        }
        
        public UIEventInfo(int id, Collider2D collider)
        {
            Collider = collider;
            Id = id;
        }
    }

    #endregion </Class>
}