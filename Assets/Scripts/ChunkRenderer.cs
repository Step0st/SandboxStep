using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ChunkRenderer : MonoBehaviour
{
    public const int chunkWidth = 10;
    public const int chunkHeight = 128;
    public const float blockScale = 0.5f;
    
    public ChunkData ChunkData;
    public GameWorld ParentWorld;

    private Mesh chunkMesh;

    private List<Vector3> _vertices = new List<Vector3>();
    private List<Vector2> _uvs = new List<Vector2>();
    private List<int> _triangles = new List<int>();

    void Start()
    {
        chunkMesh = new Mesh();

        RegenerateMesh();

        GetComponent<MeshFilter>().mesh = chunkMesh;

    }

    private void RegenerateMesh()
    {
        _vertices.Clear();
        _uvs.Clear();
        _triangles.Clear();
        
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        chunkMesh.triangles = Array.Empty<int>();
        chunkMesh.vertices = _vertices.ToArray();
        chunkMesh.uv = _uvs.ToArray();
        chunkMesh.triangles = _triangles.ToArray();

        chunkMesh.Optimize();
        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();
        
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Dirt;
        RegenerateMesh();
    }
    
    public void DestroyBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Air;
        RegenerateMesh();
    }

    private void GenerateBlock(int x, int y, int z)
    {
        Vector3Int blockPosition = new Vector3Int(x,y,z);
        BlockType blockType = GetBlockAtPosition(blockPosition);
        
        if (GetBlockAtPosition(blockPosition) == 0) return;

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0)
        {
            GenerateRightSide(blockPosition);
            AddUvs(blockType, Vector2Int.right);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)
        {
            GenerateLeftSide(blockPosition);
            AddUvs(blockType, Vector2Int.left);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(blockPosition);
            AddUvs(blockType, (Vector2Int)Vector3Int.forward);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)
        {
            GenerateBackSide(blockPosition);
            AddUvs(blockType, (Vector2Int)Vector3Int.back);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)
        {
            GenerateTopSide(blockPosition);
            AddUvs(blockType, Vector2Int.up);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0)
        {
            GenerateBottomSide(blockPosition);
            AddUvs(blockType, Vector2Int.down);
        }

    }

    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < chunkWidth &&
            blockPosition.y >= 0 && blockPosition.y < chunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < chunkWidth)
        {
            return ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            if (blockPosition.y < 0 || blockPosition.y >= chunkHeight) return BlockType.Air;

            Vector2Int adjacentChunkPosition = ChunkData.chunkPosition;
            if (blockPosition.x <0)
            {
                adjacentChunkPosition.x--;
                blockPosition.x += chunkWidth;
            } 
            else if (blockPosition.x >= chunkWidth)
            {
                adjacentChunkPosition.x++;
                blockPosition.x -= chunkWidth;
            }
            
            if (blockPosition.z <0)
            {
                adjacentChunkPosition.y--;
                blockPosition.z += chunkWidth;
            } 
            else if (blockPosition.z >= chunkWidth)
            {
                adjacentChunkPosition.y++;
                blockPosition.z -= chunkWidth;
            }

            if (ParentWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }
            else
            {
                return BlockType.Air;
            }
        }

 
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(1,0,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,1,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,0,1) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,1,1) + blockPosition)*blockScale);
        
        AddSquareToVertices();
    }
    
    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0,0,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(0,0,1) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(0,1,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(0,1,1) + blockPosition)*blockScale);

        AddSquareToVertices();
    }
    
    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0,0,1) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,0,1) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(0,1,1) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,1,1) + blockPosition)*blockScale);
        
        AddSquareToVertices();
    }
    
    private void GenerateBackSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0,0,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(0,1,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,0,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,1,0) + blockPosition)*blockScale);
        
        AddSquareToVertices();
    }
    
    private void GenerateTopSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0,1,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(0,1,1) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,1,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,1,1) + blockPosition)*blockScale);
        
        AddSquareToVertices();
    }
    
    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0,0,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,0,0) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(0,0,1) + blockPosition)*blockScale);
        _vertices.Add((new Vector3(1,0,1) + blockPosition)*blockScale);
        
        AddSquareToVertices();
    }

    private void AddSquareToVertices()
    {
        _triangles.Add(_vertices.Count-4);
        _triangles.Add(_vertices.Count-3);
        _triangles.Add(_vertices.Count-2);
        
        _triangles.Add(_vertices.Count-3);
        _triangles.Add(_vertices.Count-1);
        _triangles.Add(_vertices.Count-2);
    }

    private void AddUvs(BlockType blockType, Vector2Int normal)
    {
        Vector2 uv;

        if (blockType == BlockType.Dirt)
        {
            uv = normal == Vector2Int.up ? new Vector2(0f / 256, 240f / 256) :
                normal == Vector2Int.down ? new Vector2(32f / 256, 240f / 256) :
                new Vector2(48f / 256, 240f / 256);
        }
        else if (blockType == BlockType.Stone)
        {
            uv = new Vector2(16f / 256, 240f / 256);
        }
        else
        {
            uv = new Vector2(160f / 256, 240f / 256);
        }

        for (int i = 0; i < 4; i++)
        {
            _uvs.Add(uv);
        }
    }
    
}
