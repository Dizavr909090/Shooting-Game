using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;

    private ObjectPool<Enemy> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Enemy>(
            createFunc: () => Instantiate(_enemyPrefab, transform),
            actionOnGet: (enemy) => {
                enemy.gameObject.SetActive(true);
                enemy.PoolableComponent.Reset();
            },
            actionOnRelease: (enemy) => enemy.gameObject.SetActive(false),
            actionOnDestroy: (enemy) => Destroy(enemy.gameObject),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 20
            );
    }

    public Enemy GetEnemy()
    {
        return _pool.Get();
    }

    public void Return(Enemy enemy)
    {
        if (enemy != null)
        {
            _pool.Release(enemy);
        }
    }
}
