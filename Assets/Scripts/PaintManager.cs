using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : Singleton<PaintManager>
{
    #region ID
    int prepareUVID = Shader.PropertyToID("_PrepareUV");
    int textureID = Shader.PropertyToID("_MainTex");
    int uvOffsetID = Shader.PropertyToID("_OffsetUV");
    int uvIslandsID = Shader.PropertyToID("_UVIslands");
    int color1ID = Shader.PropertyToID("_Color1");
    int color2ID = Shader.PropertyToID("_Color2");
    int color3ID = Shader.PropertyToID("_Color3"); 
    int logo1ID = Shader.PropertyToID("_Logo1");
    int logo2ID = Shader.PropertyToID("_Logo2");
    int logo3ID = Shader.PropertyToID("_Logo3");
    #endregion

    public static PaintManager Instance {get; protected set;}

    CommandBuffer command;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;
    }

    public void initTextures(Paintable paintable)
    {
        RenderTexture mask = paintable.GetMask();
        RenderTexture uvIslands = paintable.GetUVIslands();
        RenderTexture extend = paintable.GetExtend();
        RenderTexture support = paintable.GetSupport();
        Renderer rend = paintable.GetRenderer();

        command.SetRenderTarget(mask);
        command.SetRenderTarget(extend);
        command.SetRenderTarget(support);

        paintable.GetPaintMaterial().SetFloat(prepareUVID, 1);
        command.SetRenderTarget(uvIslands);
        command.DrawRenderer(rend, paintable.GetPaintMaterial(), 0);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }

    
    public void MergeColors(Paintable paintable, Color color1, Color color2, Color color3)
    {
        var uvIslands = paintable.GetUVIslands();
        var support = paintable.GetSupport();
        paintable.GetPaintMaterial().SetFloat(prepareUVID, 0);
        paintable.GetPaintMaterial().SetTexture(textureID, support);
        paintable.GetExtendMaterial().SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        paintable.GetExtendMaterial().SetTexture(uvIslandsID, uvIslands);
        paintable.GetPaintMaterial().SetColor(color1ID,color1);
        paintable.GetPaintMaterial().SetColor(color2ID,color2);
        paintable.GetPaintMaterial().SetColor(color3ID,color3);
        Command(paintable.GetMask(),paintable.GetRenderer(),support,paintable.GetExtend(), paintable);
    }

    public void MergeLogos(Paintable paintable, Texture logo1, Texture logo2, Texture logo3)
    {
        var uvIslands = paintable.GetUVIslands();
        var support = paintable.GetSupport();
        paintable.GetPaintMaterial().SetFloat(prepareUVID, 0);
        paintable.GetPaintMaterial().SetTexture(textureID, support);
        paintable.GetExtendMaterial().SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        paintable.GetExtendMaterial().SetTexture(uvIslandsID, uvIslands);
        paintable.GetPaintMaterial().SetTexture(logo1ID,logo1);
        paintable.GetPaintMaterial().SetTexture(logo2ID,logo2);
        paintable.GetPaintMaterial().SetTexture(logo3ID,logo3);
        Command(paintable.GetMask(),paintable.GetRenderer(),support,paintable.GetExtend(), paintable);
        paintable.GetPaintMaterial().SetTexture(logo2ID,null);
        paintable.GetPaintMaterial().SetTexture(logo3ID,null);
    }
    
    public void SetColor(Paintable paintable, Color color)
    {
        var uvIslands = paintable.GetUVIslands();
        var support = paintable.GetSupport();
        paintable.GetPaintMaterial().SetFloat(prepareUVID, 0);
        paintable.GetPaintMaterial().SetTexture(textureID, support);
        paintable.GetExtendMaterial().SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        paintable.GetExtendMaterial().SetTexture(uvIslandsID, uvIslands);
        paintable.GetPaintMaterial().SetColor(color1ID,color);
        paintable.GetPaintMaterial().SetColor(color2ID,new Color(0,0,0,0));
        paintable.GetPaintMaterial().SetColor(color3ID,new Color(0,0,0,0));
        Command(paintable.GetMask(),paintable.GetRenderer(),support,paintable.GetExtend(), paintable);
    }

    public void SetLogo(Paintable paintable, Texture logo)
    {
        var uvIslands = paintable.GetUVIslands();
        var support = paintable.GetSupport();
        paintable.GetPaintMaterial().SetFloat(prepareUVID, 0);
        paintable.GetPaintMaterial().SetTexture(textureID, support);
        paintable.GetExtendMaterial().SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        paintable.GetExtendMaterial().SetTexture(uvIslandsID, uvIslands);
        paintable.GetPaintMaterial().SetTexture(logo1ID,logo);
        
        Command(paintable.GetMask(),paintable.GetRenderer(),support,paintable.GetExtend(), paintable);
    }
    
    public void Rotate(Paintable paintable, float angel)
    {
       
    }

    private void Command(RenderTexture mask, Renderer rend, RenderTexture support, RenderTexture extend, Paintable paintable)
    {
        command.SetRenderTarget(mask);
        command.DrawRenderer(rend, paintable.GetPaintMaterial(), 0);
        command.SetRenderTarget(support);
        command.Blit(mask, support);
        command.SetRenderTarget(extend);
        command.Blit(mask, extend, paintable.GetExtendMaterial());
        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }
}
