using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script handling current shape's status (game progress) indicator with text
/// </summary>
public class UIShapeStatusController : MonoBehaviour
{

    /// <summary>
    /// Singleton's instance.
    /// </summary>
    public static UIShapeStatusController instance;
    UIShapeStatusController() { instance = this; }

    // UI elements
    public Text ShapeStatusText;

    // Help variables
    private readonly float referenceValue = 16.0575f;
    private readonly float gameoverPercentage = 2f / 3f;

    // Update is called once per frame
    void Update()
    {
        float percentage = Math3d.GetPolygonAreaSize(ShapeController.instance.GetAllVerticesPositions()) / referenceValue;
        percentage = (percentage - gameoverPercentage) / (1f - gameoverPercentage);
        ShapeStatusText.text = Mathf.RoundToInt(percentage * 100f) + "%";
        ShapeStatusText.enabled = percentage > 0f;

        if (UIController.instance.GameInteractive && Mathf.RoundToInt(percentage * 100f) <= 0f)
        {
            UIController.instance.FireGameOver();
        }
    }
}
