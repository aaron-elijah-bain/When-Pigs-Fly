using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterScript : MonoBehaviour
{
    public int[] Sides;
    public string[] Rots;

    public string Type = "";

    private PigMoveScript playerScript;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        Type = gameObject.name;
        Type = Type.Replace("(Clone)","");
        playerScript = GameObject.FindWithTag("Player").GetComponent<PigMoveScript>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Type == "Wheel"){
            if(playerScript.isDriving){
                playerScript.transform.position = transform.position + transform.forward * 1.3f;
            }
        }
        
    }
}
