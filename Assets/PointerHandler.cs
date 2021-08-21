using System;
using Oculus.Platform;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerHandler : MonoBehaviour
{
    private LineRenderer lineRenderer = null;
    private float delta = 0.0f;

    [SerializeField] private GameObject tip;

    [SerializeField] private float increment = 0.02f;
    [SerializeField] private float maxPointerLength = 30f;
    
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.2f;
    
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        
        delta = increment;
    }
    
    private void Update()
    {
        if (((OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) >= 0.5f) && !tip.activeSelf) ||
            (tip.activeSelf && (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) < 0.5f)))
        {
            TogglePointer();
        }

        if ((OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) >= 0.2f) && !tip.activeSelf)
        {
            UpdateLength();
        }
        if (!tip.activeSelf) return;
        UpdateLength();

        // if (Input.GetKey(KeyCode.A))
        // {
        //     CalculateDelta(0.5f);
        // } else if (Input.GetKey(KeyCode.D))
        // {
        //     CalculateDelta(-0.5f);
        // }
        //
        // return;
        var thumbstickPos = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
        if (thumbstickPos != 0)
        {
            CalculateDelta(thumbstickPos);
        }
    }
    
    private void UpdateLength()
    {
        var start = transform.position;
        var end = Vector3.SmoothDamp(lineRenderer.GetPosition(1), transform.position + transform.forward * delta, ref velocity, smoothTime);
        
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        tip.transform.position = end;
        tip.transform.rotation = transform.rotation;
    }

    private void CalculateDelta(float factor)
    {
        var newDelta = delta + Math.Sign(factor) * factor * factor * increment;
        var lineLength = Vector3.Distance(lineRenderer.GetPosition(0), transform.position + transform.forward * newDelta);
        if ((lineLength <= increment) || (lineLength > maxPointerLength))
        {
            return;
        }
        delta = newDelta;
    }

    private void TogglePointer()
    {
        tip.SetActive(!tip.activeSelf);
        lineRenderer.enabled = !lineRenderer.enabled;
    }
}