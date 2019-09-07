using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bine : MonoBehaviour {
    bool addLeaf = true;
    public int currentNumberOfBeeds = 0;
    public int currentNumberOfLeaves = 0;
    public float leafGrowthRate;
    public GameObject seed;
    public GameObject leaf;
    public GameObject newLeaf;
    private int frameCount = 0;
    public int growthTime = 50;
    public int particleCount = 10;
    public GameObject beed;
    public float radius = 1;
    GameObject[] plant;
    GameObject[] leaves;
    GameObject lastBeed;
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
    public float growthAngleBias = 30;
    public float supportedCircumnutationSpeed = 1;
    TubeRenderer tubeRenderer;
    Vector3[] beedPositions;
    float[] girths;
    public float maxGirth = 2;
    public bool hasGroped = false;
    private bool IKInitiated = false;
    public int NoOfIKIterations = 50;
    private bool groping = false;
    Collider collidingObject;
    Collider collidingBeed;
    Leaf theLeaf;
    // Use this for initialization
    void Start() {
        thisGameObject = this.gameObject;
        seed = GameObject.Find("seed1");
        plant = new GameObject[particleCount];
        leaves = new GameObject[particleCount / 5];
        beedPositions = new Vector3[particleCount];
        girths = new float[particleCount];
        lastBeed = Instantiate(beed, transform.position, transform.rotation);
        lastBeed.name = thisGameObject.name +"_beed_0";
        lastBeed.transform.parent = thisGameObject.transform;
        //Vector3 initRotation = new Vector3(270, 0, 0); //sphericalbeed
        tubeRenderer=thisGameObject.GetComponent<TubeRenderer>();

        Vector3 initRotation = new Vector3(270, 0, 0); //cylindericalBeed

        lastBeed.transform.Rotate(initRotation);
        plant[currentNumberOfBeeds] = lastBeed;
        currentNumberOfBeeds++;
        theLeaf =leaf.GetComponent<Leaf>();
        theLeaf.FirstLeaf = true;
        theLeaf.plant = plant;
        theLeaf.destinationBeadNumber = 5;
        theLeaf.currentBeadNumber = currentNumberOfBeeds;
        leaves[0] = leaf;
        currentNumberOfLeaves++;
    }
    //this is called in fixed intervals
    private void FixedUpdate() {
        if (frameCount >= growthTime)
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

                else if (!hasGroped)
                {
                    initGrope();
                    groping = true;
                }

                else
                {
                    Debug.Log("supportFoundGrowth\n");
                    //Debug.DrawRay(beedCollision.contacts[0].point, collisionNormal, Color.green, 60*5, false);
                    newGrowthDirection = collisionNormal;

                    newGrowthDirection = Vector3.RotateTowards(lastBeed.transform.forward, newGrowthDirection, Mathf.Deg2Rad * growthAngleBias, 0.0f);
                    //Debug.DrawRay(lastBeed.transform.position, collisionNormal*20, Color.yellow, 60 * 5, false);           
                    //Debug.DrawRay(lastBeed.transform.position, lastBeed.transform.forward, Color.blue, 60*5, false);
                    //Debug.DrawRay(lastBeed.transform.position, newGrowthDirection * 2, Color.red, 60 * 5, false);
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
            else if (groping)
            {
                grope();
                
            }
        }
        beedPositions = new Vector3[currentNumberOfBeeds];
        for (int i = 0; i < currentNumberOfBeeds; i++)
        {   
            beedPositions[i] = plant[i].transform.localPosition;
            girths[i] = tapperFunction_1(i,currentNumberOfBeeds, particleCount, maxGirth);
        }
        //tubeRenderer.SetPoints(beedPositions, 1, Color.cyan);
        if (currentNumberOfBeeds > 1) {
            tubeRenderer.SetBinePoints(beedPositions, girths, Color.cyan);
        }
        
        LeafGrowth();
    }

    void LeafGrowth()
    {
        if ((currentNumberOfBeeds % 5 == 0) && addLeaf)
        {
            newLeaf = Instantiate(leaf, leaf.transform.position, leaf.transform.rotation);
            newLeaf.transform.rotation = Quaternion.Euler(newLeaf.transform.rotation.eulerAngles + new Vector3(0, 90,0));
            theLeaf = newLeaf.GetComponent<Leaf>();
            theLeaf.FirstLeaf = false;
            theLeaf.plant = plant;
            theLeaf.destinationBeadNumber = currentNumberOfBeeds + 5;
            theLeaf.currentBeadNumber = currentNumberOfBeeds;
            leaves[currentNumberOfLeaves] = newLeaf;
            currentNumberOfLeaves++;
            addLeaf = false;
        }
        if (!(currentNumberOfBeeds % 5 == 0))
        {
            addLeaf = true;

        }
        for (int i = 0; i< currentNumberOfLeaves; i++)
        {
            theLeaf = leaves[i].GetComponent<Leaf>();
            theLeaf.currentBeadNumber = currentNumberOfBeeds;

        }
            
    }


    void grope()
    {
        if (collidingBeed.bounds.Intersects(collidingObject.bounds))
        {
            Vector3 newDirection = lastBeed.transform.forward;
            newDirection.y = 0.0f;
            Vector3 translatingDirection = Vector3.RotateTowards(Vector3.Normalize(newDirection), -1*Vector3.Normalize(collisionNormal), 1, 0);
            lastBeed.transform.Translate(0.001f * translatingDirection);
           //Vector3 newRotation = Vector3.RotateTowards(lastBeed.transform.forward, collisionNormal, supportLostGravitrophismAdjustment, 0);
            //lastBeed.transform.rotation = Quaternion.LookRotation(newRotation);
            Debug.Log("goaping: translating");

        }
        else {
            groping = false;
            circumnutationOn = true;
            hasGroped = true;

        }


    }

    void initGrope()
    {
        
        //if (!IKInitiated) {
        //    circumnutationOn = false;
        //    Debug.Log("initialting IK\n");
        //    IKSolver theIKSolver =lastBeed.AddComponent<IKSolver>() as IKSolver;
        //    GameObject g;
        //    g = new GameObject();
        //    g.name = "IKpoletarget";
        //    theIKSolver.poleTarget = g.transform;
        //    theIKSolver.enable = false;
        //    theIKSolver.endPointOfLastBone= lastBeed.transform;
        //    theIKSolver.iterations = NoOfIKIterations;
        //    IKSolver.Bone bone;
        //    Debug.Log("current bones: " + currentNumberOfBeeds+"\n");
        //    IKSolver.Bone[] bones = new IKSolver.Bone[currentNumberOfBeeds-1];
        //    for (int i=1;i<currentNumberOfBeeds; i++)
        //    {
        //        bone = new IKSolver.Bone();
        //        bone.bone= plant[currentNumberOfBeeds - i-1].transform;
        //        bones[i - 1] = bone;
        //        //Debug.Log("bone added: "+ bone.bone.name+ "\n");

        //    }
        //    Debug.Log("adding bones to IK solver\n");
        //    theIKSolver.bones = bones;
        //    IKInitiated = true;
        //    Debug.Log("initialting IKSolver completes\n");            
        //    theIKSolver.enable = true;
        //    theIKSolver.Initialize();

        //    Debug.Log("DONE initialting IKSolver\n");

        //}

        
        circumnutationOn = false;




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
        lastBeed = Instantiate(beed, newPosition, Quaternion.LookRotation(newGrowthDirection));

        //Instantiate(beed, newPosition, Quaternion.LookRotation(newGrowthDirection));

        lastBeed.name = "Beed" + currentNumberOfBeeds;
        lastBeed.transform.parent = thisGameObject.transform;
        //lastBeed.transform.Rotate(getRelativeTilt(currentNumberOfBeeds, maxBeedRotation));
        plant[currentNumberOfBeeds] = lastBeed;
        //Debug.DrawRay(lastBeed.transform.position, lastBeed.transform.forward * 2, Color.red, 5, false);
        currentNumberOfBeeds++;
    }



    void SupportLessGrowth() {
        Vector3 newPosition = lastBeed.transform.position + lastBeed.transform.forward * beedDistance;
        // removing the colider of previouse beed
        //DestroyImmediate(lastBeed.GetComponent<Collider>());
        //DestroyImmediate(lastBeed.GetComponent<Beed>());
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
        int supportedStartingBeed = startingBeed-1;
        //Debug.Log("supportedCircumnutation"+supportedStartingBeed+"\n");
        //Debug.DrawRay(plant[startingBeed].transform.position, plant[startingBeed].transform.forward * 2, Color.blue, 5*60, false);
        Vector3 newRotation = Vector3.RotateTowards(plant[supportedStartingBeed].transform.forward, collisionNormal * -1, 0.01f, 0);
        //Debug.DrawRay(plant[startingBeed].transform.position, plant[startingBeed].transform.forward * 2, Color.blue, 5*60, false);
        plant[supportedStartingBeed].transform.rotation = Quaternion.LookRotation(newRotation);

      

        //Debug.DrawRay(plant[startingBeed].transform.position, collisionNormal * -5, Color.black, 60 * 5, false);
        //Debug.DrawRay(plant[startingBeed].transform.position, plant[startingBeed].transform.forward * 2, Color.red, 60*5, false);

        GameObject prevBeed = plant[supportedStartingBeed];
        for (int i = supportedStartingBeed + 1; i < currentNumberOfBeeds; i++)
        {
            GameObject currentBeed = plant[i];
            Vector3 newPosition = prevBeed.transform.position + prevBeed.transform.forward * beedDistance;
            currentBeed.transform.position = newPosition;
            newRotation= Vector3.RotateTowards(currentBeed.transform.forward, collisionNormal * -1, Mathf.Deg2Rad* supportedCircumnutationSpeed, 0);
            currentBeed.transform.rotation = Quaternion.LookRotation(newRotation);

            //grvitrophism adjustment
            newRotation = plant[supportedStartingBeed].transform.forward;
            newRotation = new Vector3(newRotation.x, Mathf.Deg2Rad * gravitrophismLimit, newRotation.z);
            newRotation = Vector3.RotateTowards(currentBeed.transform.forward, newRotation, Mathf.Deg2Rad * gravitrophismCorrectionValue, 0);
            currentBeed.transform.rotation = Quaternion.LookRotation(newRotation);

            //Debug.DrawRay(currentBeed.transform.position, newRotation * 0.5f, Color.cyan, 60 * 5, false);
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
        collidingObject = collision.collider;
        collidingBeed = lastBeed.GetComponent<Collider>();
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

    float tapperFunction_0(int i, int currentBeedCount, int maxBeedCount, float maxGirth)
    {
        return maxGirth;
    }

    float tapperFunction_1(int i,int currentBeedCount, int maxBeedCount, float maxGirth)
    {
        return (maxGirth* (currentBeedCount-i) / maxBeedCount);
    }

    float tapperFunction_2(int i, int currentBeedCount, int maxBeedCount, float maxGirth)
    {
        float girth = Mathf.Pow(2, -1*i/10);
        return girth;
        //return (maxGirth * (currentBeedCount - i) / maxBeedCount);
    }

}
