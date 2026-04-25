using UnityEngine;
using UnityEngine.Assertions;

namespace Enemy.Bosses
{
    [RequireComponent(typeof(TargetJoint2D))]
    public class JointPlayerFollow : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private TargetJoint2D _joint;

        private void Start()
        {
            _joint = GetComponent<TargetJoint2D>();
            Assert.IsNotNull(_joint);
        }

        private void Update()
        {
            _joint.target = _player.position;
        }
    }
}