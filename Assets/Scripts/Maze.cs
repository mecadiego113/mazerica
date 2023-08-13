using DG.Tweening;
using System;
using UnityEngine;

public class Maze : MonoBehaviour
{
    private float ANIMATION_TIME = 2;
    private const float HIDDEN_Y = -50;

    public void Show(Action _action = null)
    {
        enabled = true;

        transform.DOLocalMoveY(0, ANIMATION_TIME).OnComplete(() =>
        {
            if (_action != null)
            {
                _action.Invoke();
            }
        });
    }

    public void Hide(Action _action = null)
    {
        enabled = true;

        transform.DOLocalMoveY(HIDDEN_Y, ANIMATION_TIME).OnComplete(() =>
        {
            enabled = false;

            if (_action != null)
            {
                _action.Invoke();
            }
        });
    }
}