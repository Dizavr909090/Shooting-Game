using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GunEventChannelSO", menuName = "Scriptable Objects/GunEventChannelSO")]
public class GunEventChannelSO : ScriptableObject
{
    public event Action<Gun> EventRaised;

    public Gun CurrentGun { get; private set; }

    private void OnEnable() => CurrentGun = null;

    public void RaiseEvent(Gun gun)
    {
        CurrentGun = gun;
        EventRaised?.Invoke(gun);
    }
}
