using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Storage {
    private string _savePath;

    public Storage() {
        _savePath = Path.Combine(Application.persistentDataPath, "saveFile");
        Debug.Log(_savePath);
    }

    public bool CheckFile() {
        return File.Exists(_savePath);
    }

    public void Save(ISaveable saveable) {
        using (var writer = new BinaryWriter(File.Open(_savePath, FileMode.Create))) {
            saveable.Save(new GameDataWriter(writer));
        }
    }

    public void Load(ISaveable saveable, PlaceableFactory factory) {
        using ( var reader = new BinaryReader(File.Open(_savePath, FileMode.Open))) {
            saveable.Load(new GameDataReader(reader), factory);
        }
    }
}
