using UnityEngine;
using UnityEngine.Assertions;

public class KnockbackReceiver : MonoBehaviour
{
    [SerializeField] private float _duration = 0.15f;
    [SerializeField] private MonoBehaviour[] _disableDuringKnockback;

    private Rigidbody2D _rb;
    private Vector2 _velocity;
    private float _timer;

    public bool IsActive => _timer > 0f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(_rb);

        foreach (var component in _disableDuringKnockback)
        {
            Assert.IsNotNull(component);
        }
    }

    private void FixedUpdate()
    {
        if (_timer <= 0f) return;

        _rb.linearVelocity = _velocity;
        _timer -= Time.fixedDeltaTime;

        if (_timer <= 0f)
        {
            _timer = 0f;
            SetComponentsEnabled(true);
        }
    }

    public void Apply(Vector2 force)
    {
        _velocity = force;
        _timer = _duration;
        _rb.linearVelocity = _velocity;
        SetComponentsEnabled(false);
    }

    private void SetComponentsEnabled(bool enabled)
    {
        foreach (var component in _disableDuringKnockback)
        {
            component.enabled = enabled;
        }
    }
}