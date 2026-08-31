using UnityEngine;
using UnityEngine.InputSystem;

public class NoteAutoScroller : MonoBehaviour
{
    public enum NoteState
    {
        HitNote,
        UnhitNote
    }

    [Header("Note Visuals")]
    [SerializeField] private GameObject hitNotePrefab;
    [SerializeField] private GameObject unhitNotePrefab;

    [Header("Note State")]
    [SerializeField] private NoteState noteState = NoteState.UnhitNote;

    [Header("Cleanup")]
    [SerializeField] private float destroyAfterSeconds = 5f;

    private RectTransform rectTransform;

    private Vector3 direction;
    private float speed;
    private double lastDspTime;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        UpdateVisual();
    }

    private void Start()
    {
        // Cleanup timer only.
        DestroyNote();
    }

    public void DestroyNote()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    public void Initialize(
        Vector3 spawnPosition,
        Vector3 moveDirection,
        float moveSpeed)
    {
        rectTransform.position = spawnPosition;
        direction = moveDirection.normalized;
        speed = moveSpeed;

        // Start the note on the same clock as the ticker/audio.
        lastDspTime = FMODBeatTicker._Instance.GetTimelineSeconds();

        // Every newly initialized note starts unhit.
        SetNoteState(NoteState.UnhitNote);
    }

    private void Update()
    {
        // TEST INPUT
        if (Keyboard.current != null &&
            Keyboard.current.yKey.wasPressedThisFrame)
        {
            SetNoteState(NoteState.HitNote);
        }

        // DSP-time movement
        double now = FMODBeatTicker._Instance.GetTimelineSeconds();
        float dt = (float)(now - lastDspTime);
        lastDspTime = now;

        rectTransform.position += direction * speed * dt;
    }

    public void SetNoteState(NoteState newState)
    {
        noteState = newState;

        UpdateVisual();

        Debug.Log(
            $"{name}: Note state changed to {noteState}."
        );
    }

    private void UpdateVisual()
    {
        if (hitNotePrefab == null || unhitNotePrefab == null)
        {
            Debug.LogWarning(
                $"{name}: Note visual prefabs are not assigned."
            );

            return;
        }

        // Remove the previous visual.
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GameObject visualPrefab = noteState == NoteState.HitNote
            ? hitNotePrefab
            : unhitNotePrefab;

        Instantiate(
            visualPrefab,
            transform.position,
            transform.rotation,
            transform
        );

        Debug.Log(
            $"{name}: Visual changed to {visualPrefab.name}."
        );
    }
}

