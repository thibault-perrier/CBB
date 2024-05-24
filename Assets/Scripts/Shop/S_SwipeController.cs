using UnityEngine;

public class S_SwipeController : MonoBehaviour
{
    [SerializeField] int _maxPage;
    int _currentPage;
    Vector3 _targetPos;
    [SerializeField] public Vector3 _pageStep;
    [SerializeField] RectTransform _levelPageRect;

    [SerializeField] float _tweenTime;
    [SerializeField] LeanTweenType _tweenType;

    public static S_SwipeController instance;

    private void Awake()
    {
        _currentPage = 1;
        _targetPos = _levelPageRect.localPosition;
        instance = this;
    }

    public void Next()
    {
        if (_currentPage < _maxPage)
        {
            _currentPage++;
            _targetPos += _pageStep;
            MovePage();
            S_HorizontalImageSelector.Instance.ChangeFrame(1);
            S_HorizontalImageSelector.Instance._pageNumber++;
        }
        S_HorizontalImageSelector.Instance.UpdateShopText();
    }

    public void Previous()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            _targetPos -= _pageStep;
            MovePage();
            S_HorizontalImageSelector.Instance.ChangeFrame(-1);
            S_HorizontalImageSelector.Instance._pageNumber--;
        }
        S_HorizontalImageSelector.Instance.UpdateShopText();
    }

    public void MovePage()
    {
        _levelPageRect.LeanMoveLocal(_targetPos, _tweenTime).setEase(_tweenType);
    }
}
