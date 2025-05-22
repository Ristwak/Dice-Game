using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Transform> pathPoints;

    public GameObject playerPawn;
    public GameObject computerPawn;

    public Button rollDiceButton;
    public Text resultText;

    private int playerIndex = 0;
    private int computerIndex = 0;

    private Dice dice;
    private PawnMovement playerMover;
    private PawnMovement computerMover;

    void Start()
    {
        dice = GetComponent<Dice>();
        playerMover = playerPawn.GetComponent<PawnMovement>();
        computerMover = computerPawn.GetComponent<PawnMovement>();

        rollDiceButton.onClick.AddListener(PlayerTurn);
    }

    void PlayerTurn()
    {
        rollDiceButton.interactable = false;
        int roll = dice.Roll();
        resultText.text = "Player : " + roll;
        Debug.Log("Player : " + roll);
        StartCoroutine(MovePawn(playerPawn, playerMover, roll, true));
    }

    IEnumerator MovePawn(GameObject pawn, PawnMovement mover, int steps, bool isPlayer)
    {
        int currentIndex = isPlayer ? playerIndex : computerIndex;
        int newIndex = currentIndex;

        yield return StartCoroutine(mover.MoveAlongPath(pathPoints, currentIndex, steps, (result) =>
        {
            newIndex = result;
        }));

        if (isPlayer) playerIndex = newIndex;
        else computerIndex = newIndex;

        // Check for win
        if (newIndex >= pathPoints.Count - 1)
        {
            resultText.text = (isPlayer ? "Player" : "Computer") + " Wins!";
            rollDiceButton.interactable = false;
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
        resultText.text = "Computer : " + roll;
        Debug.Log("Computer : " + roll);
        yield return StartCoroutine(MovePawn(computerPawn, computerMover, roll, false));
    }
}
