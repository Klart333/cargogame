using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Save
{
    public const int AmountOfCars = 6;
    public const int AmountOfColors = 8;

    public static StarTimes[] AllStarTimes = new StarTimes[3] { new StarTimes(new float[3] { 80, 60, 50 }), new StarTimes(new float[3] { 360, 240, 180 }), new StarTimes(new float[3] { 120, 80, 60 })};

    private static Dictionary<int, float> cachedTrackTimes = new Dictionary<int, float>();

    #region Track Times
    public static void SaveTrackTime(int trackIndex, float time)
    {
        float previousTime = GetTrackTime(trackIndex);
        if (time < previousTime || previousTime == -1)
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream createdFile = File.Create(GetTrackFileName(trackIndex));

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

        if (File.Exists(GetTrackFileName(trackIndex)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(GetTrackFileName(trackIndex), FileMode.Open);
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

    private static string GetTrackFileName(int trackIndex)
    {
        return string.Format("{0}/Track{1}.Time", Application.persistentDataPath, trackIndex);
    }
    #endregion

    #region Unlocked Cars 
    public static void SetUnlockedCars(int index)
    {
        var cars = GetUnlockedCars();
        if (!cars[index])
        {
            cars[index] = true;

            BinaryFormatter bf = new BinaryFormatter();

            FileStream createdFile = File.Create(string.Format("{0}/UnlockedCars", Application.persistentDataPath));

            bf.Serialize(createdFile, cars);

            createdFile.Close();
        }
    }

    public static bool[] GetUnlockedCars()
    {
        if (File.Exists(string.Format("{0}/UnlockedCars", Application.persistentDataPath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(string.Format("{0}/UnlockedCars", Application.persistentDataPath), FileMode.Open);
            bool[] cars = (bool[])bf.Deserialize(openedFile);
            openedFile.Close();

            return cars;
        }
        else
        {
            return new bool[AmountOfCars];
        }
    }
    #endregion

    #region Unlocked Car Colors

    public static void SetUnlockedColors(int carIndex, bool[] colors)
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream createdFile = File.Create(string.Format("{0}/UnlockedColors_{1}", Application.persistentDataPath, carIndex));

        bf.Serialize(createdFile, colors);

        createdFile.Close();
    }

    public static bool[] GetUnlockedColors(int carIndex)
    {
        if (File.Exists(string.Format("{0}/UnlockedColors_{1}", Application.persistentDataPath, carIndex)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(string.Format("{0}/UnlockedColors_{1}", Application.persistentDataPath, carIndex), FileMode.Open);
            bool[] colors = (bool[])bf.Deserialize(openedFile);
            openedFile.Close();

            return colors;
        }
        else
        {
            return new bool[AmountOfColors];
        }
    }

    #endregion

    #region Loot

    public static void SaveOrb(Rarity rarity)
    {
        List<Rarity> orbs = GetOrbs();
        if (orbs != null)
        {
            orbs.Add(rarity);
        }
        else
        {
            orbs = new List<Rarity>();
            orbs.Add(rarity);
        }

        BinaryFormatter bf = new BinaryFormatter();

        FileStream createdFile = File.Create(string.Format("{0}/LootOrbs", Application.persistentDataPath));

        bf.Serialize(createdFile, orbs);

        createdFile.Close();
    }

    public static void RemoveOrb(Rarity rarity)
    {
        var orbs = GetOrbs();

        if (orbs != null && orbs.Count != 0)
        {
            if (orbs.Contains(rarity))
            {
                orbs.Remove(rarity);

                BinaryFormatter bf = new BinaryFormatter();

                FileStream createdFile = File.Create(string.Format("{0}/LootOrbs", Application.persistentDataPath));

                bf.Serialize(createdFile, orbs);

                createdFile.Close();
            }
            else
            {
                Debug.LogError("Can't remove what's not there");
            }
        }
        else
        {
            Debug.LogError("There are not saved orbs to remove");
        }
    }

    public static List<Rarity> GetOrbs()
    {
        if (File.Exists(string.Format("{0}/LootOrbs", Application.persistentDataPath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(string.Format("{0}/LootOrbs", Application.persistentDataPath), FileMode.Open);
            List<Rarity> orbs = (List<Rarity>)bf.Deserialize(openedFile);
            openedFile.Close();

            return orbs;
        }
        else
        {
            return null;
        }
    }

    #endregion
}
