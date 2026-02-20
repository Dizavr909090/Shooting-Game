using System;
using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public event Action OnAttackStarted;
    public event Action OnAttackFinished;


    private ITargetable _target;
    private EnemyStats _stats;
    private Coroutine _attackCoroutine;

    public bool IsAttacking { get; private set; }

    public void Initialize(ITargetable target, EnemyStats stats)
    {
        if (stats == null)
        {
            Debug.LogError("NO COMPONENTS");
            return;
        }

        _target = target;
        _stats = stats;
    }

    public void UpdateTarget(ITargetable newTarget)
    {
        _target = newTarget;
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
    
        _target = null;
        IsAttacking = false;
    }

    public IEnumerator AttackRoutine()
    {
        if (_target == null || _target.Transform == null) yield break;

        IsAttacking = true;
      
        OnAttackStarted?.Invoke();

        Vector3 startPosition = transform.position;

        Vector3 targetPosition = _target.Transform.position;
        Vector3 dirToTarget = (targetPosition - startPosition).normalized;
        targetPosition = targetPosition - dirToTarget * (_stats.AttackDistanceThreshold);

        float attackPercent = 0f;

        bool hasAppliedDamage = false;

        while (attackPercent <= 1)
        {
            attackPercent += Time.deltaTime * _stats.AttackSpeed;

            float interpolation = (-Mathf.Pow(attackPercent, 2) + attackPercent) * 4;
            transform.position = Vector3.Lerp(startPosition, targetPosition, interpolation);

            if (attackPercent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;

                if (_target.Transform.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(_stats.Damage);
                }
            }

            yield return null;
        }

        transform.position = startPosition;

        OnAttackFinished?.Invoke();

        yield return new WaitForSeconds(_stats.TimeBetweenAttacks);

        IsAttacking = false;
    }
}
