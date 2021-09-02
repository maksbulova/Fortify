using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain size")]
    public int hight, width;

    public HexagonalRuleTile[] tiles;

    [Range(1, 5), Delayed]
    public int octaves;
    [Range(0, 1)]
    public float persistence;

    [Range(1, 256)]
    public int resolution;
    public List<float> scale, offsetX, offsetY;

    private Renderer mainRenderer;

    private void Awake()
    {
        mainRenderer = GetComponent<Renderer>();
        GenerateTexture();
    }

    private void OnValidate()
    {
        // уменьшение октав
        if (octaves < scale.Count)
        {
            scale = new List<float>(scale.GetRange(0, octaves));
            offsetX = new List<float>(offsetX.GetRange(0, octaves));
            offsetY = new List<float>(offsetY.GetRange(0, octaves));
        }
        // увеличение октав
        else if (octaves > scale.Count)
        {
            int difference = octaves - scale.Count;
            scale.AddRange(new float[difference]);
            offsetX.AddRange(new float[difference]);
            offsetY.AddRange(new float[difference]);
        }

        GenerateTexture();
    }

    [ContextMenu("Generate")]
    void GenerateTexture()
    {
        if (!mainRenderer)
        {
            mainRenderer = GetComponent<Renderer>();
        }

        Texture2D texture = new Texture2D(resolution, resolution);

        for (int xPixel = 0; xPixel < resolution; xPixel++)
        {
            for (int yPixel = 0; yPixel < resolution; yPixel++)
            {
                Color color = CalculateColor(xPixel, yPixel);
                texture.SetPixel(xPixel, yPixel, color);
            }
        }
        texture.Apply();

        mainRenderer.material.mainTexture = texture;
    }

    // колір у координаті
    Color CalculateColor(float x, float y)
    {
        
        float amplitude = 1;
        float maxAmplitude = 0;
        float sample = 0;
        for (int i = 0; i < octaves; i++)
        {
            float xCoord = (float)x / resolution * scale[i] + offsetX[i];
            float yCoord = (float)y / resolution * scale[i] + offsetY[i];


            sample += Mathf.PerlinNoise(xCoord, yCoord) * amplitude;

            maxAmplitude += amplitude;
            amplitude *= persistence;
        }
        sample /= maxAmplitude;

        return new Color(sample, sample, sample);
    }

}
