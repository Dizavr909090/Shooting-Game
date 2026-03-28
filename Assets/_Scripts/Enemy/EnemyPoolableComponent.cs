using UnityEngine;

public class EnemyPoolableComponent : MonoBehaviour, IPoolableComponent
{
    [SerializeField] private HealthComponent _health;
    [SerializeField] private EnemyMovement _movement;
    [SerializeField] private EnemyAttack _attack;
    [SerializeField] private StateMachine _stateMachine;

    private void Awake()
    {
        GetComponents();
    }

    public void Reset()
    {
        _health.ResetHealth();
        _movement.ResetMovement();
        _attack.ResetAttack();
        _stateMachine.ResetLogic();
    }

    private void GetComponents()
    {
        _health = GetComponent<HealthComponent>();
        _movement = GetComponent<EnemyMovement>();
        _attack = GetComponent<EnemyAttack>();
        _stateMachine = GetComponent<StateMachine>();
    }
}
