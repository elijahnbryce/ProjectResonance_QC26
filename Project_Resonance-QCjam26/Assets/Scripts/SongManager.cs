using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SongManager : MonoBehaviour
{
    public static SongManager _Instance;

    public List<Song> songs = new List<Song>();
    public int currentSong = 0;
    private Song songPlaying;
    public Song SongPlaying { 
        get => songPlaying;
        set
        {
            if (songPlaying != value)
            {
                notePositions = value != null
                    ? new HashSet<int>(value.notes)
                    : new HashSet<int>();
                songPlaying = value;
            }
        }
    }

    private HashSet<int> notePositions = new HashSet<int>();

    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        var song = SongReader.LoadData();
        songs.Add(song);
        SetSong(0);
    }

    public bool CheckNote(int index) => notePositions.Contains(index);
    public void SetSong(int inx)
    {
        currentSong = inx;
        SongPlaying = songs[currentSong];
    }
}

/*
 * End of SongManager
 */


[System.Serializable]
public class Song
{
    public string name;
    public int tempo;
    public List<int> notes = new();

    public Song(string name, int tempo, List<int> notes)
    {
        this.name = name;
        this.tempo = tempo;
        this.notes = notes;
    }
}

[System.Serializable]
public class Note
{
    public int pos;

    public Note(int pos = 0)
    {
        this.pos = pos;
    }
}

public static class SongReader
{
    public static Song LoadData(string filePath = null)
    {
        filePath ??= Path.Combine(Application.dataPath, "Audio/Music/noteslist_girl-from-iponema.json");

        if (File.Exists(filePath))
        {
            // Read the entire JSON file into a string variable
            string jsonText = File.ReadAllText(filePath);

            // Convert the JSON string into the C# object data
            Song data = JsonUtility.FromJson<Song>(jsonText);
            return data;
        }
        else
        {
            Debug.LogError($"Cannot find JSON file at: {filePath}");
            return null;
        }
    }
}