using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GlobalHighscores : Singleton<GlobalHighscores>
{
    const string privateCode = "nh5bSdHSV02t_sQKY98MKwHFPF3lsNyU6zVTaGyu2Dbw";
    const string publicCode = "621bfafb8f40bb12584a2d92";
    const string webURL = "http://dreamlo.com/lb/";

    public Highscore[] Highscores { get; private set; }

    public void AddNewHighscore(string trackIndex, float score)
    {
        StartCoroutine(UploadNewHighscore(trackIndex, score));
    }

    private IEnumerator UploadNewHighscore(string trackIndex, float score)
    {
        int intScore = 1000000 - Mathf.RoundToInt(score * 1000.0f);
        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(trackIndex) + "/" + intScore);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Upload Succesful");
        }
        else
        {
            print("Error uploading " + www.error);
        }
    }

    public void DownloadHighscores()
    {
        StartCoroutine(DownloadHighscoresFromDatabase());
    }

    public IEnumerator DownloadHighscores(Action<Highscore[]> method)
    {
        yield return DownloadHighscoresFromDatabase();
        method(Highscores);
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

        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            username = username.Replace("+", " ");
            float score = int.Parse(entryInfo[1]);
            score = Mathf.Abs(score - 1000000);
            score /= 1000.0f;

            Highscores[i] = new Highscore(username, score);
        }
    }
}

public struct Highscore
{
    public string TrackIndex;
    public float Score;

    public Highscore(string trackIndex, float score)
    {
        this.TrackIndex = trackIndex;
        this.Score = score;
    }
}
