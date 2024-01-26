using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private float speed = 2f;

    public bool canMove = true;
    void Start()
    {
        transform.position = new Vector3(-1, 0, -1);


    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
        {
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, 0, -speed) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, 0, speed) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
        }
    }
}
