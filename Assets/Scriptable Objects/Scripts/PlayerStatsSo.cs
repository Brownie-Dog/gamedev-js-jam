using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats")]
public class PlayerStatsSo : ScriptableObject
{
    public float MovementSpeed = 5f;
    public int MaxHealth = 100;
}
