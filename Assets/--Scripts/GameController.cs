
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
public class GameController : MonoBehaviour
{
    public Button[] buttons;
    public Button playerVsComputerButton;
    public Button playerVsPlayerButton;

    public Sprite player1Icon;
    public Sprite player2Icon;
    
    public Timer timerManager;

    public enum GameMode { PlayerVsComputer, PlayerVsPlayer }
    public GameMode currentMode;

    private List<int> player1Moves = new List<int>();
    private List<int> player2Moves = new List<int>();

    private bool isPlayer1Turn = true;
    private int moveCountPlayer1 = 0;
    private int moveCountPlayer2 = 0;

    void Start()
    {
        playerVsComputerButton.onClick.AddListener(SetPlayerVsComputerMode);
        playerVsPlayerButton.onClick.AddListener(SetPlayerVsPlayerMode);

        foreach (var button in buttons)
        {
            button.interactable = false;
            button.onClick.AddListener(() => OnButtonClick(button));
        }
        //StartTurnTimer();
    }

    void SetPlayerVsComputerMode()
    {
        currentMode = GameMode.PlayerVsComputer;
        StartGame();
    }

    void SetPlayerVsPlayerMode()
    {
        currentMode = GameMode.PlayerVsPlayer;
        StartGame();
    }

    void StartGame()
    {
        foreach (var button in buttons)
        {
            button.interactable = true;
        }
        StartTurnTimer();

        playerVsComputerButton.gameObject.SetActive(false);
        playerVsPlayerButton.gameObject.SetActive(false);
    }

    void OnButtonClick(Button button)
    {
        if (button.GetComponent<Image>().sprite != null) return;

        if (currentMode == GameMode.PlayerVsComputer)
        {
            if (!isPlayer1Turn) return; 

            PlayerMove(button, true);  // Player 1's move

            if (CheckWin(player1Moves))
            {
                WinLogic(true);
                return;
            }

            if (!timerManager.MatchTimeOver())
            {
                isPlayer1Turn = false;
                StartTurnTimer();
                Invoke(nameof(ComputerTurn), 1f);
            }
            else
            {
                DrawLogic();
            }
        }
        else if (currentMode == GameMode.PlayerVsPlayer)
        {
            PlayerMove(button, isPlayer1Turn);  // Player 1 or P 2 move

            if (CheckWin(isPlayer1Turn ? player1Moves : player2Moves))
            {
                WinLogic(isPlayer1Turn);
                return;
            }

            if (!timerManager.MatchTimeOver())
            {
                isPlayer1Turn = !isPlayer1Turn;
                StartTurnTimer();
            }
            else
            {
                DrawLogic();
            }
        }
    }

    void ComputerTurn()
    {
        List<int> availableMoves = GetAvailableMoves();
        if (availableMoves.Count == 0 || timerManager.MatchTimeOver())
        {
            DrawLogic();
            return;
        }

        int bestMove = FindBestMoveForComputer();
        PlayerMove(buttons[bestMove], false);  // Computer's move

        if (CheckWin(player2Moves))
        {
            WinLogic(false);
            return;
        }

        isPlayer1Turn = true;
        StartTurnTimer();
    }

    void PlayerMove(Button button, bool isPlayer1)
    {
        int buttonIndex = System.Array.IndexOf(buttons, button);
        button.GetComponent<Image>().sprite = isPlayer1 ? player1Icon : player2Icon;
        button.interactable = false;

        if (isPlayer1)
        {
            player1Moves.Add(buttonIndex);
            moveCountPlayer1++;
            HandleMoveDisappearance(moveCountPlayer1, isPlayer1);
        }
        else
        {
            player2Moves.Add(buttonIndex);
            moveCountPlayer2++;
            HandleMoveDisappearance(moveCountPlayer2, isPlayer1);
        }
    }

    void HandleMoveDisappearance(int moveCount, bool isPlayer1)
    {
        int indexToRemove = moveCount - 4;
       
        if (indexToRemove >= 0)
        {
            if (isPlayer1)
            {
                Debug.Log(moveCount + "-- playermoves list count" + player1Moves.Count);
                //if (indexToRemove < player1Moves.Count)
                //{
                    int buttonIndexToClear = player1Moves[0];
                    buttons[buttonIndexToClear].GetComponent<Image>().sprite = null;
                    buttons[buttonIndexToClear].interactable = true;
                    player1Moves.RemoveAt(0);
                //}
            }
            else
            {
                //if (indexToRemove < player2Moves.Count)
                //{
                    int buttonIndexToClear = player2Moves[0];
                    buttons[buttonIndexToClear].GetComponent<Image>().sprite = null;
                    buttons[buttonIndexToClear].interactable = true;
                    player2Moves.RemoveAt(0);
                //}
            }
        }
    }

    List<int> GetAvailableMoves()
    {
        List<int> availableMoves = new List<int>();

        for (int i = 1; i < buttons.Length; i++)
        {
            if (buttons[i].interactable)
            {
                availableMoves.Add(i);
            }
        }
        return availableMoves;
    }

    int FindBestMoveForComputer()
    {
        List<int> availableMoves = GetAvailableMoves();

        foreach (int move in availableMoves)
        {
            if (WouldBlockPlayerWin(move)) return move;
        }

        return availableMoves[Random.Range(0, availableMoves.Count)];
    }

    bool WouldBlockPlayerWin(int move)
    {
        player1Moves.Add(move);
        bool wouldBlock = CheckWin(player1Moves);
        player1Moves.Remove(move);
        return wouldBlock;
    }

    bool CheckWin(List<int> playerMoves)
    {
        int[,] winCombinations = new int[8, 3] {
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, 9},
            {1, 4, 7},
            {2, 5, 8},
            {3, 6, 9},
            {1, 5, 9},
            {3, 5, 7}
        };

        for (int i = 0; i < winCombinations.GetLength(0); i++)
        {
            if (playerMoves.Contains(winCombinations[i, 0]) &&
                playerMoves.Contains(winCombinations[i, 1]) &&
                playerMoves.Contains(winCombinations[i, 2]))
            {
                return true;
            }
        }

        return false;
    }

    void WinLogic(bool player1Won)
    {
        string winner = player1Won ? "Player 1 (X)" : (currentMode == GameMode.PlayerVsComputer ? "Computer (O)" : "Player 2 (O)");
        Debug.Log(winner + " wins!");
        UiManager.Instance.WinnerText.text = winner + " wins!";
        EndGame();
    }

    void DrawLogic()
    {
        Debug.Log("It's a draw!");
        EndGame();
    }

    void StartTurnTimer()
    {
        timerManager.StartTurnTimer();
    }

    void EndGame()
    {
        timerManager.StopTimers();
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }

    public void ResetEverything()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}