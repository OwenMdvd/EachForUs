using UnityEditor;
using UnityEngine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;

public class FmodSetup : EditorWindow
{
    public static FmodSetup Instance { get; private set; }
    public static bool IsOpen
    {
        get { return Instance != null; }
    }

    static bool trigger;

    bool toggle;

    const string CacheAssetName = "FMODStudioCache";
    const string CacheAssetFullName = "Assets/Plugins/FMOD/Resources/" + CacheAssetName + ".asset";
    EventCache events;

    int index;

    List<string> refString = new List<string>();

    GameObject soundSelect;
    GameObject lastSound;
    soundFmod[] allSound = new soundFmod[0];
    int indexForce;
    bool doOnce;
    int indexSound;
    int indexSearch;
    int countRefresh;
    int lastIndex;
    bool noEventObj;

    void OnEnable()
    {
        Instance = this;
        trigger = false;

        events = AssetDatabase.LoadAssetAtPath(CacheAssetFullName, typeof(EventCache)) as EventCache;
        refString.Add("");
        foreach (var item in events.EditorEvents)
        {
            refString.Add(item.ToString().Replace(" (FMODUnity.EditorEventRef)", "")); 
        }

        doOnce = true;
        countRefresh = 1;
    }

    [MenuItem("Window/Fmod Editor %f")]
    public static void ShowWindow()
    {
        FmodSetup window = (FmodSetup)EditorWindow.GetWindow(typeof(FmodSetup));
        if (!FmodSetup.trigger)
        {
            window.Show();
        }
        else
        {
            window.Close();
        }
        FmodSetup.trigger = !FmodSetup.trigger;
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        if(lastSound != soundSelect)
        {
            doOnce = true;
            lastSound = soundSelect;
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Switch entre 'soundFmod'", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(allSound.Length > countRefresh)
        {
            List<string> allName = new List<string>();
            foreach (var item in allSound)
            {
                if(item != null)
                    allName.Add(item.name);
            }
            indexForce = EditorGUILayout.Popup(indexForce, allName.ToArray());
            //Debug.Log("indexForce " + indexForce + " allName.count " + allName.Count);
            if (allSound[indexForce].gameObject != null)
            {
                soundSelect = allSound[indexForce].gameObject;
                Selection.activeGameObject = soundSelect;
                doOnce = true;
            }
        }
        if (GUILayout.Button(new GUIContent("Refresh", "Refresh la liste d'objet 'soundFmod'")))
        {
            allSound = GameObject.FindObjectsOfType<soundFmod>();
            noEventObj = false;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (allSound.Length > countRefresh)
        {
            indexSearch = EditorGUILayout.Popup(indexSearch, refString.ToArray());

            if (true)
            {
                if (GUILayout.Button(new GUIContent("Chercher", "Chercher les objets avec tel Event")))
                {
                    allSound = GameObject.FindObjectsOfType<soundFmod>();
                    List<soundFmod> soundTempo = new List<soundFmod>();
                    foreach (var item in allSound)
                    {
                        if (item.soundslist.Count > 0)
                        {
                            foreach (var items in item.soundslist)
                            {
                                if (items.Event == refString[indexSearch] && !soundTempo.Contains(item))
                                {
                                    soundTempo.Add(item);
                                }
                            }
                        }
                    }
                    if (soundTempo.Count < 1)
                    {
                        noEventObj = true;
                    }
                    else
                    {
                        allSound = soundTempo.ToArray();
                        countRefresh = 0;
                        noEventObj = false;
                    }
                }
                if (GUILayout.Button(new GUIContent("X", "Annule la recherche")))
                {
                    countRefresh = 1;
                    allSound = GameObject.FindObjectsOfType<soundFmod>();
                    indexSearch = 0;
                    noEventObj = false;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(noEventObj)
        {
            EditorGUILayout.HelpBox("Pas d'objet avec cet Event", MessageType.Warning);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("<--", "Precedent 'soundFmod'")))
        {
            RefreshNodeForce(-1);
        }
        if (GUILayout.Button(new GUIContent("-->", "Prochain 'soundFmod'")))
        {
            RefreshNodeForce();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Toggle Gizmos");
        toggle = GUILayout.Toggle(toggle, "");
        if(allSound.Length > 0)
        {
            foreach (var item in allSound)
            {
                item.showDebug = toggle;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Modification soundFmod", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Event Apply to prefab", EditorStyles.boldLabel);
        soundSelect = (GameObject)EditorGUILayout.ObjectField(soundSelect, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        if (soundSelect == null)
        {
            EditorGUILayout.HelpBox("Il manque un objet a modifié", MessageType.Warning);
        }
        else
        {
            if(!soundSelect.GetComponent<soundFmod>())
            {
                if (GUILayout.Button("Ajouter soundFmod"))
                {
                    soundSelect.AddComponent<soundFmod>().soundslist = new List<soundFmod.sound>();
                    indexSound = 0;
                }
                return;
            }
            soundSelect.GetComponent<soundFmod>().focus = indexSound;
            EditorGUILayout.BeginHorizontal();
            if (soundSelect.GetComponent<soundFmod>().soundslist.Count > 0)
            {
                if (GUILayout.Button(new GUIContent("-", "Enlever Emitter")))
                {
                    //Resize<soundFmod.sound>(soundSelect.GetComponent<soundFmod>().soundslist, soundSelect.GetComponent<soundFmod>().soundslist.Count - 1);
                    soundSelect.GetComponent<soundFmod>().soundslist.Remove(soundSelect.GetComponent<soundFmod>().soundslist[indexSound]);
                    indexSound = addIndexSound(soundSelect.GetComponent<soundFmod>().soundslist, -1);
                    Selection.activeGameObject = soundSelect;
                    doOnce = true;
                }
            }
            if (GUILayout.Button(new GUIContent("+", "Ajouter Emitter")))
            {
                Resize<soundFmod.sound>(soundSelect.GetComponent<soundFmod>().soundslist, soundSelect.GetComponent<soundFmod>().soundslist.Count+1);
                indexSound = soundSelect.GetComponent<soundFmod>().soundslist.Count - 1;
                Selection.activeGameObject = soundSelect;
                doOnce = true;
            }
            EditorGUILayout.EndHorizontal();

            if (soundSelect.GetComponent<soundFmod>().soundslist.Count > 1)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("<--", "Precedent sound dans soundFmod")))
                {
                    indexSound = addIndexSound(soundSelect.GetComponent<soundFmod>().soundslist, -1);
                    Selection.activeGameObject = soundSelect;
                    doOnce = true;
                }
                if (GUILayout.Button(new GUIContent("-->", "Prochain sound dans soundFmod")))
                {
                    indexSound = addIndexSound(soundSelect.GetComponent<soundFmod>().soundslist);
                    Selection.activeGameObject = soundSelect;
                    doOnce = true;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (soundSelect.GetComponent<soundFmod>().soundslist.Count > 0 && soundSelect.GetComponent<soundFmod>().soundslist[indexSound] != null)
            {
                EditorGUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Event");
                if(soundSelect.GetComponent<soundFmod>().soundslist[indexSound] != null)
                    soundSelect.GetComponent<soundFmod>().soundslist[indexSound].Event = EditorGUILayout.TextField(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].Event);
                if (doOnce)
                {
                    doOnce = false;
                    if (string.IsNullOrEmpty(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].Event))
                    {
                        index = 0;
                    }
                    else
                    {
                        index = refString.IndexOf(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].Event);
                    }
                }
                index = EditorGUILayout.Popup(index, refString.ToArray());
                soundSelect.GetComponent<soundFmod>().soundslist[indexSound].Event = refString[index];
                EditorGUILayout.EndHorizontal();

                if (!string.IsNullOrEmpty(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].Event))
                {
                    EditorGUILayout.Space(15);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Type trigger start event", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();
                    var countStart = soundSelect.GetComponent<soundFmod>().soundslist[indexSound].triggerEvent.Count;
                    countStart = EditorGUILayout.IntField("Nombre de trigger start:", countStart);
                    Resize<soundFmod.sound.EmitterGameEvents>(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].triggerEvent, countStart);
                    Resize<string>(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagName, countStart);
                    EditorGUILayout.EndHorizontal();


                    if (soundSelect.GetComponent<soundFmod>().soundslist[indexSound].triggerEvent.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Pas de start trigger, le son ne se lancera pas", MessageType.Info);
                    }
                    else
                    {
                        for (int i = 0; i < soundSelect.GetComponent<soundFmod>().soundslist[indexSound].triggerEvent.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.Space(5);
                            GUILayout.Label("Trigger Start: ");
                            soundSelect.GetComponent<soundFmod>().soundslist[indexSound].triggerEvent[i] = (soundFmod.sound.EmitterGameEvents)EditorGUILayout.EnumPopup(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].triggerEvent[i]);
                            if (soundSelect.GetComponent<soundFmod>().soundslist[indexSound].triggerEvent[i] == soundFmod.sound.EmitterGameEvents.TriggerEnter || soundSelect.GetComponent<soundFmod>().soundslist[indexSound].triggerEvent[i] == soundFmod.sound.EmitterGameEvents.TriggerExit)
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.Label("Name or Tag trigger: ");
                                soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagName[i] = EditorGUILayout.TextField(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagName[i]);
                            }
                            else if(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].triggerEvent[i] == soundFmod.sound.EmitterGameEvents.customVoid)
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.Label("Name CustomVoid: ");
                                soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagName[i] = EditorGUILayout.TextField(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagName[i]);
                            }
                            else
                            {
                                soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagName[i] = "";
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }


                    EditorGUILayout.Space(15);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Type trigger end event", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();
                    var countEnd = soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stopTriggerEvent.Count;
                    countEnd = EditorGUILayout.IntField("Nombre de trigger end:", countEnd);
                    Resize<soundFmod.sound.EmitterGameEvents>(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stopTriggerEvent, countEnd);
                    Resize<string>(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagNameExit, countEnd);
                    Resize<bool>(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stop, countEnd);
                    EditorGUILayout.EndHorizontal();


                    if (soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stopTriggerEvent.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Pas de end trigger, le son ne s'arretera pas s'il loop", MessageType.Info);
                        for (int i = 0; i < soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stop.Count; i++)
                        {
                            soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stop[i] = false;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stopTriggerEvent.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stop[i] = true;
                            GUILayout.Label("Trigger End: ");
                            soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stopTriggerEvent[i] = (soundFmod.sound.EmitterGameEvents)EditorGUILayout.EnumPopup(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stopTriggerEvent[i]);
                            if (soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stopTriggerEvent[i] == soundFmod.sound.EmitterGameEvents.TriggerEnter || soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stopTriggerEvent[i] == soundFmod.sound.EmitterGameEvents.TriggerExit)
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.Label("Name or Tag trigger: ");
                                soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagNameExit[i] = EditorGUILayout.TextField(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagNameExit[i]);
                            }
                            else if (soundSelect.GetComponent<soundFmod>().soundslist[indexSound].stopTriggerEvent[i] == soundFmod.sound.EmitterGameEvents.customVoid)
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.Label("Name CustomVoid: ");
                                soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagNameExit[i] = EditorGUILayout.TextField(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagNameExit[i]);
                            }
                            else
                            {
                                soundSelect.GetComponent<soundFmod>().soundslist[indexSound].tagNameExit[i] = "";
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }


                    EditorGUILayout.Space(10);

                    Resize<soundFmod.sound.param>(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters, events.EditorEvents[index - 1].Parameters.Count);
                    if (soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Pas de Param pour cet Event", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.Space(15);
                        for (int j = 0; j < events.EditorEvents[index - 1].Parameters.Count; j++)
                        {
                            if (soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j] != null)
                            {
                                soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].paramName = events.EditorEvents[index - 1].Parameters[j].Name;

                                EditorGUILayout.BeginHorizontal();
                                GUILayout.FlexibleSpace();
                                GUILayout.Label(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].paramName, EditorStyles.boldLabel);
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                var countParam = soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].triggerParam.Count;
                                countParam = EditorGUILayout.IntField("Nombre de trigger param:", countParam);
                                Resize<soundFmod.sound.EmitterGameEvents>(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].triggerParam, countParam);
                                Resize<string>(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].tagNameCol, countParam);
                                Resize<int>(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].valueToChange, countParam);
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.Space(5);


                                if (soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].triggerParam.Count == 0)
                                {
                                    EditorGUILayout.HelpBox("Pas de trigger, le son ne se sera pas modifier", MessageType.Info);
                                }
                                else
                                {
                                    for (int i = 0; i < soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].triggerParam.Count; i++)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Label("Trigger Start: ");
                                        soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].triggerParam[i] = (soundFmod.sound.EmitterGameEvents)EditorGUILayout.EnumPopup(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].triggerParam[i]);
                                        GUILayout.Label("Value to change: ");
                                        soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].valueToChange[i] = EditorGUILayout.IntSlider(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].valueToChange[i], (int)events.EditorEvents[index - 1].Parameters[0].Min, (int)events.EditorEvents[index - 1].Parameters[0].Max);
                                        if (soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].triggerParam[i] == soundFmod.sound.EmitterGameEvents.TriggerEnter || soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].triggerParam[i] == soundFmod.sound.EmitterGameEvents.TriggerExit)
                                        {
                                            GUILayout.FlexibleSpace();
                                            GUILayout.Label("Name or Tag trigger: ");
                                            soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].tagNameCol[i] = EditorGUILayout.TextField(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].tagNameCol[i]);
                                        }
                                        else if (soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].triggerParam[i] == soundFmod.sound.EmitterGameEvents.customVoid)
                                        {
                                            GUILayout.FlexibleSpace();
                                            GUILayout.Label("Name CustomVoid: ");
                                            soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].tagNameCol[i] = EditorGUILayout.TextField(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].tagNameCol[i]);
                                        }
                                        else
                                        {
                                            soundSelect.GetComponent<soundFmod>().soundslist[indexSound].allParameters[j].tagNameCol[i] = "";
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                            }
                        }
                    }

                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    soundSelect.GetComponent<soundFmod>().soundslist[indexSound].minDist = Mathf.Clamp(EditorGUILayout.FloatField("Min distance", soundSelect.GetComponent<soundFmod>().soundslist[indexSound].minDist), 0, soundSelect.GetComponent<soundFmod>().soundslist[indexSound].MaxDist) ;
                    soundSelect.GetComponent<soundFmod>().soundslist[indexSound].MaxDist = Mathf.Clamp(EditorGUILayout.FloatField("Max distance", soundSelect.GetComponent<soundFmod>().soundslist[indexSound].MaxDist), soundSelect.GetComponent<soundFmod>().soundslist[indexSound].minDist, 100);
                    if(soundSelect.GetComponent<soundFmod>().soundslist[indexSound].MaxDist == 0 && soundSelect.GetComponent<soundFmod>().soundslist[indexSound].minDist == 0)
                    {
                        EditorGUILayout.HelpBox("La max distance doit etre superieur à 0 pour entendre le son", MessageType.Error);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Enlever soundFmod"))
            {
                DestroyImmediate(soundSelect.GetComponent<soundFmod>());
                allSound = GameObject.FindObjectsOfType<soundFmod>();
            }
        }
    }



    void RefreshNodeForce(int coef = 1)
    {
        doOnce = true;
        noEventObj = false;
        allSound = GameObject.FindObjectsOfType<soundFmod>();
        if (allSound.Length > 0)
        {
            indexForce = (indexForce + 1 * coef) % allSound.Length;
            if (indexForce < 0)
                indexForce = allSound.Length - 1;

            soundSelect = allSound[indexForce].gameObject;
            Selection.activeGameObject = soundSelect;
        }
    }

    int addIndexSound(List<soundFmod.sound> sound, int coef = 1)
    {
        int indexReturn = 0;
        if (sound.Count > 0)
        {
            indexReturn = (indexSound + 1 * coef) % sound.Count;
            if (indexReturn < 0)
                indexReturn = sound.Count - 1;
        }
        return indexReturn;
    }

    public void Resize<T>(List<T> list, int newCount)
    {
        if (newCount <= 0)
        {
            list.Clear();
        }
        else
        {
            while (list.Count > newCount) list.RemoveAt(list.Count - 1);
            while (list.Count < newCount) list.Add(default(T));
        }
    }
}
