using System.Collections;
using UnityEngine;

namespace Weapons
{
    public class NoOpWeaponBehaviour : MonoBehaviour, IWeaponBehaviour
    {
        public IEnumerator DoAttack()
        {
            yield break;
        }
    }
}