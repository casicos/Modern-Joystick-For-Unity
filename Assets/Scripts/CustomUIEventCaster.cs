using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomUIEventCaster : Singleton<CustomUIEventCaster> {

    #region <Consts>
	
    public const int MouseInputTouchId = MaxTouchCount;
    
    private const int MaxTouchCount = 10;

    #endregion </Consts>
	
    #region <Fields>

    public Camera RayCastCamera;
    public List<CustomUIEventListener> EventListenerGroup;
    public UIEventInfo GetLastUIEventInfo { get; private set; }

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

    public void SetLastUIEventInfo(UIEventInfo eventInfo)
    {
        GetLastUIEventInfo = eventInfo;
    }
    
    /// <summary>
    /// Called by update per frame to listen something to event.
    /// </summary>
    private void ListenEvent()
    {
        var isEventOccur = Input.touchCount > 0;
    #if UNITY_EDITOR
        var isLeftMouseButtonUp = Input.GetMouseButtonUp(0);
        var isLeftMouseButtonDown = Input.GetMouseButtonDown(0);
        isEventOccur = isEventOccur || isLeftMouseButtonUp || isLeftMouseButtonDown;
    #endif
        
        // Touch count > 0 means there is something to event 
        if (isEventOccur)
        {
            UserInputEvent();
        }
    }

    private void UserInputEvent(bool isTouched = false)
    {
        #if UNITY_EDITOR
            InputProcess(Input.mousePosition, onClick: Input.GetMouseButton(0));
        #else
            var touchCount = Input.touchCount;
            // Iterate for the checking of each touch events.
            for (var touchId = 0; 
                touchId < MaxTouchCount && touchId < touchCount; ++touchId)
            {
                var touch = Input.GetTouch(touchId);
                var touchPhase = touch.phase;                   

                switch (touchPhase)
                {
                    case TouchPhase.Began:
                        InputProcess(touch.position, onClick: true, eventTriggerId: touch.fingerId);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        InputProcess(touch.position, onClick: false, eventTriggerId: touch.fingerId);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        #endif
    }

    private void InputProcess(Vector2 inputPosition, bool onClick, int eventTriggerId = MouseInputTouchId)
    {
        // Event has been started.
        if (onClick)
        {
            var rayCastCollider = GetReceiverColliderOnRayCast(inputPosition);					            
			
            Send("OnClickBegin", new UIEventInfo(eventTriggerId, inputPosition, rayCastCollider));
        }
        // Event has been terminated.
        else
        {
            var rayCastCollider = GetReceiverColliderOnRayCast(inputPosition);                        
            
            Send("OnClickEnd", new UIEventInfo(eventTriggerId, inputPosition, rayCastCollider));
        }
    }
	
    private Collider2D GetReceiverColliderOnRayCast(Vector3 rayCastPoint)
    {
        var ray = RayCastCamera.ScreenPointToRay(rayCastPoint);
        var rayCastHit = Physics2D.Raycast(ray.origin, ray.direction);

        return rayCastHit.collider;
    }
    
    private void Send(string eventMessage, int id, Vector2 position)
    {
        Send(eventMessage, new UIEventInfo(id, position));
    }
        
    private void Send(string eventMessage, UIEventInfo eventInfo)
    {
        GetLastUIEventInfo = eventInfo;
        
        for (var eventListenerIndex = 0; eventListenerIndex < EventListenerGroup.Count; ++eventListenerIndex)
        {
            var eventListener = EventListenerGroup[eventListenerIndex];            
            var isListenableForBroadcast = eventListener.ListenableForBroadcast;
            var isCollidedEventListener = 
                eventInfo.Collider != null && eventListener.gameObject == eventInfo.Collider.gameObject;
            var hasEqualEventId = eventListener.EventInfo.IsActive && eventListener.EventInfo.Id == eventInfo.Id;

            if (isListenableForBroadcast 
                || isCollidedEventListener 
                || hasEqualEventId)
            {
                eventListener.SendMessage(eventMessage);                
            }
        }
    }

    #endregion

    #region <Structs>

    public struct UIEventInfo
    {
        public readonly int Id;
        public readonly Vector2 Position;
        public Collider2D Collider;
        public bool IsActive;
        
        public UIEventInfo(int id, Vector2 position)
        {
            Collider = null;
            Position = position;
            Id = id;
            IsActive = true;
        }
        
        public UIEventInfo(int id, Vector2 position, Collider2D collider)
        {
            Collider = collider;
            Position = position;
            Id = id;
            IsActive = true;
        }
        
    }

    #endregion </Structs>
}
