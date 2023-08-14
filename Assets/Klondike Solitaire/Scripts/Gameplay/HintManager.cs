using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
// This script contains a class called HintPossibilities that holds several lists of cards and transforms
public class HintPossibilities {
	// The lists represent different possibilities for the cards in a game of solitaire
	public List<Card> possibleCardsTableau { get; private set; }	// The possibleCardsTableau list contains all the cards that are in the Tableau piles
	public List<Card> possibleCardsFoundation { get; private set; } // The possibleCardsFoundation list contains all the cards that are in the Foundation piles
	public List<Card> possibleOnTopCardsTableau { get; private set; } // The possibleOnTopCardsTableau list contains all the cards that are on top of the Tableau 
	public List<Transform> possibleEmptyTableau; // The possibleEmptyTableau list contains all the empty Tableau pile transforms
	public List<Transform> possibleEmptyFoundation; // The possibleEmptyFoundation list contains all the empty Foundation pile transforms
	public List<Card> possibleWasteCards { get; private set; } // The possibleWasteCards list contains all the cards in the Waste pile
	public List<Card> possibleStockCards { get; private set; } // The possibleStockCards list contains all the cards in the Stock pile

	// The constructor takes in a boolean parameter "searchAllStockAndWaste"
	// If searchAllStockAndWaste is true, the constructor retrieves all the cards in the Stock pile
	// If searchAllStockAndWaste is false, the constructor only retrieves the top card in the Waste pile
	public HintPossibilities(bool searchAllStockAndWaste) {
		if (BoardManager.instance != null)
		{
			possibleCardsTableau = BoardManager.instance.GetTableauCards();
			possibleOnTopCardsTableau = BoardManager.instance.GetOnTopTableauCards(out possibleEmptyTableau);
			possibleCardsFoundation = BoardManager.instance.GetFoundationCards(out possibleEmptyFoundation);
			possibleWasteCards = BoardManager.instance.GetCurrentWasteCards(searchAllStockAndWaste);
			if (searchAllStockAndWaste)
				possibleStockCards = BoardManager.instance.GetCurrentStockCards();
		}
	}

	private void DebugInfo() {
		Debug.Log("Possbile tableaus cards");

		foreach (var item in possibleCardsTableau)
		{
			Debug.Log(item.name);
		}
		Debug.Log("Possbile empty tableaus");

		foreach (var item in possibleEmptyTableau)
		{
			Debug.Log(item.name);
		}
		Debug.Log("Possbile foundation cards");

		foreach (var item in possibleCardsFoundation)
		{
			Debug.Log(item.name);
		}
		Debug.Log("Possbile empty founds");

		foreach (var item in possibleEmptyFoundation)
		{
			Debug.Log(item.name);
		}
		Debug.Log("Possbile waste card");

		foreach (var item in possibleWasteCards)
		{
			Debug.Log(item.name);
		}

		Debug.Log("Possbile stock card");

		foreach (var item in possibleStockCards)
		{
			Debug.Log(item.name);
		}
	}
}

public class HintManager : MonoBehaviour {

	[Serializable]
	public class HintMove {
		public Card card; // represents the playing card to be moved
		public Vector3 offset; // offset from the destination position
		public Transform destParent; // destination transform where the card will be moved to
		public bool parentCardsBelow; // determines whether the cards below the current card should also be moved
		public Transform customHintElement; // custom hint element for the move
	}

	public static HintManager instance; // singleton instance of the HintManager
	private HintPossibilities hintPossibilities; // holds the possibilities for hints
	private Queue<HintMove> hintMoves; // queue of moves to be performed as hints
	private Transform undoHolder; // reference to the undo holder transform
	private bool isInHintMode; // flag indicating whether the game is in hint mode
	private Card currentCard; // reference to the current card being moved
	private Stock stock; // reference to the stock object
	private Waste waste; // reference to the waste object
	private AnimationQueueController animationQueueController; // reference to the animation queue controller
	private HintDimScreen dimScreen; // reference to the dim screen object

	// Awake is called when the script instance is being loaded
	private void Awake() {
		instance = this; // set the singleton instance
		stock = FindObjectOfType<Stock>(); // get reference to the stock object
		waste = FindObjectOfType<Waste>(); // get reference to the waste object
		animationQueueController = FindObjectOfType<AnimationQueueController>(); // get reference to the animation queue controller
		hintMoves = new Queue<HintMove>(); // initialize the hint moves queue
		undoHolder = GameObject.FindGameObjectWithTag("UndoHolder").transform; // get reference to the undo holder transform
		dimScreen = FindObjectOfType<HintDimScreen>(); // get reference to the dim screen object
	}

	// Update is called once per frame
	private void Update() {
		// check if in hint mode and mouse button 0 is pressed
		if (isInHintMode && Input.GetMouseButtonDown(0)) { 
			isInHintMode = false; // set isInHintMode to false
		}
	}

	// TryHint method tries to perform a hint move
	public void TryHint() {
		animationQueueController.AddActionToQueue(OnHint);
	}
	private void OnHint() {
		BoardManager.instance.LockBoard();
		SearchForPossibilitiesOfMove(false);
		if (hintMoves.Count == 0)
		{
			if (stock.transform.childCount > 0)
			{
				//If there is no hints try to highlight stock if any card available
				hintMoves.Enqueue(new HintMove()
				{
					card = stock.transform.GetChild(0).GetComponent<Card>(),
					destParent = stock.transform,
					offset = Constants.vectorZero
				});
			}
			else if (waste.transform.childCount > 0)
			{
				//If there is no cards on stock but cards are on waste highlight stock too
				hintMoves.Enqueue(new HintMove()
				{
					customHintElement = stock.stockGraphics
				});
			}
		}

		if (hintMoves.Count > 0)
		{
			dimScreen.MakeScreenDark(0.8f);
			animationQueueController.SetAnimationStatus(AnimationStatus.inProgress);
			isInHintMode = true;
			DelayedHint();
		}
		else
		{
			Debug.Log("No hints!");
			NoHints();
		}
	}

	// This function searches for possible moves in the game of Solitaire
	public bool SearchForPossibilitiesOfMove(bool searchAllStockAndWaste) {
	    // Creates a new HintPossibilities object with the value of searchAllStockAndWaste
	    hintPossibilities = new HintPossibilities(searchAllStockAndWaste);
	    // Clears the hintMoves list
	    hintMoves.Clear();
	    // Searches for possible moves in the foundation using the possibleWasteCards
	    SearchForPossibleMovesInFoundation(hintPossibilities.possibleWasteCards);
	    // Searches for possible moves in the tableau using the possibleWasteCards
	    SearchForPossibleMovesInTableau(hintPossibilities.possibleWasteCards);
	    // If searchAllStockAndWaste is true
	    if (searchAllStockAndWaste)
	    {
	        // Searches for possible moves in the foundation using the possibleStockCards
	        SearchForPossibleMovesInFoundation(hintPossibilities.possibleStockCards);
	        // Searches for possible moves in the tableau using the possibleStockCards
	        SearchForPossibleMovesInTableau(hintPossibilities.possibleStockCards);
	    }
	    // Searches for possible moves in the foundation using the possibleOnTopCardsTableau
	    SearchForPossibleMovesInFoundation(hintPossibilities.possibleOnTopCardsTableau);
	    // Searches for possible moves in the tableau using the possibleCardsTableau
	    SearchForPossibleMovesInTableau(hintPossibilities.possibleCardsTableau);
	    // Returns true if the number of possible moves is greater than 0, false otherwise
	    return hintMoves.Count > 0;
	}

	private void SearchForPossibleMovesInFoundation(List<Card> possibleCardsToCheck) {
		for (int j = 0; j < possibleCardsToCheck.Count; j++)
		{
			Card cardToCheck = possibleCardsToCheck[j];
			if (cardToCheck != null)
			{
				//Dont Autofit while shake anim
				if (cardToCheck.transform.childCount > 0)
					continue;
				if (cardToCheck.cardValue == Enums.CardValue.Ace)
				{
					//If card is a ace search empty places in foundation
					for (int i = 0; i < hintPossibilities.possibleEmptyFoundation.Count; i++)
					{
						if (CardsHaveDifferentParents(cardToCheck.transform.parent, hintPossibilities.possibleEmptyFoundation[i]))
						{
							hintMoves.Enqueue(new HintMove()
							{
								card = cardToCheck,
								destParent = hintPossibilities.possibleEmptyFoundation[i].transform,
								offset = Constants.vectorZero
							});
						}
					}
				}
				else
				{
					for (int i = 0; i < hintPossibilities.possibleCardsFoundation.Count; i++)
					{
						Card fCard = hintPossibilities.possibleCardsFoundation[i];
						if (fCard != null && CardsAreInTheSameColor(cardToCheck, fCard) && CardHasNextValue(cardToCheck, fCard)
							&& CardsHaveDifferentParents(cardToCheck.transform.parent, fCard.transform.parent))
						{
							hintMoves.Enqueue(new HintMove()
							{
								card = cardToCheck,
								destParent = fCard.transform,
								offset = Constants.vectorZero
							});
						}
					}
				}
			}
		}
	}

	/*
	 * This method loops through the list of "possibleCardsToCheck" to search for possible moves in a foundation.
	 *  
	 * For each card in the list, it checks if the card is not null. If it's not, it continues with the next step. 
	 * If the card has a child transform, it skips this iteration using "continue".
	 * 
	 * If the card value is an Ace, it searches for empty places in the foundation. If it finds one, it creates a new 
	 * instance of the "HintMove" class, sets its properties "card", "destParent", and "offset", and enqueues it into the "hintMoves" queue.
	 * 
	 * If the card is not an Ace, it loops through another list of cards "possibleCardsFoundation". For each card in that list, it checks if the 
	 * color of the two cards match and if the value of the first card is one less than the second card. If both conditions are true, it creates a 
	 * new instance of the "HintMove" class, sets its properties "card", "destParent", and "offset", and enqueues it into the "hintMoves" queue.
	 */

	private void SearchForPossibleMovesInTableau(List<Card> possibleCardsToCheck) {
		for (int j = 0; j < possibleCardsToCheck.Count; j++)
		{
			Card cardToCheck = possibleCardsToCheck[j];
			if (cardToCheck != null)
			{
				if (cardToCheck.cardValue == Enums.CardValue.King)
				{
					//Check if it is valid hint
					if (cardToCheck.transform.GetSiblingIndex() == 0)
					{
						continue;
					}
					//If card is a king search empty places in tableau
					for (int i = 0; i < hintPossibilities.possibleEmptyTableau.Count; i++)
					{
						if (CardsHaveDifferentParents(cardToCheck.transform.parent, hintPossibilities.possibleEmptyTableau[i]))
						{
							hintMoves.Enqueue(new HintMove()
							{
								card = cardToCheck,
								destParent = hintPossibilities.possibleEmptyTableau[i].transform,
								offset = Constants.vectorZero,
								parentCardsBelow = true
							});
						}
					}
				}
				else
				{
					for (int i = 0; i < hintPossibilities.possibleOnTopCardsTableau.Count; i++) {
						Card tCard = hintPossibilities.possibleOnTopCardsTableau [i];

						if (tCard == null)
							continue;

						//Dont Autofit while shake anim
						if (tCard.transform.childCount > 0)
							continue;
		
						if (cardToCheck.isRed != tCard.isRed && CardHasNextValue (tCard, cardToCheck) && CardsHaveDifferentParents (cardToCheck.transform.parent, tCard.transform.parent)) {
							//Chech if it is valid no redundant hint
							if (tCard == cardToCheck.lastGoodParametres.lastCardAbove) {
								continue;
							}

							hintMoves.Enqueue (new HintMove () {
								card = cardToCheck,
								destParent = tCard.transform,
								offset = -Vector3.up * CalculateCardsOffset (tCard.size.y),
								parentCardsBelow = true
							});
						}
					}
				}
			}
		}
	}

	//This method checks if two cards are of the same color
	private bool CardsAreInTheSameColor(Card c1, Card c2) {
		//Return true if the card colors match, false otherwise
		return c1.cardColor == c2.cardColor;
	}

	//This method checks if the value of the next card is one greater than the previous card
	private bool CardHasNextValue(Card previousCard, Card nextCard) {
		//Cast the card values to integers and check if the previous card value is one less than the next card value
		return (int)previousCard.cardValue == (int)nextCard.cardValue + 1;
	}

	//This method checks if two transforms have different parents
	private bool CardsHaveDifferentParents(Transform p1, Transform p2) {
		//Return true if the transforms have different parents, false otherwise
		return p1 != p2;
	}

	//This method calculates the offset between two cards
	private float CalculateCardsOffset(float cardHeight) {
		//Initialize the offset with the portrait or landscape spacing value depending on screen orientation
		float offset = ScreenOrientationSwitcher.IsPortrait() ? Constants.TABLEAU_SPACING : Constants.LANDSCAPE_TABLEAU_SPACING;
		//Return the calculated offset multiplied by the scale factor
		return (cardHeight + offset) * BehaviourSettings.instance.GetScaleFactor();
	}

	//This method starts the hint
	private void RunHint() {
		//Create a new Timer with the move animation time and the DelayedHint method
		new Timer(Constants.MOVE_ANIM_TIME, DelayedHint);
	}

    // This method performs a delayed hint animation in a game.
    private void DelayedHint() {
		// Check if there are no moves in the hintMoves list or if the player is not in hint mode.
		if (hintMoves.Count == 0 || !isInHintMode) {
			// If either of the above conditions is true, call the FinishHintAnim method.
			FinishHintAnim();
		} else {
			// If there are moves in the hintMoves list and the player is in hint mode, get the next move.
			HintMove hm = hintMoves.Dequeue();
			if (hm.card != null) {
				// If the move is a card move, set the currentCard to the move's card.
				currentCard = hm.card;
				// Check if the card's parent cards should be below it.
				if (hm.parentCardsBelow) {
					currentCard.ParentCardsBelow();
				}
				// Set the return point for the currentCard to its original parent and offset.
				currentCard.SetReturnPoint(hm.card.transform.parent, hm.offset);
				// Set the currentCard's parent to the undoHolder.
				currentCard.transform.SetParent(undoHolder, true);
				// Play the pick card sound.
				SoundManager.instance.PlayPickCardSound();
				// Register a callback to run the RunHint method after the animation is finished.
				currentCard.RegisterOnAnimationFinishCB(RunHint);
				// Move the currentCard to its destination parent.
				currentCard.MoveCard(Constants.HINT_ANIM_TIME, hm.destParent);
			} else if (hm.customHintElement != null) {
				// If the move is a custom hint element move, get its previous parent.
				Transform prevParent = hm.customHintElement.parent;
				// Set the custom hint element's parent to the undoHolder.
				hm.customHintElement.transform.SetParent(undoHolder, true);
				// Play the pick card sound.
				SoundManager.instance.PlayPickCardSound();
				// Create a timer to return the custom hint element to its previous parent after the animation is finished.
				new Timer(Constants.HINT_ANIM_TIME, () => {
					hm.customHintElement.transform.SetParent(prevParent, true);
					hm.customHintElement.SetAsFirstSibling();
					RunHint();
				});
			}
		}
	}

	private void FinishHintAnim() {
		dimScreen.MakeScreenLight();
		BoardManager.instance.UnlockBoard();
		isInHintMode = false;
		animationQueueController.CastNextAnimation();
	}

	public void StopHintMode() {
		isInHintMode = false;
	}

	private void NoHints() {
		BoardManager.instance.UnlockBoard();
		animationQueueController.CastNextAnimation();
	}
}