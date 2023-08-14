using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// DropZone is a script that is responsible for handling card drops in a canvas.
// The script requires a CanvasGroup component to be attached to the same GameObject.
// It implements the IDropHandler interface and is a MonoBehaviour class.

[RequireComponent(typeof(CanvasGroup))]
public class DropZone : MonoBehaviour, IDropHandler
{
	// LayoutGroup component reference
	private LayoutGroup layoutGroup;
	// CanvasGroup component reference
	private CanvasGroup canvasGroup;
	// reference to a raycast helper GameObject
	GameObject raycastHelper;

    protected AnimationQueueController animationQueueController;

    protected Waste waste;

    protected Stock stock;

    // virtual method that is called to assign a new child to the DropZone
    public virtual void AssignNewChild(Transform child) {
		// play a sound effect
		SoundManager.instance.PlayDropCardSound();
		// set the child as the child of the DropZone's transform
		child.SetParent(transform, true);
	}


	// method called when the script is being initialized
	protected virtual void Awake() {
		// get the LayoutGroup component reference
		layoutGroup = GetComponent<LayoutGroup>();
		// get the CanvasGroup component reference
		canvasGroup = GetComponent<CanvasGroup>();
	}

    protected void Start()
    {
        waste = FindObjectOfType<Waste>();
        animationQueueController = FindObjectOfType<AnimationQueueController>();
        stock = FindObjectOfType<Stock>();
    }

    // method that calculates the spacing between the children of the DropZone
    private float CalculateSpacing()
	{
		if (layoutGroup is HorizontalOrVerticalLayoutGroup)
		{
			return  (layoutGroup as HorizontalOrVerticalLayoutGroup).spacing;
		}
		else if (layoutGroup is GridLayoutGroup)
		{
			return (layoutGroup as GridLayoutGroup).spacing.y;
		}
		return 0;
	}

	// virtual method called when a drop event occurs
	virtual public void OnDrop(PointerEventData eventData) {
		if (eventData != null) {
			// get the Card component of the dragged object
			Card c = eventData.pointerDrag.GetComponent<Card>();
			OnDrop(c);
		}
	}

	// virtual method called when a drop event occurs
	virtual public void OnDrop(Card card) {
		if (card != null) {
			// set the return point of the card to the current position of the DropZone
			card.SetReturnPoint(transform, -Vector3.up * transform.childCount * BehaviourSettings.instance.CalculateCardsOffset(card.size.y) * BehaviourSettings.instance.GetScaleFactor());
		}
	}

	// method that calculates the position of a new child in the DropZone
	private Vector3 CalculatePosition(float height) {
		if(transform.childCount > 0) {
			float spacing = CalculateSpacing();
			float offset =  (height + spacing)*BehaviourSettings.instance.GetScaleFactor();
			return transform.GetChild(transform.childCount - 1).position - new Vector3(0,offset, 0) - transform.position;
		}

		return Constants.vectorZero;
	}

	// method that calculates the position of a new child in the DropZone
	private void LockZone(float unlockTime) {
		canvasGroup.blocksRaycasts = false;
		CancelInvoke("UnlockZone");
		Invoke("UnlockZone",unlockTime);
	}

	// unlock DropZone
	private void UnlockZone() {
		canvasGroup.blocksRaycasts = true;
	}

	// Refresh DropZone
	private void RefreshZone()
	{
		if (layoutGroup == null)
		{
			return;
		}

		layoutGroup.enabled = false;
		layoutGroup.enabled = true;
	}

	// Activate extra DropZone
	private void ActivateExtraZone()
	{
		Vector3 pos = transform.position;
		if (transform.childCount > 0)
			pos = transform.GetChild(transform.childCount - 1).position;
		pos += Vector3.up * CalculateSpacing() * BehaviourSettings.instance.GetScaleFactor()/2;
		raycastHelper = Instantiate(Resources.Load("RaycastHelper"), pos, Quaternion.identity, transform) as GameObject;
	}
	
	// Deactivete extra DropZone
	private void DeactivateExtraZone()
	{
		if (raycastHelper != null)
		{
			raycastHelper.transform.SetParent(transform.root);
			Destroy(raycastHelper);
		}
	}
}
