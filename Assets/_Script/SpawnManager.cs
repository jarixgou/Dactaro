using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnManager : MonoBehaviour
{
    
    [SerializeField] protected CompositeCollider2D _tilemap; // Reference au Tilemap ou les ennemis pourront spawn, props, etc...

    [SerializeField] protected int numberOfEnnemies = 10; // Nombre d'ennemis qui spawn
    [SerializeField] protected GameObject[] ennemiesPrefab;
    
    public void SpwanEnnemies(HashSet<Vector2Int> floorPosition)
    {
        for (int i = 0; i < numberOfEnnemies; i++)
        {
            var positionIndex = Random.Range(0, floorPosition.Count);
            List<Vector2Int> _floorPosition = floorPosition.ToList();
            var floor = _floorPosition[positionIndex];
            new GameObject("Ennemies" + i).transform.position = new Vector2(floor.x, floor.y);
        }
    }
}
