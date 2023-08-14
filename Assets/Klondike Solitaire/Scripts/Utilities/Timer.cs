using System;

/// <summary>
/// Class runs delayed actions
/// </summary>
public class Timer {

    // Constructor that takes in a time and an action to run when the time is finished
    public Timer(float time, Action finishAction) {
        // Calls the RunTimer method on the TweenAnimator instance, passing in the time and finishAction as parameters
        TweenAnimator.instance.RunTimer(time, finishAction);
    }
}
