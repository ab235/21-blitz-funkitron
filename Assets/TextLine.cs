using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TextLine : MonoBehaviour
{
    // Start is called before the first frame update

    private Foundation[] foundations;
    private float padding;
    private float timer;
    void Start()
    {
        foundations = FindObjectsOfType<Foundation>();
        foundations = foundations.OrderBy((x) => x.name).ToArray();
        foreach (Transform child in transform)
        {
            transform.Find("21popup").gameObject.SetActive(false);
        }
    }

    public void UpdatePositions()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.x, (float)(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y * 0.09));
        GetComponent<RectTransform>().localPosition = new Vector3(0, (float)(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y * 0.44), 0);
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(GridTop.cardWidth+20, 100);
        padding = (BoardManager.instance.GetComponent<RectTransform>().sizeDelta.x - (60 + GridTop.foundWidth * 4))/2;
        GetComponent<GridLayoutGroup>().padding.right = (int) padding+Constants.PADDING_SIDE;
        GetComponent<GridLayoutGroup>().padding.left = (int)padding+Constants.PADDING_SIDE;
    }

    public void showWild()
    {
        for (int i = 0; i < Constants.NUMBER_OF_FOUNDATIONS; i++)
        {
            transform.GetChild(i).GetComponent<Text>().text = "Wild";
        }
    }

    public void showStreak(int found_num)
    {
        transform.GetChild(found_num).GetComponent<Text>().text = "Good\nStreak!";
        Invoke("hideText", (float)2);
    }

    public void hideText()
    {
        for (int i = 0; i < Constants.NUMBER_OF_FOUNDATIONS; i++)
        {
            transform.GetChild(i).GetComponent<Text>().text = "";
        }
    }

    public void Flash21(int found_num)
    {
        transform.GetChild(found_num + Constants.NUMBER_OF_FOUNDATIONS).Find("PointSign").gameObject.SetActive(false);
        transform.GetChild(found_num + Constants.NUMBER_OF_FOUNDATIONS).Find("21popup").gameObject.SetActive(true);
        transform.GetChild(found_num + Constants.NUMBER_OF_FOUNDATIONS).GetComponent<Text>().text = "";
        StartCoroutine(Reactivate_PS(found_num));
    }
    public IEnumerator Reactivate_PS(int found_num)
    {
        yield return new WaitForSeconds(1);
        transform.GetChild(found_num + Constants.NUMBER_OF_FOUNDATIONS).Find("PointSign").gameObject.SetActive(true);
        transform.GetChild(found_num + Constants.NUMBER_OF_FOUNDATIONS).Find("21popup").gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < Constants.NUMBER_OF_FOUNDATIONS; i++)
        {
            int points = foundations[i].GetComponent<Foundation>().indipoints;
            int ace_count = foundations[i].GetComponent<Foundation>().ace_count;
            if (foundations[i].isStreak)
            {
                transform.GetChild(i).GetComponent<Text>().text = "Good\nStreak!";
            }
            else
            {
                if (points <= 11 && ace_count > 0)
                {
                    transform.GetChild(i+Constants.NUMBER_OF_FOUNDATIONS).GetComponent<Text>().text = (points.ToString() + "/" + (points + 10).ToString());
                }
                else
                {
                    transform.GetChild(i+Constants.NUMBER_OF_FOUNDATIONS).GetComponent<Text>().text = points.ToString();
                }
            }
        }
    }
}
