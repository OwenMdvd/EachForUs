using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CSV Object")]
public class CSVLoader : ScriptableObject
{
    public List<PrefabCSV> prefabCSV = new List<PrefabCSV>();

    [System.Serializable]
    public class PrefabCSV
    {
        public string nameTile;
        public int ID;
        public GameObject[] prefab;
    }
}
