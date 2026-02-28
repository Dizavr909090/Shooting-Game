using UnityEngine;

public abstract class BaseDetector : MonoBehaviour, ITargetProvider
{
    protected ITargetable _currentTarget;
    protected bool _isVisible;

    public ITargetable Target => _currentTarget;
    public bool IsVisible => _isVisible;

    protected abstract void Detect();
}
