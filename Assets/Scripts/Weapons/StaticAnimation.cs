using System.Collections;
using UnityEngine;

namespace Weapons
{
    public class StaticAnimation : MonoBehaviour, IWeaponAnimation
    {
        public IEnumerator PlayAnimation()
        {
            yield return null;
        }
    }
}