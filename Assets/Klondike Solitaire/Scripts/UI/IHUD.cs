using UnityEngine;
using System.Collections;

// This script implements the interface for the HUD (Heads Up Display) in Unity
public class IHUD : MonoBehaviour {

	// Sets the active state of the game object to the state passed in as a parameter
	public void SetActive(bool state)
	{
		gameObject.SetActive(state);
	}

	// Method to be called when the HUD should be shown
	public virtual void OnShow()
	{
		// Calls the MakeScreenLight method on the instance of the DimScreen class
		DimScreen.instance.MakeScreenDark();
		SetActive(true);
	}

	//Method to be called when the HUD should be hidden
	public virtual void OnBack()
	{
		// Calls the MakeScreenLight method on the instance of the DimScreen class
		DimScreen.instance.MakeScreenLight();
		SetActive(false);
	}
}
