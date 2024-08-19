using UnityEngine;

public class AirshipGunScript : MonoBehaviour
{
    public float BulletSpeed = 100;
    public float FireRate = 1;
    private float reload = 0;

    private CenterScript GunHolder;
    public GameObject Bullet;
    private AirshipAimScript Aim;
    private GameObject Barrel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GunHolder = transform.parent.gameObject.GetComponent<CenterScript>();
        Aim = GetComponentInChildren<AirshipAimScript>();
        Barrel = GetComponentInChildren<MeshRenderer>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(GunHolder.playerScript.isDriving  && GunHolder.AWS.Crystal == GunHolder.Crystalrb.gameObject  && GunHolder.AWS.Building == false){
            //Do gun things
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                float MouseDist = 1000;
                if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
                    if(hit.distance > 10){
                    MouseDist = hit.distance;
                    }
                }
                Vector3 mousePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, MouseDist));
                
                reload+=Time.deltaTime;
                if(reload > FireRate){
                    reload = 0;
                }
                if(Vector3.Angle(-transform.right*transform.parent.parent.localScale.x,mousePoint - transform.position) < 30){
                    Aim.gameObject.SetActive(true);
                    Aim.velocity = transform.InverseTransformPoint(mousePoint).normalized * BulletSpeed;
                    Barrel.transform.LookAt(mousePoint);
                if(Input.GetKey(GunHolder.posInput)){
                    if(reload == 0){
                    //GameObject bullet = Instantiate(Bullet,transform.position, Quaternion.FromToRotation(Vector3.forward,mousePoint - transform.position)) as GameObject;
                    GameObject bullet = Instantiate(Bullet,transform.position, Quaternion.identity) as GameObject;
                    bullet.transform.LookAt(mousePoint);                 
                    bullet.SetActive(true);

                    bullet.GetComponent<Rigidbody>().linearVelocity = GunHolder.Crystalrb.linearVelocity;
                    AirshipBulletScript BScript = bullet.GetComponent<AirshipBulletScript>();

                    BScript.ParentCryst = GunHolder.Crystalrb.gameObject;
                    BScript.Speed = BulletSpeed;
                    }
                    
                }
                Debug.DrawRay(transform.position, mousePoint - transform.position, Color.red);
                }else{
                    Aim.gameObject.SetActive(false);
                }
                    
                
                Debug.DrawRay(transform.position,-transform.right*transform.parent.localScale.x*100, Color.blue);
        }else{

            if(Aim != null){
                Aim.gameObject.SetActive(false);
            }
        }
    }
}
