using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CanvasAlphaTweener : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    public float startAlpha;
    public float endAlpha;
    public float tweenTime;

    private void Start()
    {
		_canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(BeginTween(startAlpha, endAlpha, tweenTime));
    }

    private IEnumerator BeginTween(float start, float end, float duration)
    {
        // Execute this loop once per frame until the timer exceeds the duration.
        float timer = 0f;
        while (timer <= duration)
        {
            _canvasGroup.alpha = Mathf.Lerp(start, end, timer / duration);

            // Increment the timer by the time between frames and return next frame.
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
