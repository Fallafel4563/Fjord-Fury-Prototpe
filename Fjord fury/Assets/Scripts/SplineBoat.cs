using UnityEngine;
using UnityEngine.InputSystem;

public class SplineBoat : MonoBehaviour
{
    public Transform boatRef;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnHorizontal(InputValue value)
    {
        float p = value.Get<float>();

        transform.localPosition += Vector3.right * p;
    }
}
