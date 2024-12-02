using TMPro;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject highlightGameObject;
    [SerializeField] private int gridSpacing = 1;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private float highlightPositionY = 0.51f;
    public GameObject[,] grid;

    [Space] [Header("Ensure that X,Z Size Match the ObstacleManager & ObstacleScriptableObject")] [SerializeField]
    public int xSize = 10;

    [SerializeField] public int zSize = 10;


    private void Start()
    {
        highlightGameObject.SetActive(false);
        CreateGrid();
    }

    private void CreateGrid()
    {
        for (var x = 0; x < xSize; x++)
        {
            for (var z = 0; z < zSize; z++)
            {
                var position = new Vector3Int(x * gridSpacing, 0, z * gridSpacing);
                var cube = Instantiate(cubePrefab, position, Quaternion.identity);
                cube.AddComponent<BoxCollider>();
                cube.AddComponent<GridTile>().SetPosition(x, z);
                cube.transform.name = $"Grid Pos: ({x}, {z})";
                cube.transform.SetParent(transform);

                if (grid == null) continue;
                grid[x, z] = cube;
            }
        }
    }

    private void Update()
    {
        RayCastOnMouseClick();
    }

    private void RayCastOnMouseClick()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            var currentTile = hit.collider.GetComponent<GridTile>();

            if (currentTile != null)
            {
                highlightGameObject.SetActive(true);
                
                highlightGameObject.transform.position = new Vector3(currentTile.transform.position.x, highlightPositionY, currentTile.transform.position.z);
                positionText.text = $"Position: ({currentTile.GridX}, {currentTile.GridZ})";
            }
            else
            {
                positionText.text = "Position: (N/A)";
                highlightGameObject.SetActive(false);
            }
        }
        else
        {
            positionText.text = "Position: (N/A)";
            highlightGameObject.SetActive(false);
        }
    }
}