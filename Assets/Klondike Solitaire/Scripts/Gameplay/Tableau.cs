using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

// This script defines a class Tableau which extends the DropZone class. 
// The script contains several methods that handle the logic of dropping a Card object onto the Tableau game object.
public class Tableau : DropZone {

    // The OnDrop method is called when a Card object is dropped onto the Tableau
    public override void OnDrop(PointerEventData eventData)
    {
        Card droppedCard = eventData.pointerDrag.GetComponent<Card>();
        transform.root.BroadcastMessage("DeactivateExtraZone", SendMessageOptions.DontRequireReceiver);
        OnDrop(droppedCard);
    }

    public override void OnDrop(Card droppedCard)
    {
        if (droppedCard != null)
        {
            bool decision = false;
            if (transform.childCount > 0)
            {
                decision = TryPutDroppedCardOnLastCard(droppedCard);
            }
            else
            {
                decision = TryPutDroppedCardOnEmptyStack(droppedCard);
            }

            if (decision)
            {
                int points = CalculatePoints(droppedCard);
                MatchStatistics.instance.AddScore(points);
                MatchStatistics.instance.moves++;
                base.OnDrop(droppedCard);
            }
        }
        else
        {
            Debug.LogError("There is no Card component in dropped object");
        }
    }

    // CalculatePoints method is called to calculate the points earned for the move, which are then added to the player's score.
    private int CalculatePoints(Card droppedCard)
    {
        if (droppedCard.previousParent == null)
        {
            Debug.LogError("CalculatePoints - Previous parent is empty " + transform.name + " : " + droppedCard.name);
            return 0;
        }
        if (droppedCard.previousParent.GetComponent<Waste>() != null)
        {
            return Constants.WASTE2TABLEAU_POINTS;
        }
        else if (droppedCard.previousParent.GetComponent<Foundation>() != null)
        {
            MatchStatistics.instance.vegasScore -= Constants.VEGAS_SCORE_PER_CARD;
            return Constants.FOUNDATION2TABLEAU_POINTS;
        }
        return 0;
    }

    // The TryPutDroppedCardOnEmptyStack method checks if the dropped Card is a king and returns a boolean indicating whether the 
    // Card can be placed on the empty Tableau. 
    private bool TryPutDroppedCardOnEmptyStack(Card droppedCard)
    {
        if (droppedCard.cardValue == Enums.CardValue.King)
            return true;
        else
            return false;
    }

    // The TryPutDroppedCardOnLastCard method checks if the last Card on the Tableau is of a different color 
    // and of the previous value to the dropped Card and returns a boolean indicating whether the Card can be placed on the Tableau.
    private bool TryPutDroppedCardOnLastCard(Card droppedCard)
    {
        Card lastCardInTableau = transform.GetChild(transform.childCount - 1).GetComponent<Card>();
        if (droppedCard.isRed != lastCardInTableau.isRed && (int)droppedCard.cardValue == (int)lastCardInTableau.cardValue - 1)
            return true;
        return false;
    }

    // The CheckIfTableauHasAllCardExposed method checks if all the Cards in the Tableau are exposed. 
    // If all cards are exposed, the method returns true, otherwise it returns false.
    public bool CheckIfTableauHasAllCardExposed()
    {
        Card[] cards = GetComponentsInChildren<Card>();
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].isReversed)
                return false;
        }
        return true;
    }
}
