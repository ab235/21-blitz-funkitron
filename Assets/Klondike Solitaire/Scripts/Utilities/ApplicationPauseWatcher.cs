using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationPauseWatcher : MonoBehaviour {

    // Declare a public static Action delegate that will be used to store callbacks
    public static Action OnAppPauseCB;

    public static void RegisterOnAppPauseCB(Action cb)
    {
        // Add the callback to the delegate
        OnAppPauseCB += cb;
    }

    // UnregisterOnAppPauseCB method to remove a callback from the OnAppPauseCB delegate
    public static void UnregisterOnAppPauseCB(Action cb)
    {
        // Remove the callback from the delegate
        OnAppPauseCB -= cb;
    }

    // The OnApplicationPause method is called by Unity when the application pauses or resumes
    private void OnApplicationPause(bool pause)
    {
        // Check if the application has been paused
        if (pause)
        {
            // Run the callbacks stored in the OnAppPauseCB delegate
            OnAppPauseCB.RunAction();
        }
    }
}