using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplineBoat : MonoBehaviour
{
    public CinemachineSplineCart dollykart;
    public SplineTrack CurrentTrack;

    bool grounded = true;
    bool jumping;
    float LRsteer;
    public float SteerSpeed=10, jumpPower=10, gravity=5, quickfallSpeed=20, ForwardSpeed=40;
    float ySpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setSpeed(ForwardSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += Vector3.right * LRsteer * Time.deltaTime * SteerSpeed;

     
        if (!grounded)
        {
            transform.localPosition += Vector3.up * ySpeed*Time.deltaTime;
            ySpeed -= Time.deltaTime*gravity;
            if(!jumping) ySpeed -= Time.deltaTime*quickfallSpeed;
            
            if(jumping && ySpeed < 0) jumping = false;

            if (ySpeed < 0 && transform.localPosition.y < 0)
            {
                if(Mathf.Abs(transform.localPosition.x) > CurrentTrack.width)
                {
                    //missed the road, uh oh
                    print("oops you died");
                    transform.localPosition = Vector3.zero;
                    jumping = false;
                    grounded = true;
                }
                else
                {
                    //landing
                    jumping = false;
                    grounded = true;
                    transform.localPosition = new Vector3(transform.localPosition.x,0,transform.localPosition.z);
                }
            }
        }
        else // grounded
        {
            if (Mathf.Abs(transform.localPosition.x) > CurrentTrack.width)
            {
                transform.localPosition = new Vector3(CurrentTrack.width* Mathf.Sign(transform.localPosition.x), transform.localPosition.y, transform.localPosition.z);
            }
        }

    }
    public void OnHorizontal(InputValue value)
    {
        float p = value.Get<float>();

        //transform.localPosition += Vector3.right * p;
        LRsteer = p;
    }

    public void OnJump(InputValue value)
    {
        print("JUMPS: "+ value.Get());
        if(grounded && value.Get()!= null) 
        {
            //jump()
            grounded = false;
            ySpeed = jumpPower;
            jumping = true;
        }
        else if(value.Get()== null) { jumping = false; }
    }

    void setSpeed(float newSpeed)
    {
        var autodolly = dollykart.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        if (autodolly != null)
        {
            autodolly.Speed = newSpeed;
        }

    }
}
