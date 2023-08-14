using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScaleFactorConverter  {
    // Private variable to store a reference to the game canvas
    private static Canvas gameCanvas;

	// Method to multiply a given value by the scale factor of the game canvas
    public static float MultiplyByScaleFactor(float value)
    {
        // Check if the canvas is empty, return 0 and log an error message if so
        if(CheckIfCanvasIsEmpty())
        {
            Debug.LogError("Empty canvas!");
            return 0;
        }
        // Return the result of multiplying the value by the canvas scale factor
        return value * gameCanvas.scaleFactor;
    }

    // Method to divide a given value by the scale factor of the game canvas
    public static float DivideByScaleFactor(float value)
    {
        // Check if the canvas is empty, return 0 and log an error message if so
        if (CheckIfCanvasIsEmpty())
        {
            Debug.LogError("Empty canvas!");
            return 0;
        }
        // Return the result of dividing the value by the canvas scale factor
        return value / gameCanvas.scaleFactor;
    }

    // Method to check if the canvas is empty
    private static bool CheckIfCanvasIsEmpty()
    {
        // If the canvas is not set, try to find it using its tag
        if (gameCanvas == null)
        {
            // Get the game object with the "GameCanvas" tag
            GameObject canvasGO = GameObject.FindGameObjectWithTag("GameCanvas");
            // If the canvas game object is found, get its Canvas component and set it to the gameCanvas variable
            if (canvasGO != null)
            {
                gameCanvas = canvasGO.GetComponent<Canvas>();
                // Return false to indicate that the canvas is not empty
                return false;
            }
            // Return true to indicate that the canvas is empty
            return true;
        }
        // Return false to indicate that the canvas is not empty
        return false;
    }
}
