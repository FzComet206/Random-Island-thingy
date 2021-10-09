using UnityEngine;

public class ActualMapDisplay : MonoBehaviour
{
    public MeshFilter meshFiler;
    public MeshRenderer textureRenderer;
    public MeshCollider meshCollider;
    
    public void DrawMeshMap(MeshData meshData)
    {
        Mesh data = meshData.CreateMesh();
        meshFiler.name = "GeneratedMesh";
        meshFiler.sharedMesh = data;
        meshCollider.sharedMesh = data;
    }
    
    public void DrawCircularMeshMap(CircularMeshData meshData)
    {
        Mesh data = meshData.CreateMesh();
        meshFiler.name = "GeneratedMesh";
        meshFiler.sharedMesh = data;
        meshCollider.sharedMesh = data;
    }
    
    public void DrawTextureMap(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
    }
}
