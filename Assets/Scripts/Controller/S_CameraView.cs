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

    private bool _isTournamentView = false;

    public bool IsTournamentView
    {
        set { _isTournamentView = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _camComponent = GetComponent<Camera>();
        _viewType = ViewType.Arena;

        if (_camArena != null )
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

    //Update the rotation of the camera depending of the center point of the robots for the Arena view
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

    //Update the position of the camera while in the tournament screen
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

    //Get the position of a point that is the center of a bound (can works with one or more robots)
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

    //Get the highest distance between all of the objects inside the bound
    private float GetGreatestDistance()
    {
        var bounds = new Bounds(_objects[0].position, Vector3.zero);

        for (int i = 0; i < _objects.Count; ++i)
        {
            bounds.Encapsulate(_objects[i].position);
        }

        return bounds.size.y + bounds.size.x;
    }

    //Change the view type of the camera
    public void SetViewType(ViewType type)
    {
        _viewType = type;

        ChangeCameraPosition();
        Debug.Log(_viewType);
    }

    //When changing the view we need to set the camera position too before updating it
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

    /*Add a robot to the list so the bound center can be calculated accordingly
    Has to be used when giving reference to the robot that are in the arena*/
    public void AddObjectToView(Transform anObject)
    {
        _objects.Add(anObject);
    }

    /*Remove a robot from the list so the bound center can be calculated accordingly
    Has to be used when a robot is defeated
    Probably used only when there is more than 2 robots ?*/
    public void RemoveObjectToView(Transform anObject)
    {
        _objects.Remove(anObject);
    }

    public void ClearObjectToView()
    {
        _objects.Clear();
    }

    public void SetPlayerRef(Transform player)
    {
        _player = player;
    }
}
