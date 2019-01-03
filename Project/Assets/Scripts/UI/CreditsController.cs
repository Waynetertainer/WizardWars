using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour {

    public float pScrollSpeed;
    private Vector3 mover;
    // Use this for initialization
    void Start () 
    {
        mover = this.transform.position;

    }
	
	// Update is called once per frame
	void Update () 
    {
        mover.y += pScrollSpeed*Time.deltaTime;
	}
}
