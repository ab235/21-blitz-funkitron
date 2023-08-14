using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Enum AnimationStatus is used to keep track of the status of the animation.
public enum AnimationStatus { none, inProgress }

// This script creates an AnimationQueueController class that implements a queue of actions and manages their execution.
// The script is responsible for ensuring that animations run one at a time and also keeping track of which animation is currently running.
public class AnimationQueueController : MonoBehaviour
{
    private Queue<Action> actionQueue;
    [SerializeField]
    private string lastRunAnimation;
    [SerializeField]
    private AnimationStatus animationStatus = AnimationStatus.none;
    private int cardsInMove = 0;

    void Awake()
    {
        ResetState();
    }

    // The ResetState() method resets the animation status, last run animation and number of cards in motion.
    public void ResetState()
    {
        animationStatus = AnimationStatus.none;
        lastRunAnimation = "";
        cardsInMove = 0;
        actionQueue = new Queue<Action>();
    }

    // The AddActionToQueue() method adds an action to the queue and immediately runs it if the animation is not in progress and there are no cards in motion.
    public void AddActionToQueue(Action action)
    {
        if (animationStatus == AnimationStatus.none && cardsInMove  <= 0)
        {
            cardsInMove = 0;
            lastRunAnimation = action.Method.Name;
            action();
        }
        else
        {
            actionQueue.Enqueue(action);
        }
    }

    // The SetAnimationStatus() method sets the status of the animation.
    public void SetAnimationStatus(AnimationStatus status)
    {
        animationStatus = status;
    }

    // The CastNextAnimation() method casts the next animation in the queue if there are any actions left.
    public void CastNextAnimation()
    {
        animationStatus = AnimationStatus.none;
        lastRunAnimation = "";
        if (actionQueue.Count > 0)
        {
            lastRunAnimation = actionQueue.Peek().Method.Name;
            actionQueue.Dequeue()();
        }
    }
    
    // The AddMovingCard() method increments the count of cards in motion.
    public void AddMovingCard()
    {
        cardsInMove++;
    }

    // The FinishCardMoving() method decrements the count of cards in motion and calls CastNextAnimation() if the count reaches 0.
    public void FinishCardMoving()
    {
        cardsInMove--;
        if(cardsInMove <= 0)
        {
            cardsInMove = 0;
            CastNextAnimation();
        }
    }

    // The IsCurrentActionUndoOrGetCard() method returns true if the last run animation is "GetCard" or "Undo", otherwise false.
    public bool IsCurrentActionUndoOrGetCard()
    {
        if (lastRunAnimation == "GetCard" || lastRunAnimation == "Undo")
            return true;
        return false;
    }

    // The IsBusy() method returns true if the animation status is "inProgress", otherwise false.
    public bool IsBusy()
    {
        return animationStatus == AnimationStatus.inProgress;
    }

    // The GetFirstActionName() method returns the name of the last run animation.
    public string GetFirstActionName()
    {
        return lastRunAnimation;
    }
}
