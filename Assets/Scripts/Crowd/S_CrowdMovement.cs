using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class S_CrowdMovement : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float _maxJumpHeight;
    private float _maxHeight;
    private float _minHeight;
    [SerializeField] private float _speed;
    [SerializeField, Range(0.1f, 1.0f)] private float _moveProbability = 0.5f;
    [SerializeField] private float _minTimerBeforeMove;
    [SerializeField] private float _maxTimerBeforeMove;
    private float _minTimerBeforeMoveThreshold;
    private float _timerBeforeMove;
    private Vector3 _endPos = Vector3.zero;
    private Vector3 _direction = Vector3.zero;
    private bool _isMoving = false;

    private Transform _transform;

    void Awake()
    {
        _transform = transform;
    }

    void Start()
    {
        _endPos = _transform.position;
        _timerBeforeMove = _maxTimerBeforeMove;
        _minTimerBeforeMoveThreshold = Mathf.Abs(_maxTimerBeforeMove - _minTimerBeforeMove);
        _minHeight = transform.position.y;
        _maxHeight = transform.position.y + _maxJumpHeight;
    }

    void Update()
    {
        if (_timerBeforeMove <= _minTimerBeforeMoveThreshold)
        {
            if (!_isMoving && (_timerBeforeMove <= 0.0f || Random.Range(0.0f, 1.0f) <= _moveProbability))
            {
                _isMoving = true;
                StartCoroutine(MakeCrowdMovement());
                _timerBeforeMove = _maxTimerBeforeMove;
            }
        }

        _timerBeforeMove -= Time.deltaTime;
    }

    IEnumerator MakeCrowdMovement()
    {
        Debug.Log("CROWD IS MOVING");
        _endPos.y = Random.Range(_minHeight, _maxHeight);

        while (Vector3.Distance(_transform.position, _endPos) >= 0.1f)
        {
            _direction = _endPos - _transform.position;
            _transform.Translate(_direction.normalized * Time.deltaTime * _speed);
            yield return null;
        }

        _isMoving = false;
        yield return null;
    }
}
