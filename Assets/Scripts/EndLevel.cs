using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public int count;
    public enum NextLevel
    {
        SameLevel,
        AnotherLevel,
        AnotherScene
    }
    public NextLevel next;

    [DrawIf("next", NextLevel.AnotherLevel)] public Level level;
    [DrawIf("next", NextLevel.AnotherLevel)] public int index;

    [DrawIf("next", NextLevel.AnotherScene)] public int indexScene;

    private void Start()
    {
        Level levelGet = FindObjectOfType<TestCSV>().levelToLoad;
        int indexLevel = FindObjectOfType<TestCSV>().numberLevel;
        next = levelGet.levels[indexLevel].next;

        switch (next)
        {
            case NextLevel.AnotherLevel:
                level = levelGet.levels[indexLevel].level;
                index = levelGet.levels[indexLevel].index;
                break;
            case NextLevel.AnotherScene:
                indexScene = levelGet.levels[indexLevel].indexScene;
                break;
        }
    }

    private void Update()
    {
        if(count == 2)
        {
            switch(next)
            {
                case NextLevel.SameLevel:
                    SwitchScene.index++;
                    SwitchScene.ResetScene();
                    break;
                case NextLevel.AnotherLevel:
                    SwitchScene.level = level;
                    SwitchScene.index = index;
                    SwitchScene.ResetScene();
                    break;
                case NextLevel.AnotherScene:
                    SwitchScene.LoadScene(indexScene);
                    break;
            }
        }
    }
}
