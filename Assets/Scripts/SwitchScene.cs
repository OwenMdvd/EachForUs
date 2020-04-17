using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SwitchScene : MonoBehaviour
{
    public static Level level;
    public static int index;
    public static int sceneIndex;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            AddScene();
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            AddScene(-1);
        }
    }

    public static void AddScene(int coef = 1)
    {
        int allScene = SceneManager.sceneCountInBuildSettings;
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (allScene > 0)
        {
            sceneIndex = (sceneIndex + 1 * coef) % allScene;
            if (sceneIndex < 0)
                sceneIndex = allScene - 1;

            SceneManager.LoadScene(sceneIndex);
        }
        StopAll();
    }

    public static  void ResetScene()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
        StopAll();
    }

    public static void LoadScene(int sceneIndexLoad)
    {
        SceneManager.LoadScene(sceneIndexLoad);
        StopAll();
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        List<RaycastResult> UIIgnore = new List<RaycastResult>();

        return results.Count > 0;
    }

    public static void StopAll()
    {
        string masterBusString = "Bus:/";
        FMOD.Studio.Bus masterBus;
        masterBus = FMODUnity.RuntimeManager.GetBus(masterBusString);
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
