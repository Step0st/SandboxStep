using UnityEngine;

public static class TerrainGenerator
{
     public static int[,,] GenerateTerrain()
     {
          var result = new int[ChunkRenderer.chunkWidth, ChunkRenderer.chunkHeight, ChunkRenderer.chunkWidth];
          
          for (int x = 0; x < ChunkRenderer.chunkWidth; x++)
          {
               for (int z = 0; z < ChunkRenderer.chunkWidth; z++)
               {
                    float height = Mathf.PerlinNoise(x * 0.2f, z * 0.2f)*5  + 10;

                    for (int y = 0; y < height; y++)
                    {
                         result[x, y, z] = 1;
                    }
               }
          }

          return result;
     }
    
}
