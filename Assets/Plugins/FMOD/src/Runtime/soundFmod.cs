using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class soundFmod : MonoBehaviour
{
    [HideInInspector]
    public int focus;
    public bool showDebug = true;

    public List<sound> soundslist;

    private void Awake()
    {
        focus = 0;
        if (!GetComponent<Rigidbody>())
        {
            gameObject.AddComponent<Rigidbody>();
        }

        GetComponent<Rigidbody>().useGravity = false;
        foreach (var item in soundslist)
        {
            InitSound(item, true);
        }
    }

    void InitSound(sound item, bool firstInstance)
    {
        if (firstInstance)
        {
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(item.Event);
            item.instance = instance;
        }
        item.instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        item.instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, item.minDist);
        item.instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, item.MaxDist);
        RuntimeManager.AttachInstanceToGameObject(item.instance, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    private void Start()
    {
        PlaySound(sound.EmitterGameEvents.ObjectStart);
    }

    private void OnTriggerEnter(Collider other)
    {
        string[] nameTag = new string[2];
        nameTag[0] = other.tag;
        nameTag[1] = other.name;
        PlaySound(sound.EmitterGameEvents.TriggerEnter, nameTag);
    }

    public void CallCustomVoid(string nameVoid)
    {
        string[] nameTag = new string[1];
        nameTag[0] = nameVoid;
        PlaySound(sound.EmitterGameEvents.customVoid, nameTag);
    }

    private void OnTriggerExit(Collider other)
    {
        string[] nameTag = new string[2];
        nameTag[0] = other.tag;
        nameTag[1] = other.name;
        PlaySound(sound.EmitterGameEvents.TriggerExit, nameTag);
    }

    void SetParameter(FMOD.Studio.EventInstance e, string name, float value)
    {
        e.setParameterByName(name, value);
    }

    void PlaySound(sound.EmitterGameEvents type, params string[] nameOrTag)
    {
        foreach (var item in soundslist)
        {
            if(item.instance.isValid() == false)
            {
                InitSound(item, true);
            }
            if(item.allParameters.Count > 0)
            {
                foreach (var param in item.allParameters)
                {
                    for (int i = 0; i < param.triggerParam.Count; i++)
                    {
                        if (param.triggerParam[i] == type)
                        {
                            CheckType(true, item, param.triggerParam[i], false, param, i, nameOrTag);
                        }
                    }
                }
            }
            if(!string.IsNullOrEmpty(item.Event))
            {
                for (int i = 0; i < item.triggerEvent.Count; i++)
                {
                    if (item.triggerEvent[i] == type)
                    {
                        CheckType(false, item, item.triggerEvent[i], false, null, i, nameOrTag);
                    }
                }
                if (item.stopTriggerEvent.Count > 0)
                {
                    for (int i = 0; i < item.stopTriggerEvent.Count; i++)
                    {
                        if (item.stop[i])
                        {
                            //Debug.Log("Stop " + type + " Event " + item.Event + " Type Stop " + item.tri);
                            if (item.stopTriggerEvent[i] == type)
                            {
                                CheckType(false, item, item.stopTriggerEvent[i], true, null, i, nameOrTag);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Switch pour determiner le type de trigger
    /// </summary>
    /// <param name="isParam">S'il s'agit d'un param (true) ou un event (false)</param>
    /// <param name="item">Item pour acceder a son instance</param>
    /// <param name="type">Le type du trigger, valable pour parametre ou event</param>
    /// <param name="param">Si isParam true alors renseigner le param a modifie</param>
    /// <param name="index">L'index du paramTrigger pour avoir la value associée</param>
    /// <param name="nameOrTag">Si type trigger alors le nom ou tag de l objet</param>
    void CheckType(bool isParam, sound item, sound.EmitterGameEvents type, bool stop = false, sound.param param = null, int index = 0, params string[] nameOrTag)
    {
        //Debug.Log(item.Event + " Type " + type.ToString());
        switch (type)
        {
            case sound.EmitterGameEvents.None:
                Debug.Log("None");
                return;
            case sound.EmitterGameEvents.customVoid:
                if (isParam)
                {
                    if (param.tagNameCol[index] == nameOrTag[0])
                        SetParameter(item.instance, param.paramName, param.valueToChange[index]);
                }
                else
                {
                    if (!stop)
                    {
                        if (item.tagName[index] == nameOrTag[0])
                        {
                            InitSound(item, false);
                            item.instance.start();
                        }
                    }
                    else
                    {
                        if (item.tagNameExit[index] == nameOrTag[0])
                        {
                            item.instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        }
                    }
                }
                break;
            case sound.EmitterGameEvents.ObjectStart:
                if(isParam)
                {
                    SetParameter(item.instance, param.paramName, param.valueToChange[index]);
                }
                else
                {
                    if(!stop)
                    {
                        InitSound(item, false);
                        item.instance.start();
                    }
                    else
                    {
                        item.instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    }
                }
                break;
            case sound.EmitterGameEvents.TriggerEnter:
                if (isParam)
                {
                    if(param.tagNameCol[index] == nameOrTag[0] || param.tagNameCol[index] == nameOrTag[1])
                        SetParameter(item.instance, param.paramName, param.valueToChange[index]);
                }
                else
                {
                    if (!stop)
                    {
                        if (item.tagName[index] == nameOrTag[0] || item.tagName[index] == nameOrTag[1])
                        {
                            InitSound(item, false);
                            item.instance.start();
                        }
                    }
                    else
                    {
                        if (item.tagNameExit[index] == nameOrTag[0] || item.tagNameExit[index] == nameOrTag[1])
                        {
                            item.instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        }
                    }
                }
                break;
            case sound.EmitterGameEvents.TriggerExit:
                if (isParam)
                {
                    if (param.tagNameCol[index] == nameOrTag[0] || param.tagNameCol[index] == nameOrTag[1])
                        SetParameter(item.instance, param.paramName, param.valueToChange[index]);
                }
                else
                {
                    if (!stop)
                    {
                        if (item.tagName[index] == nameOrTag[0] || item.tagName[index] == nameOrTag[1])
                        {
                            InitSound(item, false);
                            item.instance.start();
                        }
                    }
                    else
                    {
                        if (item.tagNameExit[index] == nameOrTag[0] || item.tagNameExit[index] == nameOrTag[1])
                        {
                            item.instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        }
                    }
                }
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "FMOD/FMODEmitter.tiff", true);
        if (soundslist.Count > 0 && showDebug)
        {
            Gizmos.color = Color.white;
            if (soundslist[focus] != null)
            {
                Gizmos.DrawWireSphere(transform.position, soundslist[focus].minDist);
                Gizmos.DrawWireSphere(transform.position, soundslist[focus].MaxDist);
            }
        }
    }

    [System.Serializable]
    public class sound
    {
        [Header("Path Event")]
        public string Event;

        public enum EmitterGameEvents
        {
            None,
            ObjectStart,
            TriggerEnter,
            TriggerExit,
            customVoid
        }

        [Header("Type trigger start event")]
        public List<EmitterGameEvents> triggerEvent = new List<EmitterGameEvents>(1);
        public List<string> tagName = new List<string>(1);

        [Header("Type trigger stop event")]
        public List<bool> stop = new List<bool>();
        public List<EmitterGameEvents> stopTriggerEvent = new List<EmitterGameEvents>(1);
        public List<string> tagNameExit = new List<string>(1);

        public FMOD.Studio.EventInstance instance;

        [Header("Parameters event")]
        public List<param> allParameters = new List<param>();
        
        [Header("Event distance 3D sound")]
        public float minDist;
        public float MaxDist = 50f;

        [System.Serializable]
        public class param
        {
            public string paramName;
            public List<EmitterGameEvents> triggerParam = new List<EmitterGameEvents>(1);
            public List<string> tagNameCol = new List<string>(1);
            public List<int> valueToChange = new List<int>(1);
        }
    }
}
