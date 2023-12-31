﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class BoardManager : MonoBehaviour {
	public static BoardManager instance { get; private set; }

	//public static GameObject[] cardPrefabs;
	public bool isGameWon;

	public bool isDragging;


	public GameObject noMoreMovesPopup;

	private GameObject cardPrefab;
	private List<GameObject> cardsPool;
	private bool isReplay;

	//private GameObject[] cardsPool;
	private Tableau[] tableaus;

	private Foundation[] foundations;
	private Waste waste;
	private Stock stock;
	private CanvasGroup canvasGroup;
	private AutofitCard autofitCard;
	private AnimationQueueController animationQueueController;
	private DimScreen dimscreen;
	private TextLine tline;
	private EndScreen endscreen;
	private GridTop gtop;
	private Camera cam;
	private GameObject gboard;
	private StockAndWaste sw;
	private ExtraInfo ei;
	private GameObject topui;
	private bool tableauCardsExposed;
	private bool probablyLostGameContinued;

	private IEnumerator dealCardsCoroutine;

	private void Awake() {
		instance = this;
		try
		{
			cardPrefab = Resources.Load<GameObject>("CardPrefabs/CardPrefab");
			tableaus = FindObjectsOfType<Tableau>();
			tableaus = tableaus.OrderBy((x) => x.name).ToArray();
			foundations = FindObjectsOfType<Foundation>();
			foundations = foundations.OrderBy((x) => x.name).ToArray();
			animationQueueController = FindObjectOfType<AnimationQueueController>();
			waste = FindObjectOfType<Waste>();
			stock = FindObjectOfType<Stock>();
			canvasGroup = GetComponent<CanvasGroup>();
			autofitCard = new AutofitCard();
			dimscreen = FindObjectOfType<DimScreen>();
			tline = FindObjectOfType<TextLine>();
			gtop = FindObjectOfType<GridTop>();
			endscreen = FindObjectOfType<EndScreen>();
			cam = FindObjectOfType<Camera>();
			gboard = GameObject.FindWithTag("GameBoard");
			sw = FindObjectOfType<StockAndWaste>();
			ei = FindObjectOfType<ExtraInfo>();
			topui = GameObject.FindGameObjectWithTag("TopUI");
            gtop.Start();
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.Message);
		}
	}

	private void Start() {
		PrepareObjectsPool();
		ResetState();
	}

	private void ResetState() {
		//Autocomplete.instance.SetButtonState(false);
		isGameWon = false;
		tableauCardsExposed = false;
		probablyLostGameContinued = false;
		if (!isReplay)
		{
			ShuffleCards();
		}

		BackCardsToInitialState();
		SaveManager.instance.SetGameCardsSet(cardsPool);
		tline.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 200);
		SoundManager.instance.PlayHH();
		SoundManager.instance.PlayCrowdNoise();
		/*if (dealCardsCoroutine != null)
			StopCoroutine(dealCardsCoroutine);
		dealCardsCoroutine = DealCards();
		StartCoroutine(dealCardsCoroutine);*/
	}

	public void NewGame() {
		isReplay = false;
		UIManager.instance.ResetGameState();
	}

	public void Replay() {
		isReplay = true;
		UIManager.instance.ResetGameState();
	}

	public void ContinueGame() {
		probablyLostGameContinued = true;
	}

	private void ShuffleCards() {
		do
		{
			for (int i = cardsPool.Count - 1; i > 0; --i)
			{
				int index = Randomizer.GetRandomNumber(0, i);
				GameObject temp = cardsPool[i];
				cardsPool[i] = cardsPool[index];
				cardsPool[index] = temp;
			}
		} while (!CheckIfGameIsPossibleToPlay());
		Resources.UnloadUnusedAssets();
	}

	private bool CheckIfGameIsPossibleToPlay() {
		Card[] cards = new Card[cardsPool.Count];
		for (int i = 0; i < cards.Length; i++)
		{
			cards[i] = cardsPool[i].GetComponent<Card>();
		}
		return ThereIsAnyAceAvailable(cards) || ThereIsAnyMovePossible(cards);
	}

	private bool ThereIsAnyAceAvailable(Card[] cards) {
		int j = 0;
		for (int i = 0; i < cards.Length; i++)
		{
			if (i < Constants.STOCK_START_INDEX)
			{
				if (i == Constants.availableCardsIndexes[j])
				{
					j++;
					if (cards[i].cardValue == Enums.CardValue.Ace)
						return true;
				}
			}
			else
			{
				if (cards[i].cardValue == Enums.CardValue.Ace)
					return true;
			}
		}
		return false;
	}

	private bool ThereIsAnyMovePossible(Card[] cards) {
		int j = 0;
		return true;
		for (int i = 0; i < cards.Length; i++)
		{
			if (i < Constants.STOCK_START_INDEX)
			{
				if (i == Constants.availableCardsIndexes[j])
				{
					j++;
					if (CheckIfCardFitToAnyAvailablePlace(cards[i], cards))
						return true;
				}
			}
			else
			{
				if (CheckIfCardFitToAnyAvailablePlace(cards[i], cards))
					return true;
			}
		}
		return false;
	}

	private bool CheckIfCardFitToAnyAvailablePlace(Card cardToFit, Card[] cards) {
		return true;
	}

	private void PrepareObjectsPool() {
		List<Enums.CardValue> valuesAsArray = Enum.GetValues(typeof(Enums.CardValue)).Cast<Enums.CardValue>().ToList();
		List<Enums.CardColor> colorsAsArray = Enum.GetValues(typeof(Enums.CardColor)).Cast<Enums.CardColor>().ToList();
		cardsPool = new List<GameObject>();
		int colorIndex = -1;

		for (int i = 0; i < Constants.CARDS_IN_DECK; i++)
		{
			if (i % Constants.CARDS_IN_COLOR == 0)
				colorIndex = (colorIndex + 1) % Constants.CARDS_COLORS;
			GameObject cardGO = Instantiate(cardPrefab, transform);
			Card card = cardGO.GetComponent<Card>();
			card.cardID = i;
			card.cardValue = valuesAsArray[i % Constants.CARDS_IN_COLOR];
			card.cardColor = colorsAsArray[colorIndex];
			card.name = card.cardColor + "_" + card.cardValue;
			cardsPool.Add(cardGO);
		}
		BackCardsToInitialState();
	}

	private void BackCardsToInitialState() {
		for (int i = 0; i < cardsPool.Count; i++)
		{
			cardsPool[i].transform.SetParent(stock.transform, transform);
			cardsPool[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			cardsPool[i].transform.localScale = Vector3.one;
			cardsPool[i].transform.SetAsLastSibling();
		}
	}

	private IEnumerator DealCards() {
		LockBoard();
		animationQueueController.SetAnimationStatus(AnimationStatus.inProgress);
		yield return new WaitForSeconds(0.1f);
		int index = 0;
		for (int i = 0; i < Constants.NUMBER_OF_TABLEAUS; i++)
		{
			for (int j = 0; j <= i; j++)
			{
				SoundManager.instance.PlayPickCardSound();
				Card card = cardsPool[index].GetComponent<Card>();
				if (j == i) //If this card is on top card uncover it
					card.isReversed = false;

				card.transform.SetParent(stock.transform.parent);
				card.SetReturnPoint(tableaus[i].transform, -Vector3.up * j * BehaviourSettings.instance.CalculateCardsOffset(card.size.y) * BehaviourSettings.instance.GetScaleFactor());

				card.MoveCard(Constants.MOVE_ANIM_TIME / 3);
				index++;
				if (index == 28)
					card.RegisterOnAnimationFinishCB(CardDealt);
				else
					yield return new WaitForSeconds(0.05f);
			}
		}
		dealCardsCoroutine = null;

	}

	private void CardDealt() {
		animationQueueController.SetAnimationStatus(AnimationStatus.none);
		SaveManager.instance.ClearSaveList();
		UnlockBoard();
	}

	public void LockBoardForOneFrame() {
		LockBoard();
		Invoke("UnlockBoard", Time.fixedDeltaTime);
	}

	public void LockBoard() {
		canvasGroup.blocksRaycasts = false;
	}

	public void UnlockBoard() {
		canvasGroup.blocksRaycasts = true;
	}

	public List<Card> GetCurrentWasteCards(bool searchAllStockAndWaste) {
		List<Card> wasteCards = new List<Card>();

		if (waste.transform.childCount == 0)
			return wasteCards;

		if (searchAllStockAndWaste)
		{
			for (int i = 0; i < waste.transform.childCount; i++)
			{
				Card c = waste.transform.GetChild(i).GetComponent<Card>();
				wasteCards.Add(c);
			}
		}
		else
		{
			Card currentWasteCard = waste.transform.GetChild(waste.transform.childCount - 1).GetComponent<Card>();
			wasteCards.Add(currentWasteCard);
		}
		return wasteCards;
	}

	internal List<Card> GetCurrentStockCards() {
		List<Card> stockCards = new List<Card>();

		if (stock.transform.childCount == 0)
			return stockCards;

		for (int i = 0; i < stock.transform.childCount; i++)
		{
			Card c = stock.transform.GetChild(i).GetComponent<Card>();
			stockCards.Add(c);
		}

		return stockCards;
	}

	public List<Card> GetTableauCards() {
		List<Card> cards = new List<Card>();
		for (int i = 0; i < tableaus.Length; i++)
		{
			if (tableaus[i] != null && tableaus[i].transform.childCount != 0)
			{
				Card[] tableauCards = tableaus[i].GetComponentsInChildren<Card>();
				for (int j = 0; j < tableauCards.Length; j++)
				{
					if (!tableauCards[j].isReversed)
					{
						cards.Add(tableauCards[j]);
					}
				}
			}
		}
		return cards;
	}

	public List<Card> GetOnTopTableauCards(out List<Transform> emptyTabelau) {
		List<Card> cards = new List<Card>();
		emptyTabelau = new List<Transform>();

		for (int i = 0; i < tableaus.Length; i++)
		{
			if (tableaus[i].transform.childCount == 0)
			{
				emptyTabelau.Add(tableaus[i].transform);
			}
			else
			{
				Card card = tableaus[i].transform.GetChild(tableaus[i].transform.childCount - 1).GetComponent<Card>();
				cards.Add(card);
			}
		}
			
		return cards;
	}

	public List<Card> GetOnTopTableauCards() {
		List<Transform> emptyList = new List<Transform>();
		return GetOnTopTableauCards(out emptyList);
	}

	public List<Card> GetFoundationCards(out List<Transform> emptyFoundation) {
		List<Card> cards = new List<Card>();
		emptyFoundation = new List<Transform>();
		for (int i = 0; i < foundations.Length; i++)
		{
			if (foundations[i].transform.childCount == 0)
			{
				emptyFoundation.Add(foundations[i].transform);
			}
			else
			{
				Card card = foundations[i].transform.GetChild(foundations[i].transform.childCount - 1).GetComponent<Card>();
				cards.Add(card);
			}
		}
		return cards;
	}

	public bool TryAutoFitCard(Card card) {
		return autofitCard.TryAutoFitCard(card);
	}

	private void UpdateScreen()
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
        cam.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
		cam.orthographicSize = Screen.height / 2;
		cam.GetComponent<RectTransform>().position = new Vector3(GetComponent<RectTransform>().position.x,
			GetComponent<RectTransform>().position.y, cam.GetComponent<RectTransform>().position.z);
        gboard.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, (float)(Screen.height*1.52));
		gboard.transform.GetComponent<RectTransform>().localPosition = 
			new Vector3(GetComponent<RectTransform>().position.x, (float)(GetComponent<RectTransform>().position.y + Screen.height*0.26),
			GetComponent<RectTransform>().position.z);
        /*endscreen.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
        endscreen.GetComponent<RectTransform>().position = new Vector3(GetComponent<RectTransform>().position.x,
            GetComponent<RectTransform>().position.y + GetComponent<RectTransform>().sizeDelta.y * GetComponent<RectTransform>().localScale.y,
            GetComponent<RectTransform>().position.z);*/
    }

    private void Update()
    {
        if (!CheckIfGameComplete() && !isGameWon && !MatchStatistics.instance.StopUpdating)
        {
            UpdateScreen();
            tline.UpdatePositions();
            gtop.UpdatePositions();
            sw.UpdatePosition();
            ei.UpdatePosition();
        }
        if (waste.transform.childCount > 0 && waste.transform.GetChild(0).GetComponent<Card>() != null && Foundation.isBJ(waste.transform.GetChild(0).GetComponent<Card>()))
        {
            tline.showWild();
			ei.ShowWild();
        }
        foreach (Foundation f in foundations)
		{
			f.pointCheck();
		}
		if (!stock.firstCardSet)
		{
            stock.dealFirstCard();
        }
        if (CheckIfGameComplete())
		{
			FinishGame();
			LockBoard();
		}
    }
    public bool CheckIfGameComplete() {
		if (MatchStatistics.instance.failures < 3 && MatchStatistics.instance.moves > 0)
		{
			isGameWon = false;
			return false;
		}
		return true;
	}

	private IEnumerator PanCameraUp()
	{
		float endheight = gboard.GetComponent<RectTransform>().sizeDelta.y - GetComponent<RectTransform>().sizeDelta.y + GetComponent<RectTransform>().position.y;
		print(endheight);
        yield return new WaitForSeconds(0.05f);
		float starty = cam.transform.localPosition.y;
        while (cam.transform.localPosition.y < endheight)
		{
			print(cam.transform.localPosition.y);
			print("endheight - starty" + (endheight - starty).ToString());
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, (float)(cam.transform.localPosition.y+(endheight-starty)/100), cam.transform.localPosition.z);
			print(new Vector3(cam.transform.localPosition.x, (float)(cam.transform.localPosition.y + (endheight - starty) / 100), cam.transform.localPosition.z));
            yield return new WaitForSeconds(0.0025f);
        }
        print(cam.transform.localPosition.y);
    }

	public void FinishGame() {
		if (!isGameWon)
		{
			SoundManager.instance.PlayEGC();
			int tb = 0;
			isGameWon = true;
			MatchStatistics.instance.StopTime();
			if (MatchStatistics.instance.moves == 0)
			{
				tb = 10 * (int)(180-MatchStatistics.instance.GetGameTime());
			}
			MatchStatistics.instance.AddScore(tb);
			//stopmenu.showMenu(tb);
			transform.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
			endscreen.EndScreenShow(tb);
			topui.SetActive(false);
			//Coroutine anim = TweenAnimator.instance.RunMoveAnimation(cam.transform, new Vector3(585, 3801, -473), 6.5f);
			StartCoroutine(PanCameraUp());
            //Autocomplete.instance.SetButtonState(false);
        }
	}

	public static void RecalculatePoints()
	{
		foreach (Foundation f in instance.foundations)
		{
			f.RecalculatePoints();
		}
	}

	public void ActivateMenu(GameObject g)
	{
		MatchStatistics.instance.StopTime();
		g.SetActive(true);
	}

    public void DeactivateMenu(GameObject g)
    {
        MatchStatistics.instance.StartTime();
        g.SetActive(false);
    }

    /*public bool CheckIfGameCanBeAutocomplete() {
		if (canvasGroup.blocksRaycasts)
		{
			if (!probablyLostGameContinued && !tableauCardsExposed && HintManager.instance.SearchForPossibilitiesOfMove(true) == false)
			{
				noMoreMovesPopup.SetActive(true);
				return false;
			}

			for (int i = 0; i < tableaus.Length; i++)
			{
				if (!tableaus[i].CheckIfTableauHasAllCardExposed())
				{
					Autocomplete.instance.SetButtonState(false);
					tableauCardsExposed = false;
					return false;
				}
			}

			if (stock.transform.childCount >= 0) {
				return false;
			}

			tableauCardsExposed = true;

			if (!CheckDraw3OrVegasStockState())
			{
				Autocomplete.instance.SetButtonState(false);
				return false;
			}

			CheckIfGameComplete();
			Autocomplete.instance.SetButtonState(true);
			return true;
		}
		return false;
	}*/

    private bool CheckDraw3OrVegasStockState() {
		if (MatchStatistics.instance.isDraw3 || MatchStatistics.instance.IsVegasGame())
		{
			if (waste.transform.childCount + stock.transform.childCount > 1)
				return false;
			return true;
		}
		return true;
	}
}