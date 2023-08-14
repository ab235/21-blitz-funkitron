using UnityEngine;
using System.Collections;

public class DimScreen : MonoBehaviour {

	// The instance variable is a static reference to the script component on a game object in the scene.
	public static DimScreen instance;

	// speed variable controls the animation speed for dimming the screen.
	public float speed;
	
	// currentDim is a read-only property that returns the current alpha value of the canvas group component.
	public float currentDim {
		get { return cg.alpha; }
		private set { cg.alpha = value; }
	}
	
	// Variables topCanvas, rt, and cg are used for accessing and manipulating the canvas and canvas group components attached to the game object.
	private Canvas topCanvas;
	private RectTransform rt;
	
	protected CanvasGroup cg;

	// Awake method is called when the script component is initialized. It sets the instance variable, initializes the rect transform component,
	protected virtual void Awake() {
		instance = this;
		rt = transform as RectTransform;
		cg = GetComponent<CanvasGroup>();

		// finds the canvas component attached to the game object with "TopUI" tag and sets it to topCanvas.
		GameObject topUI = GameObject.FindGameObjectWithTag("TopUI");
		if (topUI != null)
			topCanvas = topUI.GetComponent<Canvas>();
	}

	// MakeScreenDark method sets the canvas group's blocksRaycasts property to true and runs a fade animation to darken the screen to the specified darkDimValue.
	public void MakeScreenDark(float darkDimValue = 0.65f)
	{
		cg.blocksRaycasts = true;
		TweenAnimator.instance.RunFadeAnimation(cg, darkDimValue, speed);
	}

	// MakeScreenLight method runs a fade animation to lighten the screen to the specified lightDimValue and sets the canvas group's blocksRaycasts property to false after the animation is complete.
	public void MakeScreenLight(float lightDimValue = 0f)
	{
		TweenAnimator.instance.RunFadeAnimation(cg, lightDimValue, speed, onComplete: () => cg.blocksRaycasts = false);
	}
	
	// Update method checks if the screen is not fully transparent, and calls the RefreshBar method.
	protected virtual void Update()
	{
		if(cg.alpha > 0)
			RefreshBar();
	}

	// RefreshBar method adjusts the position of the dimming screen game object based on the scale factor of the top canvas and the constant size of the top bar.
	private void RefreshBar()
	{
		float top =  (topCanvas.scaleFactor / BehaviourSettings.instance.GetScaleFactor()) * Constants.TOP_BAR_SIZE;
		rt.offsetMax = new Vector2 (rt.offsetMax.x, -top);
	}
}
