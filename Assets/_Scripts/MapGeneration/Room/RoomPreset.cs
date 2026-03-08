using UnityEngine;

[CreateAssetMenu(fileName = "RoomPreset", menuName = "Scriptable Objects/RoomPreset")]
public class RoomPreset : ScriptableObject
{
    [field: SerializeField] public Vector2Int Size { get; private set; }
}
