using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] GameObject winUI;
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] GameObject wantsRematch;
    [SerializeField] GameObject noRematch;
    [SerializeField] Animator menuAnimator;
    [SerializeField] TMP_InputField addressInput;
    [SerializeField] Button rematchButton;
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

    public void OnGameEnd()
    {
        canvasUI.SetActive(true);
        boardUI.SetActive(false);
        DisableWinScreen();
        menuAnimator.SetTrigger("StartMenu");
    }

    public void EnableWinScreen(string text)
    {
        winText.text = text;
        winUI.SetActive(true);
    }

    public void WantsRematch(bool rematch)
    {
        if (rematch)
            wantsRematch.SetActive(true);

        else noRematch.SetActive(true);
    }

    public void DisableWinScreen()
    {
        winUI.SetActive(false);
        wantsRematch.SetActive(false);
        noRematch.SetActive(false);
    }

    public void DisableRematchButton()
    {
        rematchButton.interactable = false;
    }

    public void EnableRematchButton()
    {
        rematchButton.interactable = true;
    }
}
