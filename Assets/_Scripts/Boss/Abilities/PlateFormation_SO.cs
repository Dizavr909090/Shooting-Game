using UnityEngine;

[CreateAssetMenu(fileName = "PlateFormation_SO", menuName = "Scriptable Objects/PlateFormation_SO")]
public class PlateFormation_SO : ScriptableObject
{
    [field: SerializeField] public float[] PlateAngles;
    [field: SerializeField] public Vector3[] WeaponOffsets;
}
