using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NoiseMaster
{
    public static float[,] GenerateHeightMap(Types.MainMapOptions nm)
    {
        int w = nm.mapWidth;
        int h = nm.mapHeight;
        
        // init noise from script
        Noise n = new Noise();
        float[,] noiseMap = new float[w, h];
        
        // Set seed
        System.Random prng = new System.Random(nm.seed);
        Vector2[] octavesOffset = new Vector2[nm.octaves];
        for (int i = 0; i < nm.octaves; i++)
        {
            // scrolling and random octaves
            float offsetXOct = prng.Next(-100000, 100000) + nm.offset.x;
            float offsetYOct = prng.Next(-100000, 100000) + nm.offset.y;
            octavesOffset[i] = new Vector2(offsetXOct, offsetYOct);
        }
        
        // to normalize
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        
        // generate noise
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
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
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                // inverseLerp returns 0 and 1
                float normalizedHeight = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                noiseMap[x, y] = normalizedHeight;
            }
        }
        
        // calculate x and y bounds for each of the four areas

        int mountainXLeft = 0;
        int valleyXLeft = 0;
        int mountainYTop = 0;
        int plainYTop = 0;

        int a = Mathf.FloorToInt(1 + w / 2f);
        int mountainXRight = a;
        int valleyXRight = a;
        int plainXLeft = a;
        int desertXLeft = a;
        
        int b = Mathf.FloorToInt(1 + (h / 2f));
        int mountainYBottom = b;
        int valleyYTop = b;
        int plainYBottom = b;
        int desertYTop = b;

        int plainXRight = w;
        int desertXRight = w;
        int valleyYBottom = w;
        int desertYBottom = h;

        float[,] finalizedMap = new float[w, h];
        
        // encode four terrains with interpolation 
        NoiseMountain.EncodeMountain( ref finalizedMap, ref noiseMap, nm, 
            mountainXLeft, 
            mountainXRight, 
            mountainYTop,
            mountainYBottom);
        
        NoiseValley.EncodeValley(ref finalizedMap, ref noiseMap, nm, 
            valleyXLeft, 
            valleyXRight, 
            valleyYTop,
            valleyYBottom);
        
        NoisePlain.EncodePlain(ref finalizedMap, ref noiseMap, nm, 
            plainXLeft, 
            plainXRight, 
            plainYTop,
            plainYBottom);
        
        NoiseDesert.EncodeDesert(ref finalizedMap, ref noiseMap, nm, 
            desertXLeft, 
            desertXRight, 
            desertYTop,
            desertYBottom);
        
        // adjustment

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                finalizedMap[x, y] = Mathf.FloorToInt(finalizedMap[x, y] * nm.heightScale);
            }
        }

        return finalizedMap;
    }
}
