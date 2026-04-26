using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Resets the entire stage: reactivates all enemies and resets all boss rooms.
    /// Call this when the player dies (before teleporting them to the respawn point).
    /// </summary>
    public void ResetStage()
    {
        ReactivateAllEnemies();
        ResetAllBossRooms();
    }

    private void ReactivateAllEnemies()
    {
        // Find all EnemyHealth components in the scene, including inactive ones.
        // This covers both pre-placed enemies and boss sub-components.
        var allEnemies = FindObjectsByType<EnemyHealth>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var enemy in allEnemies)
        {
            if (enemy == null) continue;
            enemy.gameObject.SetActive(true);
        }
    }

    private void ResetAllBossRooms()
    {
        var bossRooms = FindObjectsByType<BossRoom>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var room in bossRooms)
        {
            if (room != null)
                room.ResetRoom();
        }
    }
}
