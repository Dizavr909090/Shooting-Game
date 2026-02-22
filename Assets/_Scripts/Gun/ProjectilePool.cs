using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Projectile _projectilePrefab;

    private ObjectPool<Projectile> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Projectile>(
            createFunc: () => {
                var proj = Instantiate(_projectilePrefab);
                proj.SetPool(this);
                return proj;
            },
            actionOnGet: (proj) => {
                proj.transform.SetParent(null);
                proj.gameObject.SetActive(true);
                proj.PoolableComponent.Reset();
            },
            actionOnRelease: (proj) => {
                proj.gameObject.SetActive(false);
                proj.transform.SetParent(transform);
            },
            actionOnDestroy: (proj) => Destroy(proj.gameObject),
            collectionCheck: true,
            defaultCapacity: 50,
            maxSize: 100
            );
    }

    public Projectile GetProjectile()
    {
        return _pool.Get();
    }

    public void Return(Projectile projectile)
    {
        if (projectile != null)
        {
            _pool.Release(projectile);
        }
    }
}


