using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public AnimationCurve curve;
    public List<Transform> players = new List<Transform>();
    public float[] distZoom;
    public float[] zoom;

    public float zoomSolo;
    [HideInInspector]
    public float distBase;
    [HideInInspector]
    public float zoomBase;


    // Start is called before the first frame update
    void Start()
    {
        players.Clear();
        foreach (var item in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(item.transform);
        }
        distBase = Vector3.Distance(players[0].position, players[1].position);
        zoomBase = transform.position.z;
        SetZoom();
        transform.position = new Vector3(CenterOfVectors().x, CenterOfVectors().y, zoomBase);
    }

    // Update is called once per frame
    void Update()
    {
        if (players.Count == 2)
            distBase = Vector3.Distance(players[0].position, players[1].position);
        else
            distBase = zoomSolo;

        SetZoom();
        
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(CenterOfVectors().x, CenterOfVectors().y, zoomBase), moveSpeed * Time.deltaTime);
        //transform.position = new Vector3(transform.position.x, transform.position.y , -(zoomBase * Vector3.Distance(players[0].position, players[1].position)) / distBase);
    }

    public Vector3 CenterOfVectors()
    {
        Vector3 sum = Vector3.zero;
        if (players == null || players.Count == 0)
        {
            return sum;
        }

        foreach (Transform vec in players)
        {
            sum += vec.position;
        }
        return sum / players.Count;
    }

    public void SetZoom()
    {
        for (int i = 0; i < distZoom.Length; i++)
        {
            if (i == 0)
            {
                if (distBase <= distZoom[i])
                {
                    zoomBase = -zoom[i];
                }
                else if (distZoom.Length > 1 && distBase > distZoom[i] && distBase <= distZoom[i + 1])
                {
                    zoomBase = -zoom[i + 1];
                }
                else if (zoom.Length > 1 && distZoom.Length == 1)
                {
                    if (distBase > distZoom[i])
                    {
                        zoomBase = -zoom[i + 1];
                    }
                }
            }
            else if (distZoom.Length > 2 && i > 0 && i < distZoom.Length - 1)
            {
                if (distBase > distZoom[i] && distBase <= distZoom[i + 1])
                {
                    zoomBase = -zoom[i + 1];
                }
            }
            else if (i == distZoom.Length - 1)
            {
                if (distBase > distZoom[i])
                {
                    zoomBase = -zoom[i + 1];
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (players.Count == 2)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(players[0].position, players[1].position);
            UnityEditor.Handles.BeginGUI();
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            UnityEditor.Handles.Label(CenterOfVectors(), Vector3.Distance(players[0].position, players[1].position).ToString(), style);
            UnityEditor.Handles.EndGUI();
        }
    }
}
