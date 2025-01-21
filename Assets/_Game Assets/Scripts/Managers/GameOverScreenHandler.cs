using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverScreenHandler : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private RectTransform animatedPopup;
    [SerializeField] private TMP_Text gameOverTextDisplay;
    [SerializeField] private string winText, loseText;
    [SerializeField] private Button rematchButton;

    [Header("Animation")]
    [SerializeField] private float scaleMultiplier;
    [SerializeField] private float scaleAnimationDuration;

    // Hide Game Over panel
    private void Awake() => gameObject.SetActive(false);

    [Button]
    public void Test()
    {
        Show(true);
    }

    public void Show(bool playerWon)
    {
        gameOverTextDisplay.text = playerWon ? winText : loseText;
        animatedPopup.DOPunchScale(transform.localScale * scaleMultiplier, scaleAnimationDuration);
        gameObject.SetActive(true);
        
        if (!GameManager.Instance.IsHost) DisableRematchButton();
    }

    private void Hide()
    {
        animatedPopup.DOPunchScale(transform.localScale * scaleMultiplier, scaleAnimationDuration);
        gameObject.SetActive(false);
    }

    private void DisableRematchButton()
    {
        rematchButton.interactable = false;
        rematchButton.image.color = Color.gray;
        Destroy(rematchButton.GetComponent<EventTrigger>());
    }

    public void OnMainMenuButtonPressed()
    {
        Hide();
        NetworkManager.Singleton.Shutdown();
        // if (NetworkManager.Singleton.IsHost)
        // {
        //     NetworkManager.Singleton.Shutdown();
        // } else NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
    }

    public void OnRematchButtonPressed()
    {
        Hide();
    }
}
