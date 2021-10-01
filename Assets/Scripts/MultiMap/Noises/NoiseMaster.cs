using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NoiseMaster
{
    public static float[,] GenerateHeightMap(Types.MainMapOptions nm) 
    {
        // init noise from script
        Noise n = new Noise();
        
        float[,] noiseMap = new float[nm.mapWidth, nm.mapHeight];
        
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
        
        // to normalize
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        
        // generate noise
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

                    // simplex value of range -1 to 1 sample
                    float simplxValue = n.Evaluate(sampleX, sampleY);

                    // noiseHeight is the key output
                    // noiseHeight += simplex * amplitude;
                    noiseHeight += simplxValue * amplitude;

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

                // set value
                noiseMap[x, y] = noiseHeight;
            }
        }
        
        // iterate again to normalize to 0 to 1
        for (int y = 0; y < nm.mapHeight; y++)
        {
            for (int x = 0; x < nm.mapWidth; x++)
            {
                // inverseLerp returns 0 and 1
                float normalizedHeight = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                noiseMap[x, y] = normalizedHeight;
            }
        }

        return noiseMap;
    }
}
