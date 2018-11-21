using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCamMove : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            transform.eulerAngles -= new Vector3(0, 1 * Time.deltaTime * 20, 0);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.eulerAngles += new Vector3(0, 1 * Time.deltaTime * 20, 0);
        }

        transform.position += new Vector3(Input.GetAxis("Vertical") * -10 * Time.deltaTime, 0, Input.GetAxis("Horizontal") * 10 * Time.deltaTime);

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.position -= new Vector3(0, 20 * Time.deltaTime,0);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.position += new Vector3(0, 20 * Time.deltaTime,0);
        }
    }
}
