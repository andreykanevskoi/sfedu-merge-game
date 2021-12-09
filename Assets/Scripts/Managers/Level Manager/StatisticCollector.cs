using System;
using System.Collections.Generic;
using UnityEngine;

public class StatisticCollector {
    public Dictionary<Sprite, int> statistic { get; private set; }
    private Func<UnityEngine.Object, Sprite> _getSprite;

    public StatisticCollector(Func<UnityEngine.Object, Sprite> getSprite) {
        statistic = new Dictionary<Sprite, int>();
        _getSprite = getSprite;
    }

    public void Update(UnityEngine.Object statisticObject) {
        Sprite sprite = _getSprite.Invoke(statisticObject);

        if (statistic.ContainsKey(sprite)) {
            statistic[sprite] += 1;
            return;
        }

        statistic.Add(sprite, 1);
    }

    public int CountElement() {
        int totalElements = 0;

        foreach(int count in statistic.Values) {
            totalElements += count;
        }

        return totalElements;
    }
}
