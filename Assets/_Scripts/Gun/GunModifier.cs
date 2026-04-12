[System.Serializable]
public struct GunModifier
{
    public ProjectileData ProjectileOverride;
    public float FireRateModifier;
    public float SpreadModifier;
    public float DamageModifier;
    public float SpeedModifier;
    public float ScaleModifier;

    public static GunModifier Default => new GunModifier()
    {
        ProjectileOverride = null,
        FireRateModifier = 1,
        SpreadModifier = 1,
        DamageModifier = 1,
        SpeedModifier = 1,
        ScaleModifier = 1
    };
}
