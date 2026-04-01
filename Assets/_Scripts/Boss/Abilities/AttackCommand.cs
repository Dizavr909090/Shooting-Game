using UnityEngine;

public class AttackCommand : ICommand
{
    public float Damage { get; }
    public float Speed { get; }

    public AttackCommand(float damage, float speed)
    {
        Damage = damage;
        Speed = speed;
    }
}
