using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider accelerationSlider;

    private Player player;

    public void AssignPlayer(Player assignedPlayer)
    {
        player = assignedPlayer;

        healthSlider.maxValue = player.GetMaxHealth();
        healthSlider.value = player.GetHealth();

        accelerationSlider.minValue = 0f;
        accelerationSlider.maxValue = 1f;
        accelerationSlider.interactable = false;

        player.OnSpeedChanged += UpdateSpeed;

        UpdateSpeed(player.GetCurrentSpeed());

        //Debug.Log($"UI assigned to {player.name}");
    }

    private void Update()
    {
        if (player == null)
            return;

        healthSlider.value = player.GetHealth();
    }

    private void UpdateSpeed(float speed)
    {
        float sliderValue = Mathf.InverseLerp(
            0f,
            player.MoveSpeed,
            speed
        );

        accelerationSlider.SetValueWithoutNotify(sliderValue);

        //Debug.Log($"Speed UI: {speed} -> {sliderValue}");
    }

    private void OnDestroy()
    {
        if (player == null)
            return;

        player.OnSpeedChanged -= UpdateSpeed;
    }
}

// ScriptRole: Displays player health and movement speed.
// RelatedScripts: Player, Entity
// UsesSO: None
// ReceivesFrom: PlayerInputManager, Player
// SendsTo: None