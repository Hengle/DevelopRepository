using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[RequireComponent(typeof (MeshFilter))]
[RequireComponent(typeof (MeshRenderer))]
public class RopeRenderer : MonoBehaviour
{
    [SerializeField] Vector3[] positions;           //
    [SerializeField] int sides = 15;                     //側面数
    [SerializeField] float radiusOne = 0.2f;               //円周
    [SerializeField] float radiusTwo;               //トップの円周
    [SerializeField] bool useWorldSpace = true;     //ワールド空間フラグ
    [SerializeField] bool useTwoRadii = false;      //トップの円周フラグ

    private Vector3[] vertices;                     //頂点情報
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    public Material material
    {
        get { return meshRenderer.material; }
        set { meshRenderer.material = value; }
    }

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        mesh = new Mesh();
        meshFilter.mesh = mesh;

    }

    private void OnEnable()
    {
        meshRenderer.enabled = true;
    }

    private void OnDisable()
    {
        meshRenderer.enabled = false;
    }

    void Update()
    {
        GenerateMesh();
    }

    private void OnValidate()
    {
        sides = Mathf.Max(3, sides);
    }

    public void SetPositions(Vector3[] positions)
    {
        this.positions = positions;
        GenerateMesh();
    }

    //メッシュ生成
    private void GenerateMesh()
    {
        if (mesh == null || positions == null || positions.Length <= 1)
        {
            mesh = new Mesh();
            return;
        }

        var verticesLength = sides * positions.Length;
        if (vertices == null || vertices.Length != verticesLength)
        {
            vertices = new Vector3[verticesLength];

            var indices = GenerateIndices();
            var uvs = GenerateUVs();

            if (verticesLength > mesh.vertexCount)
            {
                mesh.vertices = vertices;
                mesh.triangles = indices;
                mesh.uv = uvs;
            }
            else
            {
                mesh.triangles = indices;
                mesh.vertices = vertices;
                mesh.uv = uvs;
            }
        }

        var currentVertIndex = 0;

        for (int i = 0; i < positions.Length; i++)
        {
            var circle = CalculateCircle(i);
            foreach (var vertex in circle)
            {
                vertices[currentVertIndex++] = useWorldSpace ? transform.InverseTransformPoint(vertex) : vertex;
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    //UVの生成
    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[positions.Length * sides];

        for (int segment = 0; segment < positions.Length; segment++)
        {
            for (int side = 0; side < sides; side++)
            {
                var vertIndex = (segment * sides + side);
                var u = side / (sides - 1f);
                var v = segment / (positions.Length - 1f);

                uvs[vertIndex] = new Vector2(u, v);
            }
        }

        return uvs;
    }

    //インデックスの生成
    private int[] GenerateIndices()
    {
        var indices = new int[positions.Length * sides * 2 * 3];

        var currentIndicesIndex = 0;
        for (int segment = 1; segment < positions.Length; segment++)
        {
            for (int side = 0; side < sides; side++)
            {
                var vertIndex = (segment * sides + side);
                var prevVertIndex = vertIndex - sides;

                indices[currentIndicesIndex++] = prevVertIndex;
                indices[currentIndicesIndex++] = (side == sides - 1) ? (vertIndex - (sides - 1)) : (vertIndex + 1);
                indices[currentIndicesIndex++] = vertIndex;


                indices[currentIndicesIndex++] = (side == sides - 1) ? (prevVertIndex - (sides - 1)) : (prevVertIndex + 1);
                indices[currentIndicesIndex++] = (side == sides - 1) ? (vertIndex - (sides - 1)) : (vertIndex + 1);
                indices[currentIndicesIndex++] = prevVertIndex;
            }
        }

        return indices;
    }

    //円周の計算
    private Vector3[] CalculateCircle(int index)
    {
        var dirCount = 0;
        var forward = Vector3.zero;

        if (index > 0)
        {
            forward += (positions[index] - positions[index - 1]).normalized;
            dirCount++;
        }

        if (index < positions.Length - 1)
        {
            forward += (positions[index + 1] - positions[index]).normalized;
            dirCount++;
        }

        forward = (forward / dirCount).normalized;
        var side = Vector3.Cross(forward, forward + new Vector3(.123564f, .34675f, .756892f)).normalized;
        var up = Vector3.Cross(forward, side).normalized;

        var circle = new Vector3[sides];
        var angle = 0f;
        var angleStep = (2 * Mathf.PI) / sides;

        var t = index / (positions.Length - 1f);
        var radius = useTwoRadii ? Mathf.Lerp(radiusOne, radiusTwo, t) : radiusOne;

        for (int i = 0; i < sides; i++)
        {
            var x = Mathf.Cos(angle);
            var y = Mathf.Sin(angle);

            circle[i] = positions[index] + side * x * radius + up * y * radius;

            angle += angleStep;
        }

        return circle;
    }
}