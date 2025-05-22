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
    private GameObject target;

    private Dice dice;
    private PawnMovement playerMover;
    private PawnMovement computerMover;
    private Vector3 initialPlayerPosition;
    private Vector3 initialComputerPosition;

    void Start()
    {
        dice = GetComponent<Dice>();
        playerMover = playerPawn.GetComponent<PawnMovement>();
        computerMover = computerPawn.GetComponent<PawnMovement>();

        rollDiceButton.onClick.AddListener(PlayerTurn);
        initialPlayerPosition = playerPawn.transform.position;
        initialComputerPosition = computerPawn.transform.position;
    }

    void PlayerTurn()
    {
        rollDiceButton.interactable = false;
        cameraFollow.target = playerPawn.transform;
        int roll = dice.Roll();
        rollText.text = "Player : " + roll;
        Debug.Log("Player : " + roll);
        StartCoroutine(MovePawn(playerPawn, playerMover, roll, true));
    }

    IEnumerator MovePawn(GameObject pawn, PawnMovement mover, int steps, bool isPlayer)
    {
        int currentIndex = isPlayer ? playerIndex : computerIndex;
        int newIndex = currentIndex;

        List<Transform> currentPath = isPlayer ? playerPathPoints : computerPathPoints;

        yield return StartCoroutine(mover.MoveAlongPath(currentPath, currentIndex, steps, (result) =>
        {
            newIndex = result;
        }));

        if (isPlayer) playerIndex = newIndex;
        else computerIndex = newIndex;

        if (newIndex >= currentPath.Count - 1)
        {
            rollText.gameObject.SetActive(false);
            resultText.text = (isPlayer ? "Player" : "Computer") + " Wins!";
            GameWon.gameObject.SetActive(true);
            rollDiceButton.interactable = false;
            rollDiceButton.gameObject.SetActive(false);
            yield break;
        }

        if (isPlayer)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(ComputerTurn());
        }
        else
        {
            rollDiceButton.interactable = true;
        }
    }

    IEnumerator ComputerTurn()
    {
        int roll = dice.Roll();
        cameraFollow.target = computerPawn.transform;
        rollText.text = "Computer : " + roll;
        Debug.Log("Computer : " + roll);
        yield return StartCoroutine(MovePawn(computerPawn, computerMover, roll, false));
    }

    public void RestartGame()
    {
        playerIndex = 0;
        computerIndex = 0;
        playerPawn.transform.position = initialPlayerPosition;
        computerPawn.transform.position = initialComputerPosition;
        GameWon.gameObject.SetActive(false);
        rollDiceButton.gameObject.SetActive(true);
        rollDiceButton.interactable = true;
        rollText.gameObject.SetActive(true);
        rollText.text = "Roll the dice!";
    }
}
