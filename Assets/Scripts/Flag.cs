using System;
using UnityEngine;

public class Flag : MonoBehaviour
{
    [SerializeField] [Range(0,6)]private float speed = 6f;
    [SerializeField] [Range(0, 1)] private float height;
    [SerializeField] private Paintable paintable;
    
    private Vector3 position;
    private Color myColor;
    private EntryPoint targetEntryPoints;


    private void Start()
    {
        Invoke(nameof(LateStart),Time.deltaTime);
    }

    private void LateStart()
    {
        SetColor(Color.white);
    }

    public void SetTargetEntryPoint(EntryPoint entryPoint)
    {
        targetEntryPoints = entryPoint;
        position = transform.position;
    }
    private void Update()
    {
        Movement();
    }
    private void Movement()
    {
        var target = targetEntryPoints.transform.position ;
        position = Vector3.MoveTowards(position,target , speed * Time.deltaTime);
        transform.LookAt(target + Vector3.up * height);
        transform.position = position + Vector3.up * height;
        var dist = Vector3.Distance(target, position);
        if (dist < 1f)
        {
            targetEntryPoints.factory.FlagReachBuilding(this, targetEntryPoints.index);
        }
    }
    public void SetPosition(Vector3 targetPosition)
    {
        position = targetPosition;
        transform.position = targetPosition;
    }
    
    
    
    
    
    public void SetColor(Color color)
    {
        myColor = color;
        PaintManager.instance.SetColor(paintable,color);
    }
    public void SetLogo(Texture logo)
    {
        PaintManager.instance.SetLogo(paintable,logo);
    }

    public void MergeColor(Color color1, Color color2, Color color3)
    {
        PaintManager.instance.MergeColors(paintable,color1,color2,color3);
    }
    public Color GetColor()
    {
        return myColor;
    }

    public void RotateUv()
    {
        
    }
}