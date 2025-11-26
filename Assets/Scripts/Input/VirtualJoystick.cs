using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    [SerializeField] private RectTransform joystickArea;
    [SerializeField] private RectTransform joystick;
    [SerializeField] private float maxRadius = 120f;
    [SerializeField] private float deadZone = 0.1f;

    public Vector2 Direction => _direction;
    public bool IsPressed => _isPressed;

    public event Action<Vector2> DirectionChanged;

    private RectTransform _area;
    private RectTransform _joystickParent;
    private Camera _uiCamera;
    private Vector2 _origin;
    private Vector2 _direction;
    private bool _isPressed;

    private void Awake() {
        _area = joystickArea != null ? joystickArea : transform as RectTransform;
        if (_area == null) {
            enabled = false;
            return;
        }

        if (joystick == null) {
            enabled = false;
            return;
        }

        _joystickParent = joystick.parent as RectTransform;
        if (_joystickParent == null) {
            enabled = false;
            return;
        }

        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) {
            _uiCamera = canvas.worldCamera;
        }

        joystick.gameObject.SetActive(false);
        maxRadius = Mathf.Max(1f, maxRadius);
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (!TryGetLocalPoint(eventData.position, out _origin)) {
            return;
        }

        joystick.anchoredPosition = AreaToJoystickParent(_origin);
        joystick.gameObject.SetActive(true);
        _isPressed = true;
        UpdateDirection(Vector2.zero);
    }

    public void OnDrag(PointerEventData eventData) {
        if (!_isPressed) {
            return;
        }

        if (!TryGetLocalPoint(eventData.position, out var localPoint)) {
            return;
        }

        var delta = localPoint - _origin;
        var clamped = Vector2.ClampMagnitude(delta, maxRadius);
        joystick.anchoredPosition = AreaToJoystickParent(_origin + clamped);
        UpdateDirection(clamped / maxRadius);
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (!_isPressed) {
            return;
        }

        _isPressed = false;
        joystick.gameObject.SetActive(false);
        UpdateDirection(Vector2.zero);
    }

    private bool TryGetLocalPoint(Vector2 screenPoint, out Vector2 localPoint) {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(_area, screenPoint, _uiCamera, out localPoint);
    }

    private void UpdateDirection(Vector2 value) {
        if (value.sqrMagnitude < deadZone * deadZone) {
            value = Vector2.zero;
        }

        if (_direction == value) {
            return;
        }

        _direction = value;
        DirectionChanged?.Invoke(_direction);
    }

    private Vector2 AreaToJoystickParent(Vector2 areaPoint) {
        var worldPoint = _area.TransformPoint(areaPoint);
        var screenPoint = RectTransformUtility.WorldToScreenPoint(_uiCamera, worldPoint);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickParent, screenPoint, _uiCamera, out var parentPoint);
        return parentPoint;
    }
}

