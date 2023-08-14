using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

// Autocomplete class is a MonoBehaviour script for the autocomplete feature in the game.
public class Autocomplete : MonoBehaviour 
{
    // The static instance of the Autocomplete class
    public static Autocomplete instance;
    
    // The New Game and Restart Game buttons
    private Button newGameBtn;
    private Button restartGameBtn;

    // The Stock class instance for the autocomplete functionality
    private Stock stock;

    // The CanvasGroup component for controlling the visibility of the autocomplete menu
    private CanvasGroup cg;

    // A flag to track if the autocomplete feature can be shown again
    private bool canShowAgain = true;

    // Awake is called when the script instance is being loaded.
    private void Awake() 
    {
        // Set the static instance to this object
        instance = this;
        
        // Get the CanvasGroup component
        cg = GetComponent<CanvasGroup>();

        // Set the time scale to 1
        Time.timeScale = 1f;

        // Get the Stock class instance
        stock = FindObjectOfType<Stock>();

        // Set up the New Game and Restart Game buttons
        SetupGameRestartBtns();

        // Hide the autocomplete menu
        ChangeVisibility(false);
    }

    // Show the autocomplete menu
    public void OnShow() 
    {
        ChangeVisibility(true);
    }

    // Hide the autocomplete menu
    public void OnHide() 
    {
        ChangeVisibility(false);
    }

    // Change the visibility of the autocomplete menu
    private void ChangeVisibility(bool state) 
    {
        cg.interactable = state;
        cg.blocksRaycasts = state;
        cg.alpha = state ? 1 : 0;
    }

    // Set up the New Game and Restart Game buttons
    private void SetupGameRestartBtns() 
    {
        try
        {
            // Get the New Game and Restart Game buttons
            restartGameBtn = GameObject.Find("ReplayBtn").GetComponent<Button>();
        }
        catch (Exception e)
        {
            // Log an error if the buttons are not found
            Debug.LogError("Empty object " + e.Message);
        }
    }

    // Set the state of the New Game and Restart Game buttons
    public void SetButtonState(bool state) 
    {
        if (state && canShowAgain)
        {
            OnShow();
            canShowAgain = false;
        }
    }

    // Start the autocomplete process
    public void StartAutocomplete() 
    {
        OnHide();
        BoardManager.instance.LockBoard();
        StartCoroutine(AutocompleteCoroutine());
    }

    // Set the interactable state of the New Game and Restart Game buttons
	private void SetInteractableOfResetGameBtns(bool state) {
		if (restartGameBtn != null && newGameBtn != null)
		{
			restartGameBtn.interactable = state;
			newGameBtn.interactable = state;
		}
	}

	private IEnumerator AutocompleteCoroutine() {
        // Set the time scale to the constant value for autocomplete
        Time.timeScale = Constants.AUTOCOMPLETE_TIME_SCALE;

        // Disable the reset game buttons
        SetInteractableOfResetGameBtns(false);
        
        // Keep looping until the game is won
        do
        {
            // Draw a card from the stock
            stock.GetCard();

            // Wait for the specified animation time
            yield return new WaitForSeconds(Constants.ANIM_TIME);
            
            // Get the top cards of the tableau piles
            List<Card> tableauCards = BoardManager.instance.GetOnTopTableauCards();

            // Try to fit each card on the foundation piles
            for (int i = 0; i < tableauCards.Count; i++)
            {
                yield return TryFitCard(tableauCards[i]);
            }

            // Get the top waste card
            Card wasteCard = BoardManager.instance.GetCurrentWasteCards(false).FirstOrDefault();

            // Try to fit the waste card on the foundation piles
            yield return TryFitCard(wasteCard);

            // Check if the game is complete
            BoardManager.instance.CheckIfGameComplete();
        } while (!BoardManager.instance.isGameWon);

        // Set the time scale back to 1
        Time.timeScale = 1f;

        // Enable the reset game buttons
        SetInteractableOfResetGameBtns(true);

        // Unlock the board
        BoardManager.instance.UnlockBoard();
    }

	private IEnumerator TryFitCard(Card card) {
        // If the card is not null
        if (card != null)
        {
            // Try to fit the card on the foundation piles
            if (BoardManager.instance.TryAutoFitCard(card))
                // Wait for the specified animation time
                yield return new WaitForSeconds(Constants.ANIM_TIME);
        }
    }

	private void ResetState() {
        // Set the canShowAgain flag to true
        canShowAgain = true;
    }
}