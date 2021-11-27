using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStatisticCollector {
    public Dictionary<FieldTile, int> tileeCounter { get; private set; }

    public TileStatisticCollector() {
        tileeCounter = new Dictionary<FieldTile, int>();
        GameEvents.current.OnTileDestroy += Update;
    }

    public void Update(FieldTile tile) {
        if (tileeCounter.ContainsKey(tile)) {
            tileeCounter[tile] += 1;
            return;
        }
        tileeCounter.Add(tile, 1);
    }
}
