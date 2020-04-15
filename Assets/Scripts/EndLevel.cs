using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public int count;

    private void Update()
    {
        if(count == 2)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
