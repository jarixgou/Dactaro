using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    protected CompositeCollider2D _tilemap; // Reference au Tilemap ou les ennemis pourront spawn, props, etc...

    [SerializeField] protected int numberOfEnnemies = 10; // Nombre d'ennemis qui spawn
    [SerializeField] protected GameObject ennemiesPrefab;
    [SerializeField] protected GameObject playerPrefab;

    public void SpwanEssantials(HashSet<Vector2Int> floorPosition)
    {
        List<Vector2Int> _floorPosition = floorPosition.ToList();

        for (int i = 0; i < numberOfEnnemies; i++)
        {
            var positionIndex = Random.Range(0, _floorPosition.Count);
            var floor = _floorPosition[positionIndex];
            // new GameObject("Ennemies" + i).transform.position = new Vector2(floor.x, floor.y);
            Instantiate(ennemiesPrefab, new Vector3(floor.x + 0.5f, floor.y + 0.5f, 0), quaternion.identity);
            _floorPosition.RemoveAt(positionIndex);
        }
    }

    public void SpawnSafePlace(HashSet<Vector2Int> floorSafePlace)
    {
        List<Vector2Int> _floorPositionOfSafePlace = floorSafePlace.ToList();

        var index = Random.Range(0, _floorPositionOfSafePlace.Count);
        var floor = _floorPositionOfSafePlace[index];
        Instantiate(playerPrefab, new Vector3(floor.x + 0.5f, floor.y + 0.5f, 0), Quaternion.identity);
    }
}
    
