using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GridTop : MonoBehaviour
{
    // Start is called before the first frame update
    public static float cardWidth;
    public static float foundWidth;
    public static GridTop instance { get; private set; }
    private float pholder;
    // Update is called once per frame
    public void Start()
    {
        instance = this;
        UpdatePositions();
    }

    public void UpdatePositions()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.x, 200);
        GetComponent<RectTransform>().localPosition = new Vector3(0, (float)(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y * 0.25), 0);
        pholder = (BoardManager.instance.GetComponent<RectTransform>().sizeDelta.x - 2 * Constants.PADDING_SIDE - 3* GetComponent<GridLayoutGroup>().spacing.x) / Constants.NUMBER_OF_FOUNDATIONS;
        if (pholder > Constants.MAX_FOUND_WIDTH)
        {
            foundWidth = Constants.MAX_FOUND_WIDTH;
        }
        else
        {
            foundWidth = pholder;
        }
        cardWidth = foundWidth - 30;
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(cardWidth, (float)(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y * 0.45));
        GetComponent<GridLayoutGroup>().padding.right = Constants.PADDING_SIDE;
        GetComponent<GridLayoutGroup>().padding.left = Constants.PADDING_SIDE;
    }
}
