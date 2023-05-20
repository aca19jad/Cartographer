using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {
    
    public static float[,] GenerateNoiseMap(int width, int height, int seed, float scale, int octaves, float persist, float lac, Vector2 offset){
        float[,] noiseMap = new float[width, height];

        System.Random rand = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++){
            float xOffset = rand.Next(-100000, 100000) + offset.x;
            float yOffset = rand.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(xOffset, yOffset);
        }

        if(scale <= 0){scale = 0.0001f;}

        float maxNoise = float.MinValue;
        float minNoise = float.MaxValue;

        float centreWidth = width / 2f;
        float centreHeight = height / 2f;

        for(int y = 0; y < height; y++){
            for (int x = 0; x < width; x++){

                float amp = 1;
                float freq = 1;

                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++){
                    float xSample = (x - centreWidth) /scale * freq + octaveOffsets[i].x;
                    float ySample = (y - centreHeight) /scale * freq + octaveOffsets[i].y;

                    float noiseValue = Mathf.PerlinNoise(xSample, ySample) * 2 - 1;
                    noiseHeight += noiseValue * amp;
                    amp *= persist;
                    freq *= lac;
                }
                
                if(noiseHeight > maxNoise) {
                    maxNoise = noiseHeight;
                } else if (noiseHeight < minNoise) {
                    minNoise = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight;
                
            }
        }

        for(int y = 0; y < height; y++){
            for (int x = 0; x < width; x++){
                noiseMap[x,y] = Mathf.InverseLerp(minNoise, maxNoise, noiseMap[x,y]);
            }
        }
        return noiseMap;
    }

}
