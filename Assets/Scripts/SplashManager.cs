using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashManager : MonoBehaviour
{
    [SerializeField][DisableInPlayMode] private Image fadeImage;

    private const string MAIN_SCENE = "MainScene";
    private const float ANIMATION_TIME = 1;
    private const float SPLASH_DURATION = 5;

    private void Start()
    {
        fadeImage.color = Color.black;

        FadeIn();
    }

    private void FadeIn()
    {
        fadeImage.DOFade(0, ANIMATION_TIME).SetDelay(ANIMATION_TIME).OnComplete(() =>
        {
            FadeOut();
        });
    }

    private void FadeOut()
    {
        fadeImage.DOFade(1, ANIMATION_TIME).SetDelay(SPLASH_DURATION).OnComplete(() =>
        {
            SceneManager.LoadScene(MAIN_SCENE);
        });
    }
}
