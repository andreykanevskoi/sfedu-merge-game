using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeStatisticCollector {
    public Dictionary<Placeable, int> mergeCounter { get; private set; }

    public MergeStatisticCollector() {
        mergeCounter = new Dictionary<Placeable, int>();
        GameEvents.current.OnPlaceableMerge += Update;
    }

    public void Update(Placeable placeable) {
        Debug.Log("MergeStatisticCollector Updated");
        if (mergeCounter.ContainsKey(placeable)) {
            mergeCounter[placeable] += 1;
            Debug.Log(mergeCounter.Count);
            return;
        }
        mergeCounter.Add(placeable, 1);
        Debug.Log(mergeCounter.Count);
    }
}
