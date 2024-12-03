using System.Collections.Generic;
using UnityEngine;

public class GridCharacter : MonoBehaviour
{
    [SerializeField] private PathfindingSystem pathfinding;
    [SerializeField] private float moveSpeed = 5f;
    
    private List<GridTile> currentPath;
    private int currentPathIndex;
    public bool isMoving;

    private void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GridTile targetTile = hit.collider.GetComponent<GridTile>();
                if (targetTile != null && targetTile.IsWalkable)
                {
                    SetDestination(targetTile.transform.position);
                }
            }
        }
    }

    private void SetDestination(Vector3 targetPosition)
    {
        List<GridTile> newPath = pathfinding.FindPath(
            GetVector3IntPosition(transform.position),
            GetVector3IntPosition(targetPosition)
        );
        if (newPath != null && newPath.Count > 0)
        {
            currentPath = newPath;
            currentPathIndex = 0;
            isMoving = true;
        }
    }

    private void HandleMovement()
    {
        if (!isMoving || currentPath == null || currentPathIndex >= currentPath.Count) return;

        Vector3 targetPosition = currentPath[currentPathIndex].transform.position;
        targetPosition.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
            if (currentPathIndex >= currentPath.Count)
            {
                isMoving = false;
            }
        }
    }
    
    private Vector3Int GetVector3IntPosition(Vector3 position)
    {
        return new Vector3Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y),
            Mathf.RoundToInt(position.z)
        );
    }
}

// IAI.cs