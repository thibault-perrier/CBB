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

    public static S_SwipeController Instance;

    private void Awake()
    {
        _currentPage = 1;
        _targetPos = _levelPageRect.localPosition;
        if (Instance == null)
            Instance = this;
    }

    public void Next() //switch next frame
    {
        if (_currentPage < _maxPage)
        {
            _currentPage++;
            _targetPos += _pageStep;
            MovePage();
            S_ShopManager.Instance.ChangeFrame(1);
        }
        S_ShopManager.Instance.UpdateShopText();
    }

    public void Previous()//switch previous frame
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            _targetPos -= _pageStep;
            MovePage();
            S_ShopManager.Instance.ChangeFrame(-1);
        }
        S_ShopManager.Instance.UpdateShopText();
    }

    public void MovePage() // anim switch frame
    {
        _levelPageRect.LeanMoveLocal(_targetPos, _tweenTime).setEase(_tweenType);
    }
}
