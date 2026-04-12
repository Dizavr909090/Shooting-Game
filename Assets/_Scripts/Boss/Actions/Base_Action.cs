using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class Base_Action : MonoBehaviour
{
    [SerializeField] private BaseAction_SO _baseData;

    protected IAbilityUser _abilityUser;

    public BaseAction_SO BaseData => _baseData;

    protected Base_Action(BaseAction_SO data, IAbilityUser abilityUser)
    {
        _baseData = data;
        _abilityUser = abilityUser;
    }

    public virtual void Cleanup() { }

    public abstract UniTask Execute();
}

public abstract class Base_Action<T> : Base_Action where T : BaseAction_SO
{
    protected T Data => (T)BaseData;

    protected Base_Action(T data, IAbilityUser abilityUser) : base(data, abilityUser){}
}
