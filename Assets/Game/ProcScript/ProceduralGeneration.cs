using UnityEngine;

[RequireComponent(typeof(Terrain), typeof(TerrainCollider))]
public class ProceduralTerrain : MonoBehaviour
{
    [Range(2, 512)]
    public int resolution = 129; // must be 2^n + 1
    public float size = 1280f;   // total terrain width/depth
    public float heightScale = 50f;

    FastNoiseLite noise = new FastNoiseLite();

    void Start()
    {
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

        // Create a new TerrainData object
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(size, heightScale, size);

        // Generate heightmap
        float[,] heights = new float[resolution, resolution];
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float nx = (float)x / resolution;
                float nz = (float)z / resolution;

                // Noise returns roughly [-1, 1], normalize to [0, 1]
                float noiseValue = (noise.GetNoise(x, z) + 1f) * 0.5f;
                heights[z, x] = 0;
            }
        }

        // Apply heightmap to terrain
        terrainData.SetHeights(0, 0, heights);

        // Assign to terrain and collider
        Terrain terrain = GetComponent<Terrain>();
        TerrainCollider collider = GetComponent<TerrainCollider>();

        terrain.terrainData = terrainData;
        collider.terrainData = terrainData;
    }
}