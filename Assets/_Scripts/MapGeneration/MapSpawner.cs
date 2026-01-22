using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapSpawner : MonoBehaviour
{
    public void SpawnObstacles(List<Vector3> positions, GameObject obstaclePrefab, Transform holder)
    {
        foreach (Vector3 pos in positions)
        {
            Transform newObstacle = Instantiate(obstaclePrefab, pos + Vector3.up * 0.5f, Quaternion.identity).transform;
            newObstacle.parent = holder;
        }
    }

    public void SpawnTiles(List<Vector3> positions, GameObject tilePrefab, float outlinePercent, Transform holder)
    {

        foreach (Vector3 pos in positions)
        {
            Transform newTile = Instantiate(tilePrefab, pos, Quaternion.Euler(Vector3.right * 90)).transform;
            newTile.localScale = Vector3.one * (1 - outlinePercent);
            newTile.parent = holder;
        }

    }

    public Transform CreateMapHolder(string name, Transform parent)
    {
        Transform oldHolder = parent.Find(name);
        if (oldHolder)
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(oldHolder.gameObject);
#else
            Destroy(oldHolder.gameObject);
#endif
        }

        GameObject go = new GameObject(name);

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(go, "Generate Map");
#endif

        go.transform.parent = parent;
        return go.transform;
    }
}
