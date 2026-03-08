using System.Collections.Generic;
using UnityEngine;
using static MapSettings;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MapMesh : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;

    public void GenerateMesh(LevelMapData mapData, 
        MapSettings settings, 
        MapConfig currentMap, 
        TileType targetTile)
    {
        MeshData meshData = new MeshData();

        Mesh generatedMesh = meshData.BuildMesh(mapData, settings, currentMap, targetTile);

        if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
            _meshFilter.sharedMesh = generatedMesh;

        if (_meshRenderer == null) 
            _meshRenderer = GetComponent<MeshRenderer>();

        if (_meshCollider == null)
            _meshCollider = GetComponent<MeshCollider>();
        _meshCollider.sharedMesh = generatedMesh;
    }

    public struct MeshData
    {
        private List<Vector3> _vertices;
        private List<int> _triangles;
        private List<Vector2> _uvs;

        public Mesh BuildMesh(
            LevelMapData mapData, 
            MapSettings settings, 
            MapConfig currentMap, 
            TileType targetType)
        {
            float halfSize = currentMap.TileSize / 2f;
            System.Random rngBySeed = new System.Random(currentMap.Seed);

            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            _uvs = new List<Vector2>();

            for (int x = 0; x < mapData.tileMap.GetLength(0); x++)
            {
                for (int y = 0; y < mapData.tileMap.GetLength(1); y++)
                {
                    Vector3 pos = mapData.grid.CoordToWorld(new Coord(x, y));

                    if (mapData.tileMap[x, y] == targetType)
                    {
                        if (targetType == TileType.Floor)
                        {
                            AddFace(
                                new Vector3(pos.x - halfSize, 0, pos.z - halfSize), // 0: BL                 
                                new Vector3(pos.x - halfSize, 0, pos.z + halfSize), // 2: FL
                                new Vector3(pos.x + halfSize, 0, pos.z - halfSize), // 1: BR
                                new Vector3(pos.x + halfSize, 0, pos.z + halfSize)  // 3: FR
                            );
                        }
                        else if (targetType == TileType.Obstacle)
                        {
                            float randomHeight = Mathf.Lerp(
                                currentMap.MinObstacleHeight, 
                                currentMap.MaxObstacleHeight, 
                                (float)rngBySeed.NextDouble());

                            AddObstacle(pos, halfSize, randomHeight);
                        }     
                    }
                    else
                    {
                        rngBySeed.NextDouble();
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.uv = _uvs.ToArray();

            return mesh;
        }

        public void AddObstacle(Vector3 pos, float halfSize, float height)
        {
            // Sides
            Vector3 b_bl = new Vector3(pos.x - halfSize, 0, pos.z - halfSize);
            Vector3 b_br = new Vector3(pos.x + halfSize, 0, pos.z - halfSize);
            Vector3 b_fl = new Vector3(pos.x - halfSize, 0, pos.z + halfSize);
            Vector3 b_fr = new Vector3(pos.x + halfSize, 0, pos.z + halfSize);

            // Top
            Vector3 t_bl = new Vector3(pos.x - halfSize, height, pos.z - halfSize);
            Vector3 t_fl = new Vector3(pos.x - halfSize, height, pos.z + halfSize);
            Vector3 t_br = new Vector3(pos.x + halfSize, height, pos.z - halfSize);
            Vector3 t_fr = new Vector3(pos.x + halfSize, height, pos.z + halfSize);

            // 1. Top
            AddFace(t_bl, t_fl, t_br, t_fr);

            // 2. Front
            AddFace(b_fl, b_fr, t_fl, t_fr);

            // 3. Back
            AddFace(b_br, b_bl, t_br, t_bl);

            // 4. Left
            AddFace(b_bl, b_fl, t_bl, t_fl);

            // 5. Right
            AddFace(b_fr, b_br, t_fr, t_br);
        }

        private void AddFace(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4)
        {
            int vIndex = _vertices.Count;

            AddVertices(vertex1,vertex2,vertex3,vertex4);
            AddTriangles(vIndex);
            AddUvs();
        }

        private void AddVertices(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4)
        {
            _vertices.Add(vertex1);
            _vertices.Add(vertex2);
            _vertices.Add(vertex3);
            _vertices.Add(vertex4);
        }

        private void AddTriangles(int index)
        {
            _triangles.Add(index + 0);
            _triangles.Add(index + 1);
            _triangles.Add(index + 2);

            _triangles.Add(index + 2);
            _triangles.Add(index + 1);
            _triangles.Add(index + 3);
        }

        private void AddUvs()
        {
            _uvs.Add(new Vector2(0, 0));
            _uvs.Add(new Vector2(1, 0));
            _uvs.Add(new Vector2(0, 1));
            _uvs.Add(new Vector2(1, 1));
        }
    }
}
