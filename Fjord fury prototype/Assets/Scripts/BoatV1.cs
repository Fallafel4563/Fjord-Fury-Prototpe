using UnityEngine;
using UnityEngine.InputSystem;

public class BoatV1 : MonoBehaviour
{
    public bool snappyTurning = true;
    public float turnSpeed = 2;
    float TurnAngle = 45; //how many degrees it turns the boat on the y-axis
    float targetAngle = 0;
    float currentAngle;
    public float Acceleration = 10;

    Rigidbody body;
    public Transform modelHolder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!snappyTurning) currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime*turnSpeed);

        body.angularVelocity = Vector3.up * currentAngle*0.1f;
        body.AddForce(transform.forward * Acceleration, ForceMode.Acceleration);
        updateBoatAnim();
    }

    public void OnHorizontal(InputValue value)
    {
        if (value == null)
        {
            //go straight
            steer(0);
        }
        else
        {
            //steer
            steer(value.Get<float>());
        }

        print(value.Get());
    }

    void steer(float SteerDir)
    {
        //sets target angle of ship to the new value (negative = left, 0 = forward, positive = right)
        targetAngle = SteerDir*TurnAngle;
        if(snappyTurning) { currentAngle = targetAngle; }
        
    }

    void updateBoatAnim()
    {
        Vector3 newRotation = new Vector3(Mathf.Sin(Time.time), currentAngle, -0.5f* currentAngle);
        modelHolder.localEulerAngles = newRotation;
    }
}
