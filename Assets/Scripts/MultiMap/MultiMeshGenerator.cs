using UnityEngine;
using UnityEngine.Rendering;

public class MultiMeshGenerator
{
    public static MeshData GenerateTerrainMesh(float [,] heightMap, Types.MainMapOptions op)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        int vertexIndex = 0;
        
        MeshData meshData = new MeshData(width, height, op.useFlatShading);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                    meshData.vertices[vertexIndex] = new Vector3(
                        x, 
                        heightMap[x, y] * op.depth,
                        y);
                    // uv tells each vertex where it locates with respect to rest vertices (0 to 1)
                    meshData.uvs[vertexIndex] = new Vector2(x / (float) width, y / (float) height);

                    // ignore right and bottom vertices for the map
                    if (x < width - 1 && y < height - 1)
                    {
                        // vertices to first triangles (i --> i + width + 1 --> i + width)
                        // vertices to second triangles (i + width + 1 --> i --> i + 1)
                        // switch the b and c order means inverting the normals here
                        meshData.AddTriangles(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);
                        meshData.AddTriangles(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                    }
                    
                    vertexIndex++;
            }
        }
        
        meshData.ProcessMesh();
        return meshData;
    }
}
