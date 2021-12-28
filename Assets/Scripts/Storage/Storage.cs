using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class Storage {
    private string _version = "1v";
    private string _savePath;

    private GameDataReader _gameDataReader;

    public Storage() {
        _savePath = Application.persistentDataPath + "/saveFile_v2";
    }

    public bool CheckFile() {
        return File.Exists(_savePath);
    }

    public void Save(ISaveable saveable) {
        using (var writer = new BinaryWriter(File.Open(_savePath, FileMode.Create))) {
            var gameDataWriter = new GameDataWriter(writer);
            gameDataWriter.Write(_version);
            saveable.Save(gameDataWriter);
        }
    }

    public void Load(ISaveable saveable, PlaceableFactory factory, Action beforeLoad) {
        using ( var reader = new BinaryReader(File.Open(_savePath, FileMode.Open))) {
            var gameDataReader = new GameDataReader(reader);
            if (gameDataReader.ReadString() != _version) {
                return;
            }
            beforeLoad?.Invoke();
            saveable.Load(gameDataReader, factory);
        }
    }
}
