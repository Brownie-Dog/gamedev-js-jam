using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Iteractables
{
    public class StageTransitionInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform _targetSpawnPoint;
        [SerializeField] private FadeTransition _fadeTransition;

        private void Awake()
        {
            Assert.IsNotNull(_targetSpawnPoint);
            Assert.IsNotNull(_fadeTransition);
        }

        public void Interact()
        {
            _fadeTransition.Play(() => TeleportPlayer());
        }

        private void TeleportPlayer()
        {
            var player = GameObject.FindGameObjectWithTag(GlobalConstants.PLAYER_TAG);
            if (player != null)
            {
                player.transform.position = _targetSpawnPoint.position;
            }

            DeathUI.CurrentRespawnPoint = _targetSpawnPoint;
        }
    }
}
