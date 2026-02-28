using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileProvider : MonoBehaviour
{
    private Dictionary<ProjectileData, ObjectPool<ProjectileVisual>> _projectilePools = new();

    public static ProjectileProvider Instance { get; private set; }

    private void Awake() => Instance = this;

    private void Start()
    {
        ProjectileSimulation.Instance.ProjectileHit += ReturnProjectile;
    }

    private void OnDisable()
    {
        if (ProjectileSimulation.Instance != null)
        {
            ProjectileSimulation.Instance.ProjectileHit -= ReturnProjectile;
        }
    }

    public ProjectileVisual GetProjectile(ProjectileData projData)
    {
        if (!_projectilePools.ContainsKey(projData))
            CreatePoolForData(projData);

        return _projectilePools[projData].Get();
    }

    public void ReturnProjectile(ProjectileData projData, ProjectileVisual projectile)
    {
        if (projectile != null && projData != null)
        {
            _projectilePools[projData].Release(projectile);
        }
    }

    private ObjectPool<ProjectileVisual> CreatePoolForData(ProjectileData projData)
    {
        _projectilePools[projData] = new ObjectPool<ProjectileVisual>(
            createFunc: () => Instantiate(projData.projPrefab, transform),
            actionOnGet: (proj) => {
                proj.gameObject.SetActive(true);
                proj.PoolableComponent.Reset();
            },
            actionOnRelease: (proj) => proj.gameObject.SetActive(false),
            actionOnDestroy: (proj) => Destroy(proj.gameObject),
            collectionCheck: true,
            defaultCapacity: 50,
            maxSize: 100
            );
        return _projectilePools[projData];
    }
}
