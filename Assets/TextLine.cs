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
    private Transform[] pointsigns;
    private float padding;
    private float timer;
    private int disable_text;
    void Start()
    {
        foundations = FindObjectsOfType<Foundation>();
        foundations = foundations.OrderBy((x) => x.name).ToArray();
        pointsigns = new Transform[Constants.NUMBER_OF_FOUNDATIONS];
        for (int i = Constants.NUMBER_OF_FOUNDATIONS; i < Constants.NUMBER_OF_FOUNDATIONS*2; i++)
        {
            pointsigns[i-Constants.NUMBER_OF_FOUNDATIONS] = transform.GetChild(i).Find("PointSign");
        }
        disable_text = -1;
    }

    public void UpdatePositions()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.x, (float)(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y * 0.09));
        GetComponent<RectTransform>().localPosition = new Vector3(0, (float)(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y * 0.42), 0);
        float big_size = (BoardManager.instance.GetComponent<RectTransform>().sizeDelta.x / Constants.NUMBER_OF_FOUNDATIONS) - 40;
        if (big_size > 225)
        {
            big_size = 225;
        }
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(big_size, (float)(BoardManager.instance.GetComponent<RectTransform>().sizeDelta.y*0.08));//new Vector2(GridTop.cardWidth-10, 150);
        for (int i = 0; i < Constants.NUMBER_OF_FOUNDATIONS; i++)
        {
            pointsigns[i].GetComponent<RectTransform>().localScale = new Vector3((float)(big_size*0.444), (float)(BoardManager.instance.
                GetComponent<RectTransform>().sizeDelta.y * 0.08*0.444), 1);
        }
        GetComponent<GridLayoutGroup>().spacing = new Vector2(30, 0);
        padding = (BoardManager.instance.GetComponent<RectTransform>().sizeDelta.x - (90 + (4*big_size+180)))/2;
        if (padding < 0)
        {
            padding = 0;
        }
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
        Invoke("hideText", (float)1);
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
        pointsigns[found_num].gameObject.SetActive(false);
        transform.GetChild(found_num + Constants.NUMBER_OF_FOUNDATIONS).Find("21popup").gameObject.SetActive(true);
        disable_text = found_num;
        StartCoroutine(Reactivate_PS(found_num));
    }
    public IEnumerator Reactivate_PS(int found_num)
    {
        yield return new WaitForSeconds(1);
        pointsigns[found_num].gameObject.SetActive(true);
        transform.GetChild(found_num + Constants.NUMBER_OF_FOUNDATIONS).Find("21popup").gameObject.SetActive(false);
        disable_text = -1;
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
                if (points <= 11 && ace_count > 0 && i != disable_text)
                {
                    transform.GetChild(i+Constants.NUMBER_OF_FOUNDATIONS).GetComponent<Text>().text = (points.ToString() + "/" + (points + 10).ToString());
                }
                else if (i != disable_text)
                {
                    transform.GetChild(i+Constants.NUMBER_OF_FOUNDATIONS).GetComponent<Text>().text = points.ToString();
                }
                else
                {
                    transform.GetChild(i + Constants.NUMBER_OF_FOUNDATIONS).GetComponent<Text>().text = "";
                }
            }
        }
    }
}
