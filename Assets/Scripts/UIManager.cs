using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI positionText;
    public TextMeshPro playerText;
    public TextMeshPro enemyText;

    public void SetPositionText(int x, int z)
    {
        if (positionText == null) return;
        positionText.text = $"Position : ( X:{x}, Z:{z})";
    }
} 