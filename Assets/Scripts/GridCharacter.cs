using System.Collections.Generic;
using UnityEngine;

public class GridCharacter : MonoBehaviour
{
    [SerializeField] private PathfindingSystem pathfinding;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    
    private List<GridTile> currentPath;
    private int currentPathIndex;
    [HideInInspector] public bool isMoving;
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    
    
    
    

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleAnimation();
    }

    //set bool isMoving to true so the player moving animation can be played.
    private void HandleAnimation()
    {
        if(animator == null) return;
        animator.SetBool(IsMoving, isMoving);
    }

    //Raycast to get the position of the mouse click and set the destination of the player to the target/destination position
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

    //Set the destination of the player to the target position on mouse click
    private void SetDestination(Vector3 targetPosition)
    {
        List<GridTile> newPath = pathfinding.FindPath(GetVector3IntPosition(transform.position), GetVector3IntPosition(targetPosition));
        if (newPath != null && newPath.Count > 0)
        {
            //Current path or current headed path, is set to the newest path on mouse click
            currentPath = newPath;
            currentPathIndex = 0;
            isMoving = true;
        }
    }

    //
    private void HandleMovement()
    {
        if (!isMoving || currentPath == null || currentPathIndex >= currentPath.Count) return;

        // Get the target position from the current path index and maintain the y position
        Vector3 targetPosition = currentPath[currentPathIndex].transform.position;
        targetPosition.y = transform.position.y;

        // Move the character towards the target position 
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        transform.LookAt(targetPosition);
        
        if (uiManager != null && uiManager.playerText != null)
        {
            uiManager.playerText.transform.rotation = Camera.main.transform.rotation;
        }
        

        // If the character is close enough to the target position, move to the next path index
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
            // If the path index is out of bounds, stop moving
            if (currentPathIndex >= currentPath.Count)
            {
                isMoving = false;
            }
        }
    }
    
    //Convert the player's position to a Vector3Int position so only return integer values
    private Vector3Int GetVector3IntPosition(Vector3 position)
    {
        return new Vector3Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y),
            Mathf.RoundToInt(position.z)
        );
    }
}
