using UnityEngine;

public class ProjectilePoolableComponent : MonoBehaviour, IPoolableComponent
{
    [SerializeField] private Projectile _projectile;

    public void Reset()
    {
        _projectile.ResetLogic();
    }
}
