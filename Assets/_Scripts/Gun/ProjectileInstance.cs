using UnityEngine;

public class ProjectileInstance
{
    public Vector3 Position { get; private set; }
    public Vector3 Velocity { get; private set; }
    public ProjectileData ProjectileData { get; private set; }
    public ProjectileVisual ProjectileVisual { get; private set; }
    public float TimeRemaining { get; private set; }
    public float Speed { get; private set; }
    public Vector3 DirectionNormalized { get; private set; }
    public LayerMask CollisionToHit { get; private set; }
    public FractionRelationsConfig.FractionType ShooterFraction { get; private set; }


    public ProjectileInstance(
        Vector3 startPos, 
        Vector3 direction, 
        float speed, 
        ProjectileData data, 
        ProjectileVisual visual,
        FractionRelationsConfig.FractionType shooterFraction)
    {
        Position = startPos;
        DirectionNormalized = direction.normalized;
        Velocity = direction * speed;
        Speed = speed;
        ProjectileData = data;
        ProjectileVisual = visual;
        TimeRemaining = data.LifeTime;
        ShooterFraction = shooterFraction;
    }

    public void UpdatePosition(float timeToUpdate)
    {
        Position += Velocity * timeToUpdate;
    }

    public void ReduceLifeTime(float deltaTime)
    {
        TimeRemaining -= deltaTime;
    }
}
