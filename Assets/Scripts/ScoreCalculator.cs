using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreCalculator
{

    int score = 0;
    
    bool isCheongDan = false;
    bool isHongDan = false;
    bool isChoDan = false;
    bool isGodori = false;

    int beforeGwangLevel = 0;

    private List<(string, Color)> scoreEvent = new();

    public List<(string, Color)> DrainScoreEvent()
    {
        List<(string, Color)> copied = new(scoreEvent);
        scoreEvent.Clear();
        return copied;
    }

    public int Calculate(IEnumerable<Hwatoo> hwatoos)
    {
        Dictionary<CardType, List<Hwatoo>> groupedHwatoos = new();

        foreach(CardType cctype in Enum.GetValues(typeof(CardType)))
        {
            groupedHwatoos[cctype] = new();
        }

        foreach(var hwatoo in hwatoos)
        {
            CardType ctype = hwatoo.hwatooData.cardType;
            groupedHwatoos[ctype].Add(hwatoo);
        }

        // TODO : 이벤트 추가
        if (beforeGwangLevel != 3 && groupedHwatoos[CardType.Gwang].Count == 3)
        {
            scoreEvent.Add(("三광", Color.cyan));
        }
        if (beforeGwangLevel != 4 && groupedHwatoos[CardType.Gwang].Count == 4)
        {
            scoreEvent.Add(("四광", Color.salmon));
        }
        if (beforeGwangLevel != 5 && groupedHwatoos[CardType.Gwang].Count == 5)
        {
            scoreEvent.Add(("五광", Color.hotPink));
        }
        beforeGwangLevel = groupedHwatoos[CardType.Gwang].Count;

        if (!isCheongDan && CheckCheongDan(groupedHwatoos[CardType.DDi])) {
            isCheongDan = true;
            scoreEvent.Add(("청단", Color.blue));
        }
        if (!isHongDan && CheckHongDan(groupedHwatoos[CardType.DDi])) {
            isHongDan = true;
            scoreEvent.Add(("홍단", Color.red));
        }
        if (!isChoDan && CheckChoDan(groupedHwatoos[CardType.DDi])) {
            isChoDan = true;
            scoreEvent.Add(("초단", Color.brown));
        }
        if (!isGodori && CheckGodori(groupedHwatoos[CardType.Yeol])) {
            isGodori = true;
            scoreEvent.Add(("고도리", Color.orange));
        }

        int score = 0;
        score += EvaluatePiScore(groupedHwatoos[CardType.Pi], groupedHwatoos[CardType.SSangPi]);
        score += EvaluateGwangScore(groupedHwatoos[CardType.Gwang]);
        score += EvaluateYeolScore(groupedHwatoos[CardType.Yeol])
                    + (isGodori ? 5 : 0);
        score += EvaluateDDiScore(groupedHwatoos[CardType.DDi]) 
                    + (isCheongDan ? 3 : 0) 
                    + (isHongDan ? 3 : 0)
                    + (isChoDan ? 3 : 0);

        return score;
    }

    private int EvaluatePiScore(List<Hwatoo> pis, List<Hwatoo> ssangpis)
    {
        int piCount = pis.Count;
        piCount += ssangpis.Count * 2;

        return Mathf.Max(0, piCount - 9);
    }

    private int EvaluateGwangScore(List<Hwatoo> gwangs)
    {
        if (gwangs.Count == 3)
        {
            return 
                gwangs.Any(hwatoo => hwatoo.hwatooData.month == 12)
                    ? 2
                    : 3;
        }
        else if (gwangs.Count == 4) { return 4; }
        else if (gwangs.Count == 5) { return 15; }

        return 0;
    }

    private int EvaluateYeolScore(List<Hwatoo> yeols)
    {
        return Mathf.Max(0, yeols.Count - 4);
    }

    public bool CheckGodori(List<Hwatoo> yeols)
    {
        if (!yeols.Any(yeol => yeol.hwatooData.month == 2)) { return false; }
        if (!yeols.Any(yeol => yeol.hwatooData.month == 4)) { return false; }
        if (!yeols.Any(yeol => yeol.hwatooData.month == 8)) { return false; }
        return true;
    }

    private int EvaluateDDiScore(List<Hwatoo> ddis)
    {
        return Mathf.Max(0, ddis.Count - 4);
    }

    public bool CheckCheongDan(List<Hwatoo> ddis)
    {
        if (ddis.Count(ddi => ddi.hwatooData.danType == DanType.Cheongdan) >= 3)
        {
            return true;
        }
        return false;
    }

    public bool CheckHongDan(List<Hwatoo> ddis)
    {
        if (ddis.Count(ddi => ddi.hwatooData.danType == DanType.Hongdan) >= 3)
        {
            return true;
        }
        return false;
    }

    public bool CheckChoDan(List<Hwatoo> ddis)
    {
        if (
            ddis.Count(ddi => 
                ddi.hwatooData.danType == DanType.Chodan 
                && ddi.hwatooData.month != 12
            ) >= 3
        )
        {
            return true;
        }
        return false;
    }

    public int Cal(IEnumerable<Hwatoo> hwatoos)
    {
        return Calculate(hwatoos);
    }
}
