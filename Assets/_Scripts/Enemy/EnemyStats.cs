using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Core Properties")]
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }

    [Header("Movement")]
    [field: SerializeField] public float MoveSpeed { get; private set; }

    [Header("Rotation")]
    [field: SerializeField] public float RotationSpeed { get; private set; }
    [field: SerializeField] public float RotationTreshold { get; private set; }

    [Header("Melee Combat")]
    [field: SerializeField] public float MeleeAttackRange { get; private set; } = .5f;
    [field: SerializeField] public float DistanceTolerance { get; private set; } = .5f;
    [field: SerializeField] public float TimeBetweenMeleeAttacks { get; private set; } = 1f;
    [field: SerializeField] public float MeleeAttackDelay { get; private set; } = 1f;
    [field: SerializeField] public float MeleeAttackDamage { get; private set; } = 1f;
    [field: SerializeField] public float MeleeAttackSpeed { get; private set; } = 3f;

    [Header("Ranged Combat")]
    [field: SerializeField] public float RangedAttackDistanceMin { get; private set; }
    [field: SerializeField] public float RangedAttackDistanceMax { get; private set; }

    [Header("Perception")]
    [field: SerializeField] public float ChaseRangeMax { get; private set; }
    [field: SerializeField] public float ViewAngle { get; private set; }
    [field: SerializeField] public float MemoryOfPlayerDuration { get; private set; } = 1;
    [field: SerializeField] public float PreferredDistance { get; private set; }
}
