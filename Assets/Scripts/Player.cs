using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask groundLayer, factoryLayer, entryPointsLayer;
    [SerializeField] private Line line;
    private List<Line> lines = new List<Line>();
    private bool getBuilding;


    private void Update() 
    { 
        MoveLine();
    }

    
    private Factory GetFactory() 
    { 
        RaycastHit hit; 
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); 
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, factoryLayer))
        {
            Factory factory = null;
            if (hit.transform.gameObject.layer == 3) factory = hit.transform.GetComponent<Factory>();
            return factory;
        }
        return null;
    }

    private EntryPoint GetEntryPoint()
    {
        RaycastHit hit; 
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); 
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, entryPointsLayer))
        {
            if (hit.transform.gameObject.layer == 7)
            {
                return  hit.transform.GetComponent<EntryPoint>();
            }
        }
        return null;
    }

    private void MoveLine()
    {
        if (Input.GetMouseButtonDown(0))
        {
            getBuilding = GetFactory();
            if (getBuilding)
            {
                var line = Instantiate(this.line);
                lines.Add(line);
                line.SetStartFactory(GetFactory());
            }
        }
        if (!getBuilding) return;
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                lines[^1].SetPosition(hit.point);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            var line = lines[^1];
            if (GetFactory() == null)
            {
                lines.Remove(lines[^1]);
            }
            line.SetEntryPoint(GetEntryPoint());
        }
    }
}
