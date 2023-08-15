using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.SegmentDisplay;

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
        mainScore.SetText(MatchStatistics.instance.score.ToString());
        baseScore.SetText((MatchStatistics.instance.score - MatchStatistics.instance.num_combos * 1000 - MatchStatistics.instance.max_streak_points - tb).ToString());
        streakScore.SetText(MatchStatistics.instance.max_streak_points.ToString());
        comboScore.SetText((MatchStatistics.instance.num_combos * 1000).ToString());
        timeScore.SetText(tb.ToString());
    }
}
