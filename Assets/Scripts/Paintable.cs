using System;
using UnityEngine;

public class Paintable : MonoBehaviour 
{
    const int TEXTURE_SIZE = 1024;
    public float extendsIslandOffset = 1;

    RenderTexture extendIslandsRenderTexture;
    RenderTexture uvIslandsRenderTexture;
    RenderTexture maskRenderTexture;
    RenderTexture supportTexture;
    Renderer rend;
    public Material paintMaterial;
    Material extendMaterial;
    public Shader texturePaint; 
    public Shader extendIslands;
    
    
    
    public RenderTexture GetMask() => maskRenderTexture;
    public RenderTexture GetUVIslands() => uvIslandsRenderTexture;
    public RenderTexture GetExtend() => extendIslandsRenderTexture;
    public RenderTexture GetSupport() => supportTexture;
    public Renderer GetRenderer() => rend;
    public Material GetPaintMaterial() => paintMaterial;
    public Material GetExtendMaterial() => extendMaterial;


    private void Awake()
    {
        paintMaterial = new Material(texturePaint);
        extendMaterial = new Material(extendIslands);
    }

    void Start() 
    {
        maskRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        maskRenderTexture.filterMode = FilterMode.Bilinear;

        extendIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        extendIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        uvIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        uvIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        supportTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        supportTexture.filterMode =  FilterMode.Bilinear;
        rend = GetComponent<Renderer>();
        rend.material.mainTexture =  extendIslandsRenderTexture;
        PaintManager.instance.initTextures(this);
    }
    
    void OnDisable()
    {
        maskRenderTexture.Release();
        uvIslandsRenderTexture.Release();
        extendIslandsRenderTexture.Release();
        supportTexture.Release();
    }
}