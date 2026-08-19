using UnityEngine;

public class NoteAutoScroller : MonoBehaviour
{
    [SerializeField] private float destroyAfterSeconds = 5f;

    private RectTransform rectTransform;

    private Vector3 direction;
    private float speed;
    private double lastDspTime;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        // Cleanup timer only — not used for gameplay-critical timing,
        // so regular Time-based Destroy is fine here.
        Destroy(gameObject, destroyAfterSeconds);
    }

    public void Initialize(Vector3 spawnPosition, Vector3 moveDirection, float moveSpeed)
    {
        rectTransform.position = spawnPosition;
        direction = moveDirection.normalized;
        speed = moveSpeed;

        // Anchor movement to the same clock the ticker/audio use, not Time.time.
        lastDspTime = AudioSettings.dspTime;
    }

    private void Update()
    {
        double now = AudioSettings.dspTime;
        float dt = (float)(now - lastDspTime);
        lastDspTime = now;

        rectTransform.position += direction * speed * dt;
    }
}