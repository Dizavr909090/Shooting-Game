using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyAttack))]
[RequireComponent(typeof(EnemyVisuals))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(EnemyPoolableComponent))]
[RequireComponent(typeof(AlwaysKnowDetector))]
[RequireComponent(typeof(GunController))]
[RequireComponent(typeof(EnemyRotator))]
public class Enemy : MonoBehaviour, IFractionProvider
{
    [SerializeField] private EnemyStats _stats;

    private StateMachine _stateMachine;
    private EnemyMovement _movement;
    private EnemyAttack _attack;
    private EnemyVisuals _visuals;
    private ITargetProvider _targetProvider;
    private HealthComponent _healthComponent;
    private EnemyPoolableComponent _poolableComponent;
    private IShootable _shootable;
    private EnemyRotator _rotator;
    private NavMeshAgent _agent;

    public HealthComponent Health => _healthComponent;
    public EnemyPoolableComponent PoolableComponent => _poolableComponent;
    public FractionRelationsConfig.FractionType FractionType => _stats.FractionType;

    private void Awake()
    {
        GetComponents();

        _visuals.Initialize(_attack);
        _movement.Initialize(_targetProvider, _agent);
        _attack.Initialize(_targetProvider, _stats);
        _stateMachine.Initialize(_movement, _attack, _stats, _healthComponent, _targetProvider, _shootable, _rotator);  
        _rotator.Initialize(_stats, _agent);
    }

    public void Activate(ITargetable target, Vector3 startPosition)
    {
        gameObject.SetActive(true);
        _stateMachine.ResetLogic();
        TeleportTo(startPosition);
    }

    public void TeleportTo(Vector3 position)
    {
        _agent.enabled = false;
        float heightOffset = _agent.baseOffset;
        position.y += heightOffset;
        transform.position = position;
        _agent.enabled = true;
    }

    private void GetComponents()
    {
        _movement = GetComponent<EnemyMovement>();
        _attack = GetComponent<EnemyAttack>();
        _agent = GetComponent<NavMeshAgent>();
        _visuals = GetComponent<EnemyVisuals>();
        _stateMachine = GetComponent<StateMachine>();
        _targetProvider = GetComponent<ITargetProvider>();
        _shootable = GetComponentInChildren<IShootable>();
        _rotator = GetComponent<EnemyRotator>();

        _healthComponent = GetComponent<HealthComponent>();
        _poolableComponent = GetComponent<EnemyPoolableComponent>();
    }
}
