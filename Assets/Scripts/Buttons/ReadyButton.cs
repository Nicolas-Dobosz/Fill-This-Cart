using System;
using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] private ButtonsManager manager;
    private Renderer rend;
    public bool IsReady { get; private set; }

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void ChangeColor()
    {
        rend.material.color = Color.green;

        if (!IsReady)
        {
            IsReady = true;
            if (manager != null)
            {
                manager.CheckButtons();
            }
        }
    }
}
