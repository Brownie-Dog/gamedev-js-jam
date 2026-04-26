using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BossRoom : MonoBehaviour
{
    [Header("Doors")]
    [Tooltip("Drag door GameObjects here, OR parent them under this object and tag them 'Door'.")]
    [SerializeField] private List<GameObject> _doors;

    [Header("Boss")]
    [SerializeField] private EnemyHealth _bossHealth;

    [Header("Optional")]
    [Tooltip("If true, doors start closed even before player enters.")]
    [SerializeField] private bool _startClosed;

    private bool _isCleared = false;
    private bool _isActive = false;

    private void Awake()
    {
        Assert.IsNotNull(_bossHealth, "BossRoom needs a Boss Health reference.");

        // Auto-collect doors tagged "Door" under this transform
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Door") && !_doors.Contains(child.gameObject))
            {
                _doors.Add(child.gameObject);
            }
        }
    }

    private void Start()
    {
        SetDoorsActive(_startClosed);
    }

    private void OnEnable()
    {
        _bossHealth.OnDeath += HandleBossDeath;
    }

    private void OnDisable()
    {
        _bossHealth.OnDeath -= HandleBossDeath;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (_isCleared) return;

        ActivateRoom();
    }

    private void ActivateRoom()
    {
        if (_isActive) return;
        _isActive = true;

        SetDoorsActive(true);
    }

    private void HandleBossDeath(object sender, EventArgs e)
    {
        _isCleared = true;
        _isActive = false;
        SetDoorsActive(false);
    }

    private void SetDoorsActive(bool active)
    {
        foreach (var door in _doors)
        {
            if (door != null)
                door.SetActive(active);
        }
    }

    /// <summary>
    /// Resets the boss room to its initial state (doors open, not cleared).
    /// Call this from StageManager when the player dies.
    /// </summary>
    public void ResetRoom()
    {
        _isCleared = false;
        _isActive = false;
        SetDoorsActive(false);
    }
}
