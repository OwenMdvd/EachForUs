using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallingCustom : MonoBehaviour
{
    public soundFmod soundPlay;
    public string voidName;

    public soundFmod.sound.EmitterGameEvents eventTrigger;

    [DrawIf("eventTrigger", soundFmod.sound.EmitterGameEvents.TriggerEnter)] public string CollisionStringEnter;
    [DrawIf("eventTrigger", soundFmod.sound.EmitterGameEvents.TriggerExit)] public string CollisionStringExit;

    // Start is called before the first frame update
    void Start()
    {
        if(eventTrigger == soundFmod.sound.EmitterGameEvents.ObjectStart)
        {
            soundPlay.CallCustomVoid(voidName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (eventTrigger == soundFmod.sound.EmitterGameEvents.TriggerEnter)
        {
            if (other.tag == CollisionStringEnter || other.name.Contains(CollisionStringEnter))
            {
                soundPlay.CallCustomVoid(voidName);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (eventTrigger == soundFmod.sound.EmitterGameEvents.TriggerExit)
        {
            if (other.tag == CollisionStringExit || other.name.Contains(CollisionStringExit))
            {
                soundPlay.CallCustomVoid(voidName);
            }
        }
    }
}
