using UnityEngine;
using System.Collections;

/// <summary>
/// This Class is the FruitLauncher(it also launches Bombs, and PowerUps Though).  Numerous Instances of this Class
/// are controller by the FruitLauncherController.
/// </summary>
public class FruitLauncher : MonoBehaviour
{


    public float force;                                     // the force at which the fruit/other objects are launched
    private float forceMin, forceMax;                        // the min and max force the fruit/other objects will be launched at
    private float basedForceMin, basedForceMax;
    [Range(0f, -1f)]
    public float minXValue;                                 // the minimum value the fruit will be fired to the left(negative number)
    [Range(0f, 1f)]
    public float maxXValue;                                 // the minimum value the fruit will be fired to the right(positive number)
    public ObjectPoolScript[] fruitPoolScripts;             // the array of all the ObjectPoolScripts for the fruit
    public ObjectPoolScript[] bombAndPowerUpPoolScripts;    // the array of all the ObjectPoolScripts for the Bomb/PowerUps
    public AudioClip cannonThud;                            // the cannonThud is the audioClip
    private bool useAudioClip;                              // boolean that is set to true if we are to use the launch sound
    private GameObject fruitPoolsGameObject;                // the gameObject that is the parent to all of the empties with each ObjectPoolScript
    private GameObject bombAndPowerUpPoolsGameObject;
    //private GameObject character;
    // the gameObject that is the parent to all of the empties with each ObjectPoolScript


    private void Awake() {
        basedForceMin = 35;
        basedForceMax = 38;
    }

    private void Update() {
        forceMax = basedForceMax;
        forceMin = basedForceMin;
    }

    // Use this for initialization
    void Start()
    {
        //call method that setups the pool reference
        PoolReferenceSetup();
        //listWords = GetAllWord();
        //if cannonThud does not equal null then we...
        if (cannonThud != null)
        {
            //useAudioClip boolean is changed to true.
            useAudioClip = true;
        }
    }


    /// <summary>
    /// The PoolReferenceSetup() Method creates our references to the fruit pools(all of them), and the Bomb/PowerUp Pools.
    /// </summary>
    public void PoolReferenceSetup()
    {
        //find the GameObject Tagged "Fruit Pools".
        fruitPoolsGameObject = GameObject.FindGameObjectWithTag(Tags.FruitPools);
        //then fill the fruitPoolScripts array with the ObjectPoolScript in all the children of the fruitPoolsGameObject.
        fruitPoolScripts = fruitPoolsGameObject.GetComponentsInChildren<ObjectPoolScript>();

        //find the GameObject Tagged "OtherPools".
        bombAndPowerUpPoolsGameObject = GameObject.FindGameObjectWithTag(Tags.OtherPools);

        //then fill the bombAndPowerUpPoolScripts array with the ObjectPoolScript in all the children of the bombAndPowerUpPoolsGameObject.
        bombAndPowerUpPoolScripts = bombAndPowerUpPoolsGameObject.GetComponentsInChildren<ObjectPoolScript>();
    }

    

    /// <summary>
    /// The Method that Loads and Fires Random Fruit... Per the Name.
    /// </summary>
    public void LoadAndFireRandomFruit(char c)
    {
        //force equals a random number between the forceMin and forceMax
        force = Random.Range(forceMin, forceMax);
        //create a new float "randomXValue" get assigned Random.Range between minXValue and maxXValue
        float randomXValue = Random.Range(minXValue, maxXValue);
        //create a new Vector3 expVec get assigned a new Vector3 (  we pass randomXValue for X  ,  1 for Y  , and  0 for Z  )
        Vector3 expVec = new Vector3(randomXValue, 1, 0);
        //call method that gets fruit from pool and make a variable to hold the reference.


        GameObject character = RetrieveFruitFromPool(c);
        Rigidbody rb = character.GetComponent<Rigidbody>();
        rb.velocity = transform.TransformDirection(expVec * force);
        character.transform.position = transform.position;

        if (useAudioClip)
        {
            //we playClipAtPoint to instantiate a oneShotAudio (our cannonThud clip, at position 0,0,-80(is closest to the camera (audioListener)
            AudioSource.PlayClipAtPoint(cannonThud, new Vector3(0, 0, -80f));
        }
    }

    /// <summary>
    /// This Method Fires all of the other non-"regular fruit" Objects.  The parameter determines what
    /// is retrieved from the pool.
    /// 0 = ScoreX2Banana, 1 = FrenzyBanana, 2 = FreezeBanana, 3 = Minus10pointsBomb, and 4 = GameOverBomb
    /// ...these are just the order that GetComponentInChildren adds them to the "other" Pooled objects
    /// list.  Whatever order they are in the SceneHierarchy(Since we "find" the pools parent GameObject),
    /// is the order they populate the list in.  If you re-arrange the order in the scene view/prefabs then
    /// they will be different.  That makes this kind of a sloppy way of doing this, but It'll do for now...
    /// </summary>
    /// <param name="type"></param>
    public void LoadAndFireOtherObject(int type)
    {
        //force gets assigned Random.Range (between forceMin and forceMax)... 
        force = Random.Range(forceMin, forceMax);
        //create a new randomXValue and assign it a Random Value between minXValue and maxXValue...
        float randomXValue = Random.Range(minXValue - 0.3f, maxXValue + 0.3f);
        //create a new expVec = new Vector ( randomXValue for X , 1 for Y, 0 for Z )
        Vector3 expVec = new Vector3(randomXValue, 1, 0);

        //call method that gets fruit from pool and make a variable to hold the reference.
        GameObject clone = RetrieveOtherFromPool(type)/*Instantiate(fruitPrefabs[Random.Range(0,fruitPrefabs.Length)], spawnPoints[randomNum].transform.position, spawnPoints[randomNum].transform.rotation)*/ as GameObject;
        //Create Rigidbody gets assigned the clones rigidbody (the clone is our launched bomb or power-up)
        Rigidbody rb = clone.GetComponent<Rigidbody>() as Rigidbody;
        //we use TransformDirection(  expVec multiplied by our force  ) and assign it to rb.velocity
        rb.velocity = transform.TransformDirection(expVec * force);
        //if type equals 3 or type equals 4...
        if (type == 4)
        {
            rb.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //new int randomDir gets a random number between 0 and 2
            int randomDir = Random.Range(0, 2);
            //if that randomDir is 1 than...
            if (randomDir == 1)
            {
                //our rb gets some torque of new Vector3 (  we torque on the z-axis , with forcemode.impulse (instant b/c we aren't in update/fixed update  )
                rb.AddTorque(new Vector3(0, 0, Random.Range(0.15f, 0.45f)), ForceMode.Impulse);
            }
            //else if its 0...
            else
            {
                // we add torque on the z-axis the opposite direction
                rb.AddTorque(new Vector3(0, 0, Random.Range(-0.15f, -0.45f)), ForceMode.Impulse);
            }

        }
    }


    /// <summary>
    /// This Method Grabs us a Fruit from the pool puts it in position and activates it.  This method is
    /// called from the "LoadAndFireRandomFruit()"
    /// </summary>
    /// <returns></returns>
    public GameObject RetrieveFruitFromPool(char c)
    {
        string s = "Char" + c.ToString().ToUpper() + "Pool";
        GameObject poolChar = GameObject.Find(s);
        GameObject obj = poolChar.GetComponent<ObjectPoolScript>().GetPooledObject();
        if (obj != null)
        {
            obj.transform.rotation = Quaternion.Euler(0, 180, 0);
            obj.SetActive(true);
            return obj;
        } else {
            Debug.Log(s);
        }
        return null;
    }


    /// <summary>
    /// This Method Grabs us a Bomb or a PowerUp from the pool puts it in position and activates it. This method is
    /// called from the "LoadAndFireOtherObject()"
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject RetrieveOtherFromPool(int type)
    {
        //create a ObjectPoolScript var and name it tempPool and then assign it the element at position "type" in the array.
        ObjectPoolScript tempPool = bombAndPowerUpPoolScripts[type];
        //new gameObject "obj" gets a pooled object from tempPool reference using GetPooledObject.
        GameObject obj = tempPool.GetPooledObject();
        //if the obj equals null...
        if (obj == null)
        {
            //return null
            return null;
        }
        //the obj's position gets our position
        obj.transform.position = transform.position;
        //the obj's rotation gets our rotation
        obj.transform.rotation = transform.rotation;
        //the obj gets set active
        obj.SetActive(true);
        //return obj...
        return obj;
    }
}
