using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ExtraInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public void UpdatePosition()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.x, (float)(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y * 0.10));
        GetComponent<RectTransform>().localPosition = new Vector3(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.x/2, (float)(-BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y * 0.24), 0);

    }

    public void ShowWild()
    {
        GetComponent<Text>().text = "Wild";
    }

    public void HideText()
    {
        GetComponent<Text>().text = "";
    }
}
