using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class SplineTrack : MonoBehaviour
{
    public UnityEvent OnEnd;

    public SplineExtrude extruder;
    public bool WidthGizmo = true;
    public float width, multiplier;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        extruder.Radius = width* multiplier;
    }
    private void OnDrawGizmosSelected()
    {
        if(WidthGizmo)
        {
            Vector3 start= extruder.Spline.EvaluatePosition(0);
            Gizmos.DrawSphere(start+transform.position, width);
        }
    }
}
