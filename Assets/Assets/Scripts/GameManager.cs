using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Transform> playerPathPoints;
    public List<Transform> computerPathPoints;
    public CameraFollow cameraFollow;
    public GameObject GameWon;

    public GameObject playerPawn;
    public GameObject computerPawn;

    public Button rollDiceButton;
    public Text rollText;
    public Text resultText;

    private int playerIndex = 0;
    private int computerIndex = 0;

    private Dice dice;
    private PawnMovement playerMover;
    private PawnMovement computerMover;

    private Vector3 initialPlayerPosition;
    private Vector3 initialComputerPosition;

    private bool isTurnInProgress;
    private bool isGameOver;
    private bool isInitialized;

    void Start()
    {
        dice = GetComponent<Dice>();

        if (!ValidateSetup())
        {
            if (rollDiceButton != null)
                rollDiceButton.interactable = false;

            return;
        }

        playerMover = playerPawn.GetComponent<PawnMovement>();
        computerMover = computerPawn.GetComponent<PawnMovement>();

        if (playerMover == null || computerMover == null)
        {
            Debug.LogError("GameManager: PawnMovement component is missing from a pawn.");
            rollDiceButton.interactable = false;
            return;
        }

        initialPlayerPosition = playerPawn.transform.position;
        initialComputerPosition = computerPawn.transform.position;

        rollDiceButton.onClick.AddListener(PlayerTurn);

        if (cameraFollow != null)
            cameraFollow.target = playerPawn.transform;

        isInitialized = true;
    }

    bool ValidateSetup()
    {
        bool valid = true;

        if (dice == null)
        {
            Debug.LogError("GameManager: Dice component is missing.");
            valid = false;
        }

        if (playerPawn == null || computerPawn == null)
        {
            Debug.LogError("GameManager: Player or Computer pawn is missing.");
            valid = false;
        }

        if (rollDiceButton == null || rollText == null || resultText == null || GameWon == null)
        {
            Debug.LogError("GameManager: One or more UI references are missing.");
            valid = false;
        }

        if (!ValidatePath(playerPathPoints, "Player") ||
            !ValidatePath(computerPathPoints, "Computer"))
        {
            valid = false;
        }

        return valid;
    }

    bool ValidatePath(List<Transform> path, string pathName)
    {
        if (path == null || path.Count < 2)
        {
            Debug.LogError("GameManager: " + pathName + " path must contain at least two points.");
            return false;
        }

        for (int i = 0; i < path.Count; i++)
        {
            if (path[i] == null)
            {
                Debug.LogError(pathName + " path contains a missing point at index " + i);
                return false;
            }
        }

        return true;
    }

    void PlayerTurn()
    {
        if (!isInitialized || isGameOver || isTurnInProgress)
            return;

        isTurnInProgress = true;
        rollDiceButton.interactable = false;

        if (cameraFollow != null)
            cameraFollow.target = playerPawn.transform;

        int roll = dice.Roll();

        rollText.text = "Player : " + roll;
        Debug.Log("Player : " + roll);

        StartCoroutine(MovePawn(playerPawn, playerMover, roll, true));
    }

    IEnumerator MovePawn(GameObject pawn, PawnMovement mover, int steps, bool isPlayer)
    {
        List<Transform> currentPath = isPlayer ? playerPathPoints : computerPathPoints;
        int currentIndex = isPlayer ? playerIndex : computerIndex;

        int newIndex = currentIndex;

        if (pawn == null || mover == null || currentPath == null || currentPath.Count < 2)
        {
            Debug.LogError("GameManager: Invalid pawn, movement component or path.");
            isGameOver = true;
            yield break;
        }

        yield return StartCoroutine(
            mover.MoveAlongPath(currentPath, currentIndex, steps, result =>
            {
                newIndex = result;
            })
        );

        if (isGameOver)
            yield break;

        if (isPlayer)
            playerIndex = newIndex;
        else
            computerIndex = newIndex;

        if (newIndex >= currentPath.Count - 1)
        {
            FinishGame(isPlayer);
            yield break;
        }

        if (isPlayer)
        {
            yield return new WaitForSeconds(1f);

            if (!isGameOver)
                StartCoroutine(ComputerTurn());
        }
    }

    IEnumerator ComputerTurn()
    {
        if (isGameOver)
            yield break;

        if (cameraFollow != null)
            cameraFollow.target = computerPawn.transform;

        int roll = dice.Roll();

        rollText.text = "Computer : " + roll;
        Debug.Log("Computer : " + roll);

        yield return StartCoroutine(
            MovePawn(computerPawn, computerMover, roll, false)
        );

        if (!isGameOver)
        {
            isTurnInProgress = false;
            rollDiceButton.interactable = true;
        }
    }

    void FinishGame(bool playerWon)
    {
        isGameOver = true;
        isTurnInProgress = false;

        rollText.gameObject.SetActive(false);

        resultText.text = playerWon ? "Player Wins!" : "Computer Wins!";

        GameWon.SetActive(true);

        rollDiceButton.interactable = false;
        rollDiceButton.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        if (!isInitialized)
            return;

        StopAllCoroutines();

        playerIndex = 0;
        computerIndex = 0;

        isGameOver = false;
        isTurnInProgress = false;

        playerPawn.transform.position = initialPlayerPosition;
        computerPawn.transform.position = initialComputerPosition;

        if (cameraFollow != null)
            cameraFollow.target = playerPawn.transform;

        GameWon.SetActive(false);

        rollDiceButton.gameObject.SetActive(true);
        rollDiceButton.interactable = true;

        rollText.gameObject.SetActive(true);
        rollText.text = "Roll the dice!";

        resultText.text = "";
    }

    void OnDestroy()
    {
        if (rollDiceButton != null)
            rollDiceButton.onClick.RemoveListener(PlayerTurn);
    }
}