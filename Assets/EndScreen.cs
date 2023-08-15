using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.SegmentDisplay;
using JetBrains.Annotations;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public SegmentDisplay mainScore;
    public SegmentDisplay baseScore;
    public SegmentDisplay streakScore;
    public SegmentDisplay comboScore;
    public SegmentDisplay timeScore;
    
    public void EndScreenShow(int tb)
    {
        GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        int len = MatchStatistics.instance.score.ToString().Length;
        string zeronum = "";
        for (int i = 0; i < 7-len; i++)
        {
            zeronum += "0";
        }
        mainScore.SetText(zeronum+MatchStatistics.instance.score.ToString());
        len = (MatchStatistics.instance.score - MatchStatistics.instance.num_combos * 1000 - MatchStatistics.instance.max_streak_points - tb).ToString().Length;
        zeronum = "";
        for (int i = 0; i < 7 - len; i++)
        {
            zeronum += "0";
        }
        baseScore.SetText(zeronum+(MatchStatistics.instance.score - MatchStatistics.instance.num_combos * 1000 - MatchStatistics.instance.max_streak_points - tb).ToString());
        len = MatchStatistics.instance.max_streak_points.ToString().Length;
        zeronum = "";
        for (int i = 0; i < 7 - len; i++)
        {
            zeronum += "0";
        }
        streakScore.SetText(zeronum + MatchStatistics.instance.max_streak_points.ToString());
        len = (MatchStatistics.instance.num_combos * 1000).ToString().Length;
        zeronum = "";
        for (int i = 0; i < 7 - len; i++)
        {
            zeronum += "0";
        }
        comboScore.SetText(zeronum + (MatchStatistics.instance.num_combos * 1000).ToString());
        len = tb.ToString().Length;
        zeronum = "";
        for (int i = 0; i < 7 - len; i++)
        {
            zeronum += "0";
        }
        timeScore.SetText(zeronum + tb.ToString());
    }
}
