using System;
using System.Collections;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameNetworkPanelsManager : MonoBehaviour
{
    [Header("Camera Animation Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector2 inOutAnimationsDelay;
    [SerializeField] private Vector2 inOutPositions;
    [SerializeField] private float animationDuration;
    [SerializeField] private Ease animationEase;

    [Header("Canvases")]
    [SerializeField] private Canvas networkPanelCanvas;
    [SerializeField] private Canvas gamePanelCanvas;
    
    private IEnumerator Start()
    {
        ToggleCanvasesVisibility(networkPanelCanvas, true, 0);
        ToggleCanvasesVisibility(gamePanelCanvas, false, 0);
        CameraExitGameFrame();
        
        yield return new WaitUntil(() => NetworkManager.Singleton != null);
    }

    public void ClientConnectedCallback(ulong clientId)
    {
        ToggleCanvasesVisibility(networkPanelCanvas, false);
        ToggleCanvasesVisibility(gamePanelCanvas, true);
        CameraEnterGameFrame();
    }
    
    public void ClientDisconnectedCallback(ulong clientId)
    {
        ToggleCanvasesVisibility(networkPanelCanvas, true);
        ToggleCanvasesVisibility(gamePanelCanvas, false);
        CameraExitGameFrame();
    }

    #region Camera Animations
    // Transition the camera from the network panel to the game panel
    private void CameraEnterGameFrame()
    {
        cameraTransform.DOMoveY(inOutPositions.x, animationDuration)
            .SetDelay(inOutAnimationsDelay.x)
            .SetEase(animationEase);
    }
    
    // Transition the camera from the game panel to the network panel
    private void CameraExitGameFrame()
    {
        cameraTransform.DOMoveY(inOutPositions.y, animationDuration)
            .SetDelay(inOutAnimationsDelay.y)
            .SetEase(animationEase);
    }
    #endregion

    #region UI Managing
    private void ToggleCanvasesVisibility(Canvas canvas, bool visible, float duration = 2f)
    {
        var children = canvas.GetComponentsInChildren<Graphic>();

        foreach (var child in children)
        {
            child.DOFade(visible ? 1f : 0f, duration);
        }

        // StartCoroutine(ToggleCanvas(canvas, visible, duration));
    }
    #endregion
}
