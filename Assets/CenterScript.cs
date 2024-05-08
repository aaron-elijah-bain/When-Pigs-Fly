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

    private GameObject Rotor;

    private float RotorRot = 0f;

    public float RotorSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        Type = gameObject.name;
        Type = Type.Replace("(Clone)","");
        playerScript = GameObject.FindWithTag("Player").GetComponent<PigMoveScript>();
        rb = transform.parent.parent.GetComponent<Rigidbody>();

        MeshRenderer[] MRS = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mr in MRS){
            if(mr.gameObject != gameObject){
                Rotor = mr.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Type == "Wheel"){
            if(playerScript.isDriving){
                playerScript.transform.position = transform.position + transform.forward * 1.3f;
            }
            if(Input.GetKey(KeyCode.A)){
                RotorRot-=RotorSpeed;
            }else if(Input.GetKey(KeyCode.D)){
                RotorRot+=RotorSpeed;
            }else{
                if(RotorRot > 0){
                    RotorRot-=RotorSpeed;
                }else if(RotorRot < 0){
                    RotorRot+=RotorSpeed;
                }
            }

            Rotor.transform.rotation = Quaternion.AngleAxis(RotorRot,transform.forward);

        }
        if(Type == "Prop"){
            rb.velocity = transform.forward * 10;
        }
        
    }
}
