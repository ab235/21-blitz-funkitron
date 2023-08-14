using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// This script handles the click functionality of a UI element in Unity.
// It implements the IPointerClickHandler interface to allow for custom handling of pointer click events.
public class ClickableBehaviour : MonoBehaviour, IPointerClickHandler
{
    // Selectable s - reference to the Selectable component attached to the same GameObject as this script.
    Selectable s;

    // Awake() - called when the script is first loaded, it retrieves the Selectable component attached to the GameObject.
    private void Awake()
    {
        s = GetComponent<Selectable>();
    }

    // OnPointerClick(PointerEventData eventData) - called when the UI element is clicked, it plays a sound through the SoundManager if the Selectable component is interactable.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (s != null && s.interactable == false)
            return;
        SoundManager.instance.PlayBtnClickSound();
    }
}
