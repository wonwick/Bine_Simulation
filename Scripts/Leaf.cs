using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    public GameObject[] plant;
    public GameObject attachedBeed;
    public int destinationBeadNumber;
    public int currentBeadNumber;
    public float growthRate;
    public float currentScale;
    public bool FirstLeaf = true;
    public bool isSimpleGrowth = true;
    GameObject subleaf1;
    GameObject subleaf2;
    Vector3 targetEuler = new Vector3(0, 0, 0);
    public float startAngle = 85f;
    public float endAngle = 30;

    // Use this for initialization
    void Start()
    {
        growthRate = 0.001f;
        currentScale = 0;
        subleaf1 = gameObject.transform.GetChild(0).gameObject;
        subleaf2 = gameObject.transform.GetChild(1).gameObject;
        reset();

    }

    // Update is called once per frame
    void Update()
    {
        if (isSimpleGrowth)
        {
            simpleLeafGrowth();
        }
        else
        {
            complexLeafGrowth();

        }
    }

    void simpleLeafGrowth()
    { if (currentBeadNumber > 1)
        {
            if (currentBeadNumber < destinationBeadNumber)
            {
                Debug.Log(gameObject.name + " : " + currentBeadNumber + "\n");
                attachedBeed = plant[currentBeadNumber - 1];

            }
            //else {
            //    if (Vector3.Distance(gameObject.transform.position, plant[destinationBeadNumber + 1].transform.position) > 0.01)
            //    {
            //        gameObject.transform.position = Vector3.Slerp(gameObject.transform.position, plant[destinationBeadNumber + 1].transform.position, 0.01f);
            //       // gameObject.transform.forward = Vector3.Slerp(gameObject.transform.forward, plant[destinationBeadNumber + 1].transform.forward, 0.0001f);
            //    }
            //    else {
            //        destinationBeadNumber++;
            //    }   
            //}

            if (!FirstLeaf)
            {
                gameObject.transform.forward = attachedBeed.transform.forward;
            }
            if (currentScale < 1.5)
            {
                gameObject.transform.localScale = currentScale * new Vector3(1, 2, 1);
                currentScale = currentScale + growthRate;
            }
            gameObject.transform.position = attachedBeed.transform.position;
        }
    }

    void complexLeafGrowth()
    {
        simpleLeafGrowth();
        subleaf1.transform.localRotation = Quaternion.Lerp(subleaf1.transform.localRotation, Quaternion.Euler(0, -1*endAngle, 0), 0.0005f);
        subleaf2.transform.localRotation = Quaternion.Lerp(subleaf2.transform.localRotation, Quaternion.Euler(0, endAngle, 0), 0.0005f);
        //subleaf1.transform.localPosition = Vector3.Lerp(subleaf1.transform.localPosition, new Vector3(-1, 0, 0), 0.005f);
        //subleaf2.transform.localPosition = Vector3.Lerp(subleaf2.transform.localPosition, new Vector3(1, 0, 0), 0.005f);

    }

    private void reset()
    {
        currentScale = 0;
        subleaf1.transform.localRotation = Quaternion.Euler(0, startAngle, 0);
        subleaf2.transform.localRotation = Quaternion.Euler(0, -1*startAngle, 0);

    }
}
