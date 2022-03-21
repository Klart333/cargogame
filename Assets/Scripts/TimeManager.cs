
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
    public static float GetTimeSinceLastSpawn(int trackIndex, int orbIndex)
    {
        var times = GetOrbTimes(trackIndex);

        if (!times.ContainsKey(orbIndex))
        {
            return 10000;
        }

        var diff = DateTime.UtcNow - times[orbIndex];
        return (float)diff.TotalHours;
    }

    public static void StoreOrbTime(int trackIndex, int orbIndex)
    {
        var timeNow = DateTime.UtcNow;

        Dictionary<int, DateTime> times = GetOrbTimes(trackIndex);
        if (times.ContainsKey(orbIndex))
        {
            times[orbIndex] = timeNow;
        }
        else
        {
            times.Add(orbIndex, timeNow);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream createdFile = File.Create(string.Format("{0}/OrbTimes.{1}", Application.persistentDataPath, trackIndex));
        bf.Serialize(createdFile, times);

        createdFile.Close();
    }

    public static Dictionary<int, DateTime> GetOrbTimes(int trackIndex)
    {
        if (File.Exists(string.Format("{0}/OrbTimes.{1}", Application.persistentDataPath, trackIndex)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(string.Format("{0}/OrbTimes.{1}", Application.persistentDataPath, trackIndex), FileMode.Open);
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
