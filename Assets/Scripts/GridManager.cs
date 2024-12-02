using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public struct GridObject
{
    public Vector3 position;
    public string tag;
}
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public float cellSize = 1.0f;
    private HashSet<GridObject> gridHashObjects;
    private Dictionary<Vector3Int, List<GridObject>> gridHashDictionary;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Another instance of Grid already exists.");
            Destroy(this);
            return;
        }
        
        Instance = this;
        
        gridHashObjects = new HashSet<GridObject>();
        gridHashDictionary = new Dictionary<Vector3Int, List<GridObject>>();
    }

    private Vector3Int GetGridPosition(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.y / cellSize),
            Mathf.FloorToInt(position.z / cellSize)
        );
    }

    
    public void AddObject(GridObject gridObject)
    {
        gridHashObjects.Add(gridObject);

        Vector3Int gridPosition = GetGridPosition(gridObject.position);
        if (!gridHashDictionary.TryGetValue(gridPosition, out var cellObjects))
        {
            cellObjects = new List<GridObject>();
            gridHashDictionary[gridPosition] = cellObjects;
        }
        cellObjects.Add(gridObject);
    }

    public void RemoveObject(Vector3 position)
    {
        Vector3Int hashKey = GetGridPosition(position);
        if (gridHashDictionary.TryGetValue(hashKey, out var gridObjects))
        {
            gridObjects.RemoveAll(obj => obj.position == position);
            if (gridObjects.Count == 0)
            {
                gridHashDictionary.Remove(hashKey);
            }
        }
        gridHashObjects.RemoveWhere(obj => obj.position == position);
    }

    public bool IsPositionOccupied(Vector3 position)
    {
        Vector3Int hashKey = GetGridPosition(position);
        if (gridHashDictionary.TryGetValue(hashKey, out var cellObjects))
        {
            return cellObjects.Any(obj => obj.position == position);
        }
        return false;
    }

    public string GetTag(Vector3 position)
    {
        Vector3Int hashKey = GetGridPosition(position);
        if (gridHashDictionary.TryGetValue(hashKey, out var cellObjects))
        {
            foreach (var obj in cellObjects)
            {
                if (obj.position == position)
                {
                    Debug.Log(obj.tag);
                    return obj.tag;
                }
            }
        }
        return null;
    }
    
    public void Reset()
    {
        gridHashObjects.Clear();
        gridHashDictionary.Clear();
    }

    private void OnDrawGizmos()
    {
        if (gridHashDictionary != null)
        {
            Gizmos.color = Color.red;
            foreach (var cell in gridHashDictionary)
            {
                foreach (var obj in cell.Value)
                {
                    Gizmos.DrawSphere(obj.position, 0.2f);
                }
            }
        }
    }
}
