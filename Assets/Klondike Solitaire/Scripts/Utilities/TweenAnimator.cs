using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenAnimator : MonoBehaviour {

	// A public static variable that holds a reference to the TweenAnimator instance.
	public static TweenAnimator instance;

	// A public variable that holds the animation method to be used.
	public Enums.LerpType animationMethod;

	// A public variable that holds the shake power value.
	public float shakePower;

	// A public variable that holds the shake speed value.
	public float shakeSpeed;

	// A private delegate variable that will be used to store a reference to a specific animation method.
	private delegate float AnimationDelegate (float start, float end, float value);

	// The Awake() method sets the instance variable to the current instance of TweenAnimator.
	private void Awake() {
		instance = this;
	}

	// The GetAnimationMethod() method returns the animation delegate for the selected animation method.
	private AnimationDelegate GetAnimationMethod() {
		// A switch statement that selects the animation delegate based on the animationMethod variable.
		switch (animationMethod)
		{
			// If the animationMethod is set to hermite, return the Mathfx.Hermite delegate.
			case Enums.LerpType.hermite:
				return Mathfx.Hermite;
			// If the animationMethod is set to sinerp, return the Mathfx.Sinerp delegate.
			case Enums.LerpType.sinerp:
				return Mathfx.Sinerp;
			// If the animationMethod is set to coserp, return the Mathfx.Coserp delegate.
			case Enums.LerpType.coserp:
				return Mathfx.Coserp;
			// If the animationMethod is set to berp, return the Mathfx.Berp delegate.
			case Enums.LerpType.berp:
				return Mathfx.Berp;
			// If the animationMethod is set to clerp, return the Mathfx.Clerp delegate.
			case Enums.LerpType.clerp:
				return Mathfx.Clerp;
			// If the animationMethod is set to lerp, return the Mathfx.Lerp delegate.
			case Enums.LerpType.lerp:
				return Mathfx.Lerp;
			// If the animationMethod is set to an unsupported value, return the Mathfx.Lerp delegate.
			default:
				return Mathfx.Lerp;
		}
	}

	// Each method starts a coroutine by calling StartCoroutine and passing the corresponding animation coroutine as a parameter.
	// The methods also take various parameters such as target transform, animation end position or value, animation time, delay, and onComplete or onUpdate actions.
	public Coroutine RunMoveAnimation(Transform target, Vector3 to, float animTime, float delay = 0, Action onComplete = null, Action onUpdate = null, bool isLocal = false) {
		// Start a coroutine that performs a move animation on the target transform.
		return StartCoroutine(MoveAnimation(target, to, animTime, delay, onComplete, onUpdate, isLocal));
	}

	public Coroutine RunRotationAnimation(Transform target, Vector3 to, float animTime, float delay = 0, Action onComplete = null, Action onUpdate = null) {
		// Start a coroutine that performs a rotation animation on the target transform.
		return StartCoroutine(RotationAnimation(target, to, animTime, delay, onComplete, onUpdate));
	}

	public Coroutine RunScaleAnimation(Transform target, Vector3 to, float animTime, float delay = 0, Action onComplete = null, Action onUpdate = null) {
		// Start a coroutine that performs a scale animation on the target transform.
		return StartCoroutine(ScaleAnimation(target, to, animTime, delay, onComplete, onUpdate));
	}

	public Coroutine RunFadeAnimation(CanvasGroup target, float to, float animTime, float delay = 0, Action onComplete = null, Action onUpdate = null) {
		// Start a coroutine that performs a fade animation on the target canvas group.
		return StartCoroutine(FadeAnimation(target, to, animTime, delay, onComplete, onUpdate));
	}

	public Coroutine RunTimer(float animTime, Action onComplete) {
		// Start a coroutine that runs a timer for a specified time.
		return StartCoroutine(RunTimerCoroutine(animTime, onComplete));
	}

	public Coroutine RunShakeAnimation(Transform target, float animTime, Action onComplete = null) {
		// Start a coroutine that performs a shake animation on the target transform.
		return StartCoroutine(ShakeAnimation(target, animTime, onComplete));
	}

	IEnumerator MoveAnimation(Transform target, Vector3 to, float animTime, float delay, Action onComplete, Action onUpdate, bool isLocal) {
		// wait for the specified delay time
		if (delay > 0)
			yield return new WaitForSeconds(delay);

		// get the delegate for animation method
		AnimationDelegate animationDelegate = GetAnimationMethod();
		Vector3 from = isLocal ? target.localPosition : target.position;

		// keep track of the remaining time for animation
		float timeRemaining = animTime;
		float t;
		// animate the position
		while (timeRemaining > 0)
		{
			// decrease the remaining time by deltaTime
			timeRemaining -= Time.deltaTime;
			t = timeRemaining / animTime;
			// animate based on whether it's local or global
			if (isLocal)
				target.localPosition = new Vector3(animationDelegate(to.x, from.x, t), animationDelegate(to.y, from.y, t), from.z);
			else
				target.position = new Vector3(animationDelegate(to.x, from.x, t), animationDelegate(to.y, from.y, t), from.z);

			// run the onUpdate action
			onUpdate.RunAction();
			yield return null;
		}

		// set the final position
		if(isLocal)
			target.localPosition = to;
		else
			target.position = to;

		// run the onComplete action
		onComplete.RunAction();
	}

	IEnumerator RotationAnimation(Transform target, Vector3 to, float animTime, float delay, Action onComplete, Action onUpdate) {
		// wait for the specified delay time
		if (delay > 0)
			yield return new WaitForSeconds(delay);

		// get the delegate for animation method
		AnimationDelegate animationDelegate = GetAnimationMethod();
		Vector3 from = target.rotation.eulerAngles;

		// keep track of the remaining time for animation
		float timeRemaining = animTime;
		float t;
		// animate the rotation
		while (timeRemaining > 0)
		{
			// decrease the remaining time by deltaTime
			timeRemaining -= Time.deltaTime;
			t = timeRemaining / animTime;
			// animate the rotation
			target.rotation = Quaternion.Euler(animationDelegate(to.x, from.x, t), animationDelegate(to.y, from.y,t), animationDelegate(to.z, from.z, t));

			// run the onUpdate action
			onUpdate.RunAction();
			yield return null;
		}
		// set the final rotation
		target.rotation = Quaternion.Euler(to);

		// run the onComplete action
		onComplete.RunAction();
	}

	// ScaleAnimation IEnumerator to animate the scaling of a Transform component
	IEnumerator ScaleAnimation(Transform target, Vector3 to, float animTime, float delay, Action onComplete, Action onUpdate) {
	    // If delay is greater than 0, wait for the specified time before starting the animation
	    if (delay > 0)
	        yield return new WaitForSeconds(delay);

	    // Get the delegate that specifies the animation method
	    AnimationDelegate animationDelegate = GetAnimationMethod();

	    // Get the starting scale of the target transform
	    Vector3 from = target.localScale;
	    float timeRemaining = animTime;
	    float t;
	    // Loop while timeRemaining is greater than 0
	    while (timeRemaining > 0)
	    {
	        timeRemaining -= Time.deltaTime;
	        // Calculate t, the time fraction for the animation
	        t = timeRemaining / animTime;
	        // Apply the animation to each component of the target's local scale
	        target.localScale = new Vector3(
	            animationDelegate(to.x, from.x, t), 
	            animationDelegate(to.y, from.y, t), 
	            animationDelegate(to.z, from.z, t)
	        );
	        // Run the onUpdate action if it's specified
	        onUpdate.RunAction();
	        yield return null;
	    }
	    // Set the target's local scale to the final scale
	    target.localScale = to;
	    // Run the onComplete action if it's specified
	    onComplete.RunAction();
	}

	// FadeAnimation IEnumerator to animate the alpha value of a CanvasGroup component
	IEnumerator FadeAnimation(CanvasGroup target, float to, float animTime, float delay, Action onComplete, Action onUpdate) {
	    // If delay is greater than 0, wait for the specified time before starting the animation
	    if (delay > 0)
	        yield return new WaitForSeconds(delay);

	    // Get the delegate that specifies the animation method
	    AnimationDelegate animationDelegate = GetAnimationMethod();

	    // Get the starting alpha value of the target CanvasGroup component
	    float from = target.alpha;
	    float timeRemaining = animTime;
	    float t;
	    // Loop while timeRemaining is greater than 0
	    while (timeRemaining > 0)
	    {
	        timeRemaining -= Time.deltaTime;
	        // Calculate t, the time fraction for the animation
	        t = timeRemaining / animTime;
	        // Apply the animation to the target's alpha value
	        target.alpha = animationDelegate(to, from, t);
	        // Run the onUpdate action if it's specified
	        onUpdate.RunAction();
	        yield return null;
	    }
	    // Set the target's alpha value to the final value
	    target.alpha = to;
	    // Run the onComplete action if it's specified
	    onComplete.RunAction();
	}

	//IEnumerator to run the timer coroutine 
	IEnumerator RunTimerCoroutine(float animTime, Action onComplete) {
		//Wait for the specified time before running the completion action
		yield return new WaitForSeconds(animTime);
		onComplete.RunAction();
	}

	//IEnumerator to run shake animation coroutine
	IEnumerator ShakeAnimation(Transform target, float animTime, Action onComplete) {
		//Record the starting position of the target transform
		Vector3 from = target.position;
		//Calculate the remaining time for the animation
		float timeRemaining = animTime;
		float t = 0;
		//Run the loop until the time for the animation is reached
		while (t < timeRemaining)
		{
			t += Time.deltaTime;
			//Set the target transform's position to the starting position + a shake offset
			target.position = from + Vector3.right * shakePower * Mathf.Sin(t * shakeSpeed);
			yield return null;
		}
		//Reset the target transform's position to its starting position
		target.transform.position = from;
		//Run the completion action
		onComplete.RunAction();
	}

	//Method to kill a coroutine
	public void Kill(Coroutine animation) {
		//Check if the passed in coroutine is not null
		if (animation != null)
			//Stop the coroutine
			StopCoroutine(animation);
	}
}
