public class EnemyPoolable : PoolableComponent
{
    private Enemy _enemy;
    private HealthComponent _health;
    private EnemyMovement _movement;
    private EnemyAttack _attack;
    private EnemyStateMachine _stateMachine;

    private void Awake()
    {
        GetComponents();
    }

    public override void Reset()
    {
        _health.ResetHealth();

        _movement.ResetMovement();
        _attack.ResetAttack();
        _stateMachine.ResetLogic();
    }

    private void GetComponents()
    {
        _enemy = GetComponent<Enemy>();
        _health = GetComponent<HealthComponent>();
        _movement = GetComponent<EnemyMovement>();
        _attack = GetComponent<EnemyAttack>();
        _stateMachine = GetComponent<EnemyStateMachine>();
    }
}
