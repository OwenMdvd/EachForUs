using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Creation")]
public class Level : ScriptableObject
{
    public List<levelAsset> levels = new List<levelAsset>();


    [System.Serializable]
    public class levelAsset
    {
        public TextAsset csvLevel;
        public Sprite miniature;
    }
}
