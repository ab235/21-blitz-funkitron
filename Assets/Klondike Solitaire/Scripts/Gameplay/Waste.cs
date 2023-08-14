using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Waste : DropZone {
	
	// variable to store the offset for card positions
	private float offset = 50;

	// delegate to store the function to be called after refreshing waste
	private Action OnRefreshWaste;

	// function to register the delegate
	public void RegisterOnRefreshWasteAction(Action cb)
	{
		OnRefreshWaste = cb;
	}
	
	// Overriding the Awake method from base class
	protected override void Awake()
	{
		base.Awake();
	}

	// Overriding the OnDrop method from base class to prevent adding other cards to waste
	public override void OnDrop(PointerEventData eventData)
	{
	  //Can't put other cards to waste
	}

	// Overriding the AssignNewChild method from base class and calling RefreshChildren to update waste after adding a new card
	public override void AssignNewChild(Transform child)
	{
		base.AssignNewChild(child);
		RefreshChildren();
	}

	// Function to refresh the child cards in the waste
	public void RefreshChildren()
	{
		// checking if there is any invoke method running with name "Refresh"
		if (IsInvoking("Refresh")) 
		{
			// cancelling the invoke method if found
			CancelInvoke("Refresh"); 
		}

		// invoking the Refresh method
		Invoke("Refresh", Time.fixedDeltaTime);
	}

	// Function to refresh the waste
	private void Refresh()
	{
		// checking if there is any invoke method running with name "RefreshCardsPos"
		if (IsInvoking("RefreshCardsPos")) 
		{
			// cancelling the invoke method if found
			CancelInvoke("RefreshCardsPos"); 
		}
		float posModifier = 0;
		float animSpeed = Constants.STOCK_ANIM_TIME / 2;

		// removing any raycast helper children
		CleanChildrenFromRaycastHelpers(); 
		int noOfChilds = transform.childCount;

		for (int i = 0; i < noOfChilds ; i++)
		{
			Transform t = transform.GetChild(i);
		  
		  	// unlocking the card
			t.SendMessage("UnlockCard",false);

			// checking if the card is the last one
			if (i== noOfChilds -1) 
			{
				posModifier = 0;

				// unlocking the card if it's the last one
				t.SendMessage("UnlockCard",true);
			}
			else if (i == noOfChilds - 2) // checking if the card is the second last one
			{
				posModifier = 1;
			}
			else // for all other cards
			{
				posModifier = 2;

			}
			MoveOnPosition(t, Constants.vectorRight * posModifier * offset, animSpeed);
		}

		// Call RefreshCardsPos
		Invoke("RefreshCardsPos", animSpeed + Time.deltaTime);
	}

	// Removes all children with the "RaycastHelper" tag
	private void CleanChildrenFromRaycastHelpers()
	{
		// Create a list to store children that need to be removed
		List<Transform> childrenToRemove = new List<Transform>();

		// Iterate through all children of the transform
		for (int i = 0; i < transform.childCount; i++)
		{
			// Get the current child
			Transform t = transform.GetChild(i);

			// If the child is not a valid child, add it to the list to be removed
			if (CheckIfItIsValidChild(t) == false)
			{
				childrenToRemove.Add(t);
				continue;
			}
		}

		// Iterate through the list of children to be removed
		for (int i = 0; i < childrenToRemove.Count; i++)
		{
			// Set the parent of the child to the root transform
			childrenToRemove[i].SetParent(transform.root);
			
			// Destroy the child game object
			Destroy(childrenToRemove[i].gameObject);
		}
	}

	// CheckIfItIsValidChild checks if a given transform is valid as a child
	private bool CheckIfItIsValidChild(Transform t)
	{
		// If the transform has the "RaycastHelper" tag, return false
		if (t.CompareTag("RaycastHelper"))
			return false;

		// Otherwise, return true
		return true;
	}

	// RefreshCardsPos refreshes the position of cards
	void RefreshCardsPos()
	{
		// Iterate through all children of the transform
		for (int i = 0; i < transform.childCount; i++)
		{
			// Get the Card component of the child
			Card card = transform.GetChild(i).GetComponent<Card>();

			// If the component exists, set its last good parameters
			if (card != null)
				card.SetLastGoodParametres();
		}
		
		// Run the OnRefreshWaste action
		OnRefreshWaste.RunAction();
	}

	// MoveOnPosition moves a transform to a specified position with a specified animation speed
	private void MoveOnPosition(Transform t, Vector3 pos, float animSpeed)
	{
		// Set the local position of the transform
		t.localPosition = -pos;
	}

	// IsWasteEmpty returns whether the waste pile is empty
	public bool IsWasteEmpty()
	{
		// Return whether the number of children of the transform is 0
		return transform.childCount == 0;
	}
}
