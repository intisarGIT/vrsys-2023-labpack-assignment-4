using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreDisplay : NetworkBehaviour
{
    [SerializeField] private TMP_Text scoreText; // Drag the TextMeshPro UI component here in the Inspector

    private PlayerScore playerScore;

    private void Start()
    {
        // Find the PlayerScore component on the player GameObject
        playerScore = GetComponent<PlayerScore>();

        // If this isn't the local player, disable the script as we don't want to show other players' scores
        if (!IsLocalPlayer)
        {
            enabled = false;
            return;
        }

        // Set initial score text
        scoreText.text = "Score: 0";
    }

    private void Update()
    {
        // Update the score text with the current score
        if (playerScore != null)
        {
            scoreText.text = "Score: " + playerScore.Score.Value.ToString();
        }
    }
}
