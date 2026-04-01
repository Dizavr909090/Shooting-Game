using UnityEngine;

public abstract class BaseModule : MonoBehaviour, IActorModule
{
    protected IModuleRegistry _moduleController;

    [field: SerializeField] public int ID {  get; private set; }

    protected virtual void OnEnable()
    {
        _moduleController = GetComponentInParent<IModuleRegistry>();
        _moduleController?.RegistryModule(this);
    }

    protected virtual void OnDisable()
    {
        _moduleController?.UnRegistryModule(this);
    }

    public abstract void HandleCommand(ICommand command);
}
