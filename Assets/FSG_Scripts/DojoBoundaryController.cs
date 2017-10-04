using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The DojoBoundary Class handles deactivating the fruit objects when the enter the colliders of the GameController Parent Object.  It also
/// places the Red X's at the position of the lost fruit in Classic Mode.  They are positioned just above the bottom of the screen.
/// </summary>
public class DojoBoundaryController : MonoBehaviour
{
    //public BoxCollider[] boundaryColliders;

    //private Vector3 resetPosition = new Vector3(0, 0, 0);         // the position that the fruit should be returned to after colliding with the boundary
    //private int usedRedXs;                                      // this is an int that represents the number of "red x's we are on".  The red X's that spawn in classic mode above lost fruit.
    public GameObject[] fruitMissedX;                           // this is an array of GameObjects that holds the red X's
    public float redXHeight;                                    // this is the height that the red X's spawn at when a fruit is lost.


    // OnTriggerEnter is called when the Collider other enters the trigger
    public void OnTriggerEnter(Collider other)
    {
        //if the other object that collides with us is a "Fruit"
        if (other.CompareTag("Fruit"))
        {
            if(other.GetComponent<Rigidbody>().velocity.y >= 0 && gameObject.tag == Tags.DojoBottom) {
                return;
            } else

            if (gameObject.tag != Tags.DojoBottom) {
                if (gameObject.tag == Tags.DojoTop) {
                    Vector3 vel = other.GetComponent<Rigidbody>().velocity;
                    other.GetComponent<Rigidbody>().velocity = new Vector3(vel.x, -vel.y * 1.5f, vel.z);
                } else {
                    Vector3 vel = other.GetComponent<Rigidbody>().velocity;
                    if (gameObject.tag == Tags.DojoLeft) {
                        other.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Abs(vel.x) * 1.5f, vel.y, vel.z);
                    } else {
                        other.GetComponent<Rigidbody>().velocity = new Vector3(-Mathf.Abs(vel.x) * 1.5f, vel.y, vel.z);
                    }
                }
            } else {

                GameObject fruitIntruder = other.gameObject;
                fruitIntruder.SetActive(false);
            }
            //we create a variable named fruitIntruder which we use to cache the reference to the "other" gameObject.

            ////if the current selected gameMode is Classic Mode
            //if (GameController.GameControllerInstance.gameModes == GameModes.Classic && GameVariables.FruitMissed < 3)
            //{
            //    //if "FruitMissed" variable of our "GameVariables" Static class is less than 3
            //    if (GameVariables.FruitMissed < 3 && GameController.GameControllerInstance.gameIsRunning)
            //    {
            //        //then... we access the first entry in our gameObject List (fruitMissedX), and we set it's position to a new Vector3 ((same x value as fruit , our redXHeight , same z value as fruit))
            //        fruitMissedX[usedRedXs].transform.position = /*fruitIntruder.transform.position +*/ new Vector3(fruitIntruder.transform.position.x, redXHeight, fruitIntruder.transform.position.z);
            //        //then we set the fruitMissedX[0 for first pass] redX to active.
            //        fruitMissedX[usedRedXs].SetActive(true);
            //        //we increment the usedRedXs++;
            //        usedRedXs++;

            //    }


            //}

            ////if we are NOT in Classic Mode

            ////set the impacted fruit inactive
            ////then we take the fruit and put it at position 0 , 0 , 0
            //fruitIntruder.transform.position = resetPosition;
            ////we also set the rotation to rotation 0 , 0 , 0
            //fruitIntruder.transform.eulerAngles = resetPosition;
            ////Fruit Missed gets increased.
            //GameVariables.FruitMissed++;
        }

        if (other.CompareTag("BombOrPowerUp"))
        {
            if(other.GetComponent<Rigidbody>().velocity.y >= 0 && gameObject.tag == Tags.DojoBottom) {
                return;
            } else
            if(gameObject.tag != Tags.DojoBottom) {
                if(gameObject.tag == Tags.DojoTop) {
                    Vector3 vel = other.GetComponent<Rigidbody>().velocity;
                    other.GetComponent<Rigidbody>().velocity = new Vector3(vel.x, -vel.y * 1.5f, vel.z);
                } else {
                    Vector3 vel = other.GetComponent<Rigidbody>().velocity;
                    if(gameObject.tag == Tags.DojoLeft) {
                        other.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Abs(vel.x) * 1.5f, vel.y, vel.z);
                    } else {
                        other.GetComponent<Rigidbody>().velocity = new Vector3(-Mathf.Abs(vel.x) * 1.5f, vel.y, vel.z);
                    }
                }
            } else
            other.gameObject.SetActive(false);
        }
    }

}
