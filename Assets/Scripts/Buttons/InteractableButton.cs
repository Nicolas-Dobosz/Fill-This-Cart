using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour
{
    public UnityEvent onPressEvent;

    public void OnPress()
    {
        onPressEvent?.Invoke();
    }
}