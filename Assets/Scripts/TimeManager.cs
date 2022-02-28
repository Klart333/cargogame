
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class TimeManager
{
    /// <summary>
    /// In Hours
    /// </summary>
    public static float GetTimeSinceLastSpawn(int trackIndex)
    {
        var times = GetOrbTimes();

        if (!times.ContainsKey(trackIndex))
        {
            return 10000;
        }

        var diff = DateTime.UtcNow - times[trackIndex];
        return (float)diff.TotalHours;
    }

    public static void StoreOrbTime(int trackIndex)
    {
        var timeNow = DateTime.UtcNow;

        Dictionary<int, DateTime> times = GetOrbTimes();
        if (times.ContainsKey(trackIndex))
        {
            times[trackIndex] = timeNow;
        }
        else
        {
            times.Add(trackIndex, timeNow);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream createdFile = File.Create(string.Format("OrbTimes", Application.persistentDataPath));
        bf.Serialize(createdFile, times);

        createdFile.Close();
    }

    public static Dictionary<int, DateTime> GetOrbTimes()
    {
        if (File.Exists(string.Format("OrbTimes", Application.persistentDataPath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(string.Format("OrbTimes", Application.persistentDataPath), FileMode.Open);
            Dictionary<int, DateTime> times = (Dictionary<int, DateTime>)bf.Deserialize(openedFile);
            openedFile.Close();

            return times;
        }
        else
        {
            var times = new Dictionary<int, DateTime>();
            return times;
        }
    }
}
