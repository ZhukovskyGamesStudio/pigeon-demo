using UnityEngine;

public class BulletController : MonoBehaviour {
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform forwardReference;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float rotationSpeed = 540f;
    [SerializeField] private float maxDeviationAngle = 45f;
    [SerializeField] private float lateralInfluence = 1f;
    [SerializeField] private float verticalInfluence = 0.5f;

    private Vector3 _currentDirection;
    private Vector3 _levelForward;
    private Vector3 _levelRight;

    private void Awake() {
        if (cameraTransform == null && Camera.main != null) {
            cameraTransform = Camera.main.transform;
        }

        InitializeLevelAxes();
    }

    private void OnEnable() {
        if (_levelForward == Vector3.zero) {
            InitializeLevelAxes();
        }

        _currentDirection = _levelForward;
        if (_currentDirection == Vector3.zero) {
            _currentDirection = transform.forward;
        }
        if (_currentDirection != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(_currentDirection);
        }
    }

    private void Update() {
        var targetDirection = GetTargetDirection();
        if (targetDirection != Vector3.zero) {
            var maxRadiansDelta = rotationSpeed * Mathf.Deg2Rad * Time.deltaTime;
            _currentDirection = Vector3.RotateTowards(_currentDirection, targetDirection, maxRadiansDelta, float.MaxValue);
            transform.rotation = Quaternion.LookRotation(_currentDirection);
        }

        transform.position += _currentDirection * moveSpeed * Time.deltaTime;
    }

    private Vector3 GetTargetDirection() {
        var baseForward = _levelForward;

        if (joystick == null || joystick.Direction.sqrMagnitude <= 0f) {
            return baseForward;
        }

        var desired = baseForward +
                      _levelRight * (joystick.Direction.x * lateralInfluence) +
                      Vector3.up * (joystick.Direction.y * verticalInfluence);
        if (desired.sqrMagnitude < 0.0001f) {
            return baseForward;
        }

        var desiredNormalized = desired.normalized;

        var limitRadians = Mathf.Max(0f, maxDeviationAngle) * Mathf.Deg2Rad;
        if (limitRadians <= 0f) {
            return baseForward;
        }

        var angle = Vector3.Angle(baseForward, desiredNormalized) * Mathf.Deg2Rad;
        if (angle <= limitRadians) {
            return desiredNormalized;
        }

        return Vector3.RotateTowards(baseForward, desiredNormalized, limitRadians, float.MaxValue);
    }

    private void InitializeLevelAxes() {
        Vector3 sourceForward;
        if (forwardReference != null) {
            sourceForward = forwardReference.forward;
        } else if (cameraTransform != null) {
            sourceForward = cameraTransform.forward;
        } else {
            sourceForward = transform.forward;
        }

        sourceForward.y = 0f;
        if (sourceForward == Vector3.zero) {
            sourceForward = Vector3.forward;
        }

        _levelForward = sourceForward.normalized;
        _levelRight = Vector3.Cross(Vector3.up, _levelForward);
        if (_levelRight == Vector3.zero) {
            _levelRight = Vector3.right;
        } else {
            _levelRight.Normalize();
        }
    }
}

