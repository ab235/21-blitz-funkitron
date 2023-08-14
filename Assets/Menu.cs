using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    private CanvasGroup group;
    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
        group.blocksRaycasts = false;
    }

    // Update is called once per frame
    public void showMenu(int tb)
    {
        group.alpha = (float)1.65;
        group.blocksRaycasts = true;
        transform.GetChild(0).GetComponent<Text>().text = "Game Over. Score: " + MatchStatistics.instance.score.ToString() + ".\n Base: " + 
            (MatchStatistics.instance.score - MatchStatistics.instance.num_combos*1000 - MatchStatistics.instance.max_streak_points - tb).ToString() + ".\n Combo: "
            + (MatchStatistics.instance.num_combos * 1000).ToString() + ".\n Streak: " + MatchStatistics.instance.max_streak_points.ToString() + ".\n Time: " + tb.ToString();
    }
}
