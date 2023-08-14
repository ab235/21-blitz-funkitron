using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BehaviourSettings : MonoBehaviour {
	// Singleton instance of the script.
    public static BehaviourSettings instance { get; private set; }

    // Boolean to check if the game is in test mode.
    public bool isTestMode;

    // Canvas component reference.
    private Canvas canvas;

    // Initialization method.
    void Awake()
    {
        // Set the target frame rate.
        Application.targetFrameRate = 60;

        // Assign the instance to this script.
        instance = this;

        // Disable multi-touch.
        Input.multiTouchEnabled = false;

        // Get the canvas component.
        canvas = GetComponent<Canvas>();
    }

    // Method to get the scale factor.
    public float GetScaleFactor()
    {
        // Return the canvas scale factor.
        return canvas.scaleFactor;
    }

    // Method to calculate the card offset.
    public float CalculateCardsOffset(float cardHeight)
    {
        // Offset based on screen orientation.
        float offset = ScreenOrientationSwitcher.IsPortrait() ? Constants.TABLEAU_SPACING : Constants.LANDSCAPE_TABLEAU_SPACING;

        // Return the offset.
        return (cardHeight + offset);
    }
}