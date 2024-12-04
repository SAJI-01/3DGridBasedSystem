using TMPro;
using TMPro.Examples;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI positionText;
    public TextMeshPro playerText;
    public TextMeshPro enemyText;

    public void SetPositionText(int x, int z)
    {
        positionText.text = $"Position : ( X:{x}, Z:{z})";
    }
} 