using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct Options
    {
        public int seed;
        
        public int mapWidth;
        public int mapHeight;
        
        public float scale;
        public int octaves;
        public float persistance;
        public float lacunarity;
        
        public int depth;
        public float heightScale;
        
        [Range(0, 1)]
        public float negativeClamp;
        [Range(0, 1)]
        public float flattenScale;

        public bool useHeightCurve;
        public AnimationCurve heightCurve;
        public bool useDepthCurve;
        public AnimationCurve depthCurve;

        public float offsetx;
        public float offsety;
        
        public bool useFlatShading;
    }

    [Header("Map Input Parameters")] [SerializeField]
    public Options options;
    
    public void DrawMesh()
    {
        ActualMapDisplay display = FindObjectOfType<ActualMapDisplay>();
        
        display.DrawMeshMap(
            MeshGenerator.GenerateTerrainMesh(NoiseMap.GetNoiseMap(options), options)
            );
    }
}