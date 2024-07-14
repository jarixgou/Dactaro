using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] 
    protected TilemapVisualizer _tilemapVisualizer = null;

    [SerializeField] 
    protected SpawnManager _spawnManager = null;
    
    [SerializeField] 
    protected Vector2Int startPosition = Vector2Int.zero;
    
    public void GenerateDungeon()
    {
        _tilemapVisualizer.ClearTile();
        Destroy(GameObject.FindGameObjectWithTag("Ennemies"));
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
