using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;


public class SplineTrack : MonoBehaviour
{
    public UnityEvent OnEnd;
    [NonSerialized] public SplineContainer Track;
    public SplineExtrude extruder;
    public bool WidthGizmo = true;
    public float width, multiplier;
    
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
            Gizmos.DrawSphere(start+transform.position, width);
        }
    }
    public EvalInfo EvaluateBasedOnWorldPosition(Vector3 WorldPos)
    {

        SplineUtility.GetNearestPoint(Track[0], WorldPos, out Unity.Mathematics.float3 nearest, out float t);

        t *= Track[0].GetLength();
        EvalInfo EvalInfo = new EvalInfo();
        EvalInfo.t = t;
        EvalInfo.SplinePos = new Vector3(nearest.x, nearest.y, nearest.z);
        return EvalInfo;
    }


}
    public struct EvalInfo
    {
        public float t;
        public Vector3 SplinePos;
    }
