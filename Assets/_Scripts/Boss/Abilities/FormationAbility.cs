using UnityEngine;

public class FormationAbility : BaseAbility<PlateFormationSO>
{
    public FormationAbility(PlateFormationSO data, IAbilityUser abilityUser) : base(data, abilityUser)
    {
    }

    public override void Execute()
    {
        for (int i = 0; i < Data.PlateAngles.Length; i++)
        {
            float angle = Data.PlateAngles[i];

            Vector3 offset = (Data.WeaponOffsets != null && i < Data.WeaponOffsets.Length)
            ? Data.WeaponOffsets[i]
            : new Vector3(0, 0, 1f);

            var command = new MovePlateCommand(i, angle, offset);
            _abilityUser.ExecuteCommand(command);
        }     
    }
}
