using UnityEngine;

public class BeatNoteSpawner : MonoBehaviour
{
    [Header("Note")]
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private RectTransform spawnPoint;
    [SerializeField] private RectTransform inputPoint;
    [SerializeField] private RectTransform noteParent;

    [SerializeField] private int lookAhead = 4;

    [Header("Timing")]
    [Tooltip("How many 16th notes make up one spawn beat. 4 = spawn once per quarter note.")]
    [SerializeField] private int sixteenthsPerSpawnBeat = 4;

    public int BeatsToInput = 4; // beats of travel time from spawn to input zone

    private FMODBeatTicker ticker;
    private int sixteenthCounter;
    private Coroutine bindRoutine;


    private void OnEnable()
    {
        bindRoutine = StartCoroutine(BindToTicker());
    }

    private System.Collections.IEnumerator BindToTicker()
    {
        while (FMODBeatTicker._Instance == null)
        {
            yield return null;
        }

        ticker = FMODBeatTicker._Instance;
        ticker.OnSixteenthNote += HandleSixteenthNote;
    }

    private void OnDisable()
    {
        if (bindRoutine != null)
        {
            StopCoroutine(bindRoutine);
            bindRoutine = null;
        }

        if (ticker != null)
        {
            ticker.OnSixteenthNote -= HandleSixteenthNote;
            ticker = null;
        }
    }

    private void HandleSixteenthNote() //increments sixteenth note and spawns. consistent but not tied to mapping.
    {
        //sixteenthCounter++; 

        //if (sixteenthCounter >= sixteenthsPerSpawnBeat)
        //{
        //    sixteenthCounter = 0;
        //    SpawnNote();  SongManager._Instance.CheckNote(position+lookAhead))
        //}

        int position = ticker.sixteenthNoteCount;
        if (SongManager._Instance.CheckNote(position+lookAhead))
        {
            SpawnNote();
        }

    }

    private void SpawnNote()
    {
        if (notePrefab == null ||
            spawnPoint == null ||
            inputPoint == null ||
            noteParent == null ||
            ticker == null)
        {
            Debug.LogError("BeatNoteSpawner is missing a reference.");
            return;
        }

        Vector3 spawnWorldPosition = spawnPoint.position;
        Vector3 inputWorldPosition = inputPoint.position;

        // Distance between spawn and input
        float distance = Vector3.Distance(spawnWorldPosition, inputWorldPosition);

        // bpm from ticker
        double secondsPerSixteenth = (60.0 / ticker.Tempo) / 4.0;
        double secondsPerSpawnBeat = secondsPerSixteenth * sixteenthsPerSpawnBeat; //for spawning consistently like in soundfall

        // Total travel time = exactly BeatsToInput spawn-beats
        double travelTime = secondsPerSpawnBeat * BeatsToInput;

        // Speed needed to cover the distance in exactly that time
        float noteSpeed = (float)(distance / travelTime);

        // Direction from spawn point to input point
        Vector3 direction = (inputWorldPosition - spawnWorldPosition).normalized;

        // Spawn the note under the Canvas
        GameObject noteObject = Instantiate(notePrefab, noteParent);

        RectTransform noteRect = noteObject.GetComponent<RectTransform>();

        if (noteRect == null)
        {
            Debug.LogError("Note prefab needs a RectTransform.");
            Destroy(noteObject);
            return;
        }

        NoteAutoScroller note = noteObject.GetComponent<NoteAutoScroller>();

        if (note == null)
        {
            Debug.LogError("Note prefab needs a NoteAutoScroller component.");
            Destroy(noteObject); // catch
            return;
        }

 
        note.Initialize(spawnWorldPosition, direction, noteSpeed); //gives note position, direction, and speed
    }
}