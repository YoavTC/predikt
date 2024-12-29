using System;
using System.Collections;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple script to "hack" the default create session widget and
/// allow doing it without needing to specify a name for the session
/// </summary>
public class CreateSessionFixer : MonoBehaviour
{
    [SerializeField] private TMP_InputField sessionNameInput;
    [SerializeField] private Button sessionCreateButton;

    public void Start()
    {
        sessionNameInput.text = Guid.NewGuid().ToString().Substring(0, 8);
        
        AuthenticationService.Instance.SignedIn += () => StartCoroutine(ClientStarted());
    }

    private IEnumerator ClientStarted()
    {
        yield return new WaitForSeconds(0.5f);
        sessionCreateButton.interactable = true;
    }
}
