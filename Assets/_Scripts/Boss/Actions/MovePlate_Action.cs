using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class MovePlateAction : Base_Action
{
    private PlateFormation_SO _formationData;

    public MovePlateAction(PlateFormation_SO data, IAbilityUser abilityUser) : base(abilityUser)
    {
        _formationData = data;
    }

    public override async UniTask Execute()
    {
        for (int i = 0; i < _formationData.PlateAngles.Length; i++)
        {
            float angle = _formationData.PlateAngles[i];
            Vector3 offset = (_formationData.WeaponOffsets != null && i < _formationData.WeaponOffsets.Length)
                ? _formationData.WeaponOffsets[i]
                : new Vector3(0, 0, 1f);

            _abilityUser.ExecuteCommand(new MovePlateCommand(i, angle, offset));
        }

        await new WaitUntil(() => _abilityUser.AllModulesReady());
    }
}
