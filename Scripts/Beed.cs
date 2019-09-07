using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beed : MonoBehaviour {

    public float acceptedCollitionAngle = 30;
    public GameObject thePlant;

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
            thePlant.GetComponent<Bine>().onHitSupportStructure(collision);


        }
        else {
            Debug.Log("not Accepted collition: " + currentCollitionAngle+"<"+acceptedCollitionAngle);
            Destroy(this.gameObject.GetComponent<Collider>());
            Destroy(this.gameObject.GetComponent<Beed>());
            
        }
            
    }
}
