using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class Stock : MonoBehaviour, IPointerClickHandler {
	// This is a C# script for managing the stock in a card game.
	public int stockLimit { get; set; }

	public bool firstCardSet { get; private set; }

	// Transform to store the graphics of the stock.
	public Transform stockGraphics { get; private set; }

	// Reference to the waste class, which keeps track of the waste cards.
	private Waste waste;

	// Image component for the stock.
	private Image stockImage;

	// Reference to the animation queue controller.
	private AnimationQueueController animationQueueController;

	// GameObject that represents the refresh image.
	private GameObject refreshImage;

	// Number of animations.
	private float noOfAnimations;

	// List to keep track of the waste order.
	private List<Transform> wasteOrderList;

	// Transform for the undo holder.
	private Transform undoHolder;

	// Boolean to determine if the stock should be sorted.
	private bool sortStock;

	// Private variable to keep track of the stock state.
	private bool _stockState;

	// Layout element component for the stock.
	private LayoutElement layoutElement;

	// Public property to access the stock state.
	public bool stockState {
		get
		{
			return _stockState;
		}
	}

	// Method to refresh the stock state.
	public void RefreshStockState(bool state, bool shouldRefreshImage = true) {
		// If the state has changed, update the stock state.
		if (state != _stockState)
		{
			_stockState = state;
			// If the stock state is true, show the stock.
			if (_stockState)
				ShowStock();
			// If the stock state is false, hide the stock.
			else
				HideStock();
		}

		// If the stock is empty, hide it and set the stock state to false.
		if (IsStockEmpty())
		{
			HideStock();
			_stockState = false;
		}
		// If the refresh image should be updated, refresh it.
		if (shouldRefreshImage)
		{
			RefreshStockRefreshImage();
		}
	}

	// Method to refresh the stock refresh image.
	public void RefreshStockRefreshImage() {
		// If the stock is not in the active state and not in a Vegas game with stock limit of 1, show the refresh image.
		refreshImage.SetActive(!_stockState && !(MatchStatistics.instance.IsVegasGame() && stockLimit <= 1));
		// If there are no cards in the stock and waste, hide the refresh image.
		if (transform.childCount == 0 && waste.transform.childCount == 0)
			refreshImage.SetActive(false);
	}
    private void Awake()
    {
		firstCardSet = false;
    }
    public void Start() {
		// Get reference to the waste class.
		waste = FindObjectOfType<Waste>();
		// Get reference to the stock image.
		stockImage = GetComponent<Image>();
		// Get reference to the animation queue controller.
		animationQueueController = FindObjectOfType<AnimationQueueController>();
		// Get reference to the stock graphics transform.
		stockGraphics = transform.parent.Find("Graphics");
		// Initialize the waste order list.
		wasteOrderList = new List<Transform>();
		// Get undo holder
		undoHolder = GameObject.FindGameObjectWithTag("UndoHolder").transform;
		// get refresh image
		refreshImage = GameObject.Find("ResetStock");
		ResetState();
    }

    public void dealFirstCard()
    {
        if (!firstCardSet)
		{
			GetCard();
			firstCardSet = true;
		}
    }

    private void ResetState() {
		// Refresh the stock state, set the flag to true to indicate that the refresh is done
		RefreshStockState(true);

		// If MatchStatistics.instance.isDraw3 is true, set the stock limit to 3, otherwise set it to 1
		stockLimit = MatchStatistics.instance.isDraw3 ? 3 : 1;
	}

	public void OnPointerClick(PointerEventData eventData) {
		// Add an action to the animation queue controller to call the GetCard method
    }

	public void GetCard() {
        // If MatchStatistics.instance.isDraw3 is true, set the number of cards to 3, otherwise set it to 1
        BoardManager.instance.LockBoard();
        int cards = MatchStatistics.instance.isDraw3 ? 3 : 1;

		// Register the waste object's refresh action and set the FinishReturnWasteAnimation method as the callback
		waste.RegisterOnRefreshWasteAction(FinishReturnWasteAnimation);
		// Set the number of animations to 1
		noOfAnimations = 1;

		// Save the current state

		// Loop through the number of cards to get
		for (int i = 0; i < cards; i++) {
			// If the stock has more than 0 cards
			if (transform.childCount > 0) {
				// If there is only 1 card left in the stock
				if (transform.childCount == 1) {
					// Refresh the stock state, set the flag to false to indicate that the refresh is not done yet
					RefreshStockState(false);
				}

				// Set the animation status to "in progress"
				animationQueueController.SetAnimationStatus(AnimationStatus.inProgress);

				// Play the pick card sound
				SoundManager.instance.PlayPickCardSound();

				// Get the top card of the stock
				Card card = transform.GetChild(0).GetComponent<Card>();

				// Set the card's parent to the undo holder
				card.transform.SetParent(undoHolder);

				// Set the return point of the card to the waste object's position and set the rotation to 0
				card.SetReturnPoint(waste.transform, Constants.vectorZero);

				// Register the callback for the reverse animation finish event and increment the number of animations
				card.RegisterOnReverseAnimationFinishCB(FinishReturnWasteAnimation);
				noOfAnimations++;

				// Rotate the card with the specified time
				card.RotateCardRight(Constants.STOCK_ANIM_TIME);

				// Register the callback for the animation finish event and increment the number of animations
				card.RegisterOnAnimationFinishCB(FinishReturnWasteAnimation);
				noOfAnimations++;

				// Move the card with the specified time
				card.MoveCard(Constants.STOCK_ANIM_TIME * 2);
			}
			else
			{
				// If first card return stock
				if (i == 0) ReturnCardsFromWasteToStock();
				break;
			}
		}
		BoardManager.instance.UnlockBoard();
	}

	// ReturnCardsFromWasteToStock method transfers cards from the waste to the stock, 
	// decreasing the stockLimit and MatchStatistics moves.
	private void ReturnCardsFromWasteToStock() {
		// Decrease the stock limit and match moves
		stockLimit--;
		MatchStatistics.instance.moves--;

		// If the game is a Vegas game and the stock limit is below 0, set the limit to 0, refresh stock state, 
		// and remove the last save.
		if (MatchStatistics.instance.IsVegasGame() && stockLimit <= 0) {
			stockLimit = 0;
			RefreshStockState(false);
			SaveManager.instance.RemoveLastSave();
			animationQueueController.CastNextAnimation();
		} 
		// If the game is not a Vegas game, transfer cards from the waste to the stock
		else {
			// Get the number of children in the waste
			int childCount = waste.transform.childCount;

			// Clear the wasteOrderList, and check if there are any children
			wasteOrderList.Clear();
			if (childCount > 0) {
				// Play pick card sound and set the number of animations
				SoundManager.instance.PlayPickCardSound();
				noOfAnimations = childCount;
				animationQueueController.SetAnimationStatus(AnimationStatus.inProgress);
				sortStock = true;
			}

			// Iterate through each child of the waste and transfer it to the stock
			for (int i = 0; i < childCount; i++) {
				// Get the child and its Card component
				Transform child = waste.transform.GetChild(0);
				wasteOrderList.Add(child);
				Card card = child.GetComponent<Card>();

				// Set the child's parent, animation end point, and rotate and move the card
				card.transform.SetParent(transform.parent);
				card.SetReturnPoint(transform, Constants.vectorZero);
				card.RegisterOnReverseAnimationFinishCB(FinishReturnWasteAnimation);
				noOfAnimations++;
				card.RotateCard(Constants.STOCK_ANIM_TIME);
				card.RegisterOnAnimationFinishCB(FinishReturnWasteAnimation);
				card.MoveCard(Constants.STOCK_ANIM_TIME * 2);
			}

			// If there are no children in the waste, cast the next animation, refresh stock state, and remove the last save.
			if (childCount == 0) {
				animationQueueController.CastNextAnimation();
				RefreshStockState(false);
				SaveManager.instance.RemoveLastSave();
			}
		}
	}

	// The private void FinishReturnWasteAnimation method is used to decrement the number of animations and refresh the stock state once the number of animations reach zero.
	private void FinishReturnWasteAnimation() 
	{
	    // Decrement the number of animations
	    noOfAnimations--;
	    
	    // Check if the number of animations is less than or equal to zero
	    if (noOfAnimations <= 0)
	    {
	        // Call the RefreshStockState method and pass in false as an argument
	        RefreshStockState(false);

	        // Check if the sortStock flag is set
	        if (sortStock)
	        {
	            // Call the RestoreGoodStockOrder method
	            RestoreGoodStockOrder();
	        }
	        // Call the CastNextAnimation method on the animationQueueController object
	        animationQueueController.CastNextAnimation();
	    }
	}

	// The private void RestoreGoodStockOrder method is used to restore the good stock order.
	private void RestoreGoodStockOrder() 
	{
	    // Reset the sortStock flag
	    sortStock = false;
	    
	    // Loop through the wasteOrderList
	    for (int i = 0; i < wasteOrderList.Count; i++)
	    {
	        // Call the SetAsLastSibling method on the current item in the wasteOrderList
	        wasteOrderList[i].SetAsLastSibling();
	        
	        // Call the ResetStockCardsPos method
	        ResetStockCardsPos();
	    }
	}

	// The public void ResetStockCardsPos method is used to reset the position of all the child elements of the transform.
	public void ResetStockCardsPos() 
	{
	    // Loop through the children of the transform
	    for (int i = 0; i < transform.childCount; i++)
	    {
	        // Reset the local position of the current child to Constants.vectorZero
	        transform.GetChild(i).localPosition = Constants.vectorZero;
	    }
	}

	// The private void ShowStock method is used to show the stock image by setting its color to white.
	private void ShowStock() 
	{
	    // Set the color of the stockImage object to white
	    stockImage.color = new Color(1, 1, 1, 1);
	}

	// The private void HideStock method is used to hide the stock image by setting its color to transparent.
	private void HideStock() 
	{
	    // Set the color of the stockImage object to transparent
	    stockImage.color = new Color(1, 1, 1, 0);
	}

	// The private bool IsStockEmpty method is used to check if the stock is empty by checking the child count of the transform.
	private bool IsStockEmpty() 
	{
	    // Return true if the child count of the transform is zero, false otherwise
	    return transform.childCount == 0;
	}

	// The public void CheckStockState method is used to check the state of the stock and refresh it accordingly.
	public void CheckStockState() 
	{
	    // Check if the stock is empty
	    if (IsStockEmpty())
	    {
	        // Call the RefreshStockState method and pass in false as an argument
	        RefreshStockState(false);
	    }
	    else
	    {
	        // Call the RefreshStockState method and pass in false as an argument
	        RefreshStockState(false);
	    }
	}

	// The public void RefreshStockSize method is used to refresh stock size
	public void RefreshStockSize() {
		if (layoutElement == null)
			layoutElement = GetComponent<LayoutElement>();

		(transform as RectTransform).sizeDelta = new Vector2(layoutElement.minWidth, layoutElement.minHeight);
		transform.localPosition = Constants.vectorZero;
	}
}