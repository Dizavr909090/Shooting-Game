using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public class Composite_Ability : Base_Ability<CompositeAbility_SO>
{
    private List<Base_Action> _sequence = new();

    private CancellationTokenSource _cts;

    public Composite_Ability(CompositeAbility_SO data, IAbilityUser abilityUser) : base(data, abilityUser) { }

    public void AddAction(Base_Action action) => _sequence.Add(action);

    public override async void Execute()
    {
        _cts = new CancellationTokenSource();
        await ExecuteSequence(_cts.Token);
    }

    public override void Stop()
    {
        _cts?.Cancel();
        _abilityUser.ExecuteCommand(new ShootCommand(-1, false));
    }

    private async UniTask ExecuteSequence(CancellationToken token)
    {
        foreach (var action in _sequence)
        {
            if (token.IsCancellationRequested) return;

            await action.Execute().AttachExternalCancellation(token);
        }
    }
}
