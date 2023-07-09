using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private void Start()
    {
        EventManager.SubscribeToEvent(EventManager.EventsType.Event_Sentinel_Alerted, Alerted);
        EventManager.SubscribeToEvent(EventManager.EventsType.Event_Game_Won, Won);
        EventManager.SubscribeToEvent(EventManager.EventsType.Event_Game_Loss, Loss);
    }

    void Default(params object[] param)
    {
        this.GetComponent<Renderer>().material = Resources.Load<Material>("Mats/Floor");
    }

    void Alerted(params object[] param)
    {
        this.GetComponent<Renderer>().material = Resources.Load<Material>("Mats/MatRed");
    }

    void Won(params object[] param)
    {        
        this.GetComponent<Renderer>().material = Resources.Load<Material>("Mats/Exit");
    }

    void Loss(params object[] param)
    {
        this.GetComponent<Renderer>().material = Resources.Load<Material>("Mats/Wall");
    }
}
