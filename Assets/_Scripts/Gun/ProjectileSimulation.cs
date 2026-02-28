using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSimulation : MonoBehaviour
{
    public event Action<ProjectileData, ProjectileVisual> ProjectileHit; 

    [SerializeField] private LayerMask collisionMask;

    public static ProjectileSimulation Instance { get; private set; }

    private List<ProjectileInstance> _activeProjectiles = new();

    private void Awake() => Instance = this;

    private void Update()
    {
        for (int i = _activeProjectiles.Count - 1; i >= 0; i--)
        {
            var projInstance = _activeProjectiles[i];

            float moveDistance = projInstance.Speed * Time.deltaTime;
            
            bool isRaycast = Physics.Raycast(
                projInstance.Position,
                projInstance.DirectionNormalized,
                out RaycastHit hit,
                moveDistance,
                collisionMask);

            if (isRaycast)
            {
                hit.collider.GetComponent<IDamageable>()?.TakeHit(projInstance.ProjectileData.Damage, hit);
                ProjectileHit?.Invoke(projInstance.ProjectileData, projInstance.ProjectileVisual);
                _activeProjectiles.RemoveAt(i);
                continue;
            }
            else
            {
                projInstance.UpdatePosition(Time.deltaTime);
                projInstance.ProjectileVisual.transform.position = projInstance.Position;
                projInstance.ProjectileVisual.transform.forward = projInstance.Velocity;
                projInstance.ReduceLifeTime(Time.deltaTime);
                
                if(projInstance.TimeRemaining <= 0)
                {
                    ProjectileHit?.Invoke(projInstance.ProjectileData, projInstance.ProjectileVisual);
                    _activeProjectiles.RemoveAt(i);
                }
            }
        }
    }

    public void AddProjectile(
        Vector3 startPos,
        Vector3 direction,
        float speed,
        ProjectileData data,
        ProjectileVisual visual)
    {
        ProjectileInstance projInstance = new(
            startPos,
            direction,
            speed,
            data,
            visual
            );
        _activeProjectiles.Add(projInstance);
    }
}
