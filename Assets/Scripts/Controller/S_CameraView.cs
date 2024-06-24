using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [SerializeField] private Transform _arenaAnchor;

    private float _smoothTime = 2f;
    private Camera _camComponent;
    private Animator _animator;

    private bool _isTournamentView = false;

    public delegate void OnShowOffComplete();
    public event OnShowOffComplete ShowOffComplete;
    public delegate void OnFadeInComplete();
    public event OnFadeInComplete FadeInComplete;
    public delegate void OnReturnToTournamentComplete();
    public event OnReturnToTournamentComplete ReturnToToTournamentComplete;

    private Coroutine _showMovement;

    private InputAction _skipAction;

    private Vector3 _lastParticipantInView;

    public bool IsTournamentView
    {
        set { _isTournamentView = value; }
    }

    private void Awake()
    {
        _camComponent = GetComponent<Camera>();
        _viewType = ViewType.Arena;

        if (_camArena != null)
        {
            _camArena.transform.position = _arenaAnchor.transform.position;
            _currentCam = _camArena;
        }
        else
        {
            Debug.Log("No camera has been referenced for the arena !");
        }

        _skipAction = new InputAction("Skip");
        _skipAction.AddBinding("<Gamepad>/buttonSouth");
        _skipAction.AddBinding("<Keyboard>/space");
        _skipAction.AddBinding("<Mouse>/leftButton");
        _skipAction.Enable();
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

            _currentCam.transform.position = Vector3.Lerp(_currentCam.transform.position, desiredPosition, Time.deltaTime * _smoothTime);
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
        float initSpeed = 200f;
        Vector3 newPos = new Vector3(firstObject.transform.position.x, firstObject.transform.position.y, 250f);

        while (Vector3.SqrMagnitude(_currentCam.transform.position - newPos) > 0.1f)
        {
            _currentCam.transform.position = Vector3.MoveTowards(_currentCam.transform.position, newPos, Time.deltaTime * initSpeed);

            if (_skipAction.triggered) //to skip the showing off
            {
                break;
            }

            yield return null;
        }

        _currentCam.transform.position = newPos;

        float showSpeed = 60f;
        Vector3 lastPos = new Vector3(lastObject.transform.position.x, lastObject.transform.position.y, 260f);

        while (Vector3.SqrMagnitude(_currentCam.transform.position - lastPos) > 0.1f)
        {
            _currentCam.transform.position = Vector3.MoveTowards(_currentCam.transform.position, lastPos, Time.deltaTime * showSpeed);

            if (_skipAction.triggered) //to skip the showing off
            {
                break;
            }

            yield return null;
        }

        transform.position = lastPos;
        _showMovement = null;
        ShowOffComplete?.Invoke(); //Send an event when the coroutine is over
    }

    /// <summary>
    /// Show the whole tournament bracket before starting showing the participants
    /// </summary>
    /// <param name="firstObject"></param>
    /// <param name="lastObject"></param>
    /// <returns></returns>
    public IEnumerator OverallTournamentView(GameObject firstObject, GameObject lastObject)
    {
        _currentCam.transform.position = new Vector3(0f, 0f, 100f);
        float timer = 0;
        float timerDuration = 2f;

        while (timer < timerDuration)
        {
            timer += Time.deltaTime;
            if (_skipAction.triggered) //to skip the showing off
            {
                break; ;
            }

            yield return null;
        }
        _showMovement = StartCoroutine(ShowOffObjects(firstObject, lastObject));
    }

    /// <summary>
    /// Another class can starts the coroutine from there
    /// </summary>
    /// <param name="firstObject"></param>
    /// <param name="lastObject"></param>
    public void StartShowOffObjects(GameObject firstObject, GameObject lastObject)
    {
        StartCoroutine(OverallTournamentView(firstObject, lastObject));
    }

    public void StopShowOffObject()
    {
        StopCoroutine(_showMovement);
    }

    /// <summary>
    /// Animate a fade in, put the camera at the arena and show the participants informations
    /// </summary>
    public IEnumerator ZoomFadeIn()
    {
        _animator = _currentCam.GetComponent<Animator>();

        _lastParticipantInView = _currentCam.transform.position;

        if (_animator != null)
        {
            _animator.SetTrigger("Start");
        }

        yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);

        _isTournamentView = false;
        _camArena.transform.position = _arenaAnchor.transform.position;
        FadeInComplete?.Invoke();

        _showMovement = null;

        yield return new WaitForSeconds(1f); //give time for the camera to rotate
        if (_animator != null)
        {
            _animator.SetTrigger("Start");
        }
    }

    /// <summary>
    /// Make the screen go black with a fade and get back to the main menu.
    /// </summary>
    public IEnumerator FadeIn()
    {
        _animator = _currentCam.GetComponent<Animator>();

        _lastParticipantInView = _currentCam.transform.position;

        if (_animator != null)
        {
            _animator.SetTrigger("Start");
        }

        yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);

        Debug.Log("Tttagrossemerelapute");
        SceneManager.LoadScene("BuildMainMenu");

        _showMovement = null;
    }

    /// <summary>
    /// After a match is over (or skip is called) the camera return in front of the tournament tree
    /// </summary>
    /// <returns></returns>
    public IEnumerator ReturnToTournament()
    {
        _animator = _currentCam.GetComponent<Animator>();

        if (_animator != null)
        {
            _animator.SetTrigger("Start");
        }

        yield return new WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);

        ClearObjectToView();
        _isTournamentView = true;
        _camArena.transform.position = _lastParticipantInView - new Vector3(0f, 0f, 50f);
        _camArena.transform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(1f); //give time for the camera to rotate
        ReturnToToTournamentComplete?.Invoke();
        if (_animator != null)
        {
            _animator.SetTrigger("Start");
        }
        _showMovement = null;
    }

    public void StartZoomFadeIn()
    {
        if (_showMovement == null)
            _showMovement = StartCoroutine(ZoomFadeIn());
    }

    public void StartFadeIn()
    {
        if (_showMovement == null)
            _showMovement = StartCoroutine(FadeIn());
    }

    public void StartReturnToTournament()
    {
        if (_showMovement == null)
            _showMovement = StartCoroutine(ReturnToTournament());
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
