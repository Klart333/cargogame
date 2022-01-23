using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Save
{
    private static Dictionary<int, float> cachedTrackTimes = new Dictionary<int, float>();

    public static void SaveTrackTime(int trackIndex, float time)
    {
        float previousTime = GetTrackTime(trackIndex);
        if (time < previousTime || previousTime == -1)
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream createdFile = File.Create(GetFileName(trackIndex));

            bf.Serialize(createdFile, time);

            createdFile.Close();

            if (cachedTrackTimes.ContainsKey(trackIndex))
            {
                cachedTrackTimes[trackIndex] = time;
            }
            else
            {
                cachedTrackTimes.Add(trackIndex, time);
            }
        }
    }

    public static float GetTrackTime(int trackIndex)
    {
        if (cachedTrackTimes.ContainsKey(trackIndex))
        {
            return cachedTrackTimes[trackIndex];
        }

        if (File.Exists(GetFileName(trackIndex)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(GetFileName(trackIndex), FileMode.Open);
            float time = (float)bf.Deserialize(openedFile);
            openedFile.Close();

            if (cachedTrackTimes.ContainsKey(trackIndex))
            {
                cachedTrackTimes[trackIndex] = time;
            }
            else
            {
                cachedTrackTimes.Add(trackIndex, time);
            }

            return time;
        }
        else
        {
            return -1;
        }
    }

    private static string GetFileName(int trackIndex)
    {
        return string.Format("{0}/Track{1}.Time", Application.persistentDataPath, trackIndex);
    }
}
