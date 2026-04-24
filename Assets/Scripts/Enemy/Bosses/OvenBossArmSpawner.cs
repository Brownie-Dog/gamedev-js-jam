using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    public class OvenBossArmSpawner : MonoBehaviour
    {
        [SerializeField] private OvenBossArm _leftArm;
        [SerializeField] private OvenBossArm _rightArm;

        public OvenBossArm LeftArm => _leftArm;
        public OvenBossArm RightArm => _rightArm;

        private void Awake()
        {
            Assert.IsNotNull(_leftArm);
            Assert.IsNotNull(_rightArm);
        }

        public void RefreshArms()
        {
            _leftArm.Initialize(_leftArm.DefaultSegmentCount, _leftArm.DefaultHandPrefab);
            _rightArm.Initialize(_rightArm.DefaultSegmentCount, _rightArm.DefaultHandPrefab);
        }
    }
}