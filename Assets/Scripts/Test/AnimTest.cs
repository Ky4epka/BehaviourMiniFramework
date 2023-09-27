using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTest : MonoBehaviour
{
    public Animator Controller = null;

    public void IdleState()
    {
        Debug.Log("Idle");
        Controller.Play("State_Idle.Idle", 0);
    }

    public void MoveState()
    {
        Debug.Log("Move");
        Controller.Play("State_Move.Move", 0);
    }

    public void FireState()
    {
        Debug.Log("Fire");
        Controller.Play("Fire", 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
