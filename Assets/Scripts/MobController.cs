using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    public Transform movePoint;
    public float moveSpeed;
    public LayerMask stopPlayer;

    public bool upMove;
    public float delayMove;
    [Range(-1,1)]
    public int inverse;


    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    public IEnumerator MoveMob(float inputAxis)
    {
        yield return new WaitForSeconds(delayMove);

        if(upMove)
        {
            if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0, .5f * inverse, 0), .2f, stopPlayer))
            {
                movePoint.position += new Vector3(0, .5f * inverse, 0);
            }
            else
            {
                inverse *= -1;
                movePoint.position += new Vector3(0, .5f * inverse, 0);
            }
        }
        else
        {
            if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(.5f * inverse, 0, 0), .2f, stopPlayer))
            {
                movePoint.position += new Vector3(.5f * inverse, 0, 0);
            }
            else
            {
                inverse *= -1;
                movePoint.position += new Vector3(.5f * inverse, 0, 0);
            }
        }
    }
}
