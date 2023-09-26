using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using Micosmo.SensorToolkit;
using Sirenix.OdinInspector;
using UnityEngine;
public class Ladder : Interactable
{
    public NavMeshSensor navMeshSensor_Down,navMeshSensor_Up;
    public Outlinable outlinable;
    [ReadOnly] public Vector2 range = Vector2.zero;
    [ReadOnly] public Vector3 downPoint, upPoint;
    // Start is called before the first frame update
    void Start()
    {
        Setting();
    }
    [Button]
    public void Setting()
    {
        navMeshSensor_Down.Pulse();
        navMeshSensor_Up.Pulse();
        outlinable.DrawingMode = OutlinableDrawingMode.Normal;
        outlinable.FrontParameters.Enabled = false;
    }

    public void DetectNavMesh_Up(IRayCastingSensor sensor)
    {
        var hit = sensor.GetObstructionRayHit();
        upPoint = hit.Point;
        range.y = upPoint.y;
    }
    public void DetectNavMesh_Down(IRayCastingSensor sensor)
    {
        var hit = sensor.GetObstructionRayHit();
        downPoint = hit.Point;
        range.x = downPoint.y;
    }

    public void DetectPlayer(GameObject g,Sensor s)
    {
        if (g.CompareTag(GameManager.s_player))
        {
            outlinable.FrontParameters.Enabled = true;
            isInteracting = false;
            currentInteractable = this;
        }
    }
    public void LostPlayer(GameObject g,Sensor s)
    {
        if (g.CompareTag(GameManager.s_player))
        {
            outlinable.FrontParameters.Enabled = false;
            isInteracting = false;
            if (currentInteractable==this) currentInteractable = null;
        }
    }
}
