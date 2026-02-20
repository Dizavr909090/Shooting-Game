using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IHealth, IDamageable
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;

    private bool _isDead;

    public float CurrentHealth => _currentHealth;

    public bool IsDead => _isDead;

    public event Action<IHealth> OnDeath;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0 && !_isDead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    protected void Die()
    {
        _isDead = true;
        StopAllCoroutines();
        OnDeath?.Invoke(this);
    }
}
