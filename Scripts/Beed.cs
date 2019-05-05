using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beed : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("a new beed\n");
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {

  
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("colided");
        GameObject.Find("plant").GetComponent<Bine>().onHitSupportStructure(collision);
    }
}
