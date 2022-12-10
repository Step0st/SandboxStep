using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
    public ChunkRenderer chunkPrefab;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                float xPos = x * ChunkRenderer.chunkWidth * ChunkRenderer.blockScale;
                float zPos = y * ChunkRenderer.chunkWidth * ChunkRenderer.blockScale;
                
                var chunkData = new ChunkData();
                chunkData.chunkPosition = new Vector2Int(x, y);
                chunkData.Blocks = TerrainGenerator.GenerateTerrain(xPos, zPos);
                ChunkDatas.Add(new Vector2Int(x,y), chunkData);

                var chunk = Instantiate(chunkPrefab, new Vector3(xPos, 0, zPos), 
                    Quaternion.identity, transform);
                chunk.ChunkData = chunkData;
                chunk.ParentWorld = this;

                chunkData.renderer = chunk;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            bool isDestroying = Input.GetMouseButtonDown(0);
            
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out var hitInfo))
            {
                Vector3 blockCenter;
                if (isDestroying)
                {
                    blockCenter = hitInfo.point - hitInfo.normal * ChunkRenderer.blockScale / 2;
                }
                else
                {
                    blockCenter = hitInfo.point + hitInfo.normal * ChunkRenderer.blockScale / 2;
                }
                Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.blockScale);
                Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);
                if (ChunkDatas.TryGetValue(chunkPos, out ChunkData chunkData))
                {
                    Vector3Int chunkOrigin = new Vector3Int(chunkPos.x, 0, chunkPos.y) * ChunkRenderer.chunkWidth;
                    if (isDestroying)
                    {
                        chunkData.renderer.DestroyBlock(blockWorldPos - chunkOrigin);
                    }
                    else
                    {
                        chunkData.renderer.SpawnBlock(blockWorldPos - chunkOrigin);
                    }

                    
                }
            }
        }
    }

    public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
    {
        return new Vector2Int(blockWorldPos.x / ChunkRenderer.chunkWidth,
            blockWorldPos.z / ChunkRenderer.chunkWidth);
    }
}
