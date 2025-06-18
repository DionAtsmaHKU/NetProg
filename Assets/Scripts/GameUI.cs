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

    [SerializeField] Animator menuAnimator;

    public void OnOnlineGameButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }

    public void OnOnlineHostButton()
    {
        menuAnimator.SetTrigger("HostMenu");
    }

    public void OnOnlineConnectButton()
    {
        Debug.Log("OnOnlineConnectButton");
    }

    public void OnOnlineBackButton()
    {
        menuAnimator.SetTrigger("StartMenu");
    }

    public void OnHostBackButton()
    {
        menuAnimator.SetTrigger("StartMenu");
    }
}
