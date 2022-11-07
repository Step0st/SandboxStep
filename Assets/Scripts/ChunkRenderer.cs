using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ChunkRenderer : MonoBehaviour
{
    public const int chunkWidth = 10;
    public const int chunkHeight = 128;
    
    public int[,,] Blocks = new int[chunkWidth, chunkHeight, chunkWidth];

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    void Start()
    {
        Mesh chunkMesh = new Mesh();

        Blocks = TerrainGenerator.GenerateTerrain();
        
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkWidth; z++)
                {
                    GenerateBlock(x,y,z);
                }
            }
        }
        
        chunkMesh.vertices = _vertices.ToArray();
        chunkMesh.triangles = _triangles.ToArray();
        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();
        
        GetComponent<MeshFilter>().mesh = chunkMesh;
    }

    private void GenerateBlock(int x, int y, int z)
    {
        Vector3Int blockPosition = new Vector3Int(x,y,z);
        
        if (GetBlockAtPosition(blockPosition) == 0) return;

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0) GenerateRightSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)GenerateLeftSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)GenerateFrontSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)GenerateBackSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)GenerateTopSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0)GenerateBottomSide(blockPosition);

    }

    private int GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < chunkWidth &&
            blockPosition.y >= 0 && blockPosition.y < chunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < chunkWidth)
        {
            return Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            return 0;
        }
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(1,0,0) + blockPosition);
        _vertices.Add(new Vector3(1,1,0) + blockPosition);
        _vertices.Add(new Vector3(1,0,1) + blockPosition);
        _vertices.Add(new Vector3(1,1,1) + blockPosition);
        
        AddSquareToVertices();
    }
    
    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0,0,0) + blockPosition);
        _vertices.Add(new Vector3(0,0,1) + blockPosition);
        _vertices.Add(new Vector3(0,1,0) + blockPosition);
        _vertices.Add(new Vector3(0,1,1) + blockPosition);

        AddSquareToVertices();
    }
    
    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0,0,1) + blockPosition);
        _vertices.Add(new Vector3(1,0,1) + blockPosition);
        _vertices.Add(new Vector3(0,1,1) + blockPosition);
        _vertices.Add(new Vector3(1,1,1) + blockPosition);
        
        AddSquareToVertices();
    }
    
    private void GenerateBackSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0,0,0) + blockPosition);
        _vertices.Add(new Vector3(0,1,0) + blockPosition);
        _vertices.Add(new Vector3(1,0,0) + blockPosition);
        _vertices.Add(new Vector3(1,1,0) + blockPosition);
        
        AddSquareToVertices();
    }
    
    private void GenerateTopSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0,1,0) + blockPosition);
        _vertices.Add(new Vector3(0,1,1) + blockPosition);
        _vertices.Add(new Vector3(1,1,0) + blockPosition);
        _vertices.Add(new Vector3(1,1,1) + blockPosition);
        
        AddSquareToVertices();
    }
    
    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0,0,0) + blockPosition);
        _vertices.Add(new Vector3(1,0,0) + blockPosition);
        _vertices.Add(new Vector3(0,0,1) + blockPosition);
        _vertices.Add(new Vector3(1,0,1) + blockPosition);
        
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
    
}
