using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[ExecuteInEditMode]
public class TestCSV : MonoBehaviour
{
    public TextAsset txt;
    public bool refresh;
    public bool destroyChild;

    public CSVLoader tiles;
    public Level levelToLoad;
    public int numberLevel;

    private void Awake()
    {
        SpawnMap();
    }

    // Update is called once per frame
    void Update()
    {
        if(refresh)
        {
            refresh = false;
            SpawnMap();
        }
        if(destroyChild)
        {
            destroyChild = false;
            DestroyAllChild();
        }
    }

    public void DestroyAllChild()
    {
        foreach (Transform item in transform)
        {
            DestroyImmediate(item.gameObject);
        }
    }

    public void SpawnMap()
    {
        txt = levelToLoad.levels[numberLevel].csvLevel;
        var rows = txt.text.Split('\n').Select(l => l.Split(',').ToArray()).ToArray();
        string[,] test2D = To2D<string>(rows);
        for (int y = 0; y < test2D.GetLength(0); y++)
        {
            for (int x = 0; x < test2D.GetLength(1); x++)
            {
                foreach (var item in tiles.prefabCSV)
                {
                    if (item.ID.ToString() == test2D[y, x])
                    {
                        foreach (var pref in item.prefab)
                        {
                            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(pref as GameObject);
                            go.transform.parent = transform;
                            go.transform.localPosition = new Vector3(x * 1, y * -1, 0);
                        }
                    }
                }
            }
        }
    }

    static T[,] To2D<T>(T[][] source)
    {
        int FirstDim = source.Length;
        int SecondDim = source.GroupBy(row => row.Length).Single().Key; // throws InvalidOperationException if source is not rectangular

        var result = new T[FirstDim, SecondDim];
        for (int i = 0; i < FirstDim; ++i)
            for (int j = 0; j < SecondDim; ++j)
                result[i, j] = source[i][j];

        return result;
    }
}
