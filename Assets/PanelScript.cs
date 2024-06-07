using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class PanelScript : MonoBehaviour
{

    private RectTransform rt;
    public bool Appearing = false;

    public float Speed = 1f;

    public string Type = "";

    public int lowerBound = 0;
    public int upperBound = 0;

    public Button button1;

    private TextMeshProUGUI butt1text;
    public Button button2;
    private TextMeshProUGUI butt2text;
    private float buttonWidth = 0;

    
    public TMP_InputField InputText;

    private AirshipWorldScript AWS;

    private CenterScript SelectedCenter;
    // Start is called before the first frame update
    void Start()
    {
        AWS = GameObject.FindWithTag("Spawner").GetComponent<AirshipWorldScript>();
        rt = GetComponent<RectTransform>();

        buttonWidth = button1.GetComponent<RectTransform>().sizeDelta.x;

        transform.position = new Vector3(rt.sizeDelta.x/2,-(rt.sizeDelta.y/2),0);
        if(Type == "Build"){
            GameObject butt;
            for(var i = lowerBound; i <= upperBound; i++){
                if((-rt.sizeDelta.x/2+buttonWidth/2) + i*(buttonWidth+10) > (rt.sizeDelta.x/2)-buttonWidth/2){
                    butt = Instantiate(button1.gameObject,transform.position+new Vector3((-rt.sizeDelta.x*1.5f+buttonWidth) + i*(buttonWidth+10),-40,0),Quaternion.identity,transform) as GameObject;
                    
                }else{
                    butt = Instantiate(button1.gameObject,transform.position+new Vector3((-rt.sizeDelta.x/2+buttonWidth/2) + i*(buttonWidth+10),40,0),Quaternion.identity,transform) as GameObject;
                }
                
                    ButtonScript BS = butt.GetComponent<ButtonScript>();
                    BS.MyGameObject = AWS.Blocks[i];
                    butt.name = AWS.Blocks[i].name;
                }
        }
        if(Type == "Edit"){
            butt1text = button1.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            butt2text = button2.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            

            butt1text.text = "None";
            butt2text.text = "None";
            

            
        }
    
    }

    // Update is called once per frame
    void Update()
    {       
        if(Type == "Build"){
            Appearing = AWS.Building;
            
            
        }

        if(Type == "Edit"){
            
            if(AirshipWorldScript.SelectedObj != null){
                Appearing = true;
                
                SelectedCenter = AirshipWorldScript.SelectedCenter;

                if(butt1text.text == "Press Key"){
                    foreach(KeyCode keycode in Enum.GetValues(typeof(KeyCode))){
                        if(Input.GetKey(keycode)) {
                            if(keycode == KeyCode.Delete){
                                butt1text.text = "None";
                                SelectedCenter.negInput = KeyCode.None;
                            }else{
                                butt1text.text = keycode.ToString();
                                
                                SelectedCenter.negInput = keycode;
                                
                            }                    
                        }
                    }
                }else{
                    if(SelectedCenter != null){
                    butt1text.text = SelectedCenter.negInput.ToString();
                    }
                }
                if(butt2text.text == "Press Key"){
                    foreach(KeyCode keycode in Enum.GetValues(typeof(KeyCode))){
                        if(Input.GetKey(keycode)) {
                            if(keycode == KeyCode.Delete){
                                butt2text.text = "None";
                                SelectedCenter.posInput =  KeyCode.None;
                            }else{
                                butt2text.text = keycode.ToString();
                                
                                SelectedCenter.posInput = keycode;
                                
                            }
                        }
                    }
                }else{
                    if(SelectedCenter != null){
                    butt2text.text = SelectedCenter.posInput.ToString();
                    }
                }

                


                

                bool Valid = false;
                if(SelectedCenter != null){
                    Valid = float.TryParse(InputText.text, out SelectedCenter.Power);
                }
            
        }else{
        Appearing = false;
        }

           
        }

        if(transform.position.y > 0-(rt.sizeDelta.y/2) && Appearing == false){
            transform.position = new Vector3(rt.sizeDelta.x/2,transform.position.y-Speed,0);
        }
        if(transform.position.y < (rt.sizeDelta.y/2) && Appearing == true){
            transform.position = new Vector3(rt.sizeDelta.x/2,transform.position.y+Speed,0);
        }

        
        
    
    }
    public void SetKey(int Index){
        
        if(Index == 1){
            butt1text.text = "Press Key";
        }
        if(Index == 2){
            butt2text.text = "Press Key";
        }
        
        
    }


}
