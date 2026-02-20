using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;

    private ObjectPool<Enemy> _enemyPool;

    private void Awake()
    {
        _enemyPool = new ObjectPool<Enemy>(
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
        Enemy enemy = _enemyPool.Get();

        return enemy;
    }

    public void Return(Enemy enemy)
    {
        if (enemy != null)
        {
            _enemyPool.Release(enemy);
        }
    }
}
