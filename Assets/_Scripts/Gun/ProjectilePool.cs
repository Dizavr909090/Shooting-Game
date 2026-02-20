using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Projectile _projectilePrefab;

    private ObjectPool<Projectile> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Projectile>(
            createFunc: () => Instantiate(_projectilePrefab, transform),
            actionOnGet: (projectile) => {
                projectile.gameObject.SetActive(true);
                projectile.PoolableComponent.Reset();
            },
            actionOnRelease: (projectile) => projectile.gameObject.SetActive(false),
            actionOnDestroy: (projectile) => Destroy(projectile.gameObject),
            collectionCheck: true,
            defaultCapacity: 50,
            maxSize: 100
            );
    }

    public Projectile GetProjectile()
    {
        Projectile projectile = _pool.Get();

        return projectile;
    }

    public void Return(Projectile projectile)
    {
        if (projectile != null)
        {
            _pool.Release(projectile);
        }
    }
}


