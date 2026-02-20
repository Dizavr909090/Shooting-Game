using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [SerializeField] private Material _material;

    [SerializeField] private float _maxHealth;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed;

    [Header("Attack Settings")]
    [SerializeField] private float _attackDistanceThreshold = .5f;
    [SerializeField] private float _attackDistanceTolerance = .1f;
    [SerializeField] private float _timeBetweenAttacks = 1;
    [SerializeField] private float _damage = 1;
    [SerializeField] private float _attackSpeed = 3;

    public float MaxHealth => _maxHealth;
    public float MoveSpeed => _moveSpeed;
    public float AttackDistanceThreshold => _attackDistanceThreshold;
    public float AttackDistanceTolerance => _attackDistanceTolerance;
    public float TimeBetweenAttacks => _timeBetweenAttacks;
    public float Damage => _damage;
    public float AttackSpeed => _attackSpeed;
}
