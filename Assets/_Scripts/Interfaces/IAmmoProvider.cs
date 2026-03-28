public interface IAmmoProvider
{
    
    int CurrentAmmoInMagazine { get; }
    bool HasReserveAmmo { get; }
    bool IsReloading { get; }
    bool IsMagazineEmpty { get; }
    bool HasAnyAmmo { get; }
    bool NeedsReload { get; }
}
