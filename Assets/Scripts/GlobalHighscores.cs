using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GlobalHighscores : Singleton<GlobalHighscores>
{
    public const string glyphs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public const string privateCode = "nh5bSdHSV02t_sQKY98MKwHFPF3lsNyU6zVTaGyu2Dbw";
    public const string publicCode = "621bfafb8f40bb12584a2d92";
    public const string webURL = "http://dreamlo.com/lb/";
    public const int ScoresPerTrack = 5;

    public Highscore[] Highscores { get; private set; }

    private bool gotScores = false;

    private float currentScore = 0;
    private int currentTrackIndex = 0;

    public void AddNewHighscore(float score, int trackIndex)
    {
        currentScore = score;
        currentTrackIndex = trackIndex;
        
        // Gotta first download the scores to compare
        StartCoroutine(DownloadHighscores(GotTheScores));
    }

    private void GotTheScores(Highscore[] scores)
    {
        StartCoroutine(UploadNewHighscore(currentScore, currentTrackIndex));
    }

    private IEnumerator UploadNewHighscore(float score, int trackIndex)
    {
        if (!CheckIfBetter(score, trackIndex))
        {
            yield break;
        }
        
        int intScore = 1000000 - Mathf.RoundToInt(score * 1000.0f);
        
        string name = PlayerPrefs.GetString("Name");
        
        string randomString = GetRandomString();
        if (string.IsNullOrEmpty(name))
        {
            name = "Can't Draw";
        }

        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(randomString) + "/" + intScore + "/" + trackIndex + "/" + WWW.EscapeURL(name));
        yield return www;

        int lowest = RemoveLowest(trackIndex);
        if (lowest == -1)
        {
            gotScores = false;
            DownloadHighscores(null);
        }
        else
        {
            Highscores[lowest] = new Highscore(randomString, trackIndex, score, name);
        }

        if (string.IsNullOrEmpty(www.error)) 
        {
            print("Upload Succesful");
        }
        else
        {
            print("Error uploading " + www.error);
        }
    }

    private int RemoveLowest(int trackIndex)
    {
        float worst = 0;
        int index = 0;
        int amount = 0;

        for (int i = 0; i < Highscores.Length; i++)
        {
            if (Highscores[i].TrackIndex == trackIndex)
            {
                amount++;
                if (Highscores[i].Score > worst)
                {
                    worst = Highscores[i].Score;
                    index = i;
                }
            }
        }

        if (amount < ScoresPerTrack)
        {
            print("Not Enough");
            return -1;
        }

        print("Deleting " + index);
        DeleteScore(Highscores[index].RandomString);
        return index;
    }

    private bool CheckIfBetter(float score, int trackIndex)
    {
        int worse = 0;
        for (int i = 0; i < Highscores.Length; i++)
        {
            if (Highscores[i].TrackIndex == trackIndex)
            {
                if (score >= Highscores[i].Score)
                {
                    worse++;
                }
                else
                {
                    print("Better than one");
                    return true;
                }
            }
        }

        if (worse >= ScoresPerTrack)
        {
            print("Worse than all");
            return false;
        }
        else
        {
            print("Not Enough Scores");
            return true;
        }
    }

    private string GetRandomString()
    {
        string x = "";

        for (int i = 0; i < 5; i++)
        {
            x = string.Concat(x, glyphs[UnityEngine.Random.Range(0, glyphs.Length)]);
        }

        return x;
    }

    public IEnumerator DownloadHighscores(Action<Highscore[]> method)
    {
        if (!gotScores)
        {
            yield return DownloadHighscoresFromDatabase();
        }

        method?.Invoke(Highscores);
    }

    private IEnumerator DownloadHighscoresFromDatabase()
    {
        WWW www = new WWW(webURL + publicCode + "/pipe/");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Downloaded scores");
            FormatHighscores(www.text);
        }
        else
        {
            print("Error Downloading " + www.error);
        }
    }

    private void FormatHighscores(string textStream)
    {
        string[] entries = textStream.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Highscores = new Highscore[entries.Length];
        print(textStream);

        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string randomString = entryInfo[0];

            int trackIndex = int.Parse(entryInfo[2]);
           
            float score = int.Parse(entryInfo[1]);
            score = Mathf.Abs(score - 1000000);
            score /= 1000.0f;

            string username = entryInfo[3];
            username = username.Replace("+", " ");

            Highscores[i] = new Highscore(randomString, trackIndex, score, username);
        }

        gotScores = true;
    }

    public void DeleteScore(string randomString)
    {
        StartCoroutine(DeletingScore(randomString));
    }

    private IEnumerator DeletingScore(string randomString)
    {
        WWW www = new WWW(webURL + privateCode + "/delete/" + WWW.EscapeURL(randomString));
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Deleted " + randomString);
        }
        else 
        {
            print("Error Deleting " + www.error);
        }
    }
}
