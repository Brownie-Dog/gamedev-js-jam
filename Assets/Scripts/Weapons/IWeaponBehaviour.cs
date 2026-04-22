using System.Collections;

namespace Weapons
{
    public interface IWeaponBehaviour
    {
        IEnumerator DoAttack();
    }
}