using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI positionText;

    public void SetPositionText(int x, int z)
    {
        positionText.text = $"Position : ( X:{x}, Z:{z})";
    }
} 