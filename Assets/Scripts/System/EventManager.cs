﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager 
{
    public delegate void EventReceiver(params object[] parameterContainer);
    private static Dictionary<EventsType, EventReceiver> _events;
    
    public enum EventsType
    {
        Event_Sentinel_Alerted,
        Event_Sentinel_Position,

        Event_Game_Won,
        Event_Game_Loss
    }
    
    public static void SubscribeToEvent(EventsType eventType, EventReceiver listener)
    {
        if (_events == null)
            _events = new Dictionary<EventsType, EventReceiver>();

        if (!_events.ContainsKey(eventType))
            _events.Add(eventType, null);

        _events[eventType] += listener;
    }
    
    public static void UnsubscribeToEvent(EventsType eventType, EventReceiver listener)
    {
        if (_events != null)
        {
            if (_events.ContainsKey(eventType))
                _events[eventType] -= listener;
        }
    }
    
    public static void TriggerEvent(EventsType eventType)
    {
        TriggerEvent(eventType, null);
    }
    
    public static void TriggerEvent(EventsType eventType, params object[] parameters)
    {
        if (_events == null)
        {
            Debug.Log("No events subscribed");
            return;
        }

        if (_events.ContainsKey(eventType))
        {
            if (_events[eventType] != null)
            {
                _events[eventType](parameters);
            }
        }
    }
}
