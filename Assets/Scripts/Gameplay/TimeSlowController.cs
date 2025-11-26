using UnityEngine;

public class TimeSlowController : MonoBehaviour {
    [SerializeField] private float slowScale = 0.5f;
    [SerializeField] private float transitionSpeed = 5f;

    private float _defaultFixedDelta;
    private float _targetScale = 1f;

    private void Awake() {
        _defaultFixedDelta = Time.fixedDeltaTime;
        _targetScale = Time.timeScale;
    }

    private void OnEnable() {
        ApplyScale(1f, true);
    }

    private void OnDisable() {
        ApplyScale(1f, true);
    }

    private void Update() {
        if (Mathf.Approximately(Time.timeScale, _targetScale)) {
            return;
        }

        var newScale = Mathf.Lerp(Time.timeScale, _targetScale, transitionSpeed * Time.unscaledDeltaTime);
        ApplyScale(newScale, false);
    }

    public void BeginSlow() {
        _targetScale = Mathf.Clamp(slowScale, 0.01f, 1f);
    }

    public void EndSlow() {
        _targetScale = 1f;
    }

    private void ApplyScale(float scale, bool instant) {
        Time.timeScale = instant ? scale : Mathf.Clamp(scale, 0.01f, 1f);
        Time.fixedDeltaTime = _defaultFixedDelta * Time.timeScale;
    }
}

