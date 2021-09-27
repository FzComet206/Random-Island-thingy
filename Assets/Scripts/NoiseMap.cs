using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NoiseMap 
{
    public static int[,] GetNoiseMap(MapGenerator.Options options)
    {
        // init noise from script
        Noise n = new Noise();
        
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

                // some terrain modifications
                float nc = nm.negativeClamp;
                float fs = nm.flattenScale;
                
                if (noiseHeight < -nc)
                {
                    noiseHeight = -nc - (noiseHeight + nc);
                } 
                else if (noiseHeight < -nc + fs)
                {
                    noiseHeight = -nc;
                }
                else
                {
                    noiseHeight = noiseHeight - fs;
                }

                noiseMap[x, y] = Mathf.RoundToInt(noiseHeight * nm.heightScale);
            }
        }
        
        
        return noiseMap;
    }
}
