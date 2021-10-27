using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine;

public static class SaveLoadTimer
{
    public static List<SaveObject.TimerData> savedTimers = new List<SaveObject.TimerData>();

    public static void Save()
    {
        savedTimers = SaveObject.SaveTimers();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create (Application.persistentDataPath + "/savedTimers.st");
        bf.Serialize(file, savedTimers);
        file.Close();
    }

    public static void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            SaveLoadTimer.savedTimers = (List<SaveObject.TimerData>)bf.Deserialize(file);
            file.Close();
        }
    }
    
}

[System.Serializable] 
public static class SaveObject
{
    private static List<TimerData> allTimers = new List<TimerData>();

    public static List<TimerData> SaveTimers()
    {
        TimerManager tm = GameObject.FindObjectOfType<TimerManager>();
        for (var i = 0; i < tm.GetNumberTimers(); i++)
        {
            var timer = tm.GetTimer(i);
            TimerData td = new TimerData(timer);
            allTimers.Add(td);
        }
        Debug.Log("Файл сохранён в " + Application.persistentDataPath);
        return allTimers;
    }

    [System.Serializable] 
    public struct TimerData
    {
        float timerTime;
        string action;
        bool isActived;
        public string name;

        public TimerData(float t, string a, bool isA, string n)
        {
            timerTime = t;
            action = a;
            isActived = isA;
            name = n;
        }

        public TimerData(Timer timer)
        {
            timerTime = timer.GetTimeTimer;
            action = timer.GetAction;
            isActived = timer.GetActive;
            name = timer.GetName;
        }
        
    }
}