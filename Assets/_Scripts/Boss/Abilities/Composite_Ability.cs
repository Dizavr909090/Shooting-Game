using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;

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
        _cts?.Dispose();
        _cts = null;

        foreach (var action in _sequence)
        {
            action.Cleanup();
        }
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
