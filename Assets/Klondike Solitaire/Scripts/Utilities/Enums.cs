using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enums : MonoBehaviour {
	// Enum to define the different suit colors in a deck of cards
	public enum CardColor { 
		clubs = 0, 	  // Represented by the symbol '♣'
		diamonds = 1, // Represented by the symbol '♦'
		hearts = 2,   // Represented by the symbol '♥'
		spades = 3 	  // Represented by the symbol '♠'
	};

	// Enum to define the different values in a deck of cards
	public enum CardValue { 
		Ace = 1, 	// Represented by the symbol 'A'
		Two = 2, 	// Represented by the symbol '2'
		Three = 3, 	// Represented by the symbol '3'
		Four = 4, 	// Represented by the symbol '4'
		Five = 5, 	// Represented by the symbol '5'
		Six = 6, 	// Represented by the symbol '6'
		Seven = 7, 	// Represented by the symbol '7'
		Eight = 8, 	// Represented by the symbol '8'
		Nine = 9, 	// Represented by the symbol '9'
		Ten = 10, 	// Represented by the symbol '10'
		Jack = 11, 	// Represented by the symbol 'J'
		Queen = 12, // Represented by the symbol 'Q'
		King = 13 	// Represented by the symbol 'K'
	};

	// Enum to define different moves that can be made in a solitaire game
	public enum MoveOptions { 
		waste2tableau, 		  // Move a card from the waste pile to a tableau pile
		waste2foundation, 	  // Move a card from the waste pile to a foundation pile
		tableau2foundation,   // Move a card from a tableau pile to a foundation pile
		foundation2tableau,   // Move a card from a foundation pile to a tableau pile
		tableau2tableau, 	  // Move a card from one tableau pile to another tableau pile
		foundation2foundation // Move a card from one foundation pile to another foundation pile
	};

	// Enum to define different scoring types that can be used in a solitaire game
	public enum ScoringType { 
		none, 		// No scoring 
		normal, 	// Normal scoring system 
		vegas 		// Vegas scoring system
	};

	// Enum to define different types of linear interpolation that can be used in animation
	public enum LerpType {
		hermite, // Hermite interpolation
		sinerp,  // Sine interpolation
		coserp,  // Cosine interpolation
		berp,    // Bounce interpolation
		clerp,   // Circular interpolation
		lerp     // Linear interpolation
	};        
}
