using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ThreeDotsAnimation : MonoBehaviour
{
    private TMP_Text text;

    [SerializeField] private float delay;
    [SerializeField] private string baseText;
    
    private float elapsedTime;
    
    IEnumerator Start()
    {
        text = GetComponent<TMP_Text>();
        string dots;

        while (true)
        {
            dots = "";
            for (int i = 0; i < 3; i++)
            {
                dots += ".";
                text.text = baseText + dots;

                yield return new WaitForSeconds(delay);
            }
        }
    }
}
