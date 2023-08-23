using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StockAndWaste : MonoBehaviour
{
    // Start is called before the first frame update
    public void UpdatePosition()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.x, 200);
        GetComponent<RectTransform>().localPosition = new Vector3(0, (float)(-BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y * 0.28), 0);
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(GridTop.cardWidth, (float)(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y * 0.10));
    }
}
