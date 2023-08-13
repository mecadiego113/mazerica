using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance;

    [SerializeField][DisableInPlayMode] private Volume volume;

    private DepthOfField depthOfField;

    private const float ANIMATION_TIME = 0.5f;

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

        volume.profile.TryGet(out depthOfField);
    }

    public void EnableDOF()
    {
        depthOfField.focalLength.value = 300;
    }

    public void FadeInDOF()
    {
        depthOfField.active = true;

        DOTween.To(() => depthOfField.focalLength.value, x => depthOfField.focalLength.value = x, 300, ANIMATION_TIME);
    }

    public void FadeOutDOF()
    {
        DOTween.To(() => depthOfField.focalLength.value, x => depthOfField.focalLength.value = x, 1, ANIMATION_TIME).OnComplete(() =>
        {
            depthOfField.active = false;
        });
    }
}
