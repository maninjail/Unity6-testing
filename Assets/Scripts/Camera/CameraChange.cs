using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange :MonoBehaviour
{

    public GameObject areaCam;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            areaCam.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            areaCam.SetActive(false);
        }
    }

}
