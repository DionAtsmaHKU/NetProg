using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;

public class TicTacToe : MonoBehaviour
{
    [SerializeField] List<GameObject> slotButtons = new List<GameObject>();
    [SerializeField] List<GameObject> slotX = new List<GameObject>();
    [SerializeField] List<GameObject> slotO = new List<GameObject>();
    private Dictionary<int, int> slots = new Dictionary<int, int>();
    private bool xTurn = true;

    // Score: slots left open
    private int slotsOpen = 9;

    // Multiplayer logic
    private int playerCount = -1;
    private int currentTeam = -1;
    private bool[] playerRematch = new bool[2];

    private void Awake()
    {
        RegisterEvents();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SlotsInit();
    }

    private void SlotsInit()
    {
        slots.Clear();
        for (int i = 0; i < 9; i++)
        {
            slots.Add(i, -1);
        }
    }

    public void PressMakeMove(int slot)
    {
        if (currentTeam == 0 && !xTurn || currentTeam == 1 && xTurn)
            return;

        MakeMove(slot);
        NetMakeMove mm = new NetMakeMove();
        mm.positionPressed = slot;
        mm.teamId = currentTeam;
        Client.Instance.SendToServer(mm);
    }

    private void MakeMove(int slot)
    {
        slotButtons[slot].SetActive(false);
        slots[slot] = Convert.ToInt32(xTurn);
        if (xTurn)
        {
            slotX[slot].SetActive(true);
        }
        else
        {
            slotO[slot].SetActive(true);
        }
        slotsOpen--;
        CheckWin();
        xTurn = !xTurn;
    }

    private void CheckWin()
    {
        if ( // Vertical Wins
            slots[0] == 1 && slots[3] == 1 && slots[6] == 1 ||
            slots[0] == 0 && slots[3] == 0 && slots[6] == 0 ||
            slots[1] == 1 && slots[4] == 1 && slots[7] == 1 ||
            slots[1] == 0 && slots[4] == 0 && slots[7] == 0 ||
            slots[2] == 1 && slots[5] == 1 && slots[8] == 1 ||
            slots[2] == 0 && slots[5] == 0 && slots[8] == 0 ||
            // Horizontal Wins
            slots[0] == 1 && slots[1] == 1 && slots[2] == 1 ||
            slots[0] == 0 && slots[1] == 0 && slots[2] == 0 ||
            slots[3] == 1 && slots[4] == 1 && slots[5] == 1 ||
            slots[3] == 0 && slots[4] == 0 && slots[5] == 0 ||
            slots[6] == 1 && slots[7] == 1 && slots[8] == 1 ||
            slots[6] == 0 && slots[7] == 0 && slots[8] == 0 ||
            // Diagonal Wins
            slots[0] == 1 && slots[4] == 1 && slots[8] == 1 ||
            slots[0] == 0 && slots[4] == 0 && slots[8] == 0 ||
            slots[2] == 1 && slots[4] == 1 && slots[6] == 1 ||
            slots[2] == 0 && slots[4] == 0 && slots[6] == 0)
        {
            PlayerWins(true);
        }
        else if (slotsOpen == 0)
        {
            PlayerWins(false);
        }
    }

    private void PlayerWins(bool gameWon)
    {
        foreach(GameObject go in slotButtons)
        {
            go.SetActive(false);
        }
        if (!gameWon)
        {
            GameUI.Instance.EnableWinScreen("It's a tie!");
        }
        else if (xTurn)
        {
            GameUI.Instance.EnableWinScreen("X wins!");
        }
        else
        {
            GameUI.Instance.EnableWinScreen("O wins!");
        }
        // Add slots to leaderboard
        // Show "Leaderboard";
    }

    public void OnRematchButton()
    {
        NetRematch rm = new NetRematch();
        rm.teamId = currentTeam;
        rm.wantRematch = 1;
        Client.Instance.SendToServer(rm);
    }

    public void OnMenuButton()
    {
        NetRematch rm = new NetRematch();
        rm.teamId = currentTeam;
        rm.wantRematch = 0;
        Client.Instance.SendToServer(rm);
        ResetGame();
        GameUI.Instance.OnGameEnd();
        Invoke("ShutdownDelayed", 1.0f);
    }

    private void ShutdownDelayed()
    {
        Client.Instance.Shutdown();
        Server.Instance.Shutdown();
        playerCount = -1;
        currentTeam = -1;
    }

    private void ResetGame()
    {
        xTurn = !xTurn;
        slotsOpen = 9;
        playerRematch[0] = playerRematch[1] = false;
        foreach(GameObject go in slotButtons)
        {
            go.SetActive(true);
        }
        foreach(GameObject go in slotX)
        {
            go.SetActive(false);
        }
        foreach (GameObject go in slotO)
        {
            go.SetActive(false);
        }
        SlotsInit();
        GameUI.Instance.DisableWinScreen();
        GameUI.Instance.EnableRematchButton();
    }

    #region Event Registering
    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;
        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
        NetUtility.S_REMATCH += OnRematchServer;
        NetUtility.C_REMATCH += OnRematchClient;
    }


    private void UnregisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_START_GAME -= OnStartGameClient;
        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;
        NetUtility.C_MAKE_MOVE -= OnMakeMoveClient;
        NetUtility.S_REMATCH -= OnRematchServer;
        NetUtility.C_REMATCH -= OnRematchClient;
    }

    // Server messages
    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        // Client has conneccted, assign a team and return the message back
        NetWelcome nw = msg as NetWelcome;

        // Assign a team
        nw.AssignedTeam = ++playerCount;

        // Return back to the client
        Server.Instance.SendToClient(cnn, nw);

        if (playerCount == 1)
        {
            Debug.Log("playerCount == 1");
            Server.Instance.Broadcast(new NetStartGame());
        }
    }

    private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn)
    {
        NetMakeMove mm = msg as NetMakeMove;

        // Space here for validation checks (how are you gonna hack tic-tac-toe...)

        // Receive and broadcast it back
        Server.Instance.Broadcast(msg);
    }

    private void OnRematchServer(NetMessage msg, NetworkConnection cnn)
    {
        // Receive and broadcast it back
        Server.Instance.Broadcast(msg);
    }

    // Client messages
    private void OnWelcomeClient(NetMessage msg)
    {
        // Receive connection mesage;
        NetWelcome nw = msg as NetWelcome;

        // Assign a team
       currentTeam = nw.AssignedTeam;

        Debug.Log("My assignmed team is: " + nw.AssignedTeam);
    }

    private void OnStartGameClient(NetMessage msg)
    {
        GameUI.Instance.OnGameStart();
    }

    private void OnMakeMoveClient(NetMessage msg)
    {
        NetMakeMove mm = msg as NetMakeMove;

        if (mm.teamId != currentTeam)
        {
            MakeMove(mm.positionPressed);
        }
    }

    private void OnRematchClient(NetMessage msg)
    {
        NetRematch rm = msg as NetRematch;

        // Set bool for rematch
        playerRematch[rm.teamId] = rm.wantRematch == 1;

        // Activate the Waiting For Rematch UI
        if (rm.teamId != currentTeam)
        {
            GameUI.Instance.WantsRematch(rm.wantRematch == 1);
            if (rm.wantRematch != 1) 
                GameUI.Instance.DisableRematchButton();
        }
            

        if (playerRematch[0] && playerRematch[1])
            ResetGame();
    }
    #endregion 
}


