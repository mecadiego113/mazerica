using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField][DisableInPlayMode] private AudioSource musicAudioSource;

    [SerializeField][DisableInPlayMode][Range(0, 1)] private float musicVolume;

    private const float FADE_TIME = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicAudioSource.volume = 0;

        Play();
    }

    private void Play()
    {
        musicAudioSource.Play();

        FadeInMusic();

        StartCoroutine(MusicLoopCoroutine());
    }

    private IEnumerator MusicLoopCoroutine()
    {
        yield return new WaitForSeconds(musicAudioSource.clip.length - FADE_TIME);

        FadeOutMusic(() =>
        {
            musicAudioSource.Stop();

            Play();
        });
    }

    private void FadeInMusic()
    {
        musicAudioSource.DOFade(musicVolume, FADE_TIME);
    }

    private void FadeOutMusic(Action _action = null)
    {
        musicAudioSource.DOFade(0, FADE_TIME).OnComplete(() =>
        {
            if (_action != null)
            {
                _action.Invoke();
            }
        });
    }

    public void MuteMusic()
    {
        musicAudioSource.DOFade(0, FADE_TIME / 2);
    }

    public void UnmuteMusic()
    {
        musicAudioSource.DOFade(musicVolume, FADE_TIME / 2);
    }
}
