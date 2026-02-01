using UnityEditor;
using UnityEngine;
using static MapSettings;

public class MapSpawner : MonoBehaviour
{
    private Transform[,] _tileMap;

    public Transform GetTileAt(int x, int y)
    {
        return _tileMap[x, y];
    }

    public void SpawnMap(LevelMapData mapData, MapSettings settings, MapConfig currentMap, Transform holder)
    {
        System.Random prng = new System.Random(currentMap.Seed);
        GameObject floorPrefab = settings.GetPrefabByType(TileType.Floor);

        _tileMap = new Transform[mapData.tileMap.GetLength(0), mapData.tileMap.GetLength(1)];

        for (int x = 0; x < mapData.tileMap.GetLength(0); x++)
        {
            for (int y = 0; y < mapData.tileMap.GetLength(1); y++)
            {
                TileType currentType = mapData.tileMap[x, y];

                Vector3 pos = mapData.grid.CoordToWorld(new Coord(x, y));

                if (floorPrefab != null)
                {
                    _tileMap[x,y] = CreateObject(floorPrefab, pos, Quaternion.Euler(Vector3.right * 90), currentMap.OutlinePercent, currentMap.TileSize, holder);
                }

                if (currentType != TileType.Floor)
                {
                    VisualModifiers? currentMods = null;

                    if (currentType == TileType.Obstacle)
                    {
                        float percent = (float)y / mapData.tileMap.GetLength(1);
                        currentMods = new VisualModifiers()
                        {
                            color = Color.Lerp(currentMap.BackgroundColour, currentMap.ForegroundColour, percent),
                            heightScale = Mathf.Lerp(currentMap.MinObstacleHeight, currentMap.MaxObstacleHeight, (float)prng.NextDouble())
                        };
                    }

                    GameObject prefab = settings.GetPrefabByType(currentType);

                    if (prefab != null)
                    {
                        CreateObject(prefab, pos + Vector3.up, Quaternion.identity, currentMap.OutlinePercent, currentMap.TileSize, holder, currentMods);
                    }
                }
            }
        }
    }

    private Transform CreateObject(GameObject prefab, Vector3 pos, Quaternion rotation, float outline, float tileSize, Transform holder, VisualModifiers? mods = null)
    {
        GameObject gameObject = Instantiate(prefab, pos, rotation);
        gameObject.transform.parent = holder;

        Vector3 scale = Vector3.one * (1 - outline) * tileSize;

        if (mods.HasValue)
        {
            scale.y = mods.Value.heightScale;
            gameObject.transform.position = new Vector3(pos.x, scale.y / 2f, pos.z);

            if (mods.Value.color.HasValue)
            {
                Renderer obstacleRenderer = gameObject.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);

                if (obstacleRenderer != null)
                {
                    obstacleMaterial.color = mods.Value.color.Value;
                    obstacleRenderer.sharedMaterial = obstacleMaterial;
                }
            }
        }
        else
        {
            gameObject.transform.localScale = scale;
        }

        gameObject.transform.localScale = scale;

        return gameObject.transform; 
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

    private struct VisualModifiers
    {
        public Color? color;
        public float heightScale;
    }
}

