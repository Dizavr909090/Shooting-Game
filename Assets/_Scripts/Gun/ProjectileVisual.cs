using System.Collections;
using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    private ProjectilePoolableComponent _poolableComponent;
    private TrailRenderer _trail;

    public ProjectilePoolableComponent PoolableComponent => _poolableComponent;
    public TrailRenderer Trail => _trail;

    private void Awake()
    {
        _poolableComponent = GetComponent<ProjectilePoolableComponent>();

        if (_trail == null)
            _trail = GetComponent<TrailRenderer>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ResetLogic()
    {
        
    }
}
