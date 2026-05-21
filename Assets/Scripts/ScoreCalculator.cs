using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    public static int Calculate(IEnumerable<Hwatoo> hwatoos)
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

        int score = 0;
        score += EvaluatePiScore(groupedHwatoos[CardType.Pi], groupedHwatoos[CardType.SSangPi]);
        score += EvaluateGwangScore(groupedHwatoos[CardType.Gwang]);
        score += EvaluateYeolScore(groupedHwatoos[CardType.Yeol])
                    + (CheckGodori(groupedHwatoos[CardType.Yeol]) ? 5 : 0);
        score += EvaluateDDiScore(groupedHwatoos[CardType.DDi]) 
                    + (CheckCheongDan(groupedHwatoos[CardType.DDi]) ? 3 : 0) 
                    + (CheckHongDan(groupedHwatoos[CardType.DDi]) ? 3 : 0)
                    + (CheckChoDan(groupedHwatoos[CardType.DDi]) ? 3 : 0);

        return score;
    }

    private static int EvaluatePiScore(List<Hwatoo> pis, List<Hwatoo> ssangpis)
    {
        int piCount = pis.Count;
        piCount += ssangpis.Count * 2;

        return Mathf.Max(0, piCount - 9);
    }

    private static int EvaluateGwangScore(List<Hwatoo> gwangs)
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

    private static int EvaluateYeolScore(List<Hwatoo> yeols)
    {
        return Mathf.Max(0, yeols.Count - 4);
    }

    public static bool CheckGodori(List<Hwatoo> yeols)
    {
        if (!yeols.Any(yeol => yeol.hwatooData.month == 2)) { return false; }
        if (!yeols.Any(yeol => yeol.hwatooData.month == 4)) { return false; }
        if (!yeols.Any(yeol => yeol.hwatooData.month == 8)) { return false; }
        return true;
    }

    private static int EvaluateDDiScore(List<Hwatoo> ddis)
    {
        return Mathf.Max(0, ddis.Count - 4);
    }

    public static bool CheckCheongDan(List<Hwatoo> ddis)
    {
        if (ddis.Count(ddi => ddi.hwatooData.danType == DanType.Cheongdan) >= 3)
        {
            return true;
        }
        return false;
    }

    public static bool CheckHongDan(List<Hwatoo> ddis)
    {
        if (ddis.Count(ddi => ddi.hwatooData.danType == DanType.Hongdan) >= 3)
        {
            return true;
        }
        return false;
    }

    public static bool CheckChoDan(List<Hwatoo> ddis)
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

    public static int Cal(IEnumerable<Hwatoo> hwatoos)
    {
        return Calculate(hwatoos);
    }
}
