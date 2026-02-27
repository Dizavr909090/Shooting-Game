using System.Collections;
using UnityEngine;

public class ProjectilePoolableComponent : MonoBehaviour, IPoolableComponent
{
    private ProjectileVisual _projectile;

    private void Awake()
    {
        if ( _projectile == null )
            _projectile = GetComponent<ProjectileVisual>();
    }

    public void Reset()
    {
        StopAllCoroutines();

        _projectile.ResetLogic();
        
        if (_projectile.Trail != null)
        {
            _projectile.Trail.emitting = false;
            _projectile.Trail?.Clear();
        }
    }

    public void ActivateTrail()
    {
        StartCoroutine(EnableTrailRoutine());
    }

    private IEnumerator EnableTrailRoutine()
    {
        if (_projectile.Trail == null) yield break;

        yield return new WaitForEndOfFrame();

        _projectile.Trail.emitting = true;
    }
}
