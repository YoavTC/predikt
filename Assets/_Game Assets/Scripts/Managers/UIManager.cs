using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIManager : MonoBehaviour, ITurnPerformListener
{
    [Header("Components")] 
    [SerializeField] private DragAndDropHandler dragAndDropHandler;
    [SerializeField] private GameOverScreenHandler gameOverScreenHandler;
    
    [Header("Toolbar UI Elements")]
    [SerializeField] private Button lockButton;
    [Space]
    [SerializeField] private Image enemyLockStateImage;
    [SerializeField] private Color[] enemyLockStateColors;
    
    [Header("Main Menu Components")]
    [SerializeField] private RectTransform howToPlayPopup;
    [SerializeField] private VideoPlayer tutorialVideoPlayer;

    private void Start()
    {
        GetComponents();
    }

    private void GetComponents()
    {
        if (dragAndDropHandler == null) dragAndDropHandler = DragAndDropHandler.Instance;
    }
    
    // UI updating
    public void UpdateEnemyLockState(LockState lockState)
    {
        enemyLockStateImage.color = enemyLockStateColors[(int) lockState];
    }
    
    public void UpdateLockButtonState(bool state)
    {
        lockButton.interactable = state;
    }
    
    // UI listening
    public void OnLockButtonPressed()
    {
        lockButton.interactable = false;
        GameManager.Instance.LockMove();
        dragAndDropHandler.ChangeInputState(false);
    }

    public void TurnPerformed()
    {
        UpdateLockButtonState(false);
        UpdateEnemyLockState(LockState.PLAYING);
    }

    #region Main Menu Buttons

    public void OnQuitButtonPressed()
    {
        #if !UNITY_WEBGL
        Application.Quit();   
        #endif
    }

    public void OnAboutButtonPressed()
    {
        StartCoroutine(GetAboutPageCoroutine());
    }

    private readonly string[] possibleAboutUrls =
    {
        "https://yoavtc.work/projects/predikt/",
        "https://yoavtc.work/projectsv2/predikt/",
        "https://yoavtc.work/",
        "https://github.com/YoavTC",
        "https://www.linkedin.com/in/yoav-trachtman-cohen/",
        "https://yoav-tc.itch.io/"
    };

    private IEnumerator GetAboutPageCoroutine()
    {
        foreach (var url in possibleAboutUrls)
        {
            Debug.Log($"Iterating over {url}...");
            using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
            {
                yield return unityWebRequest.SendWebRequest();
                
                Debug.Log($"result {unityWebRequest.result}!");
                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {
                    Application.OpenURL(url);
                    yield break;
                }
            }

            yield return null;
        }
    }

    public void OnHowToPlayButtonPressed()
    {
        howToPlayPopup.DOKill();
        howToPlayPopup.DOAnchorPosX(0, 1f);
        
        tutorialVideoPlayer.Play();
    }

    public void OnHowToPlayCloseButtonPressed()
    {
        howToPlayPopup.DOKill();
        howToPlayPopup.DOAnchorPosX(-700, 0.5f).OnComplete(() =>
        {
            tutorialVideoPlayer.Stop();
        });
    }

    #endregion

    public void ShowGameOverScreen(bool playerWon)
    {
        gameOverScreenHandler.Show(playerWon);
    }
}