using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;

public static class SaveLoadTimer
{
    public static SaveObject So { get; private set; }
    private const string SaveNameFile = "/savedTimers.st";

    public static void Save()
    {
        So = new SaveObject();
        So.SaveData();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create (Application.persistentDataPath + SaveNameFile);
        bf.Serialize(file, So);
        file.Close();
        Debug.Log("Файл сохранён в " + Application.persistentDataPath);
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