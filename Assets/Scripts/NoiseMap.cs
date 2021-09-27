using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using Unity.Mathematics;
using UnityEngine;

public class NoiseMap 
{
    public static float[,] GetNoiseMap(MapGenerator.Options options)
    {
        // init noise from script
        Noise n = new Noise();
        
        MapGenerator.Options nm = options;
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
        if (nm.useHeightCurve)
        {
            for (int y = 0; y < nm.mapHeight; y++)
            {
                for (int x = 0; x < nm.mapWidth; x++)
                {
                    // inverseLerp returns 0 and 1
                    float normalizedHeight = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                    noiseMap[x, y] = Mathf.RoundToInt(
                        nm.heightScale * 
                        (nm.heightCurve.Evaluate(normalizedHeight) / 2 + normalizedHeight));
                }
            }
        }
        else
        {
            for (int y = 0; y < nm.mapHeight; y++)
            {
                for (int x = 0; x < nm.mapWidth; x++)
                {
                    float normalizedHeight = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                    noiseMap[x, y] = Mathf.RoundToInt(nm.heightScale * normalizedHeight);
                }
            }
        }

        return noiseMap;
    }
}
