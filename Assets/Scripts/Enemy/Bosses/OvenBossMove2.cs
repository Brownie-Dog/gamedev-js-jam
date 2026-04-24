using UnityEngine;

namespace Enemy.Bosses
{
    public class OvenBossMove2 : MonoBehaviour, IOvenBossMove
    {
        public bool IsComplete { get; private set; }

        public void Execute(Transform boss, Transform player)
        {
            IsComplete = false;
        }
    }
}