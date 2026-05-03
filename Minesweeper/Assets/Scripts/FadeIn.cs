using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public float fadeTime = 3f; // 渐隐时间
    public CanvasGroup canvasGroup;

    public void StartFadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0;
        while ( elapsedTime < fadeTime )
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1 - (elapsedTime / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 0;
        //渐隐完成后禁用物体
        //gameObject.SetActive(false);
    }
}
