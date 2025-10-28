using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;


public class SplineTrack : MonoBehaviour
{
    public bool IsGrindRail;
    public UnityEvent OnBoatEnter,OnEnd;
    [NonSerialized] public SplineContainer Track;
    public SplineExtrude extruder;
    public bool WidthGizmo = true;
    public float width, multiplier;
    Vector3 lastSplinePos = Vector3.zero, lastPlayerPos = Vector3.zero;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Track = GetComponent<SplineContainer>();
        extruder = GetComponent<SplineExtrude>();
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
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(start+transform.position, width);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lastSplinePos, 10);
        Gizmos.color= Color.blue;
        Gizmos.DrawSphere(lastPlayerPos, 10);
        Gizmos.color=(Color.white);
        Gizmos.DrawLine(lastSplinePos, lastPlayerPos);

    }
    public EvalInfo EvaluateBasedOnWorldPosition(Vector3 WorldPos)
    {
        lastPlayerPos = WorldPos;
        
        SplineUtility.GetNearestPoint(Track[0], WorldPos - transform.position, out Unity.Mathematics.float3 nearest, out float t);
        print("T: " + t.ToString());
        t *= Track[0].GetLength();
        EvalInfo EvalInfo = new EvalInfo();
        EvalInfo.t = t;
        EvalInfo.SplinePos = new Vector3(nearest.x, nearest.y, nearest.z) + transform.position;
        lastSplinePos = EvalInfo.SplinePos;
        print("LAST SPLINE POS: " + lastSplinePos.ToString());
        return EvalInfo;
    }


}
    public struct EvalInfo
    {
        public float t;
        public Vector3 SplinePos;
    }
