using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [Header("Player UI")]
    [SerializeField] private GameObject[] playerCanvasPrefabs;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private int maxPlayers = 2;

    private int playersJoined = 0;

    private void Update()
    {
        if (playersJoined >= maxPlayers)
            return;

        JoinKeyboardPlayers();
        JoinGamepadPlayers();
    }

    private void JoinKeyboardPlayers()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnPlayer("WASD", Keyboard.current);
        }
        else if (Keyboard.current.rightCtrlKey.wasPressedThisFrame)
        {
            SpawnPlayer("Arrows", Keyboard.current);
        }
    }

    private void JoinGamepadPlayers()
    {
        foreach (Gamepad gamepad in Gamepad.all)
        {
            if (gamepad.startButton.wasPressedThisFrame)
            {
                SpawnPlayer("Gamepad", gamepad);
                break;
            }
        }
    }

    private void SpawnPlayer(string controlScheme, InputDevice device)
    {
        if (playersJoined >= maxPlayers)
            return;

        int playerIndex = playersJoined;

        PlayerInput playerInput = PlayerInput.Instantiate(
            playerPrefab,
            controlScheme: controlScheme,
            pairWithDevice: device
        );

        Player player = playerInput.GetComponent<Player>();

        if (spawnPoints.Length > playerIndex)
        {
            player.transform.position = spawnPoints[playerIndex].position;
        }

        SpawnCanvas(playerIndex, player);

        playersJoined++;

        Debug.Log($"Player {playersJoined} joined using {device.name}");
    }

    private void SpawnCanvas(int playerIndex, Player player)
    {
        if (playerIndex >= playerCanvasPrefabs.Length)
        {
            Debug.LogWarning("Missing player canvas prefab.");
            return;
        }

        GameObject canvas = Instantiate(playerCanvasPrefabs[playerIndex]);

        PlayerUI playerUI = canvas.GetComponent<PlayerUI>();

        if (playerUI != null)
        {
            playerUI.AssignPlayer(player);
        }
        else
        {
            Debug.LogWarning("No PlayerUI component found on canvas.");
        }

        Debug.Log($"Spawned Canvas for Player {playerIndex + 1}");
    }
}
