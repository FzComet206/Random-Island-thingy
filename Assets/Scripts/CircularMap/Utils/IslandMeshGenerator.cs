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

        int r = op.ring0RadiusFractions + op.ring1RadiusFractions + op.ring2RadiusFractions;
        
        // vertices is a one dimensional array
        CircularMeshData meshData = new CircularMeshData(
            r,
            op.ring0DegreeFractions * 7, 
            true);

        int r0 = radiusFrac0;
        int r1 = r0 + radiusFrac1;
        int r2 = r1 + radiusFrac2;
        
        for (int i = 0; i < r; i++)
        {
            int cap = heightMap[i].Length;
            
            for (int j = 0; j < cap; j++)
            {
                meshData.vertices[vertexIndex] = heightMap[i][j];
                
                if (i < r0)
                {
                    vertexIndex = TriangulateAtRing(i, r0, j, degreeFrac0, vertexIndex, cap, meshData);
                } 
                else if (i < r1)
                {
                    vertexIndex = TriangulateAtRing(i, r1, j, degreeFrac1, vertexIndex, cap, meshData);
                }
                else
                {
                    vertexIndex = TriangulateAtRing(i, r2, j, degreeFrac2, vertexIndex, cap, meshData, false);
                }
            }
        }

        meshData.ProcessMesh();
        return meshData;
    }

    private static int TriangulateAtRing(int i, int _r, int j, int degreeFrac, int vertexIndex, int cap,
        CircularMeshData meshData, bool inside = true)
    {
        // triangulation r0
        // ignore right and bottom vertices for the map
        if (i < _r - 1 && j < degreeFrac - 1)
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
        if (j == degreeFrac - 1 && i < _r - 1)
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

        if (!inside)
        {
            vertexIndex++;
            return vertexIndex;
        }
        
        if (i == _r - 1)
        {
            if (j < degreeFrac - 1)
            {
                // ok idk but +j works
                int _b = vertexIndex + 1;
                int _c = vertexIndex + cap + j + 1;
                int _d = vertexIndex;
                int _a = vertexIndex + cap + j + 2;
                int _e = vertexIndex + cap + j;
                
                meshData.AddTriangles(_a, _c, _b);
                meshData.AddTriangles(_d, _b, _c);
                meshData.AddTriangles(_e, _d, _c);
                
                vertexIndex++;
                return vertexIndex;

            }
            else
            {
                // this also took a while
                int _a = vertexIndex + 1;
                int _b = vertexIndex - cap + 1;
                int _d = vertexIndex;
                int _c = vertexIndex + cap + j + 1;
                int _e = vertexIndex + cap + j;
                
                meshData.AddTriangles(_a, _c, _b);
                meshData.AddTriangles(_d, _b, _c);
                meshData.AddTriangles(_e, _d, _c);
                
                vertexIndex++;
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
