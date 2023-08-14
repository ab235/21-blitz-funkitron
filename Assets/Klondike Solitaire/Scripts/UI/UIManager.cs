using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

// This script is the UIManager for the game.
// It's responsible for managing the UI screens and animations in the game.
public class UIManager : MonoBehaviour {
	// The UIManager uses the singleton pattern so there's only one instance of the UIManager in the game.
	// The instance variable holds a reference to the UIManager instance.
	IHUD[] UIScreens;
	public static UIManager instance;
	private AnimationQueueController animationQueueController;

	// The Awake() method initializes the UIManager when the game starts.
	// It sets the instance variable to the current instance of the UIManager.
	// The method then finds all the UI screens in the scene and sets them to active.
	// It also finds the AnimationQueueController component in the scene and stores a reference to it in the animationQueueController variable.
	void Awake()
	{

		instance = this;
		UIScreens = GetComponentsInChildren<IHUD>(true);
		animationQueueController = FindObjectOfType<AnimationQueueController>();
		for (int i = 0; i < UIScreens.Length; i++)
		{
			UIScreens[i].SetActive(true);
		}
	}

	// The ResetGameState() method resets the state of the game by calling the ResetState method on all UI screens and the animationQueueController.
	// The BroadcastMessage method is used to send a message to all components attached to the UIManager game object.
	public void ResetGameState()
	{
		BroadcastMessage("ResetState");
		animationQueueController.ResetState();
	}
}