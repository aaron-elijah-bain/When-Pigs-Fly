using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour
{

    private RectTransform rt;
    public bool Appearing = false;

    public float Speed = 1f;

    public string Type = "";

    public int lowerBound = 0;
    public int upperBound = 0;

    public GameObject button;

    private float buttonWidth = 0;

    private AirshipWorldScript AWS;
    // Start is called before the first frame update
    void Start()
    {
        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();
        rt = GetComponent<RectTransform>();

        buttonWidth = button.GetComponent<RectTransform>().sizeDelta.x;

        transform.position = new Vector3(rt.sizeDelta.x/2,-(rt.sizeDelta.y/2),0);
        GameObject butt;
        for(var i = lowerBound; i <= upperBound; i++){
            if((-rt.sizeDelta.x/2+buttonWidth/2) + i*(buttonWidth+10) > (rt.sizeDelta.x/2)-buttonWidth/2){
                butt = Instantiate(button,transform.position+new Vector3((-rt.sizeDelta.x*1.5f+buttonWidth) + i*(buttonWidth+10),-40,0),Quaternion.identity,transform) as GameObject;
                
            }else{
                butt = Instantiate(button,transform.position+new Vector3((-rt.sizeDelta.x/2+buttonWidth/2) + i*(buttonWidth+10),40,0),Quaternion.identity,transform) as GameObject;
            }
            
                ButtonScript BS = butt.GetComponent<ButtonScript>();
                BS.MyGameObject = AWS.Blocks[i];
                butt.name = AWS.Blocks[i].name;
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        if(Type == "Build"){
            Appearing = AWS.Building;
            
            
        }
        if(transform.position.y > 0-(rt.sizeDelta.y/2) && Appearing == false){
            transform.position = new Vector3(rt.sizeDelta.x/2,transform.position.y-Speed,0);
        }
        if(transform.position.y < (rt.sizeDelta.y/2) && Appearing == true){
            transform.position = new Vector3(rt.sizeDelta.x/2,transform.position.y+Speed,0);
        }
    }
}
