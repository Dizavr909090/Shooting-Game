using Cysharp.Threading.Tasks;
using UnityEngine;

public class MovePlateAction : Base_Action<PlateFormation_SO>
{
    public MovePlateAction(PlateFormation_SO data, IAbilityUser abilityUser) : base(data, abilityUser){}

    public override async UniTask Execute()
    {
        for (int i = 0; i < Data.PlateAngles.Length; i++)
        {
            float angle = Data.PlateAngles[i];
            Vector3 offset = (Data.WeaponOffsets != null && i < Data.WeaponOffsets.Length)
                ? Data.WeaponOffsets[i]
                : new Vector3(0, 0, 1f);

            _abilityUser.ExecuteCommand(new MovePlateCommand(i, angle, offset));
        }

        await new WaitUntil(() => _abilityUser.AllModulesReady());
    }
}
