using UnityEngine;
using CardToolTips;
using KarmaLogic.Cards;
using System.Collections.Generic;
using System.IO;

public class CardTipTextManager : Singleton<CardTipTextManager>
{
    CardToolTipText[] _cardToolTipTexts;

    void Start()
    {
        string cardToolTipsAll = File.ReadAllText(Application.streamingAssetsPath + "/Text/CardToolTips.csv");

        string[] lines = cardToolTipsAll.Split('\n');

        List<CardToolTipText> tipTexts = new ();

        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Length == 0) { continue; }

            string[] row = lines[i].Split(',');

            tipTexts.Add(new CardToolTipText(row[0], row[1], row[2], row[3]));
        }

        _cardToolTipTexts = tipTexts.ToArray();
    }

    public CardToolTipText GetCardToolTipText(CardValue cardValue)
    {
        return _cardToolTipTexts[(int)cardValue - 2];
    }
}
