using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public NotifyEvent<KeyCode> OnKeyDown = new NotifyEvent<KeyCode>();
    public NotifyEvent<KeyCode> OnKeyUp = new NotifyEvent<KeyCode>();
    public NotifyEvent<KeyCode> OnKeyPress = new NotifyEvent<KeyCode>();
    public List<KeyCode> ReactKeys = new List<KeyCode>();

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode key in ReactKeys)
        {
            if (Input.GetKeyDown(key))
            {
                OnKeyDown.Invoke(key);
            }

            if (Input.GetKeyUp(key))
            {
                OnKeyUp.Invoke(key);
            }

            if (Input.GetKey(key))
            {
                OnKeyPress.Invoke(key);
            }
        }
    }
}
