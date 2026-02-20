using System;

public interface IHealth
{
    event Action<IHealth> OnDeath;

    float CurrentHealth { get; }
    bool IsDead { get; }

    void TakeDamage(float damage);
}
