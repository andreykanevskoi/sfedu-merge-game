using System;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;

public static class SaveLoadTimer
{
    public static SaveObject So { get; private set; } = new SaveObject();
    private const string SaveNameFile = "/savedTimers.st";

    public static void Save()
    {
        So.SaveData();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create (Application.persistentDataPath + SaveNameFile);
        bf.Serialize(file, So);
        file.Close();
    }

    public static void Load()
    {
        if(File.Exists(Application.persistentDataPath + SaveNameFile)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + SaveNameFile, FileMode.Open);
            So = (SaveObject)bf.Deserialize(file);
            file.Close();
        }
    }
    
    
}