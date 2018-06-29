using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(UISprite))]
public class ModernJoystickController : CustomUIEventListener
{
    [SerializeField] private Transform _focus;
    [SerializeField] private float _radius;
    [SerializeField] private float _fadeSpeed;
    
    private Transform _transform;
    private UISprite _uiSprite;
    private Vector3 _entryPoint;

    protected override void Awake()
    {
        base.Awake();
        _transform = GetComponent<Transform>();
        _uiSprite = GetComponent<UISprite>();
    }

    private void Update()
    {
        if (IsEventHoldOn)
        {
            if (_uiSprite.alpha < 1.0f)
                _uiSprite.alpha = Mathf.Min(1.0f, _uiSprite.alpha + Time.deltaTime * _fadeSpeed);
        }
        else
        {
            if (_uiSprite.alpha > Mathf.Epsilon)
                _uiSprite.alpha = Mathf.Max(0.0f, _uiSprite.alpha - Time.deltaTime * _fadeSpeed * 2.0f);
        }
    }

    private void LateUpdate()
    {
        if (!IsEventHoldOn) return;

        var destination = GetEventPosition();
        var distance = Vector3.Distance(_entryPoint, destination);
        var normVector = (destination - _entryPoint).normalized;

        if (distance <= _radius)
        {
            _transform.position = destination;
        }
        else
        {
            _transform.position = _entryPoint + normVector * _radius;
        }

        var forceVector = Mathf.Min(_radius, distance) * normVector / _radius;
        print(forceVector);
    }

    protected override void OnClickBegan(CustomUIEventCaster.UIEventInfo eventInfo)
    {
        if (eventInfo.Collider != null || EventInfo != null) return;
        
        EventInfo = eventInfo;
        _entryPoint = GetEventPosition();
    }

    protected override void OnClickEnded(CustomUIEventCaster.UIEventInfo eventInfo)
    {
        if (EventInfo == null || EventInfo.Id != eventInfo.Id) return;        
        
        EventInfo = null;
    }        
}