using System.Collections;
using UnityEngine;

public class FormationAttackAbility : BaseAbility<FormationAttackAbilitySO>
{
    public FormationAttackAbility(FormationAttackAbilitySO data, IAbilityUser abilityUser) : base(data, abilityUser) { }

    public override void Execute()
    {
        var formation = Data.Formation;

        for (int i = 0; i < formation.PlateAngles.Length; i++)
        {
            Vector3 offset = (formation.WeaponOffsets != null && i < formation.WeaponOffsets.Length)
                ? formation.WeaponOffsets[i]
                : new Vector3(0, 0, 1f);

            _abilityUser.ExecuteCommand(new MovePlateCommand(i, formation.PlateAngles[i], offset));
        }

        if (_abilityUser is MonoBehaviour bossMono)
        {
            bossMono.StartCoroutine(WaitAndFire());
        }
    }

    public override void Stop()
    {
        _abilityUser.ExecuteCommand(new ShootCommand(-1, false));
    }

    private IEnumerator WaitAndFire()
    {
        yield return new WaitUntil(() => _abilityUser.AllModulesReady());

        _abilityUser.ExecuteCommand(new ShootCommand(-1, true));
    }
}
