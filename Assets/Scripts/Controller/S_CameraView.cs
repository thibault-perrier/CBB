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
    [SerializeField] private List<Transform> _robots = new List<Transform>();

    //Needed when changing view to keep track of which camera have to be displayed
    [Header("Camera references")]
    [SerializeField] private GameObject _camArena;
    [SerializeField] private GameObject _camFps;

    [SerializeField] private Transform _player;

    private ViewType _viewType;
    private GameObject _currentCam;
    private Vector3 _arenaAnchor = new Vector3(0f, 20f, -30f);

    // Start is called before the first frame update
    void Start()
    {
        _viewType = ViewType.Arena;
        _camArena.transform.position = _arenaAnchor;
        _currentCam = _camArena;
    }

    /*LateUpdate is called after all Update functions have been called.
    For example a follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update.*/
    void LateUpdate()
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

    //Update the rotation of the camera depending of the center point of the robots for the Arena view
    private void ArenaViewMovement()
    {
        if (_robots.Count > 0)
        {
            Vector3 centerPoint = GetCenterPoint();
            Quaternion prevRot = _camArena.transform.rotation;

            Vector3 lookDirection = (centerPoint - _camArena.transform.position).normalized;

            _camArena.transform.rotation = Quaternion.Slerp(prevRot, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
        }
    }

    //Get the position of a point that is the center of a bound (can works with one or more robots)
    private Vector3 GetCenterPoint()
    {
        if (_robots.Count == 1)
        {
            return _robots[0].position;
        }

        Bounds bounds = new Bounds(_robots[0].position, Vector3.zero);

        for (int i = 0; i < _robots.Count; i++)
        {
            bounds.Encapsulate(_robots[i].position);
        }

        return bounds.center;
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
                if (_player != null)
                {
                    _currentCam.gameObject.SetActive(false);
                    _camFps.gameObject.SetActive(true);

                    _currentCam = _camFps;
                }
                else
                {
                    Debug.LogError("No player is referenced");
                }
                break;
            default:
                Debug.LogWarning("There is no implementation for this type of view ! : " + _viewType);
                break;
        }
    }

    /*Add a robot to the list so the bound center can be calculated accordingly
    Has to be used when giving reference to the robot that are in the arena*/
    public void AddRobotToView(Transform robot)
    {
        _robots.Add(robot);
    }

    /*Remove a robot from the list so the bound center can be calculated accordingly
    Has to be used when a robot is defeated
    Probably used only when there is more than 2 robots ?*/
    public void RemoveRobotToView(Transform robot)
    {
        _robots.Remove(robot);
    }

    public void SetPlayerRef(Transform player)
    {
        _player = player;
    }
}
