using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Save
{
    public const int AmountOfCars = 6;
    public const int AmountOfColors = 8;
    public const int AmountOfAccesories = 3;
    public const int AmountOfTracks = 3;

    public static StarTimes[] AllStarTimes = new StarTimes[AmountOfTracks] { new StarTimes(new float[3] { 60, 48, 38 }), new StarTimes(new float[3] { 360, 240, 180 }), new StarTimes(new float[3] { 120, 80, 60 })};

    private static Dictionary<int, float> cachedTrackTimes = new Dictionary<int, float>();
    private static Dictionary<int, bool[]> cachedCarColors = new Dictionary<int, bool[]>();
    private static Dictionary<int, bool[]> cachedCarAccesories = new Dictionary<int, bool[]>();

    #region Track Times
    public static bool SaveTrackTime(int trackIndex, float time)
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

            return true;
        }
        else
        {
            return false;
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
    public static void SetUnlockedCars(int index, bool unlockIt = true)
    {
        var cars = GetUnlockedCars();
        if (!cars[index] || !unlockIt)
        {
            cars[index] = unlockIt;

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

        if (cachedCarColors.ContainsKey(carIndex))
        {
            cachedCarColors[carIndex] = colors;
        }
        else
        {
            cachedCarColors.Add(carIndex, colors);
        }
    }

    public static bool[] GetUnlockedColors(int carIndex)
    {
        if (cachedCarColors != null && cachedCarColors.ContainsKey(carIndex))
        {
            return cachedCarColors[carIndex];
        }

        if (File.Exists(string.Format("{0}/UnlockedColors_{1}", Application.persistentDataPath, carIndex)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(string.Format("{0}/UnlockedColors_{1}", Application.persistentDataPath, carIndex), FileMode.Open);
            bool[] colors = (bool[])bf.Deserialize(openedFile);
            openedFile.Close();

            cachedCarColors.Add(carIndex, colors);

            return colors;
        }
        else
        {
            return new bool[AmountOfColors];
        }
    }

    #endregion

    #region Unlocked Car Accesories

    public static void SetUnlockedAccesories(int carIndex, bool[] accesories)
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream createdFile = File.Create(string.Format("{0}/UnlockedAccesories_{1}", Application.persistentDataPath, carIndex));

        bf.Serialize(createdFile, accesories);

        createdFile.Close();

        if (cachedCarAccesories.ContainsKey(carIndex))
        {
            cachedCarAccesories[carIndex] = accesories;
        }
        else
        {
            cachedCarAccesories.Add(carIndex, accesories);
        }
    }

    public static bool[] GetUnlockedAccesories(int carIndex)
    {
        if (cachedCarAccesories != null && cachedCarAccesories.ContainsKey(carIndex))
        {
            return cachedCarAccesories[carIndex];
        }

        if (File.Exists(string.Format("{0}/UnlockedAccesories_{1}", Application.persistentDataPath, carIndex)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(string.Format("{0}/UnlockedAccesories_{1}", Application.persistentDataPath, carIndex), FileMode.Open);
            bool[] accesories = (bool[])bf.Deserialize(openedFile);
            openedFile.Close();

            cachedCarAccesories.Add(carIndex, accesories);

            return accesories;
        }
        else
        {
            return new bool[AmountOfAccesories];
        }
    }

    #endregion

    #region Orbs

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

    #region Stickers

    public static void AddSticker(int index)
    {
        List<int> stickers = GetStickers().ToList();
        stickers.Add(index);

        BinaryFormatter bf = new BinaryFormatter();

        FileStream createdFile = File.Create(string.Format("{0}/Stickers", Application.persistentDataPath));

        bf.Serialize(createdFile, stickers.ToArray());

        createdFile.Close();
    }

    public static int[] GetStickers()
    {
        if (File.Exists(string.Format("{0}/Stickers", Application.persistentDataPath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(string.Format("{0}/Stickers", Application.persistentDataPath), FileMode.Open);
            int[] stickers = (int[])bf.Deserialize(openedFile);
            openedFile.Close();

            return stickers;
        }
        else
        {
            return new int[0];
        }
    }

    #endregion

    #region Completed Tracks

    public static void CompleteTrack(int trackIndex)
    {
        List<bool> completedTracks = GetCompletedTracks();

        completedTracks[trackIndex] = true;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream createdFile = File.Create(string.Format("Tracks.Completed", Application.persistentDataPath));
        bf.Serialize(createdFile, completedTracks);

        createdFile.Close();
    }

    public static List<bool> GetCompletedTracks()
    {
        if (File.Exists(string.Format("Tracks.Completed", Application.persistentDataPath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream openedFile = File.Open(string.Format("Tracks.Completed", Application.persistentDataPath), FileMode.Open);
            List<bool> completedTracks = (List<bool>)bf.Deserialize(openedFile);
            openedFile.Close();

            return completedTracks;
        }
        else
        {
            var list = new List<bool>(AmountOfTracks) { false, false, false};
            return list;
        }
    }

    #endregion
}
