using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Line : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Renderer render;
    [SerializeField] [Range(0,1)] private float size = 0.2f;
    [SerializeField] private float arrowsSpeed = 6f;
    [SerializeField] [Range(0, 1)] private float height = 0.1f;
    private Vector3 startPoint, endPoint;
    private Factory _startFactory;
    private EntryPoint _lineEndPoint;
    private Mesh mesh;


    private void Update()
    {
        if(_lineEndPoint != null) render.material.mainTextureOffset += new Vector2(0,-arrowsSpeed * Time.deltaTime);
    }

    #region Factory

    public void SetStartFactory(Factory factory)
    {
        _startFactory = factory;
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        transform.position = Vector3.zero + Vector3.up * height;
        transform.rotation = Quaternion.identity;
        startPoint = factory.transform.position;
        if (_startFactory.IsFull())
        {
            render.material.color = Color.red;
            render.material.mainTexture = null;
        }
    }
    public void SetEntryPoint(EntryPoint entryPoint)
    {
        if (entryPoint == null || _startFactory.IsFull() || entryPoint.factory == _startFactory || entryPoint.full)
        {
            Destroy(gameObject);
            return;
        }
        entryPoint.full = true;
        _lineEndPoint = entryPoint;
        SetPosition(entryPoint.transform.position);
        _startFactory.SetTargetedEntryPoint(entryPoint, this);
    }

    #endregion
    
    #region Mesh
    public void SetPosition(Vector3 end)
    {
        endPoint = end;
        RecalculateMesh();
    }
    private void RecalculateMesh()
    {
        if (mesh == null) return;
        mesh.vertices = Vertices();
        mesh.triangles = new[] { 0, 1, 2, 2, 1, 3};
        mesh.uv = new[] {new Vector2(0,0),new Vector2(1,0),new Vector2(0,1),new Vector2(1,1)};
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateUVDistributionMetrics();
        var dist = Vector3.Distance(startPoint, endPoint);
        render.material.mainTextureScale = new Vector2(1, dist);
    }
    private Vector3[] Vertices()
    {
        var points = new Vector3[4];
        var direction = (endPoint - startPoint).normalized;
        direction = Quaternion.AngleAxis(90, Vector3.up) * direction;
        points[0] = startPoint + direction * size;
        points[1] = startPoint - direction * size;
        points[2] = endPoint + direction * size; 
        points[3] = endPoint - direction * size;
        return points;
    }

    #endregion
}
