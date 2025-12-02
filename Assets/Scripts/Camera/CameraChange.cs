using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//changes the camera to a different one depending on box colliders the player enters/leaves
//try to have the camera box collider overlap eachother where they meet so a camera is always atleast being made active

public class CameraChange :MonoBehaviour
{

    public GameObject areaCam;

//when objest tagged "player" enters the box collider the camera becomes active
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            areaCam.SetActive(true);
        }
    }
    
//when objest tagged "player" leaves the box collider the camera becomes inactive
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            areaCam.SetActive(false);
        }
    }
}
