using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirshipCrystalScript : MonoBehaviour
{
    private AirshipBlockScript MyBlock;
    private AirshipWorldScript AWS;
    private bool hasSaved = false;
    public string SaveString = "";
    public int BalloonCount = 0;
    public float MaxPower = 100;
    public float TotalPowerUsage = 0;

    public Vector3 ActualCOM = Vector3.zero;

    public bool IsAI = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        MyBlock = GetComponent<AirshipBlockScript>();
        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();
        
    }

    public void SpawnShip(){
        if(SaveString != ""){
                
                ship myShip = JsonUtility.FromJson<ship>(SaveString);
                
                for(int i = 0; i < myShip.shipParts.Count; i++){
                    shipPart part = myShip.shipParts[i];
                    
                    AWS.SpawnBlock(new shipPart(part.Type, transform.TransformPoint(part.Pos), part.Rot*transform.rotation, part.MyMirror, part.PosInput, part.NegInput, part.Speed, part.Decks,part.GunIndex),gameObject);
                    
                }
                
                MyBlock.myDecks = myShip.CrystDecks;
                
                
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag == "Crystal"){
            
            if(AWS.Building && AWS.Crystal == gameObject){
                transform.rotation = Quaternion.Euler(0,0,0);
                
                MyBlock.rb.isKinematic = true;
                transform.position = new Vector3(Mathf.Round(transform.position.x),AWS.BuildHeight,Mathf.Round(transform.position.z));
                
            }else{
                MyBlock.rb.isKinematic = false;
                MyBlock.rb.linearVelocity = Vector3.ClampMagnitude(MyBlock.rb.linearVelocity,10);

                
                
            }

            
            bool wasFound = false;
            foreach(GameObject Obj in AirshipWorldScript.SelectedObj){
                if(Obj == gameObject){
                    

                    if(hasSaved == false){
                        ship Ship = new ship(MyBlock.myDecks, new List<shipPart>());
                        
                        foreach(AirshipBlockScript child in GetComponentsInChildren<AirshipBlockScript>()){
                            if(child.gameObject.tag != "Crystal"){

                            Ship.shipParts.Add(new shipPart(child.Index,child.transform.localPosition, child.transform.localRotation, child.center.transform.localScale.x, child.centerScript.posInput, child.centerScript.negInput,child.centerScript.Speed, child.myDecks, child.gunIndex));
                            }
                            
                        }
                        SaveString = JsonUtility.ToJson(Ship);
                        hasSaved = true;
                    }
                    wasFound = true;
                    
                    
                }

            }
            if(!wasFound){
                hasSaved = false;
            }

            
        }

        if(IsAI){
            transform.position = new Vector3(transform.position.x, 30, transform.position.z) + -transform.forward*20*Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(10 * Time.deltaTime,Vector3.up) * transform.rotation;

        }
    }
}
