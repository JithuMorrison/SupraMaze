using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class MazeController : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public Text winText;
    public Button restartButton;
    public GameObject player;
    private Vector3 startPosition;

    public InputAction restartAction;

    void Start()
    {
        startPosition = player.transform.position;
        restartButton.onClick.AddListener(RestartGame);
        winText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        restartAction.Enable();
    }

    void Update()
    {
        Vector3 endPosition = new Vector3((mazeGenerator.width - 2) * mazeGenerator.cellSize, 0, (mazeGenerator.height - 2) * mazeGenerator.cellSize);

        if (Vector3.Distance(player.transform.position, endPosition) < 0.5f)
        {
            DisplayWinMessage();
        }
        else{
            winText.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
        }

        if (restartAction.triggered)
        {
            RestartGame();
        }

        if (winText.gameObject.activeSelf || restartButton.gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
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
        player.SetActive(false);
        yield return null;
        player.SetActive(true);
        player.transform.position = startPosition;
    }

    void OnDisable()
    {
        restartAction.Disable();
    }
}
