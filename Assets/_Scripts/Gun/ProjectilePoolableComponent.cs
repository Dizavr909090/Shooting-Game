using UnityEngine;

public class ProjectilePoolableComponent : MonoBehaviour, IPoolableComponent
{
    [SerializeField] private Projectile _projectile;

    private void Awake()
    {
        if ( _projectile == null )
            _projectile = GetComponent<Projectile>();
    }

    public void Reset()
    {
        _projectile.ResetLogic();
    }
}
