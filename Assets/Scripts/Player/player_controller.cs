using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour
{

    public GameObject head;
    private Transform headTransform;

    // Start is called before the first frame update
    void Start()
    {
        headTransform = head.transform;
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
