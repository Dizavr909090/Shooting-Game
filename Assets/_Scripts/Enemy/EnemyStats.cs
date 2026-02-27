using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Core Properties")]
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }

    [Header("Movement")]
    [field: SerializeField] public float MoveSpeed { get; private set; }

    [Header("Melee Combat")]
    [field: SerializeField] public float AttackDistanceThreshold { get; private set; } = .5f;
    [field: SerializeField] public float AttackDistanceTolerance { get; private set; } = .1f;
    [field: SerializeField] public float TimeBetweenAttacks { get; private set; } = 1f;
    [field: SerializeField] public float Damage { get; private set; } = 1f;
    [field: SerializeField] public float AttackSpeed { get; private set; } = 3f;

    [Header("Ranged Combat")]
    [field: SerializeField] public float RangedAttackDistanceMin { get; private set; }
    [field: SerializeField] public float RangedAttackDistanceMax { get; private set; }

    [Header("Perception")]
    [field: SerializeField] public float ChaseRangeMax { get; private set; }
    [field: SerializeField] public float ViewAngle { get; private set; }
    [field: SerializeField] public float MemoryOfPlayerDuration { get; private set; } = 1;
    [field: SerializeField] public float PreferredDistance { get; private set; }
}
