using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipBulletScript : MonoBehaviour
{
    public float Speed = 10;
    private Rigidbody rb;

    public GameObject Explotion;

    private AudioSource myAudio;

    public GameObject ParentCryst;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * Speed, ForceMode.Impulse);
        myAudio = GetComponent<AudioSource>();

        myAudio.pitch = 4;
        
    }
    float counter = 0;
    // Update is called once per frame
    void Update()
    {
       counter += Time.deltaTime;
       if(counter > 10){
        OnHit(gameObject);
       }
       RaycastHit hit;
       if(Physics.Raycast(transform.position, transform.forward, out hit, 2 ,~LayerMask.GetMask("Bullet"))){
            if(hit.collider.gameObject != null){
            OnHit(hit.collider.gameObject);
            
            }   
       }

       if(myAudio.pitch > 0){
            myAudio.pitch -= 1f * Time.deltaTime;
       }
       
    }

    private void OnTriggerEnter(Collider other) {
        
            OnHit(other.gameObject); 
    }

    void OnHit(GameObject other){
        
        bool ShouldDestroy = true;
            
        AirshipBlockScript otherScript = other.GetComponent<AirshipBlockScript>();
        if(otherScript != null){
            if(otherScript.crystal.gameObject != ParentCryst){
            otherScript.BlockHeath-= 110;
            }
            
            if(otherScript.crystal.gameObject == ParentCryst){
                ShouldDestroy = false;
            }
        }
        if(ShouldDestroy){
            if(Explotion != null){
                GameObject explo = Instantiate(Explotion, transform.position, transform.rotation) as GameObject;
                explo.SetActive(true);
            }
            Destroy(gameObject);
            
            }
    }
}
