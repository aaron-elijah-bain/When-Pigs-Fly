using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipExplosionScript : MonoBehaviour
{

    private SphereCollider myCollider;
    public List<GameObject> AlreadyHurt = new List<GameObject>();
    
    void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.enabled = false;
        myCollider.enabled = true;
        myCollider.radius = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 2.5f);

        if(myCollider.radius < 5){
            myCollider.radius += 0.5f;
        }
    }
    
    private void OnTriggerStay(Collider other){
        AirshipBlockScript otherScript = other.gameObject.GetComponent<AirshipBlockScript>();

        if(otherScript != null){
            bool wasFound = false;
            foreach(GameObject Obj in AlreadyHurt){
                if(Obj == otherScript.gameObject){
                    wasFound = true;
                }
            }
            if(wasFound == false){
                otherScript.BlockHeath -= 100;
                AlreadyHurt.Add(otherScript.gameObject);
            }
            
        }
    }
}
