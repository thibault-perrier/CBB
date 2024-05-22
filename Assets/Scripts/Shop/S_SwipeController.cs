using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SwipeController : MonoBehaviour
{
    [SerializeField] int _maxPage;
     int _currentPage;
    Vector3 _targetPos;
    [SerializeField] public  Vector3 _pageStep;
    [SerializeField] RectTransform _levelPageRect;

    [SerializeField] float _tweenTime;
    [SerializeField] LeanTweenType _tweenType;

    public static S_SwipeController instance;
    private void Awake()
    {
        _currentPage = 1;
        _targetPos = _levelPageRect.localPosition;
    }
    public void Next()
    {
        if (_currentPage < _maxPage)
        {
            _currentPage++;
            _targetPos += _pageStep;
            MovePage();

        }
    }

    public void Previous()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            _targetPos -= _pageStep;
            MovePage();

        }

    }

    public void MovePage()
    {
        _levelPageRect.LeanMoveLocal(_targetPos, _tweenTime).setEase(_tweenType);
    }

}
