using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform movePoint;
    public float moveSpeed;
    public LayerMask stopPlayer;
    public bool move;
    float timer;
    public float delayInput;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, movePoint.position) <= .05f && timer >= delayInput)
        {
            move = false;
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0), .2f, stopPlayer))
                {
                    movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
                    timer = 0;
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0,Input.GetAxisRaw("Vertical"), 0), .2f, stopPlayer))
                {
                    movePoint.position += new Vector3(0,Input.GetAxisRaw("Vertical"), 0);
                    timer = 0;
                }
            }
        }
        else
        {
            move = true;
        }

        timer += Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap") && !move)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        if (collision.CompareTag("End") && !move)
        {
            Camera.main.GetComponent<CameraController>().players.Remove(transform);
            Camera.main.GetComponent<EndLevel>().count++;
            collision.GetComponent<SpriteRenderer>().color = Color.white;
            collision.tag = "Untagged";
            Destroy(gameObject);
        }
    }
}
