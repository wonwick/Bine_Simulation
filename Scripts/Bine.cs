using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bine : MonoBehaviour {

    private int frameCount = 0;
    public int growthTime = 50;
    public int particleCount = 10;
    public GameObject beed;
    public float radius = 1;
    GameObject[] plant;
    GameObject lastBeed;
    private int currentNumberOfBeeds = 0;
    public float beedDistance = 1;
    public Vector3 maxBeedRotation = new Vector3(5, 5, 5);
    public Vector3 circumnutationSpeed = new Vector3(0, 5, 0);
    public bool circumnutationOn = true;
    private Collision beedCollision;
    private GameObject thisGameObject;
    private bool supportFound=false;

    // Use this for initialization
    void Start() {
        thisGameObject =GameObject.Find("plant");
        plant = new GameObject[particleCount];
        lastBeed = Instantiate(beed, transform.position, transform.rotation);
        lastBeed.name = "beed0";
        lastBeed.transform.parent = thisGameObject.transform;
        Vector3 initRotation = new Vector3(270, 0, 0);
        lastBeed.transform.Rotate(initRotation);
        plant[currentNumberOfBeeds] = lastBeed;

        currentNumberOfBeeds++;
    }
    //this is called in fixed intervals
    private void FixedUpdate() {
        if (frameCount == growthTime)
        {
            //Debug.Log("growthTime\n");
            frameCount = 0;
            if (currentNumberOfBeeds < particleCount)
            {
                if (!supportFound)
                {
                    SupportLessGrowth();
                }

                else {
                    Debug.Log("supportFoundGrowth\n");
                }
            }
        }
        else {
            frameCount++;
            //check whether there is a colision on lastbeed
            

            if (circumnutationOn)
            {
                circumnutation();
            }
        }
    }

    void SupportLessGrowth() {
        Vector3 newPosition = lastBeed.transform.position + lastBeed.transform.forward * beedDistance;
        DestroyImmediate(lastBeed.GetComponent<Collider>());
        DestroyImmediate(lastBeed.GetComponent<Beed>());
        lastBeed = Instantiate(beed, newPosition, lastBeed.transform.rotation);
        lastBeed.name = "Beed" + currentNumberOfBeeds;
        lastBeed.transform.parent = thisGameObject.transform;
        lastBeed.transform.Rotate(getRelativeTilt(currentNumberOfBeeds, maxBeedRotation));
        plant[currentNumberOfBeeds] = lastBeed;
        currentNumberOfBeeds++;
    }

    void circumnutation() {
        plant[0].transform.Rotate(getRelativeTilt(1, circumnutationSpeed), Space.World);
        GameObject prevBeed = plant[0];
        for (int i = 1; i < currentNumberOfBeeds; i++)
        {
            GameObject currentBeed = plant[i];
            Vector3 newPosition = prevBeed.transform.position + prevBeed.transform.forward * beedDistance;
            currentBeed.transform.position = newPosition;
            currentBeed.transform.Rotate(getRelativeTilt(i, circumnutationSpeed), Space.World);
            prevBeed = currentBeed;
        }
    }

    Vector3 getRelativeTilt(int currentBeedNumber, Vector3 angle) {
        float percentage = getPercentagee(currentNumberOfBeeds);
        return angle * percentage;
    }

    float getPercentagee(int currentBeedNumber) {
        return currentBeedNumber / currentNumberOfBeeds;
    }

    public void onHitSupportStructure(Collision collision) {
        beedCollision = collision;
        circumnutationOn = false;
        //supportFound = true;
    }



}
