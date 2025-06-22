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

    public GameObject playerPrefab;
    private GameObject currentPlayer;
    public CinemachineVirtualCamera virtualCamera;

    public InputAction restartAction;

    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
        winText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        restartAction.Enable();

        // Initial player spawn
        SpawnNewPlayer();
    }

    void Update()
    {
        Vector3 endPosition = new Vector3((mazeGenerator.width - 2) * mazeGenerator.cellSize, 0, (mazeGenerator.height - 2) * mazeGenerator.cellSize);

        if (currentPlayer && Vector3.Distance(currentPlayer.transform.position, endPosition) < 0.5f)
        {
            DisplayWinMessage();
        }
        else
        {
            winText.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
        }

        if (restartAction.triggered)
        {
            RestartGame();
        }

        Cursor.lockState = (winText.gameObject.activeSelf || restartButton.gameObject.activeSelf)
            ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = Cursor.lockState == CursorLockMode.None;
    }

    void DisplayWinMessage()
    {
        winText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    void RestartGame()
    {
        winText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
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
