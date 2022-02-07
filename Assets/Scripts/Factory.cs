using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public enum FactoryType
{
    FlagMaker, ColorFactory, MergeFactory, RotateFactory, LogoFactory
}
public class Factory : MonoBehaviour
{
    [SerializeField] private int size = 2;
    [SerializeField] private EntryPoint[] entryPoints;
    
    public FactoryType factoryType;
    
    private List<EntryPoint> targetEntryPoints;
    private List<Line> myLines;
    
    //Flag Maker
    [HideInInspector] public Flag flag;
    [HideInInspector] public float timeToMakeFlag = 1f;
    
    // Change Color
    [HideInInspector] public Color flagColor;
    [HideInInspector] public Renderer render;
    
    //Merge Factoty
    private Flag[] flags;
    private bool[] entryPointsFilled;
    
    //Logo Factory
    [HideInInspector] public Texture2D logo;

    private void Start()
    {
        targetEntryPoints = new List<EntryPoint>();
        flags = new Flag[entryPoints.Length];
        entryPointsFilled = new bool[entryPoints.Length];
        myLines = new List<Line>();
        for (int i = 0; i < entryPoints.Length; i++)
        {
            entryPoints[i].factory = this;
            entryPoints[i].index = i;
        }
    }

    public void SetTargetedEntryPoint(EntryPoint entryPoint, Line line)
    { 
        myLines.Add(line);
        targetEntryPoints.Add(entryPoint);
        line.transform.SetParent(transform);
        if(factoryType == FactoryType.FlagMaker) StartCoroutine(MakeFlag(entryPoint,myLines.Count - 1));
    }

    private IEnumerator MakeFlag(EntryPoint targetPoint, int index)
    {
        while (index < myLines.Count && myLines[index] != null)
        {
            var c = Instantiate(flag, transform.position, Quaternion.identity);
            c.SetTargetEntryPoint(targetPoint);
            c.transform.SetParent(transform);
            yield return new WaitForSeconds(timeToMakeFlag);
        }
    }
    public bool IsFull()
    {
        return myLines.Count >= size;
    }
    

    
    public void FlagReachBuilding(Flag localFlag, int index)
    {
        entryPointsFilled[index] = true;
        if (targetEntryPoints.Count == 0)
        {
            Destroy(localFlag.gameObject);
            return;
        }
        switch (factoryType)
        {
            case FactoryType.ColorFactory:
                localFlag.SetColor(flagColor);
                break;
            case FactoryType.MergeFactory:
                MergeFlags(localFlag, index);
                return;
            case FactoryType.RotateFactory:
                localFlag.RotateUv();
                break;
            case FactoryType.LogoFactory:
                localFlag.SetLogo(logo);
                break;
        }
        localFlag.SetPosition(transform.position);
        localFlag.SetTargetEntryPoint(targetEntryPoints[0]);
    }

    private void MergeFlags(Flag savedFlag, int index)
    {
        flags[index] = savedFlag;
        var full = 0;
        var allFull = 0;
        for (int i = 0; i < flags.Length; i++)
        {
            if (flags[i] == null) full++;
            if (entryPointsFilled[i]) allFull++;
        }

        if(allFull == 1) return;
        if (allFull <= 2) if (full > 1) return;
        if (allFull > 2) if (full > 0) return;
        
        flags[index].SetPosition(transform.position);
        flags[index].SetTargetEntryPoint(targetEntryPoints[0]);

        if (flags[0] == null)
        {
            flags[index].MergeColor(flags[1].GetColor(),flags[2].GetColor(),Color.clear);
        }
        else if(flags[1] == null)
        {
            flags[index].MergeColor(flags[0].GetColor(),flags[2].GetColor(),Color.clear);
        }
        else if(flags[2] == null)
        {
            flags[index].MergeColor(flags[0].GetColor(),flags[1].GetColor(),Color.clear);
        }
        else
        {
            flags[index].MergeColor(flags[0].GetColor(),flags[1].GetColor(),flags[2].GetColor());
        }
        for (int i = 0; i < flags.Length; i++)
        {
            if (flags[i] != null && i != index)Destroy(flags[i].gameObject);
            flags[i] = null;
        }
    }
}

#region Editor

#if UNITY_EDITOR

[CustomEditor(typeof(Factory))]
public class FactoryEditor: Editor
{ 
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var factory = (Factory)target;
        switch (factory.factoryType)
        {
            case FactoryType.FlagMaker:
                factory.flag = (Flag)EditorGUILayout.ObjectField("Flag", factory.flag, typeof(Flag));
                factory.timeToMakeFlag = EditorGUILayout.Slider("Time To Make Flag", factory.timeToMakeFlag, 1, 9);
                break;
            case FactoryType.ColorFactory:
                factory.flagColor = EditorGUILayout.ColorField("Color", factory.flagColor);
                factory.render = (Renderer) EditorGUILayout.ObjectField("Render", factory.render, typeof(Renderer));
                break;
            case FactoryType.LogoFactory:
                factory.logo = (Texture2D)EditorGUILayout.ObjectField("Logo", factory.logo, typeof(Texture2D));
                break;
        }
        if (GUI.changed) 
        {
            if (factory.factoryType == FactoryType.ColorFactory)
            {
                factory.render.material.color = factory.flagColor;
            }
            EditorUtility.SetDirty(factory);
        }
    }
}
#endif

#endregion



