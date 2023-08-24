using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using JetBrains.Annotations;

public class SaveManager : MonoBehaviour {
	public static SaveManager instance { get; private set; }

	private List<SaveState> saveList = new List<SaveState>();

	private Card[] gameCardsSet;
	private Stock stock;
	private Waste waste;
	private Transform undoHolder;
	private Dictionary<Card, CardInfo> cardsToAnimate = new Dictionary<Card, CardInfo>();
	private List<Card> cardsToRotate = new List<Card>();
	private List<CardInfo> filteredCardList = new List<CardInfo>();
	private AnimationQueueController animationQueueController;
	private float noOfAnimations;
	private bool allMovesSet;
	private bool refreshWaste;

	// The Awake method initializes the instance of the SaveManager class and sets various fields, such as the animationQueueController, stock, and waste.
	private void Awake() {
		instance = this;
		animationQueueController = FindObjectOfType<AnimationQueueController>();
		stock = FindObjectOfType<Stock>();
		waste = FindObjectOfType<Waste>();
		undoHolder = GameObject.FindGameObjectWithTag("UndoHolder").transform;
	}

	/// <summary>
	/// Save current game state
	/// The Save method adds a new SaveState instance to the saveList to save the current game state. 
	/// The SaveState object contains information about the game's moves, score, vegas score, and card information.
	/// </summary>
	public void Save() {
		SaveState newSave = new SaveState();
		newSave.moves = MatchStatistics.instance.moves;
		newSave.score = MatchStatistics.instance.score;
		newSave.vegasScore = MatchStatistics.instance.vegasScore;
		newSave.stockState = stock.stockState;
		newSave.stockLimit = stock.stockLimit;

		for (int i = 0; i < gameCardsSet.Length; i++)
		{
			Card myCard = gameCardsSet[i];
			CardInfo myCardInfo = new CardInfo(myCard.lastGoodParametres, myCard);
			newSave.cardsInfo.Add(myCard, myCardInfo);
		}

		saveList.Add(newSave);
	}

	/// <summary>
	/// The TryUndo method adds an undo action to the animationQueueController's animation queue. 
	/// </summary>
	public void TryUndo() {
		print("hi");
		animationQueueController.AddActionToQueue(Undo);
	}

	/// <summary>
	/// The Undo method retrieves the last saved game state and updates the game to that state.
	/// </summary>
	private void Undo() {
		noOfAnimations = 0;
		waste.RegisterOnRefreshWasteAction(null);
		allMovesSet = false;

		animationQueueController.SetAnimationStatus(AnimationStatus.inProgress);
		if (saveList.Count > 0)
		{
			cardsToAnimate.Clear();
			cardsToRotate.Clear();
			SaveState lastSave = saveList[saveList.Count - 1];
			MatchStatistics.instance.score = lastSave.score;
			MatchStatistics.instance.moves = lastSave.moves;
			MatchStatistics.instance.vegasScore = lastSave.vegasScore;
			stock.stockLimit = lastSave.stockLimit;
			for (int i = 0; i < saveList.Count; i++)
			{
				print(saveList[i]);
			}
			for (int i = 0; i < lastSave.cardsInfo.Count; i++)
			{
				Card myCard = gameCardsSet[i];
				CardInfo myCardInfo = lastSave.cardsInfo[myCard];
				myCard.gameObject.SetActive(true);
				CheckIfCardNeedRotateAnim(myCard, myCardInfo);

				//trasfrom local position to world pos
				Vector3 prevPos = myCardInfo.GetParent().TransformPoint(myCardInfo.GetPos());
				//if card change position play anim
				if (myCard.transform.parent != myCardInfo.GetParent())
				{
					if (cardsToAnimate.ContainsKey(myCard))
					{
						Debug.LogError("Dictionary has already key " + myCard.name);
					}
					else
					{
						myCard.lastGoodParametres.lastCardAbove = myCardInfo.GetCardAbove();
						myCard.SetReturnPoint(myCardInfo.GetParent(), prevPos - myCardInfo.GetParent().position);
						cardsToAnimate.Add(myCard, myCardInfo);
					}
				}
				else
				{
					myCard.transform.SetParent(myCardInfo.GetParent(), true);
					myCard.transform.position = new Vector3(myCard.transform.position.x, prevPos.y);
				}

				myCard.transform.parent.SendMessage("RefreshZone", SendMessageOptions.DontRequireReceiver);
			}

			saveList.Remove(lastSave);
			AnimCards();
			stock.RefreshStockState(lastSave.stockState, false);
		}
		else
		{
			Debug.Log("No moves to undo");
			animationQueueController.CastNextAnimation();
		}
		allMovesSet = true;
		BoardManager.RecalculatePoints();
	}

	private void AnimCards() {
		filteredCardList = FilterCardsToAnimate();
		noOfAnimations += filteredCardList.Count;

		refreshWaste = false;
		for (int i = 0; i < filteredCardList.Count; i++)
		{
			Card card = filteredCardList[i].GetCard();
			card.RegisterOnAnimationFinishCB(FinishUndoAnimation);
			float animTime = Constants.MOVE_ANIM_TIME;
			if (filteredCardList[i] == null)
			{
				print("xd");
			}
			if (CheckIfPreviousCardParentIsStock(filteredCardList[i]) || CheckIfCurrentParentIsStock(filteredCardList[i]))
			{
				refreshWaste = true;
				animTime = Constants.STOCK_ANIM_TIME * 2;
			}
			else
			{
				card.ParentCardsBelow();
			}

			SoundManager.instance.PlayPickCardSound();
			card.transform.SetParent(undoHolder, true);
			card.MoveCard(animTime);
		}

		for (int i = 0; i < cardsToRotate.Count; i++)
		{
			RotateCard(cardsToRotate[i]);
		}

		if (noOfAnimations == 0)
		{
			FinishUndoAnimation();
		}
	}

	private void RotateCard(Card myCard) {
		SoundManager.instance.PlayReverseCardSound();
		myCard.RegisterOnReverseAnimationFinishCB(FinishUndoAnimation);

		noOfAnimations++;
		myCard.RotateCard(Constants.STOCK_ANIM_TIME);
	}

	private void FinishUndoAnimation() {
		noOfAnimations--;

		if (noOfAnimations <= 0 && allMovesSet)
		{
			if (refreshWaste)
			{
				waste.RegisterOnRefreshWasteAction(CastNextAnimation);
				waste.RefreshChildren();
				stock.ResetStockCardsPos();
			}
			else
			{
				CastNextAnimation();
			}
		}
	}

	private void CastNextAnimation() {
		noOfAnimations = 0;
		RememberCardParameters();
		stock.CheckStockState();
		animationQueueController.CastNextAnimation();
	}

	private List<CardInfo> FilterCardsToAnimate() {
		List<CardInfo> cards = new List<CardInfo>();
		List<CardInfo> stockCards = new List<CardInfo>();
		Dictionary<Transform, CardInfo> theBestCardInTransform = new Dictionary<Transform, CardInfo>();

		foreach (CardInfo cardInfo in cardsToAnimate.Values)
		{
			if (cardInfo.GetParent().name == "Waste")
			{
				cards.Add(cardInfo);
			}
			else if (cardInfo.GetParent().name == "Stock") //Check Waste or Stock card to animate separate without children
			{
				stockCards.Add(cardInfo);
			}
			else if (theBestCardInTransform.ContainsKey(cardInfo.GetParent())) //Create ditionary with top card in order in each pile
			{
				if (cardInfo.GetChildOrder() < theBestCardInTransform[cardInfo.GetParent()].GetChildOrder())
				{
					theBestCardInTransform[cardInfo.GetParent()] = cardInfo;
				}
			}
			else
			{
				theBestCardInTransform.Add(cardInfo.GetParent(), cardInfo);
			}
		}
		stockCards.Reverse();
		cards.AddRange(stockCards);
		cards.AddRange(theBestCardInTransform.Values);
		return cards;
	}

	private void CheckIfCardNeedRotateAnim(Card myCard, CardInfo myCardInfo) {
		myCard.RegisterOnReverseAnimationFinishCB(null);
		if ((CheckIfPreviousCardParentIsStock(myCardInfo) && myCard.isReversed == false)
			|| (myCard.isReversed != myCardInfo.GetReversed()))
		{
			cardsToRotate.Add(myCard);
		}
	}

	private bool CheckIfPreviousCardParentIsStock(CardInfo myCardInfo) {

		Transform t = myCardInfo.GetParent();
		if (t != null)
		{
			Stock stock = t.GetComponent<Stock>();
			return (stock != null);
		}
		return true;
	}

	private bool CheckIfCurrentParentIsStock(CardInfo myCardInfo) {
		Stock stock = myCardInfo.GetCard().transform.parent.GetComponent<Stock>();
		return (stock != null) ? true : false;
	}

	/// <summary>
	/// Create array of current game cards
	/// </summary>
	/// <param name="cardsGO"></param>
	public void SetGameCardsSet(List<GameObject> cardsGO) {
		gameCardsSet = new Card[cardsGO.Count];
		for (int i = 0; i < cardsGO.Count; i++)
		{
			gameCardsSet[i] = cardsGO[i].GetComponent<Card>();
		}
	}

	/// <summary>
	/// Wipe list of saves
	/// </summary>
	public void ClearSaveList() {
		saveList.Clear();
		RememberCardParameters();
	}

	/// <summary>
	/// Remember all cards stable position
	/// </summary>
	private void RememberCardParameters() {
		foreach (Card card in gameCardsSet)
		{
			card.SetLastGoodParametres();
		}
	}

	/// <summary>
	/// Remove last entry from list
	/// </summary>
	public void RemoveLastSave() {
		if (saveList.Count > 0)
		{
			saveList.RemoveAt(saveList.Count - 1);
		}
	}
}