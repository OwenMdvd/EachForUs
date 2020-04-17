using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public Text nameChapter;
    public GameObject parentButton;
    public GameObject button;
    public int scene;

    public void QuitAppli()
    {
        Application.Quit();
    }
    public void ResetScene()
    {
        SwitchScene.ResetScene();
    }
    public void ChangeScene(int coef)
    {
        SwitchScene.AddScene(coef);
    }
    public void LoadScene(Level level)
    {
        SwitchScene.level = level;
        SwitchScene.index = 0;
        SwitchScene.LoadScene(scene);
    }
    public void LoadLevel(Level level, int index)
    {
        SwitchScene.level = level;
        SwitchScene.index = index;
        SwitchScene.LoadScene(1);
    }
    public void LoadChapterLevel(Level level)
    {
        foreach (Transform item in parentButton.transform)
        {
            Destroy(item.gameObject);
        }
        nameChapter.text = level.Name;
        int i = 1;
        foreach (var item in level.levels)
        {
            GameObject go = Instantiate(button);
            go.transform.SetParent(parentButton.transform);
            go.GetComponent<Button>().onClick.AddListener(() => GetComponent<PauseManager>().LoadLevel(level, level.levels.IndexOf(item)));
            go.GetComponent<Image>().sprite = item.miniature;
            //go.GetComponentInChildren<Text>().text = i.ToString();
            i++;
        }
    }
}