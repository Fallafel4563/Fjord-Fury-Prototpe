using System.Collections;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplineBoat : MonoBehaviour
{
    public CinemachineCamera Camera;
    public CinemachineSplineCart dollykart;
    public SplineTrack CurrentTrack, MainTrack;
    public Transform ModelHolder;
    public Collider ColRef;
    bool grounded = true;
    bool jumping;
    float LRsteer;
    public float SteerSpeed=10, jumpPower=10, gravity=10, quickfallSpeed=20, ForwardSpeed=40,FrontBackOffsetLimit=3;
    float ySpeed,resetLerp, timeSinceJump, FwdInput;
    bool deathLerp, died;
    public GameObject DeathPos;
    Transform camDeathPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setSpeed(ForwardSpeed);
    }

    // Update is called once per frame
    void Update()
    {


        if(died) { Camera.transform.position =camDeathPos.position; }
        LerpPos();

        transform.localPosition += Vector3.right * LRsteer * Time.deltaTime * SteerSpeed;

        if (!grounded)
        {
            timeSinceJump += Time.deltaTime;
            transform.localPosition += Vector3.forward * ForwardSpeed * Time.deltaTime;
            transform.localPosition += Vector3.up * ySpeed*Time.deltaTime;
            ySpeed -= Time.deltaTime* gravity *timeSinceJump*timeSinceJump;
            if(!jumping) ySpeed -= Time.deltaTime*quickfallSpeed *timeSinceJump*timeSinceJump;
            
            if(jumping && ySpeed < 0) jumping = false;

            if (ySpeed < 0 && transform.localPosition.y < -25 && !died)
            {
                camDeathPos = Camera.transform;
                died = true;

                //missed the road, uh oh
                print("oops you died");

                jumping = false;
                dollykart.AutomaticDolly.Enabled = true;
                
                ySpeed = 0;

                StartCoroutine(DeathMoment());
                
                
                    
            }
        }
        else // grounded
        {

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Clamp(transform.localPosition.z + FwdInput*SteerSpeed *( 1.1f- Mathf.Abs( transform.localPosition.z)/FrontBackOffsetLimit ) * Time.deltaTime, -FrontBackOffsetLimit, FrontBackOffsetLimit));

            timeSinceJump = 0.2f;
            if (Mathf.Abs(transform.localPosition.x) > CurrentTrack.width)
            {
                transform.localPosition = new Vector3(CurrentTrack.width* Mathf.Sign(transform.localPosition.x), transform.localPosition.y, transform.localPosition.z);
            }

            if (dollykart.SplinePosition > 0.99f * CurrentTrack.Track.Spline.GetLength())
            {
                if (CurrentTrack.IsGrindRail)
                {
                    Jump();
                }
                CurrentTrack.OnEnd.Invoke();
            }

        }

        BoatAnim();
    }
    public void OnHorizontal(InputValue value)
    {
        float p = value.Get<float>();

        //transform.localPosition += Vector3.right * p;
        LRsteer = p;
    }
    public void OnVertical(InputValue value)
    {
        float f = value.Get<float>();

        FwdInput = f;
    }

    public void OnJump(InputValue value)
    {
        //print("JUMPS: "+ value.Get());
        if(grounded && value.Get()!= null) 
        {
            Jump();
        }
        else if(value.Get()== null) { jumping = false; }
    }

    void Jump()
    {
        if (CurrentTrack.IsGrindRail)
        {
            Vector3 globalpos = transform.position;
            CurrentTrack = MainTrack;
            dollykart.Spline = MainTrack.Track;
            //Find right relative position
            EvalInfo Dupeditt = MainTrack.EvaluateBasedOnWorldPosition(transform.position);
            Vector3 newpos = globalpos - Dupeditt.SplinePos;
            
            dollykart.SplinePosition = Dupeditt.t;

            transform.localPosition = newpos;

        }
        timeSinceJump = 0.2f;
            StartCoroutine(DisableColliderBriefly());
            //jump()
            dollykart.AutomaticDolly.Enabled = false;
            grounded = false;
            ySpeed = jumpPower;
            jumping = true;
    }

    public void OnH(InputValue inputValue)
    {
        dollykart.SplinePosition += 0.1f*CurrentTrack.Track.Spline.GetLength();
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

    IEnumerator DeathMoment()
    {
        yield return new WaitForSeconds(0.5f);
        died = false;
        deathLerp = true;
        resetLerp = .5f;
        grounded = true;
        ySpeed = 0;
        transform.localEulerAngles = Vector3.zero;
    
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
            ST.OnBoatEnter.Invoke();
            
            EvalInfo Dupeditt = ST.EvaluateBasedOnWorldPosition(transform.position);
            dollykart.SplinePosition = Dupeditt.t;


            ySpeed = 0;
            dollykart.AutomaticDolly.Enabled = true;
            transform.parent = dollykart.transform;

            //Debug.Log(Vector3.Distance(transform.position, Dupeditt.SplinePos) * Mathf.Sign(transform.localPosition.x));
            float Lpos = Vector3.Distance(transform.position, Dupeditt.SplinePos) * -1f;
            float Rpos = Vector3.Distance(transform.position, Dupeditt.SplinePos);
            float NewPos = Vector3.Distance(Dupeditt.SplinePos + Lpos * transform.right, transform.position) > Vector3.Distance(Dupeditt.SplinePos+Rpos * transform.right, transform.position)? Rpos:Lpos;

            transform.localPosition = new Vector3( NewPos,0,0);
        }
    }

    void BoatAnim()
    {
        Vector3 NewRot = transform.localEulerAngles;

        NewRot.x = -ySpeed * 2;
        NewRot.y = Mathf.LerpAngle(NewRot.y, LRsteer * SteerSpeed *2, Time.deltaTime*10); 
        NewRot.z = Mathf.LerpAngle(NewRot.z,-LRsteer*SteerSpeed,Time.deltaTime*5);
   //         -LRsteer * SteerSpeed;

        ModelHolder.localScale = grounded ? Vector3.one : new Vector3(Mathf.Clamp( timeSinceJump*timeSinceJump*1.5f - (jumping ? .2f : 0), 0.5f,1), Mathf.Clamp(1-timeSinceJump*timeSinceJump - (jumping?1f:0),Mathf.Max(timeSinceJump, 1),2f), Mathf.Clamp(timeSinceJump*timeSinceJump * 1.5f - (jumping ? .2f : 0), 0.5f, 1));

        transform.localEulerAngles = NewRot;
    }
}
