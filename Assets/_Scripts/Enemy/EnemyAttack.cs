using System;
using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public event Action OnAttackStarted;
    public event Action OnAttackFinished;

    private ITargetProvider _targetProvider;
    private EnemyStats _stats;
    private Coroutine _attackCoroutine;

    private ITargetable _target => _targetProvider?.Target;
    public bool IsAttacking { get; private set; }

    public void Initialize(ITargetProvider targetProvider, EnemyStats stats)
    {
        _targetProvider = targetProvider;
        _stats = stats;
    }

    public void PerformAttack()
    {
        if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
        _attackCoroutine = StartCoroutine(AttackRoutine());
    }

    public void ResetAttack()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    
        IsAttacking = false;
    }

    public IEnumerator AttackRoutine()
    {
        ITargetable currentTarget = _target;
        if (currentTarget == null || currentTarget.Transform == null) yield break;

        IsAttacking = true;
      
        OnAttackStarted?.Invoke();

        // анимация замаха тут!
        yield return new WaitForSeconds(_stats.MeleeAttackDelay);

        float distance = Vector3.Distance(transform.position, currentTarget.Transform.position);

        if (distance < _stats.MeleeAttackRange)
        {
            if (currentTarget.Transform.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(_stats.MeleeAttackDamage);
            }
        }

        OnAttackFinished?.Invoke();

        yield return new WaitForSeconds(_stats.TimeBetweenMeleeAttacks);

        IsAttacking = false;
    }
}
