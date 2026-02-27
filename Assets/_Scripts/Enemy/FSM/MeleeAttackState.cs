public class MeleeAttackState : BaseState
{
    private EnemyMovement _movement;
    private EnemyStats _stats;
    private EnemyAttack _attack;

    public MeleeAttackState(
        StateMachine stateMachine, 
        EnemyMovement movement, 
        EnemyStats stats,
        EnemyAttack attack) : base(stateMachine)
    {
        _movement = movement;
        _stats = stats;
        _attack = attack;
    }

    public override void OnEnter()
    {
        _movement.StopMoving();
        _movement.SetKinematic(true);
    }

    public override void Update()
    {
        if (_attack.IsAttacking) return;

        if (_movement.DistanceToTarget >= _stats.AttackDistanceThreshold + _stats.AttackDistanceTolerance)
        {
            _stateMachine.SwitchState<ChaseState>();
            return;
        }

        _attack.PerformAttack();
    }

    public override void OnExit()
    {
        _movement.SetKinematic(false);
    }
}
