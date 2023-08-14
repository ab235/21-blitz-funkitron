using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class Parametres {
	public Transform lastParent;
	public Vector3 lastPosition;
	public int lastChildOrder;
	public bool isReverse;
	public Card lastCardAbove;
}

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	// The "size" property is a public Vector2 type, which represents the size of an object in the game.
	public Vector2 size { get; set; }

	// The "lastGoodParametres" property is a public Parametres type, which holds the last good parameters of the object.
	public Parametres lastGoodParametres { get; protected set; }

	// The "previousParent" property is a public Transform type, which holds the parent transform of the object before it was moved.
	public Transform previousParent { get; protected set; }

	// The "canvasGroup" field is a protected CanvasGroup type, which holds a reference to the canvas group component of the object.
	protected CanvasGroup canvasGroup;

	// The "_isReversed" field is a protected boolean, which indicates whether the object is in a reversed state.
	protected bool _isReversed;

	// The "canDrag" field is a protected boolean, which indicates whether the object can be dragged.
	public bool canDrag { get; protected set; }

	// The "OnAnimationFinishCB" field is a protected Action type, which holds a reference to a callback function that is called when an animation finishes.
	protected Action OnAnimationFinishCB;

	// The "animationQueueController" field is a protected AnimationQueueController type, which holds a reference to the animation queue controller component of the object.
	protected AnimationQueueController animationQueueController;

	// The "isShaking" field is a protected boolean, which indicates whether the object is currently shaking.
	protected bool isShaking;

	// The "stock" field is a protected Stock type, which holds a reference to the stock component of the object.
	protected Stock stock;

	// The "animations" field is a protected list of Coroutines, which holds references to the animations that are currently playing on the object.
	protected List<Coroutine> animations = new List<Coroutine>();

	// The "draggableParent" field is a private Transform type, which holds a reference to the parent transform of the object when it is being dragged.
	private Transform draggableParent;

	// The "parentToReturnTo" field is a private Transform type, which holds a reference to the parent transform to return to after the object has been dragged.
	private Transform parentToReturnTo = null;

	// The "offsetToMove" field is a private Vector3, which holds the offset to move the object when it is being dragged.
	private Vector3 offsetToMove;

	// The "screenPoint" field is a private Vector2, which holds the screen point of the object when it is being dragged.
	private Vector2 screenPoint;

	// The "offset" field is a private Vector2, which holds the offset of the object when it is being dragged.
	private Vector2 offset;

	// The "children" field is a private list of Transforms, which holds references to the children of the object.
	private List<Transform> children;

	// The "siblings" field is a private list of Transforms, which holds references to the siblings of the object.
	private List<Transform> siblings = new List<Transform>();

	// The "shakeTween" field is a private Coroutine, which holds a reference to the coroutine that is used for shaking the object.
	private Coroutine shakeTween;

	// The "isOnDrag" field is a private boolean, which indicates whether the object is currently being dragged.
	private bool isOnDrag;

	protected bool isDealt;

	public Transform GetParentToReturnTo() {
		return parentToReturnTo;
	}

	public void RegisterOnAnimationFinishCB(Action cb) {
		OnAnimationFinishCB = cb;
	}

	public virtual void Awake() {
		children = new List<Transform>();
		draggableParent = GameObject.FindGameObjectWithTag("UndoHolder").transform;
		canvasGroup = GetComponent<CanvasGroup>();
		LayoutElement layoutElement = GetComponent<LayoutElement>();
		size = new Vector2(layoutElement.minWidth, layoutElement.minHeight);
		stock = FindObjectOfType<Stock>();
		animationQueueController = FindObjectOfType<AnimationQueueController>();
		lastGoodParametres = new Parametres();
		ApplicationPauseWatcher.RegisterOnAppPauseCB(ForceStopDrag);
		canDrag = true;
		isDealt = false;
	}

	private void ForceStopDrag() {
		if (isOnDrag)
			OnEndDrag(null);
	}

	private void SavePreviousState() {
		previousParent = transform.parent;
	}

	public void UnlockCard(bool state) {
		canvasGroup.blocksRaycasts = state;
	}

	public void StartAutoFit() {
		OnEndDrag(null);
		animationQueueController.SetAnimationStatus(AnimationStatus.inProgress);
		animationQueueController.AddMovingCard();
		OnAnimationFinishCB = animationQueueController.FinishCardMoving;
	}

	virtual public void OnBeginDrag(PointerEventData eventData) {
		if (CanDrag())
		{
            BoardManager.instance.isDragging = true;
            UnlockCard(false);
            SoundManager.instance.PlayPickCardSound();
			SetReturnPoint(previousParent, transform.position - previousParent.position);
			transform.SetParent(draggableParent, true);
			if (eventData != null)
			{
				isOnDrag = true;
				GrowCard();
				PrepareDropZones();
			}
			screenPoint = new Vector2(transform.position.x, transform.position.y);
			if (eventData != null)
				offset = screenPoint - eventData.pressPosition;
			SaveManager.instance.Save();
		}
	}

	private void GrowCard() {
		Coroutine anim = TweenAnimator.instance.RunScaleAnimation(transform, Constants.vectorZoom, Constants.CARD_SIZE_ANIM_SPEED);
		animations.Add(anim);
	}

	private void ShrinkCard() {
		Coroutine anim = TweenAnimator.instance.RunScaleAnimation(transform, Constants.vectorOne, Constants.CARD_SIZE_ANIM_SPEED);
		animations.Add(anim);
	}

	private void PrepareDropZones() {
		transform.root.BroadcastMessage("ActivateExtraZone", SendMessageOptions.DontRequireReceiver);
	}

	private void DestroyDropZones() {
		transform.root.BroadcastMessage("DeactivateExtraZone", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Prevent bad began drags when card move between points
	/// </summary>
	/// <returns></returns>
	private bool CanDrag() {
		SavePreviousState();
		canDrag = previousParent.GetComponent<DropZone>() != null ? true : false;
		canDrag = canDrag && (previousParent.GetComponent<Foundation>() == null) && previousParent.GetComponent<Card>() == null;
		return canDrag;
	}

	public void OnDrag(PointerEventData eventData) {
		if (canDrag)
		{
			transform.position = eventData.position + offset;
		}
	}
	public void OnEndDrag(PointerEventData eventData) {
		if (canDrag)
		{
			isOnDrag = false;
			ShrinkCard();
			DestroyDropZones();
			MoveCard(Constants.MOVE_ANIM_TIME);
			CheckLastCardOfPreviousStack();
            animationQueueController.AddActionToQueue(stock.GetCard);
            BoardManager.instance.isDragging = false;
			isDealt = true;
		}
	}

	private void CheckLastCardOfPreviousStack() {
		if (previousParent != parentToReturnTo)
		{
			lastGoodParametres.lastCardAbove = null;
			Tableau tableau = previousParent.GetComponent<Tableau>();
			if (tableau && previousParent.childCount > 0)
			{
				Card card = previousParent.GetChild(previousParent.childCount - 1).GetComponent<Card>();
				if (card.isReversed)
				{
					animationQueueController.AddMovingCard();
					card.RegisterOnReverseAnimationFinishCB(animationQueueController.FinishCardMoving);
					card.RotateCard(Constants.STOCK_ANIM_TIME);
					MatchStatistics.instance.AddScore(Constants.TURN_OVER_CARD_POINTS);
					SoundManager.instance.PlayReverseCardSound();
				}
				else
				{
					lastGoodParametres.lastCardAbove = card;
				}
			}
		}
		else
		{
			//if parent not changed remove last save
			animationQueueController.AddMovingCard();
			OnAnimationFinishCB = animationQueueController.FinishCardMoving;
			SaveManager.instance.RemoveLastSave();
		}
	}

	public void MoveCard(float time) {
		if (gameObject.activeSelf)
		{
			UnlockCard(false);
			StopCardAnim();
			parentToReturnTo.SendMessage("LockZone", time * 0.9f, SendMessageOptions.DontRequireReceiver);
			Coroutine anim = TweenAnimator.instance.RunMoveAnimation(transform, parentToReturnTo.transform.position + offsetToMove, time, onComplete: FinishCardAnim);
			animations.Add(anim);
            RefreshIfPreviousParentIsWaste();
            UnlockCard(true);
        }
	}

	public void MoveCard(float time, Transform destination) {
		if (gameObject.activeSelf)
		{
			UnlockCard(false);
			StopCardAnim();
			parentToReturnTo.SendMessage("LockZone", time * 0.9f, SendMessageOptions.DontRequireReceiver);
			Coroutine anim = TweenAnimator.instance.RunMoveAnimation(transform, destination.position + offsetToMove, time, onComplete: FinishCardAnim);
			animations.Add(anim);
            RefreshIfPreviousParentIsWaste();
            UnlockCard(true);
        }
	}

	private void RefreshIfPreviousParentIsWaste() {
		if (previousParent != null && previousParent != parentToReturnTo)
		{
			RefreshWaste(previousParent);
		}
	}

	private void RefreshWaste(Transform wasteTransform) {
		Waste waste = wasteTransform.GetComponent<Waste>();
		if (waste)
		{
			//If previous parent is Waste refresh it
			waste.RegisterOnRefreshWasteAction(null);
			waste.RefreshChildren();
			stock.RefreshStockRefreshImage();
		}
	}

	private void FinishCardAnim() {
		SetNewParent();
		CheckIfParentIsStock(transform.parent);
		UnleashChildren();
		UnlockCard(!_isReversed);
		Invoke("RefreshThisZone", Time.deltaTime);

	}

	private void SetNewParent() {
		DropZone dropZone = parentToReturnTo.GetComponent<DropZone>();
		if (dropZone)
		{
			dropZone.AssignNewChild(transform);
		}
		else
		{
			transform.SetParent(parentToReturnTo, true);
		}
	}

	private void RefreshThisZone() {
		//BoardManager.instance.CheckIfGameCanBeAutocomplete();

		transform.parent.SendMessage("RefreshZone", SendMessageOptions.DontRequireReceiver);
		SetLastGoodParametres();
		OnAnimationFinishCB.RunAction();
	}

	public virtual void BackCardToParent() {
		UnleashChildren();
		UnlockCard(!_isReversed);
		SetNewParent();
	}

	private void CheckIfParentIsStock(Transform parent) {
		Stock stock = parent.GetComponent<Stock>();
		if (stock)
		{
			//Stock card is first on  stack after undo
			transform.SetAsFirstSibling();
			(this as Card).isReversed = true;
		}
	}

	public void SetLastGoodParametres() {
		//Remember Good position only if card as proper parent
		if (transform.parent.GetComponent<DropZone>() != null || transform.parent.GetComponent<Stock>() != null)
		{
			lastGoodParametres.isReverse = _isReversed;
			lastGoodParametres.lastChildOrder = transform.GetSiblingIndex();
			lastGoodParametres.lastParent = transform.parent;
			lastGoodParametres.lastPosition = transform.localPosition;
		}
	}

	public void ParentCardsBelow() {
		siblings.Clear();
		for (int i = transform.GetSiblingIndex() + 1; i < transform.parent.childCount; i++)
		{
			siblings.Add(transform.parent.GetChild(i));
		}

		for (int i = 0; i < siblings.Count; i++)
		{
			Card childCard = siblings[i].GetComponent<Card>();
			if (childCard != null)
			{
				childCard.transform.SetParent(transform, true);
				childCard.StopCardAnim();
			}
		}
		ReorderChildPos();
	}

	private void ReorderChildPos() {
		for (int i = 0; i < transform.childCount; i++)
		{
			Card c = transform.GetChild(i).GetComponent<Card>();
			if (c != null)
				c.transform.localPosition = GetLocalPosition(i + 1, c.size.y);
		}
	}

	public Vector3 GetLocalPosition(int cardOrder, float cardHeight) {
		return -Vector3.up * cardOrder * BehaviourSettings.instance.CalculateCardsOffset(cardHeight);
	}

	protected void UnleashChildren() {
		children.Clear();
		for (int i = 0; i < transform.childCount; i++)
		{
			children.Add(transform.GetChild(i));
		}

		for (int i = 0; i < children.Count; i++)
		{
			children[i].SetParent(transform.parent, true);
			children[i].SendMessage("SetLastGoodParametres", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void SetReturnPoint(Transform parentToReturn, Vector3 offset) {
		parentToReturnTo = parentToReturn;
		offsetToMove = offset;
	}

	protected void ShakeCardAnim() {
		if (!isShaking)
		{
			isShaking = true;
			ParentCardsBelow();
			shakeTween = TweenAnimator.instance.RunShakeAnimation(transform, 0.5f, FinishShakingAnim);
		}
		else
		{
			ForceStopShakingAnim();
			ShakeCardAnim();
		}
	}

	private void ForceStopShakingAnim() {
		isShaking = false;
		KillTween();
		RestorePosition();
	}

	private void RestorePosition() {
		if (IsWasteParented())
		{
			if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
			{
				transform.localPosition = Constants.vectorZero;
			}
		}
		else if (IsDropZoneParented())
		{
			transform.localPosition = GetLocalPosition(transform.GetSiblingIndex(), size.y);
		}
	}

	private bool IsWasteParented() {
		if (transform.parent.GetComponent<Waste>() == null)
			return false;
		return true;
	}

	private bool IsDropZoneParented() {
		if (transform.parent.GetComponent<DropZone>() == null)
			return false;
		return true;
	}

	protected bool StopCardAnim() {
		if (!isShaking)
			return false;
		FinishShakingAnim();
		KillTween();
		return true;
	}

	private void TryStopCardAnim(Draggable d) {
		if (d != this)
			StopCardAnim();
	}

	private void FinishShakingAnim() {
		if (isShaking)
		{
			isShaking = false;
			UnleashChildren();
			RestorePosition();
		}
	}

	private void KillTween() {
		if (shakeTween != null)
		{
			TweenAnimator.instance.Kill(shakeTween);
			shakeTween = null;
		}
	}
}