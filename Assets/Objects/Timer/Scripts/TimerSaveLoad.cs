using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;

public static class SaveLoadTimer
{
    private const string saveNameFile = "/savedTimers.st";
    public static List<SaveObject.TimerData> savedTimers = new List<SaveObject.TimerData>();

    public static void Save()
    {
        savedTimers = SaveObject.SaveTimers();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create (Application.persistentDataPath + saveNameFile);
        bf.Serialize(file, savedTimers);
        file.Close();
    }

    public static void Load()
    {
        if(File.Exists(Application.persistentDataPath + saveNameFile)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + saveNameFile, FileMode.Open);
            savedTimers = (List<SaveObject.TimerData>)bf.Deserialize(file);
            file.Close();
        }
    }
}