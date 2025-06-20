using System.Collections.Generic;
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

    // Multiplayer logic
    private int playerCount = -1;
    private int currentTeam = -1;

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
        slots[slot] = currentTeam;
        if (xTurn)
        {
            slotX[slot].SetActive(true);
        }
        else
        {
            slotO[slot].SetActive(true);
        }
        xTurn = !xTurn;
        CheckWin();
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
            slots[0] == 1 && slots[4] == 1 && slots[8] == 1 ||
            slots[2] == 0 && slots[4] == 0 && slots[6] == 0 ||
            slots[2] == 0 && slots[4] == 0 && slots[6] == 0)
        {
            PlayerWins(currentTeam);
            return;
        }
        // 
    }

    private void PlayerWins(int team)
    {
        // send message that a player has won
    }

    private void ResetGame()
    {
        foreach(GameObject go in slotButtons)
        {
            go.SetActive(true);
        }
        SlotsInit();
    }

    #region Event Registering
    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;
        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
    }


    private void UnregisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_START_GAME -= OnStartGameClient;
        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;
        NetUtility.C_MAKE_MOVE -= OnMakeMoveClient;
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
    #endregion 
}


