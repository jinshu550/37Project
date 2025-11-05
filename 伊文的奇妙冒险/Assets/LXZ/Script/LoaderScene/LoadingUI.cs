using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dotText;
    private float dotRate = 0.3f;
    private void Start()
    {
        StartCoroutine(DotAnimation());
    }
    IEnumerator DotAnimation()
    {
        while (true)
        {
            dotText.text = "Loading.";
            yield return new WaitForSeconds(dotRate);
            dotText.text = "Loading..";
            yield return new WaitForSeconds(dotRate);
            dotText.text = "Loading...";
            yield return new WaitForSeconds(dotRate);
            dotText.text = "Loading....";
            yield return new WaitForSeconds(dotRate);
            dotText.text = "Loading.....";
            yield return new WaitForSeconds(dotRate);
            dotText.text = "Loading......";
            yield return new WaitForSeconds(dotRate);
        }
    }
}
