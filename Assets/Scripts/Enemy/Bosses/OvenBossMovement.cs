using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.Bosses
{
    public class OvenBossMovement : EnemyMovement
    {
        protected override void ChasePlayer()
        {
        }

        protected override void PickNewWanderDirection()
        {
            float x = Random.value < 0.5f ? -1f : 1f;
            _wanderDirection = new Vector2(x, 0f);
            _wanderMoveTimer = _stats.WanderMoveDuration;
        }

        protected override void HandlePlayerDetected(object sender, EventArgs e)
        {
        }

        protected override void HandlePlayerLost(object sender, EventArgs e)
        {
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if ((_stats.WallLayer & (1 << col.gameObject.layer)) != 0)
            {
                _wanderDirection = -_wanderDirection;
                _wanderState = WanderState.Moving;
            }
        }
    }
}