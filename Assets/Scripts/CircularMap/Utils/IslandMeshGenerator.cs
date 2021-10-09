using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class IslandMeshGenerator : MonoBehaviour
{
    public static CircularMeshData GenerateTerrainMesh(Vector3[,] heightMap)
    {
        int a = heightMap.GetLength(0);
        int r = heightMap.GetLength(1);
        int vertexIndex = 0;

        CircularMeshData meshData = new CircularMeshData(r, a, true);

        for (int y = 0; y < r; y++)
        {
            for (int x = 0; x < a; x++)
            {
                meshData.vertices[vertexIndex] = heightMap[x, y];
                
                // uv tells each vertex where it locates with respect to rest vertices (0 to 1)
                // meshData.uvs[vertexIndex] = new Vector2(x / (float) width, y / (float) height);

                // ignore right and bottom vertices for the map
                if (x < a - 1 && y < r - 1)
                {
                    // vertices to first triangles (i --> i + width + 1 --> i + width)
                    // vertices to second triangles (i + width + 1 --> i --> i + 1)
                    // switch the b and c order means inverting the normals here

                    int _a = vertexIndex + a + 1;
                    int _b = vertexIndex + 1;
                    int _c = vertexIndex;
                    int _d = vertexIndex + a;
                    
                    meshData.AddTriangles(_c, _b, _a);
                    meshData.AddTriangles(_a, _d, _c);
                }

                // edge case where 0 degree doesnt connect with 360 degree
                if (x == a - 1 && y < r - 1)
                {
                    // this fking edge case took a while im so dumb

                    int _a = vertexIndex + 1;
                    int _b = vertexIndex - a + 1;
                    int _c = vertexIndex;
                    int _d = vertexIndex + a;

                    meshData.AddTriangles(_c, _b, _a);
                    meshData.AddTriangles(_a, _d, _c);
                }
                vertexIndex++;
            }
        }

        meshData.ProcessMesh();
        return meshData;
    }
}

public class CircularMeshData
{
    // vertices
    public Vector3[] vertices;
    
    // 3 vertices for a triangle, 6 for a square
    public int[] triangles;
    
    // one uv for each vertex
    public Vector2[] uvs;

    // keep track of vertices to triangles reference
    private int triangleIndex;

    private bool useFlatShading;
    
    // constructor
    public CircularMeshData(int radiusFrac, int degreeFrac, bool useFlatShading)
    {
        this.useFlatShading = useFlatShading;
        vertices = new Vector3[radiusFrac * degreeFrac];
        triangles = new int[(radiusFrac - 1) * degreeFrac * 6];
        uvs = new Vector2[radiusFrac* degreeFrac];
    }

    public void AddTriangles(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    void FlatShading()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            // i dont fking understand this
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }

    public void ProcessMesh()
    {
        if (useFlatShading)
        {
            FlatShading();
        }
    }
    public Mesh CreateMesh()
    {
        // build mesh
        Mesh mesh = new Mesh(); 
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.Optimize();
        return mesh;
    }
}
