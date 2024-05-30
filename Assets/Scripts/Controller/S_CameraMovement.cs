using UnityEngine;

public class S_CameraMovement : MonoBehaviour
{
    private Vector2 _movement;
    public float Speed = 10f;

    private void Update()
    {
        transform.position += new Vector3(_movement.x, _movement.y, 0f) * Speed * Time.deltaTime;
    }

    public void SetMovement(Vector2 movement)
    {
        _movement = movement;
    }
}
