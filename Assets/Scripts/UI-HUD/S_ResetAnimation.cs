using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Reset the animation even when the object is deactivated
/// </summary>
public class S_ResetAnimation : MonoBehaviour, IPointerEnterHandler
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.writeDefaultValuesOnDisable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
