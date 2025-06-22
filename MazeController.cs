using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Cinemachine;

public class MazeController : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public Text winText;
    public Button restartButton;
    public Button nextLevelButton;

    public GameObject playerPrefab;
    private GameObject currentPlayer;
    public CinemachineVirtualCamera virtualCamera;

    public InputAction restartAction;

    public float timeLimit = 60f;
    private float timer;
    public Text timerText;
    public Text totalTime;
    public Text levelCount;

    private int currentLevel = 1;
    private bool hasWon = false;

    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
        nextLevelButton.onClick.AddListener(ProceedToNextLevel);
        winText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        restartAction.Enable();
        timer = 0f;
        hasWon = false;
        SpawnNewPlayer();
        totalTime.text = $"Max Time: {timeLimit}";
        levelCount.text = $"Level {currentLevel}";
    }

    void Update()
    {
        if (!hasWon)
        {
            timer += Time.deltaTime;
            if (timerText != null)
                timerText.text = $"Time: {timer:F1}s";

            if (timer > timeLimit)
            {
                RestartGame();
                return;
            }

            Vector3 endPosition = new Vector3((mazeGenerator.width - 2) * mazeGenerator.cellSize, 0, (mazeGenerator.height - 2) * mazeGenerator.cellSize);

            if (currentPlayer && Vector3.Distance(currentPlayer.transform.position, endPosition) < 0.5f)
            {
                DisplayWinMessage();
            }
        }

        if (restartAction.triggered)
        {
            RestartGame();
        }

        Cursor.lockState = (winText.gameObject.activeSelf || restartButton.gameObject.activeSelf || nextLevelButton.gameObject.activeSelf)
            ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = Cursor.lockState == CursorLockMode.None;
    }

    void DisplayWinMessage()
    {
        if (hasWon) return;

        hasWon = true;
        winText.text = $"Level {currentLevel} Complete!";
        winText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        nextLevelButton.gameObject.SetActive(true);
    }

    void ProceedToNextLevel()
    {
        currentLevel++;
        mazeGenerator.width = 10 + currentLevel * 2;
        mazeGenerator.height = 10 + currentLevel * 2;
        timeLimit = timeLimit + Mathf.Log10(currentLevel + 1) * 5;
        totalTime.text = $"Max Time: {timeLimit}";
        levelCount.text = $"Level {currentLevel}";

        hasWon = false;
        RestartGame();
    }

    IEnumerator NextLevelDelay()
    {
        yield return new WaitForSeconds(2f);
        RestartGame();
    }

    void RestartGame()
    {
        winText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        timer = 0f;
        hasWon = false;

        mazeGenerator.RestartMaze();
        StartCoroutine(RespawnPlayer());
    }

    IEnumerator RespawnPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        yield return null; // Wait 1 frame to avoid issues

        SpawnNewPlayer();
    }

    void SpawnNewPlayer()
    {
        Vector3 spawnPos = mazeGenerator.GetStartWorldPosition();
        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        // Find the camera root or armature under the player
        Transform followTarget = currentPlayer.transform.Find("PlayerCameraRoot"); // or "PlayerCameraRoot"

        if (followTarget != null && virtualCamera != null)
        {
            virtualCamera.Follow = followTarget;
        }
        else
        {
            Debug.LogWarning("Follow target or virtual camera not assigned/found.");
        }
    }

    void OnDisable()
    {
        restartAction.Disable();
    }
}
