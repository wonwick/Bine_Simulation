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
    public int startingBeed = 0;
    private Vector3 newGrowthDirection;
    private Vector3 collisionNormal;
    public float gravitrophismLimit = 45;
    public float gravitrophismCorrectionValue = 0.01f;
    public float gravitrophismAbsoluteLimit = 80;
    private bool supportLost=false;
    private Vector3 theUpward = new Vector3(0, 100, 0);
    public float supportLostGravitrophismAdjustment = 0.01f;
    public float reCircumnutateReadyAngle = 45;


    // Use this for initialization
    void Start() {
        thisGameObject =GameObject.Find("plant");
        plant = new GameObject[particleCount];
        lastBeed = Instantiate(beed, transform.position, transform.rotation);
        lastBeed.name = "beed0";
        lastBeed.transform.parent = thisGameObject.transform;
        //Vector3 initRotation = new Vector3(270, 0, 0); //sphericalbeed
       
        Vector3 initRotation = new Vector3(270, 0, 0); //cylindericalBeed

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
                if (supportLost)
                {
                    supportLostGrowth();
                }

                else if (!supportFound)
                {
                    SupportLessGrowth();
                }

                else {
                    Debug.Log("supportFoundGrowth\n");                                
                    //Debug.DrawRay(beedCollision.contacts[0].point, collisionNormal, Color.green, 60*5, false);
                    newGrowthDirection = collisionNormal;
                    newGrowthDirection = Vector3.RotateTowards(newGrowthDirection, lastBeed.transform.forward, 3, 0.0f);
                    //Debug.DrawRay(lastBeed.transform.position, newGrowthDirection*10, Color.red, 60*5, false);
                    SupportedGrowth(newGrowthDirection);
                    //supportFound = false;
                    circumnutationOn = true;


                }
            }
        }
        else {
            frameCount++;
            //check whether there is a colision on lastbeed
            
            

            if (circumnutationOn)
            {
                if (!supportFound)
                {
                    supportLessCircumnutation();
                }
                else
                {
                    //add new supportfoundCircumnutation here
                    supportedCircumnutation();
                }
            }
        }
    }

    void supportLostGrowth()
    {
        float gravitrophismDifference = Vector3.Angle(lastBeed.transform.forward, theUpward);
        Debug.Log("gravitrophismDifference:" + gravitrophismDifference);
        if (gravitrophismDifference > reCircumnutateReadyAngle)
        {
            circumnutationOn = false;
            Vector3 newRotation = Vector3.RotateTowards(lastBeed.transform.forward, theUpward, supportLostGravitrophismAdjustment, 0);
            lastBeed.transform.rotation = Quaternion.LookRotation(newRotation);
            newGrowthDirection = lastBeed.transform.forward;
            SupportedGrowth(newGrowthDirection);

        }
        else
        {
            lastBeed.transform.rotation = Quaternion.LookRotation(theUpward);
            startingBeed = currentNumberOfBeeds-1;
            Debug.Log("ready to Circumnutate!!");
            circumnutationOn = true;
            supportLost = false;
        }



    }

    void SupportedGrowth(Vector3 newGrowthDirection)
    {
        Vector3 newPosition = lastBeed.transform.position + Vector3.Normalize(newGrowthDirection)* beedDistance;
        DestroyImmediate(lastBeed.GetComponent<Collider>());
        DestroyImmediate(lastBeed.GetComponent<Beed>());
        lastBeed = Instantiate(beed, newPosition, lastBeed.transform.rotation);
        lastBeed.name = "Beed" + currentNumberOfBeeds;
        lastBeed.transform.parent = thisGameObject.transform;
        lastBeed.transform.Rotate(getRelativeTilt(currentNumberOfBeeds, maxBeedRotation));
        plant[currentNumberOfBeeds] = lastBeed;
        //Debug.DrawRay(lastBeed.transform.position, lastBeed.transform.forward * 2, Color.red, 5, false);
        currentNumberOfBeeds++;
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

    void supportLessCircumnutation() {
        plant[startingBeed].transform.Rotate(getRelativeTilt(1, circumnutationSpeed), Space.World);
        GameObject prevBeed = plant[startingBeed];
        for (int i= startingBeed+1; i < currentNumberOfBeeds; i++)
        {
            GameObject currentBeed = plant[i];
            Vector3 newPosition = prevBeed.transform.position + prevBeed.transform.forward * beedDistance;
            currentBeed.transform.position = newPosition;
            currentBeed.transform.Rotate(getRelativeTilt(i, circumnutationSpeed), Space.World);
            prevBeed = currentBeed;
        }
    }

    void supportedCircumnutation()
    {
        int supportedStartingBeed = startingBeed;
        //Debug.Log("supportedCircumnutation"+supportedStartingBeed+"\n");
        //Debug.DrawRay(plant[startingBeed].transform.position, plant[startingBeed].transform.forward * 2, Color.blue, 5*60, false);
        Vector3 newRotation = Vector3.RotateTowards(plant[supportedStartingBeed].transform.forward, collisionNormal * -1, 0.01f, 0);
        plant[supportedStartingBeed].transform.rotation = Quaternion.LookRotation(newRotation);

        float gravitrophismDifference = Vector3.Angle(plant[supportedStartingBeed].transform.forward, theUpward);
        //Debug.Log("the angle:" + gravitrophismDifference);
        if (gravitrophismDifference >= gravitrophismLimit)
        {
            //newRotation = Vector3.RotateTowards(plant[supportedStartingBeed].transform.forward, theUpward, gravitrophismCorrectionValue, 0);
            //plant[supportedStartingBeed].transform.rotation = Quaternion.LookRotation(newRotation);
            float addition = (gravitrophismDifference - gravitrophismLimit) / gravitrophismCorrectionValue;
            newRotation = plant[supportedStartingBeed].transform.forward;
            newRotation = new Vector3(newRotation.x, newRotation.y+Mathf.Deg2Rad* addition, newRotation.z);
            plant[supportedStartingBeed].transform.rotation = Quaternion.LookRotation(newRotation);
        }

        //Debug.DrawRay(plant[startingBeed].transform.position, collisionNormal * -5, Color.black, 60 * 5, false);
        //Debug.DrawRay(plant[startingBeed].transform.position, plant[startingBeed].transform.forward * 2, Color.red, 60*5, false);
        //Debug.DrawRay(plant[startingBeed].transform.position, newRotation, Color.yellow, 60 * 5, false);

        GameObject prevBeed = plant[supportedStartingBeed];
        for (int i = supportedStartingBeed + 1; i < currentNumberOfBeeds; i++)
        {
            GameObject currentBeed = plant[i];
            Vector3 newPosition = prevBeed.transform.position + prevBeed.transform.forward * beedDistance;
            currentBeed.transform.position = newPosition;
            newRotation= Vector3.RotateTowards(currentBeed.transform.forward, collisionNormal * -1, 0.01f, 0);
            currentBeed.transform.rotation = Quaternion.LookRotation(newRotation);
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
        startingBeed = currentNumberOfBeeds;
        collisionNormal = collision.contacts[0].normal;
        float collitionangleRelativeToUpward= Vector3.Angle(collisionNormal, theUpward);
    
        if (collitionangleRelativeToUpward < gravitrophismAbsoluteLimit)
        {
            Debug.Log("Support Lost !!!");
            supportFound = false;
            circumnutationOn = false;
            supportLost = true;
        }
        else {
            supportFound = true;
            circumnutationOn = false;

        }
    }



}
