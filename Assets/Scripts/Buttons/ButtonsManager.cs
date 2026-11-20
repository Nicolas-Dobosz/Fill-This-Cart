using UnityEngine;
using UnityEngine.Events;

public class ButtonsManager : MonoBehaviour
{
    [SerializeField] private ReadyButton buttonA;
    [SerializeField] private ReadyButton buttonB;

    public UnityEvent onBothReady;

    private bool wasTriggered = false;

    public void CheckButtons()
    {
        if (wasTriggered) return;
        if (buttonA == null || buttonB == null) return;

        if (buttonA.IsReady && buttonB.IsReady)
        {
            wasTriggered = true;
            HandleAction();
        }
    }

    private void HandleAction()
    {
        Debug.Log("Les joueurs sont pret");
        onBothReady?.Invoke();
    }
}