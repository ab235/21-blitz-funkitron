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
        mainScore.DigitCount = len;
        mainScore.SetText(MatchStatistics.instance.score.ToString());
        len = (MatchStatistics.instance.score - MatchStatistics.instance.num_combos * 1000 - MatchStatistics.instance.max_streak_points - tb).ToString().Length;
        baseScore.SetText((MatchStatistics.instance.score - MatchStatistics.instance.num_combos * 1000 - MatchStatistics.instance.max_streak_points - tb).ToString());
        len = MatchStatistics.instance.max_streak_points.ToString().Length;
        streakScore.SetText(MatchStatistics.instance.max_streak_points.ToString());
        len = (MatchStatistics.instance.num_combos * 1000).ToString().Length;
        comboScore.SetText((MatchStatistics.instance.num_combos * 1000).ToString());
        len = tb.ToString().Length;
        timeScore.SetText(tb.ToString());
    }
}
