using System.Collections.Generic;
using UnityEngine;
using static MapSettings;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapMesh : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        if ( _meshFilter == null ) _meshFilter = GetComponent<MeshFilter>();
        if ( _meshRenderer == null ) _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void CreateFloor(LevelMapData mapData, MapSettings settings, MapConfig currentMap)
    {
        MeshData meshData = new MeshData();

        Mesh generatedMesh = meshData.BuildMesh(mapData, settings, currentMap);

        _meshFilter.mesh = generatedMesh;
    }

    public struct MeshData
    {
        private List<Vector3> _vertices;
        private List<int> _triangles;
        private List<Vector2> _uvs;

        public Mesh BuildMesh(LevelMapData mapData, MapSettings settings, MapConfig currentMap)
        {
            float halfSize = currentMap.TileSize / 2f;

            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            _uvs = new List<Vector2>();

            int vIndex = 0;

            for (int x = 0; x < mapData.tileMap.GetLength(0); x++)
            {
                for (int y = 0; y < mapData.tileMap.GetLength(1); y++)
                {
                    Vector3 pos = mapData.grid.CoordToWorld(new Coord(x, y));

                    _vertices.Add(new Vector3(pos.x - halfSize, 0, pos.z - halfSize));
                    _vertices.Add(new Vector3(pos.x + halfSize, 0, pos.z - halfSize));
                    _vertices.Add(new Vector3(pos.x - halfSize, 0, pos.z + halfSize));
                    _vertices.Add(new Vector3(pos.x + halfSize, 0, pos.z + halfSize));

                    _triangles.Add(vIndex + 0);
                    _triangles.Add(vIndex + 2);
                    _triangles.Add(vIndex + 1);

                    _triangles.Add(vIndex + 1);
                    _triangles.Add(vIndex + 2);
                    _triangles.Add(vIndex + 3);

                    _uvs.Add(new Vector2(0, 0));
                    _uvs.Add(new Vector2(1, 0));
                    _uvs.Add(new Vector2(0, 1));
                    _uvs.Add(new Vector2(1, 1));

                    vIndex += 4;
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.uv = _uvs.ToArray();

            return mesh;
        }
    }
}
