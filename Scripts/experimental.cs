using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class experimental : MonoBehaviour {

    GameObject go;
    // Use this for initialization
    void Start () {
        //GameObject go = this.gameObject;
    }

    // Update is called once per frame
    void Update () {
        
        Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward*10, Color.blue, 60 * 60, false);

    }
}
