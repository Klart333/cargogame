public struct Highscore
{
    public string RandomString;
    public int TrackIndex;
    public float Score;
    public string Name;

    public Highscore(string randomString, int trackIndex, float score, string name)
    {
        RandomString = randomString;
        TrackIndex = trackIndex;
        Score = score;
        Name = name;
    }
}
