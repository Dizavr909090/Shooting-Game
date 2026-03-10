using UnityEngine;

[CreateAssetMenu(fileName = "VisualSettings", menuName = "Scriptable Objects/VisualSettings")]
public class MapVisualDatabase : ScriptableObject
{
    [field: SerializeField] public MapMesh FloorMesh;
    [field: SerializeField] public MapMesh ObstacleMesh;
    [field: SerializeField] public MapMesh BoundaryMesh;
    [field: SerializeField] public MapMesh LavaMesh;
}
