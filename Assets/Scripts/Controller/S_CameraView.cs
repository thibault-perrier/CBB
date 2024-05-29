using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CameraView : MonoBehaviour
{
    public enum ViewType
    {
        Arena,
        Pilot,
        ThirdPerson,
        FirstPerson
    }

    //Needed for Arena view
    [Header("Bots reference")]
    [SerializeField] private List<Transform> _objects = new List<Transform>();

    //Needed when changing view to keep track of which camera have to be displayed
    [Header("Camera references")]
    [SerializeField] private GameObject _camArena;
    [SerializeField] private GameObject _camFps;

    [SerializeField] private Transform _player;

    private ViewType _viewType;
    private GameObject _currentCam;
    private Vector3 _arenaAnchor = new Vector3(0f, 20f, -30f); //Should be different depending of the arena

    private float _smoothTime = 2f;
    private Camera _camComponent;
    private Animator _animator;

    private bool _isTournamentView = false;

    public delegate void OnShowOffComplete();
    public event OnShowOffComplete ShowOffComplete;
    private Coroutine _showMovement;

    public bool IsTournamentView
    {
        set { _isTournamentView = value; }
    }

    private void Awake()
    {
        _camComponent = GetComponent<Camera>();
        _animator = GetComponent<Animator>();
        _viewType = ViewType.Arena;

        if (_camArena != null)
        {
            _camArena.transform.position = _arenaAnchor;
            _currentCam = _camArena;
        }
        else
        {
            Debug.Log("No camera has been referenced for the arena !");
        }
    }

    /*LateUpdate is called after all Update functions have been called.
    For example a follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update.*/
    void LateUpdate()
    {
        if (_showMovement == null)
        {
            if (!_isTournamentView)
            {
                switch (_viewType)
                {
                    case ViewType.Arena:
                        ArenaViewMovement();
                        break;
                    default:
                        //TODO remove this debug log to stop the flood
                        //Debug.LogWarning("There is no implementation for this type of view ! : " +  _viewType);
                        break;
                }
            }
            else
            {
                TournamentViewMovement();
            }
        }
    }

    /// <summary>
    /// Update the rotation of the camera depending of the center point of the robots for the Arena view
    /// </summary>
    private void ArenaViewMovement()
    {
        if (_objects.Count > 0)
        {
            Vector3 centerPoint = GetCenterPoint();
            Quaternion prevRot = _camArena.transform.rotation;

            Vector3 lookDirection = (centerPoint - _camArena.transform.position).normalized;

            _camArena.transform.rotation = Quaternion.Slerp(prevRot, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
        }
    }

    /// <summary>
    /// Update the position of the camera while in the tournament screen
    /// </summary>
    private void TournamentViewMovement()
    {
        if (_objects.Count > 0)
        {
            Vector3 centerPoint = GetCenterPoint();
            float greatestDistance = GetGreatestDistance();

            // Calculate desired camera position
            Vector3 desiredPosition = centerPoint - new Vector3(0, 0, greatestDistance / 2f + 30f);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * _smoothTime);
        }
    }

    /// <summary>
    /// Get the position of a point that is the center of a bound (can works with one or more robots)
    /// </summary>
    /// <returns></returns>
    private Vector3 GetCenterPoint()
    {
        if (_objects.Count == 1)
        {
            return _objects[0].position;
        }

        Bounds bounds = new Bounds(_objects[0].position, Vector3.zero);

        for (int i = 0; i < _objects.Count; i++)
        {
            bounds.Encapsulate(_objects[i].position);
        }

        return bounds.center;
    }

    /// <summary>
    /// Get the highest distance between all of the objects inside the bound
    /// </summary>
    /// <returns></returns>
    private float GetGreatestDistance()
    {
        var bounds = new Bounds(_objects[0].position, Vector3.zero);

        for (int i = 0; i < _objects.Count; ++i)
        {
            bounds.Encapsulate(_objects[i].position);
        }

        return bounds.size.y + bounds.size.x;
    }

    /// <summary>
    /// Change the view type of the camera
    /// </summary>
    /// <param name="type"></param>
    public void SetViewType(ViewType type)
    {
        _viewType = type;

        ChangeCameraPosition();
        Debug.Log(_viewType);
    }

    /// <summary>
    /// When changing the view we need to set the camera position too before updating it
    /// </summary>
    private void ChangeCameraPosition()
    {
        switch (_viewType)
        {
            case ViewType.Arena:
                _currentCam.gameObject.SetActive(false);
                _camArena.gameObject.SetActive(true);

                _currentCam = _camArena;
                break;
            case ViewType.FirstPerson:
                if (_player != null && _camFps != null)
                {
                    _currentCam.gameObject.SetActive(false);
                    _camFps.gameObject.SetActive(true);

                    _currentCam = _camFps;
                }
                else
                {
                    Debug.LogError("No player and / or camera is referenced");
                }
                break;
            default:
                Debug.LogWarning("There is no implementation for this type of view ! : " + _viewType);
                break;
        }
    }

    /// <summary>
    /// Add a robot to the list so the bound center can be calculated accordingly
    /// Has to be used when giving reference to the robot that are in the arena
    /// </summary>
    /// <param name="anObject"></param>
    public void AddObjectToView(Transform anObject)
    {
        _objects.Add(anObject);
    }

    /// <summary>
    /// Remove a robot from the list so the bound center can be calculated accordingly
    /// Has to be used when a robot is defeated
    /// Probably used only when there is more than 2 robots ?
    /// </summary>
    /// <param name="anObject"></param>
    public void RemoveObjectToView(Transform anObject)
    {
        _objects.Remove(anObject);
    }

    /// <summary>
    /// Move the camera accross 2 gameobjects
    /// </summary>
    /// <param name="firstObject"></param>
    /// <param name="lastObject"></param>
    /// <returns></returns>
    private IEnumerator ShowOffObjects(GameObject firstObject, GameObject lastObject)
    {
        _currentCam.transform.position = new Vector3(firstObject.transform.position.x, firstObject.transform.position.y, 50f);
        Vector3 lastPos = new Vector3(lastObject.transform.position.x, lastObject.transform.position.y, 50f);

        while (Vector3.SqrMagnitude(transform.position - lastPos) > 0.1f)
        {
            _currentCam.transform.position = Vector3.MoveTowards(transform.position, lastPos, Time.deltaTime * 20f);
            Debug.Log(transform.position);
            yield return null;
        }

        transform.position = lastPos;
        _showMovement = null;
        ShowOffComplete?.Invoke(); //Send an event when the coroutine is over
    }

    /// <summary>
    /// Another class can starts the coroutine from there
    /// </summary>
    /// <param name="firstObject"></param>
    /// <param name="lastObject"></param>
    public void StartShowOffObjects(GameObject firstObject, GameObject lastObject)
    {
        _showMovement = StartCoroutine(ShowOffObjects(firstObject, lastObject));
    }

    public IEnumerator ZoomFadeIn()
    {
        _animator.SetTrigger("Start");
        yield return null;
    }

    public void StartZoomFadeIn()
    {
        StartCoroutine(ZoomFadeIn());
    }

    /// <summary>
    /// Remove every object the camera follows
    /// </summary>
    public void ClearObjectToView()
    {
        _objects.Clear();
    }

    public void SetPlayerRef(Transform player)
    {
        _player = player;
    }
}
