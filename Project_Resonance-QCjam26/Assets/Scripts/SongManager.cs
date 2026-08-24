using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SongManager : MonoBehaviour
{
    public static SongManager _Instance;

    public List<AudioSource> sources = new List<AudioSource>();
    public List<Song> songs = new List<Song>();
    public int currentSong = 0;
    public int noteInx = 0;

    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        songs.Add(SongReader.LoadData());
        SampleTicker._Instance.OnSixteenthNote += UpdateNoteInx;
    }

    private void OnDisable()
    {
        SampleTicker._Instance.OnSixteenthNote -= UpdateNoteInx;
    }

    public int GetCurrentNote() => GetNote(noteInx);

    public int GetNote(int inx)
    {
        ref List<int> currNotes = ref songs[currentSong].notes;
        if (inx >= currNotes.Count) 
            return -1;
        else 
            return currNotes[inx];
    }
    public bool CheckNote(int inx) => GetNote(noteInx) == inx;

    public void UpdateNoteInx()
    {
        int noteCount = SampleTicker._Instance.sixteenthNoteCount;
        while (GetCurrentNote() <= noteCount)
        {
            noteInx++;
        }
    }
}

//[System.Serializable]
//public class Note
//{
//    public int pos;

//    public Note(int pos = 0)
//    {
//        this.pos = pos;
//    }
//}

[System.Serializable]
public class Song
{
    public string name;
    public int tempo;
    //public List<Note> notes = new();
    public List<int> notes = new();

    public Song (string name, int tempo, List<int> notes)
    {
        this.name = name;
        this.tempo = tempo;
        this.notes = notes;
    }
}

public static class SongReader
{
    public static Song LoadData(string filePath = null)
    {
        // Define the file path (saves inside your project's Assets folder)
        filePath ??= Path.Combine(Application.dataPath, "Audio/Music/noteslist_girl-from-iponema.json");

        // Verify that the file actually exists before reading
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