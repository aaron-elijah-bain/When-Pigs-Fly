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

    private KeyCode Pos;

    private KeyCode Neg;

    private float Power = 0;
    private bool AllSame = true;

    private bool[] NeededUI = new bool[3];
    

    
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
                BS.Type = "Block";
                butt.name = AWS.Blocks[i].name;
            }
            for(int i = 0; i<AWS.Guns.Length+1; i++){
                butt = Instantiate(button1.gameObject,transform.position+new Vector3(550+(i*buttonWidth),-40,0),Quaternion.identity,transform) as GameObject;
                ButtonScript BS = butt.GetComponent<ButtonScript>();
                BS.num = i;
                BS.Type = "Gun";
                if(i != 0){
                butt.name = AWS.Guns[i-1].name;
                }else{
                    butt.name = "No Gun";
                }
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
        

            
            if(AirshipWorldScript.SelectedObj.Count != 0){

                bool needsSomthing = false;
                AllSame = true;
                Power = AirshipWorldScript.SelectedCenter[0].Speed;
                foreach(CenterScript myCenter in AirshipWorldScript.SelectedCenter){

                    if(myCenter.Speed != Power){
                        AllSame = false;
                    }
                     for(var i = 0; i< myCenter.UINeeds.Length; i++){//for every ui element
                            if(myCenter.UINeeds[i] == true){//if this block needs it, add it to the list
                                NeededUI[i] = true;
                                needsSomthing = true;
                            }
                        }
                }


                if(needsSomthing){
                Appearing = true;
                }else{
                    Appearing = false;
                }
                if(!InputText.isFocused){
                    if(AllSame == true){
                        InputText.text = Power.ToString();
                    }else{
                        InputText.text = "---";
                    }
                }

                
            
                if(AirshipWorldScript.SelectedCenter.Count == 1){

                    
                    
                    Neg = AirshipWorldScript.SelectedCenter[0].negInput;
                    Pos = AirshipWorldScript.SelectedCenter[0].posInput;


                }else{
                    
                    Neg = KeyCode.Backspace;
                    Pos = KeyCode.Backspace;
                }


                

                if(butt1text.text == "Press Key"){
                    foreach(KeyCode keycode in Enum.GetValues(typeof(KeyCode))){
                        if(Input.GetKey(keycode)) {
                            if(keycode == KeyCode.Delete){
                                butt1text.text = "None";
                                Neg = KeyCode.None;
                            }else{
                                butt1text.text = keycode.ToString();
                                
                                Neg = keycode;
                                
                            }                    
                        }
                    }
                }
                if(butt2text.text == "Press Key"){
                    foreach(KeyCode keycode in Enum.GetValues(typeof(KeyCode))){
                        if(Input.GetKey(keycode)) {
                            if(keycode == KeyCode.Delete){
                                butt2text.text = "None";
                                Pos =  KeyCode.None;
                            }else{
                                butt2text.text = keycode.ToString();
                                
                                Pos = keycode;
                                
                            }
                        }
                    }
                }
                    if(butt1text.text != "Press Key"){
                    butt1text.text =  AirshipWorldScript.SelectedCenter[0].negInput.ToString();
                    }
                    if(butt2text.text != "Press Key"){
                    butt2text.text =  AirshipWorldScript.SelectedCenter[0].posInput.ToString();
                    }

                    
                    foreach(CenterScript myCenter in AirshipWorldScript.SelectedCenter){
                        
                       

                        if(myCenter.posInput.ToString() != butt2text.text && butt2text.text != "Press Key"){
                            butt2text.text = "---";
                        }
                        if(myCenter.negInput.ToString() != butt1text.text && butt1text.text != "Press Key"){
                            butt1text.text = "---";
                        }
                        if(Pos != KeyCode.Backspace && myCenter.UINeeds[0]){
                            myCenter.posInput = Pos;
                        }
                        if(Neg != KeyCode.Backspace && myCenter.UINeeds[1]){
                            myCenter.negInput = Neg;
                        }
                        if(InputText.text != "" && InputText.text != "---"){
                        bool Success = float.TryParse(InputText.text, out myCenter.Speed);//Evenualy Slider
                        }
                
                    }
                    
                    
                    button2.gameObject.SetActive(NeededUI[0]); //if you need a button, show the button
                    button1.gameObject.SetActive(NeededUI[1]);
                    InputText.gameObject.SetActive(NeededUI[2]);

                    for(int i = 0; i< NeededUI.Length; i++){
                        NeededUI[i] = false;
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
        
        if(Index == 1 && button1.gameObject.activeInHierarchy){
            butt1text.text = "Press Key";
        }
        if(Index == 2 && button2.gameObject.activeInHierarchy ){
            butt2text.text = "Press Key";
        }
        
        
    }


}
