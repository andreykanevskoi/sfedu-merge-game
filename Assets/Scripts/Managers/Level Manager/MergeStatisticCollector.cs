using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeStatisticCollector {
    public Dictionary<Sprite, int> mergeCounter { get; private set; }

    public MergeStatisticCollector() {
        mergeCounter = new Dictionary<Sprite, int>();
        GameEvents.current.OnPlaceableMerge += Update;
    }

    public void Update(Placeable placeable) {
        Debug.Log("MergeStatisticCollector Updated");
        Sprite sprite = placeable.GetComponent<SpriteRenderer>().sprite;
        if (mergeCounter.ContainsKey(sprite)) {
            mergeCounter[sprite] += 1;
            Debug.Log(mergeCounter.Count);
            return;
        }
        mergeCounter.Add(sprite, 1);
        Debug.Log(mergeCounter.Count);
    }
}
