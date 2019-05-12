using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beed : MonoBehaviour {

    public float acceptedCollitionAngle = 30;

	// Use this for initialization
	void Start () {
        Debug.Log("a new beed\n");
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {

  
    }

    void OnCollisionEnter(Collision collision)
    {
        
        
        
        float currentCollitionAngle = Vector3.Angle(this.gameObject.transform.forward, collision.contacts[0].normal*-1);

        Debug.Log("colided: " + currentCollitionAngle);

        if (currentCollitionAngle > acceptedCollitionAngle)
        {
            GameObject.Find("plant").GetComponent<Bine>().onHitSupportStructure(collision);

        }
        else {
            Debug.Log("not Accepted collition: " + currentCollitionAngle+"<"+acceptedCollitionAngle);
            DestroyImmediate(this.gameObject.GetComponent<Collider>());
            DestroyImmediate(this.gameObject.GetComponent<Beed>());
            
        }
            
    }
}
