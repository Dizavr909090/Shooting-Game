using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyStateMachine))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyAttack))]
[RequireComponent(typeof(EnemyVisuals))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(EnemyPoolableComponent))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyStats _stats;

    private EnemyStateMachine _stateMachine;
    private EnemyMovement _movement;
    private EnemyAttack _attack;
    private EnemyVisuals _visuals;

    private HealthComponent _healthComponent;
    private EnemyPoolableComponent _poolableComponent;

    private NavMeshAgent _agent;

    private ITargetable _target;

    public HealthComponent Health => _healthComponent;
    public EnemyPoolableComponent PoolableComponent => _poolableComponent;

    private void Awake()
    {
        GetComponents();

        _visuals.Initialize(_attack);
        _movement.Initialize(_target, _stats, _agent);
        _attack.Initialize(_target, _stats);
        _stateMachine.Initialize(_movement, _attack, _stats, _healthComponent);  
    }

    public void Activate(ITargetable target, Vector3 startPosition)
    {
        SetTarget(target);
        TeleportTo(startPosition);
        gameObject.SetActive(true);
        _stateMachine.StartStateMachine();
    }

    public void TeleportTo(Vector3 position)
    {
        _agent.enabled = false;
        float heightOffset = _agent.baseOffset;
        position.y += heightOffset;
        transform.position = position;
        _agent.enabled = true;
    }

    public void SetTarget(ITargetable target)
    {
        if (target == null)
            Debug.LogError("NO target for SetTarget()");

        _target = target;
        _movement.UpdateTarget(target);
        _attack.UpdateTarget(target);
        _stateMachine.UpdateTarget(target);
    }

    private void GetComponents()
    {
        _movement = GetComponent<EnemyMovement>();
        _attack = GetComponent<EnemyAttack>();
        _agent = GetComponent<NavMeshAgent>();
        _visuals = GetComponent<EnemyVisuals>();
        _stateMachine = GetComponent<EnemyStateMachine>();

        _healthComponent = GetComponent<HealthComponent>();
        _poolableComponent = GetComponent<EnemyPoolableComponent>();
    }
}
