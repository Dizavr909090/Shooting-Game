using UnityEditor;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public void SpawnMap(LevelMapData mapData, MapSettings settings, Transform holder)
    {
        GameObject floorPrefab = settings.GetPrefabByType(TileType.Floor);

        for (int x = 0; x < mapData.tileMap.GetLength(0); x++)
        {
            for (int y = 0; y < mapData.tileMap.GetLength(1); y++)
            {
                TileType currentType = mapData.tileMap[x, y];

                Vector3 pos = Utility.CoordToPosition(mapData.mapSize, x, y, settings.TileSize);

                if (floorPrefab != null)
                {
                    CreateObject(floorPrefab, pos, Quaternion.Euler(Vector3.right * 90), settings.OutlinePercent, settings.TileSize, holder);
                }

                if (currentType != TileType.Floor)
                {
                    GameObject prefab = settings.GetPrefabByType(currentType);
                    if (prefab != null)
                    {
                        CreateObject(prefab, pos + Vector3.up, Quaternion.identity, settings.OutlinePercent, settings.TileSize, holder);
                    }
                }
            }
        }
    }

    private void CreateObject(GameObject prefab, Vector3 pos, Quaternion rotation, float outline, float tileSize, Transform holder)
    {
        GameObject gameObject = Instantiate(prefab, pos, rotation);
        gameObject.transform.localScale = Vector3.one * (1 - outline) * tileSize;
        gameObject.transform.parent = holder;
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
