using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable{
    void Save(GameDataWriter writer);
    void Load(GameDataReader reader, PlaceableFactory factory);
}
