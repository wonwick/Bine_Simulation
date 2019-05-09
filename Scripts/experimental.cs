using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class experimental : MonoBehaviour {

    GameObject go;
    // Use this for initialization
    void Start () {
       
    }

    // Update is called once per frame
    void Update () {
        go = GameObject.Find("GO");
        Debug.DrawRay(go.transform.position, go.transform.forward, Color.blue, 60 * 5, false);

    }
}
