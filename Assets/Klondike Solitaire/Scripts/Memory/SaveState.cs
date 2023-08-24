using UnityEngine;
using System.Collections.Generic;
using System;

/*
 * CardInfo class is a data structure that holds information about a playing card, 
 * such as its reversed state, position, child order, parent transform, the card object, 
 * and the card above it. It has several methods to retrieve this information, such as GetCard(), 
 * GetCardAbove(), GetReversed(), GetPos(), GetParent(), and GetChildOrder().
 */
[Serializable]
public class CardInfo {
	private bool reversed;
	private Vector3 pos;
	private int childOrder;
	private Transform parentTransform;
	private Card card;
	private Card cardAbove;

	public CardInfo(Parametres parameters, Card card) {
		reversed = parameters.isReverse;
		pos = parameters.lastPosition;
		childOrder = parameters.lastChildOrder;
		parentTransform = parameters.lastParent;
		this.card = card;
		cardAbove = parameters.lastCardAbove;
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="value"></param>
	public CardInfo(CardInfo cardInfo) {
		this.reversed = cardInfo.GetReversed();
		this.pos = cardInfo.GetPos();
		this.childOrder = cardInfo.GetChildOrder();
		this.parentTransform = cardInfo.GetParent();
		this.card = cardInfo.GetCard();
	}

	public Card GetCard() {
		return card;
	}

	public Card GetCardAbove() {
		return cardAbove;
	}

	public bool GetReversed() {
		return reversed;
	}

	public Vector3 GetPos() {
		if (parentTransform == null)
		{
			return pos;
		}
		if (parentTransform.GetComponent<Tableau>() != null)
		{
			Vector3 newPos = card.GetLocalPosition(childOrder, card.size.y);

			return newPos;
		}
		return pos;
	}

	public Transform GetParent() {
		return parentTransform;
	}

	public int GetChildOrder() {
		return childOrder;
	}
}

/* 
 * SaveState class is a data structure that holds information about a saved game state, 
 * such as the score, vegas score, moves, experience points, stock state, stock limit, and 
 * information about all the cards in the game. It has several properties to retrieve this information, 
 * such as score, vegasScore, moves, exp, stockState, stockLimit, and cardsInfo. The cardsInfo 
 * property is a dictionary that maps Card objects to CardInfo objects. The class also has a copy 
 * constructor to make a deep copy of a SaveState object.
 */
[Serializable]
public class SaveState {
	public int score { get; set; }
	public int vegasScore { get; set; }
	public int moves { get; set; }
	public int exp { get; set; }
	public bool stockState { get; set; }
	public int stockLimit { get; set; }
	public Dictionary<Card, CardInfo> cardsInfo { get; set; }

	public SaveState() {
		cardsInfo = new Dictionary<Card, CardInfo>();
	}

	//Copy constructor
	public SaveState(SaveState saveState) {
		cardsInfo = new Dictionary<Card, CardInfo>();
		score = saveState.score;
		vegasScore = saveState.vegasScore;
		moves = saveState.moves;
		stockState = saveState.stockState;
		stockLimit = saveState.stockLimit;
		
		foreach (var item in saveState.cardsInfo)
		{
			cardsInfo.Add(item.Key, new CardInfo(item.Value));
		}
	}
}