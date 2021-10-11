using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class IslandMeshGenerator : MonoBehaviour
{
    public static CircularMeshData GenerateTerrainMesh(Vector3[][] heightMap, IslandTypes.IslandOptions op)
    {
        int radiusFrac0 = op.ring0RadiusFractions;
        int radiusFrac1 = op.ring1RadiusFractions;
        int radiusFrac2 = op.ring2RadiusFractions;
        
        int degreeFrac0 = op.ring0DegreeFractions;
        int degreeFrac1 = degreeFrac0 * 2;
        int degreeFrac2 = degreeFrac1 * 2;
        
        int vertexIndex = 0;

        int r = op.ring0RadiusFractions + op.ring1RadiusFractions + op.ring2RadiusFractions * 2;
        
        // vertices is a one dimensional array
        CircularMeshData meshData = new CircularMeshData(
            r,
            op.ring0DegreeFractions * 7, 
            true);

        int r0 = radiusFrac0;
        int r1 = r0 + radiusFrac1;
        int r2 = r1 + radiusFrac2;
        int r3 = r2 + radiusFrac2;
        
        for (int i = 0; i < r; i++)
        {
            int cap = heightMap[i].Length;
            
            for (int j = 0; j < cap; j++)
            {
                meshData.vertices[vertexIndex] = heightMap[i][j];
                
                if (i < r0)
                {
                    vertexIndex = TriangulateAtRing(i, r0, j, degreeFrac0, vertexIndex, cap, meshData, r);
                } 
                else if (i < r1)
                {
                    
                }
                else if (i < r2)
                {
                    
                }
                else
                {
                    
                }


            }
        }

        meshData.ProcessMesh();
        return meshData;
    }

    private static int TriangulateAtRing(int i, int r0, int j, int degreeFrac0, int vertexIndex, int cap,
        CircularMeshData meshData, int r)
    {
        // triangulation r0
        // ignore right and bottom vertices for the map
        if (i < r0 - 1 && j < degreeFrac0 - 1)
        {
            int _a = vertexIndex + cap + 1;
            int _b = vertexIndex + 1;
            int _c = vertexIndex;
            int _d = vertexIndex + cap;

            meshData.AddTriangles(_c, _b, _a);
            meshData.AddTriangles(_a, _d, _c);
            vertexIndex++;
            return vertexIndex;
        }

        // edge case where 0 degree doesnt connect with 360 degree
        if (j == degreeFrac0 - 1 && i < r - 1)
        {
            // this fking edge case took a while im so dumb

            int _a = vertexIndex + 1;
            int _b = vertexIndex - cap + 1;
            int _c = vertexIndex;
            int _d = vertexIndex + cap;

            meshData.AddTriangles(_c, _b, _a);
            meshData.AddTriangles(_a, _d, _c);
            vertexIndex++;
            return vertexIndex;
        }

        // this will be the cases where ring intercepts outer ring
        // there is a difference is degree fractions count so we use another triangulation
        // seems like this must be taken care of before next ring
        if (i == r - 1)
        {
            if (j < degreeFrac0)
            {
            }
            else
            {
                // i believe this will also be an edge case
            }
        }

        return vertexIndex;
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
