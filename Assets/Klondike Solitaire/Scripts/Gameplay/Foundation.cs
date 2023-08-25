using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine.UI;

// Class "Foundation" derived from "DropZone" class
public class Foundation : DropZone, IPointerClickHandler
{

    public int indipoints { get; private set; }
    private int prev_ccount;
    public int ace_count { get; private set; }
    public bool isStreak { get; private set; }

    private CanvasGroup hider;

    private TextLine tline;

    private ExtraInfo ei;

    // Event handler for "OnDrop" method when a card is dropped on the foundation stack

    protected override void Awake()
    {
        indipoints = 0;
        ace_count = 0;
        prev_ccount = 0;
        isStreak = false;
        hider = transform.parent.parent.Find("Hider").GetComponent<CanvasGroup>();
        hideHider();
        if (!hider)
        {
            Debug.Log("Hider not found.");
        }
        Debug.Log(GetComponentInParent<GridLayoutGroup>().cellSize);
        base.Awake();
    }

    protected override void Start()
    {
        tline = FindObjectOfType<TextLine>();
        ei = FindObjectOfType<ExtraInfo>();
        base.Start();
    }

    private int FoundNum()
    {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i) == transform)
            {
                return i;
            }
        }
        return -1;
    }
    public override void OnDrop(PointerEventData eventData)
    {
        //Get the "Card" component from the dropped object
        Card droppedCard = eventData.pointerDrag.GetComponent<Card>();
        
        //Deactivate any extra zones
        transform.root.BroadcastMessage("DeactivateExtraZone", SendMessageOptions.DontRequireReceiver);

        //Call the parent class' "OnDrop" method
        OnDrop(droppedCard);
    }
    // Overridden "OnDrop" method when a card is dropped

    public static bool isBJ(Card c)
    {
        return ((c.cardColor == Enums.CardColor.spades || c.cardColor == Enums.CardColor.clubs) && c.cardValue == Enums.CardValue.Jack);
    }
    public override void OnDrop(Card droppedCard)
    {
        if (droppedCard != null && droppedCard.GetComponent<Card>() != null && droppedCard.canDrag)
        {
            if (droppedCard.transform.childCount == 0)
            {
                // Calculate the score for putting the card in the foundation stack
                int points = CalculatePoints(droppedCard);

                // Add the score to the match statistics

                MatchStatistics.instance.AddScore(points);

                // Add the Vegas score to the match statistics
                MatchStatistics.instance.vegasScore += Constants.VEGAS_SCORE_PER_CARD;

                // Increment the number of moves in the match statistics
                MatchStatistics.instance.moves--;

                // Call the parent class' "OnDrop" method
                if (isBJ(droppedCard))
                {
                    indipoints = 21;
                }
                else if ((int)droppedCard.cardValue > 10)
                {
                    indipoints += 10;
                }
                else
                {
                    indipoints += (int)droppedCard.cardValue;
                }
                if ((int)droppedCard.cardValue == 1)
                {
                    ace_count++;
                }
                base.OnDrop(droppedCard);
            }
            // If the dropped card can be put in the foundation stack and it has no child objects
        }
        else
        {
            /*
            if (droppedCard == null)
            {
                Debug.LogError("Null card drop");
            }
            else if (droppedCard.GetComponent<Card>() == null)
            {
                Debug.LogError("There is no Card component in dropped object.");
            }
            else if (!droppedCard.canDrag)
            {
                Debug.LogError("Card cannot be dragged.");
            }
            else
            {
                Debug.LogError("Unknown Bug.");
            }*/
            // Log an error message if there is no "Card" component in the dropped object
            
        }
    }
    private int checkChildren()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                i++;
            }
        }
        return i;
    }
    public void pointCheck()
    {
        if (isStreak)
        {
            isStreak = false;
        }
        if (prev_ccount < transform.childCount)
        {
            if (checkChildren() == 5)
            {
                MatchStatistics.instance.score += 600;
            }
            BoardManager.instance.LockBoard();
            if (indipoints >= 21 || (indipoints == 11 && ace_count > 0))
            {
                if (indipoints == 21 || (indipoints == 11 && ace_count > 0))
                {
                    SoundManager.instance.PlayFBC();
                    SoundManager.instance.PlayTD();
                    MatchStatistics.instance.AddScore(Constants.STACK_CLEAR_POINTS);
                    MatchStatistics.instance.AddScore(Constants.STREAK_POINTS*MatchStatistics.instance.streak);
                    MatchStatistics.instance.max_streak_points += Constants.STREAK_POINTS * MatchStatistics.instance.streak;
                    MatchStatistics.instance.streak++;
                    tline.Flash21(FoundNum());
                    if (MatchStatistics.instance.streak > 1)
                    {
                        int fnum = FoundNum();
                        tline.showStreak(fnum);
                    }
                    if (checkChildren() >= 5)
                    {
                        MatchStatistics.instance.num_combos++;
                    }
                }
                else
                {
                    MatchStatistics.instance.failures++;
                    MatchStatistics.instance.streak = 0;
                }
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
                indipoints = 0;
                ace_count = 0;
            }
            else
            {
                MatchStatistics.instance.streak = 0;
            }
            BoardManager.instance.UnlockBoard();
        }
        prev_ccount = transform.childCount;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!BoardManager.instance.isDragging)
            {
                tline.hideText();
                ei.HideText();
                showHider();
                if (waste.transform.childCount > 0 && waste.transform.GetChild(0).GetComponent<Card>() != null)
                {
                    Card child = waste.transform.GetChild(0).GetComponent<Card>();
                    if (child != null)
                    {
                        child.OnBeginDrag(null);
                        if (child.canDrag)
                        {
                            OnDrop(child);
                            child.UnlockCard(false);
                        }
                        child.OnEndDrag(null);
                    }
                }
                Invoke("hideHider", (float)0.5);
            }
        }
    }
    public void RecalculatePoints()
    {
        indipoints = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            indipoints += (int)transform.GetChild(i).GetComponent<Card>().cardValue;
        }
    }
    private void showHider()
    {
        hider.blocksRaycasts = true;
    }
    private void hideHider()
    {
        hider.blocksRaycasts = false;
    }
    // Assign a new child to the foundation stack
    public override void AssignNewChild(Transform child)
    {
        // Call the parent class' "AssignNewChild" method
        base.AssignNewChild(child);
    }

    // Calculate the score for putting the card in the foundation stack
    private int CalculatePoints(Card droppedCard)
    {
        // If the card came from the waste stack
        if (droppedCard.previousParent.GetComponent<Waste>() != null)
        {
            // Return the score for putting a card from the waste stack to the foundation stack
            return Constants.WASTE2FOUNDATIONS_POINTS;
        }
        // If the card came from a tableau stack
        else if (droppedCard.previousParent.GetComponent<Tableau>() != null)
        {
            return Constants.TABLEAU2FOUNDATIONS_POINTS;
        }
        return 0;
    }

    // This is the "TryPutDroppedCardOnEmptyStack" method.

    // This is the "TryPutDroppedCardOnLastCard" method.
    private int pointcount(Card droppedCard)
    {
        // It gets the last card in the tableau and compares the dropped card's color and value to it.
        Card lastCardInTableau = transform.GetChild(transform.childCount - 1).GetComponent<Card>();
        int points = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            points += (int)transform.GetChild(i).GetComponent<Card>().cardValue;
        }
        points += (int)droppedCard.cardValue;
        if (points > 21)
        {
            return 0;
        }
        if (points == 21)
        {
            return 2;
        }
        return 1;
        // If the color is the same and the value of the dropped card is one greater, it returns true.
        // Otherwise, it returns false.
		/*if (droppedCard.cardColor == lastCardInTableau.cardColor && (int)droppedCard.cardValue == (int)lastCardInTableau.cardValue + 1)
            return true;
        return false;*/
    }

    // This is the "CheckIsFoundationComplete" method.
    public bool CheckIsFoundationComplete()
    {
        // It checks if the number of child objects in the transform is equal to the full number of color cards.
        // If it is, it returns true, otherwise it returns false.
        int points = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            points += (int)transform.GetChild(i).GetComponent<Card>().cardValue;
        }
        if (points == 21)
        {
            return true;
        }
        return false;
    }
}