using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Creation")]
public class Level : ScriptableObject
{
    public string Name;
    public List<levelAsset> levels = new List<levelAsset>();

    [System.Serializable]
    public class levelAsset
    {
        public TextAsset csvLevel;
        public Sprite miniature;
        [Header("Enchainement Niveau")]
        public EndLevel.NextLevel next;

        [DrawIf("next", EndLevel.NextLevel.AnotherLevel)] public Level level;
        [DrawIf("next", EndLevel.NextLevel.AnotherLevel)] public int index;

        [DrawIf("next", EndLevel.NextLevel.AnotherScene)] public int indexScene;

        [Header("Zoom Camera")]
        public float[] distZoom;
        public float[] zoom;

        public float zoomSolo;
    }
}
