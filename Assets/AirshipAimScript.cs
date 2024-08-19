using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipAimScript : MonoBehaviour
{

    public Vector3 velocity;

    public int Steps = 10;
    public float gravityScale = 1;

    private LineRenderer lr;

    void Awake(){
        lr = GetComponent<LineRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPositions(CalculatePoints());
    }

    Vector3[] CalculatePoints(){

        Vector3[] Points = new Vector3[lr.positionCount+1];
        
        Vector3 a = transform.InverseTransformVector(Physics.gravity*gravityScale);

        for(int i = 0; i <= lr.positionCount; i++){
            int d = i*Steps;
            float t = d/velocity.magnitude;

            Vector3 p = transform.TransformPoint((velocity * t + (0.5f * a * (t * t))));

        
            Points[i] = p;

        }

        return(Points);

    }


}
