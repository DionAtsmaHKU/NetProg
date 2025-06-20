using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    #region Singleton Implementation
    public static GameUI Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [SerializeField] GameObject canvasUI;
    [SerializeField] GameObject boardUI;
    [SerializeField] Animator menuAnimator;
    [SerializeField] TMP_InputField addressInput;
    public Server server;
    public Client client;

    public void OnOnlineGameButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }

    public void OnOnlineHostButton()
    {
        server.Init(9000);
        client.Init("127.0.0.1", 9000);
        menuAnimator.SetTrigger("HostMenu");
    }

    public void OnOnlineConnectButton()
    {
        Debug.Log("OnOnlineConnectButton();");
        Debug.Log(addressInput.text);
        client.Init(addressInput.text, 9000);
    }

    public void OnOnlineBackButton()
    {
        menuAnimator.SetTrigger("StartMenu");
    }

    public void OnHostBackButton()
    {
        server.Shutdown();
        client.Shutdown();
        menuAnimator.SetTrigger("OnlineMenu");
    }

    public void OnGameStart()
    {
        canvasUI.SetActive(false);
        boardUI.SetActive(true);
    }
}
