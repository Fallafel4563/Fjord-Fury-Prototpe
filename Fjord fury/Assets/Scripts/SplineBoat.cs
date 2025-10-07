using System.Collections;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplineBoat : MonoBehaviour
{
    public CinemachineSplineCart dollykart;
    public SplineTrack CurrentTrack;

    public Collider ColRef;
    bool grounded = true;
    bool jumping;
    float LRsteer;
    public float SteerSpeed=10, jumpPower=10, gravity=5, quickfallSpeed=20, ForwardSpeed=40;
    float ySpeed,resetLerp;
    bool deathLerp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setSpeed(ForwardSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        LerpPos();

        transform.localPosition += Vector3.right * LRsteer * Time.deltaTime * SteerSpeed;

     
        if (!grounded)
        {
            transform.localPosition += Vector3.forward * ForwardSpeed * Time.deltaTime;
            transform.localPosition += Vector3.up * ySpeed*Time.deltaTime;
            ySpeed -= Time.deltaTime*gravity;
            if(!jumping) ySpeed -= Time.deltaTime*quickfallSpeed;
            
            if(jumping && ySpeed < 0) jumping = false;

            if (ySpeed < 0 && transform.localPosition.y < -20)
            {
                //missed the road, uh oh
                print("oops you died");
                deathLerp = true;
                resetLerp = .5f;
                jumping = false;
                dollykart.AutomaticDolly.Enabled = true;
                grounded = true;
                ySpeed = 0;
                

                
                
                    
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
        //print("JUMPS: "+ value.Get());
        if(grounded && value.Get()!= null) 
        {
            StartCoroutine(DisableColliderBriefly());
            //jump()
            dollykart.AutomaticDolly.Enabled = false;
            grounded = false;
            ySpeed = jumpPower;
            jumping = true;
        }
        else if(value.Get()== null) { jumping = false; }
    }

    public void OnH(InputValue inputValue)
    {
        dollykart.SplinePosition += 0.1f*100000;
        print("HYPEERJUMP");   
    }
    void setSpeed(float newSpeed)
    {
        var autodolly = dollykart.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        if (autodolly != null)
        {

            autodolly.Speed = newSpeed;
        }

    }
    void LerpPos()
    {
        if(resetLerp > 0)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(deathLerp?0:transform.localPosition.x, 0, 0),1-resetLerp);
            resetLerp -= Time.deltaTime;
        }
        else
        {
            deathLerp = false;
        }
    }
    IEnumerator DisableColliderBriefly()
    {
        ColRef.enabled = false;
        yield return new WaitForSeconds(0.15f);
        ColRef.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        

        //Debug.Log(other.name);
        if (other.gameObject.TryGetComponent<SplineTrack>(out SplineTrack ST)&& !grounded)
        {
            if(CurrentTrack != ST)
            {
                //new TRACK!
                dollykart.Spline = ST.Track;
                CurrentTrack = ST;
            }
            //landing
            grounded=true;
            
            EvalInfo Dupeditt = ST.EvaluateBasedOnWorldPosition(transform.position);
            dollykart.SplinePosition = Dupeditt.t;


            ySpeed = 0;
            dollykart.AutomaticDolly.Enabled = true;
            transform.parent = dollykart.transform;

            Debug.Log(Vector3.Distance(transform.position, Dupeditt.SplinePos) * Mathf.Sign(transform.localPosition.x));
            transform.localPosition = new Vector3(Vector3.Distance(transform.position,Dupeditt.SplinePos) * Mathf.Sign(transform.localPosition.x), 0, 0);
        }
    }
}
