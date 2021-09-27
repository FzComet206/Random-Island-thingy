using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NoiseMap 
{
    public static int[,] GetNoiseMap(MapGenerator.Options options)
    {
        MapGenerator.Options nm = options;
        int[,] noiseMap = new int[nm.mapWidth, nm.mapHeight];
        
        // Set seed
        System.Random prng = new System.Random(nm.seed);
        Vector2[] octavesOffset = new Vector2[nm.octaves];
        for (int i = 0; i < nm.octaves; i++)
        {
            // scrolling and random octaves
            float offsetXOct = prng.Next(-100000, 100000) + nm.offsetx;
            float offsetYOct = prng.Next(-100000, 100000) + nm.offsety;
            octavesOffset[i] = new Vector2(offsetXOct, offsetYOct);
        }
        
        // generate noise
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < nm.mapHeight; y++)
        {
            for (int x = 0; x < nm.mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < nm.octaves; i++)
                {
                    // apply octaves sample and offsets with scale and freq
                    float sampleX = x / nm.scale * frequency + octavesOffset[i].x;
                    float sampleY = y / nm.scale * frequency + octavesOffset[i].y;

                    // perlinValue of range -1 to 1 sample
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    // noiseHeight is the key output
                    noiseHeight += perlinValue * amplitude;

                    // update frequence and amplitude with persistance and lacunarity
                    amplitude *= nm.persistance;
                    frequency *= nm.lacunarity;
                }

                // get the max and min noise height in order to normalize the noise map
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                
                if (noiseHeight < -0.3f)
                {
                    noiseHeight = -0.3f - (noiseHeight + 0.3f);
                } else if (noiseHeight < -0.3 + 0.5f)
                {
                    noiseHeight = -0.3f;
                }
                else
                {
                    noiseHeight = noiseHeight - 0.5f;
                }


                noiseMap[x, y] = Mathf.RoundToInt(noiseHeight * nm.heightScale);
            }
        }
        
        
        return noiseMap;
    }
}
