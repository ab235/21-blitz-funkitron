using UnityEngine;
using System.Collections.Generic;
using System;

// AutofitCard is a class that performs the auto-fitting logic for cards in a card game.
public class AutofitCard  {
    // The list of cards in the foundation that can possibly be auto-fitted
    public List<Card> possibleCardsFoundation { get; private set; }
    // The list of cards in the tableau that can possibly be auto-fitted on top of them
    public List<Card> possibleOnTopCardsTableau { get; private set; }
    // The list of empty tableau columns where cards can be auto-fitted
    private List<Transform> possibleEmptyTableau;
    // The list of empty foundation columns where cards can be auto-fitted
    private List<Transform> possibleEmptyFoundation;

    // Gets the list of possible cards and empty columns in the foundation and tableau.
    void GetPossibleCards()
    {
        // Gets the on-top cards and empty tableau columns using the BoardManager instance.
        possibleOnTopCardsTableau = BoardManager.instance.GetOnTopTableauCards(out possibleEmptyTableau);

        // Gets the cards and empty foundation columns using the BoardManager instance.
        possibleCardsFoundation = BoardManager.instance.GetFoundationCards(out possibleEmptyFoundation);
    }

    // Tries to auto-fit the given card in the foundation or tableau.
    public bool TryAutoFitCard(Card card)
    {
        GetPossibleCards();

        // Try to auto-fit the card in the foundation
        if(SearchInFoundation(card))
        {
            return true;
        }
        // Try to auto-fit the card in the tableau
        else if(SearchInTableau(card))
        {
            return true;
        }
        
        // Return false if auto-fit was not successful in either the foundation or tableau.
        return false;
    }

    private bool SearchInFoundation(Card card)
    {
        // Check if the card is the last card in the tableau
        if (card.transform.GetSiblingIndex() != card.transform.parent.childCount - 1)
            return false;

        // Check if the card is an Ace
        if (card.cardValue == Enums.CardValue.Ace)
        {
            // Check if there is an empty foundation
            if (possibleEmptyFoundation.Count > 0)
            {
                // Start the animation to move the card to the empty foundation
                StartCardAutoFitAnimation(card, possibleEmptyFoundation[0]);
                return true;
            }
        }
        else
        {
            // Don't perform auto-fit if there is a shake animation
            if (card.transform.childCount > 0)
                return false;

            // Loop through all possible cards in the foundation
            for (int i = 0; i < possibleCardsFoundation.Count; i++)
            {
                Card pCard = possibleCardsFoundation[i];

                // Check if the current card in the foundation is the same color and value + 1 of the card being moved
                if (card.cardColor == pCard.cardColor && (int)(card.cardValue) == (int)(pCard.cardValue + 1))
                {
                    // Start the animation to move the card to the foundation
                    StartCardAutoFitAnimation(card, pCard.transform.parent);
                    return true;
                }
            }
        }
        return false;
    }

    private bool SearchInTableau(Card card)
    {
        // Check if the card is a King
        if (card.cardValue == Enums.CardValue.King)
        {
            // Check if it is a valid hint and it's the first sibling in its parent transform
            if (IsCardTableauParented(card) && card.transform.GetSiblingIndex() == 0)
            {
                return false;
            }

            // If the card is a King, search for empty places in the tableau
            for (int i = 0; i < possibleEmptyTableau.Count; i++)
            {
                // If the current possible empty tableau is not the parent of the card
                if (card.transform.parent != possibleEmptyTableau[i])
                {
                    StartCardAutoFitAnimation(card, possibleEmptyTableau[i]);
                    return true;
                }
            }
        }
        else
        {
            // Loop through the possible on top cards in the tableau
            for (int i = 0; i < possibleOnTopCardsTableau.Count; i++)
            {
                Card pCard = possibleOnTopCardsTableau[i];
                // Don't perform the autofit if the current possible on top card is null or has children (while shake anim)
                if (pCard == null || pCard.transform.childCount > 0)
                    continue;
                // Check if the current card and possible on top card are of different colors and the value of the current card is one less than the value of the possible on top card and their parent transforms are different
                if (card.isRed != pCard.isRed && (int)(card.cardValue + 1) == (int)pCard.cardValue && card.transform.parent != pCard.transform.parent)
                {
                    StartCardAutoFitAnimation(card, pCard.transform.parent);
                    return true;
                }
            }
        }
        return false;
    }

    // Check if the parent of the card is a Tableau object
    private bool IsCardTableauParented(Card card)
    {
        return card.transform.parent.GetComponent<Tableau>() != null;
    }

    private void StartCardAutoFitAnimation(Card card, Transform zoneParent)
    {
        // Start dragging the card
        card.OnBeginDrag(null);

        // Check if the parent is a Tableau object
        if (zoneParent.GetComponent<Tableau>() != null)
        {
            // Call the OnDrop function of the Tableau object
            zoneParent.GetComponent<Tableau>().OnDrop(card);
            // Start the AutoFit animation of the card
            card.StartAutoFit();
        }
        else if (zoneParent.GetComponent<Foundation>() != null)
        {
            // Call the OnDrop function of the Foundation object
            zoneParent.GetComponent<Foundation>().OnDrop(card);
            // Start the AutoFit animation of the card
            card.StartAutoFit();
        }
        else
        {
            // End dragging the card
            card.OnEndDrag(null);
        }
    }
}
