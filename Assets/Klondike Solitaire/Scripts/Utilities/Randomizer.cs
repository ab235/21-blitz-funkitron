using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Randomizer : MonoBehaviour {
	
	// Function to get a random number between two given floats
	public static float GetRandomNumber(float min, float max) {
		int seed = Guid.NewGuid().GetHashCode(); // Generate a new seed value each time this function is called
		UnityEngine.Random.InitState(seed); // Set the seed for Unity's random number generator
		return UnityEngine.Random.Range(min, max); // Return a random number between the given min and max values
	}

	// Function to get a random number between two given integers
	public static int GetRandomNumber(int min, int max) {
		int seed = Guid.NewGuid().GetHashCode(); // Generate a new seed value each time this function is called
		UnityEngine.Random.InitState(seed); // Set the seed for Unity's random number generator
		return UnityEngine.Random.Range(min, max); // Return a random number between the given min and max values
	}
}
