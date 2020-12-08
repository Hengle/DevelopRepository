// Author: Mathias Soeholm
// Date: 05/10/2016
// No license, do whatever you want with this script
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class TubeRenderer : MonoBehaviour
{
    [SerializeField] Vector3[] _positions;
    [SerializeField] int _sides;
    [SerializeField] float _radiusOne;
    [SerializeField] float _radiusTwo;
    [SerializeField] bool _useWorldSpace = true;
    [SerializeField] bool _useTwoRadii = false;

    private Vector3[] _vertices;
    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    [SerializeField] Transform[] targets;
    [SerializeField] Transform mask;
    bool isReturn;
    Transform beforeTarget;
    int t = 0;
    int nowIndex;

    public Material material
    {
        get { return _meshRenderer.material; }
        set { _meshRenderer.material = value; }
    }

    void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        if (_meshFilter == null)
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
        {
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        beforeTarget = transform;

        var segments = 32*10;

        _positions = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            Debug.Log(36 * (1 + segments / 36));
            var rad = Mathf.Deg2Rad * (i * 360f / 36f);
            var x = Mathf.Sin(rad) * 0.1f;
            var y = Mathf.Cos(rad) * 0.1f;
            _positions[i] = new Vector3(x, i/36f * 0.1f, y);
        }

    }

    private void OnEnable()
    {
        _meshRenderer.enabled = true;
    }

    private void OnDisable()
    {
        _meshRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (mask.localScale.y <= 0f) return;
            if (nowIndex >= targets.Length)
            {
                mask.localScale = mask.localScale.AddY(-0.001f);
                return;
            };
            var position = CalcMoveControllPoint(targets[nowIndex] ,t * 0.01f);
            System.Array.Resize(ref _positions, _positions.Length + 1);
            _positions[_positions.Length - 1] = position;
            t++;
            if (t * 0.01f >= 1.0f)
            {
                t = 0;
                isReturn = !isReturn;
                beforeTarget = targets[nowIndex];
                nowIndex++;
            }
        }
        GenerateMesh();
    }

    Vector3 CalcMoveControllPoint(Transform target, float t)
    {
        var controllPoint = Vector3.Lerp(beforeTarget.position, target.position, 0.5f);
        controllPoint = isReturn ? controllPoint.AddZ(0.1f) : controllPoint.AddZ(-0.1f);

        Vector3 M0 = Vector3.Lerp(beforeTarget.position, controllPoint, t);
        Vector3 M1 = Vector3.Lerp(controllPoint, target.position, t);
        return Vector3.Lerp(M0, M1, t);
    }

    //public static Vector3 CalcBezier(Vector3 start, Vector3 end, Vector3 control1, Vector3 control2, float t)
    //{
    //    Vector3 M0 = Vector3.Lerp(start, control1, t);
    //    Vector3 M1 = Vector3.Lerp(control1, control2, t);
    //    Vector3 M2 = Vector3.Lerp(control2, end, t);
    //    Vector3 B0 = Vector3.Lerp(M0, M1, t);
    //    Vector3 B1 = Vector3.Lerp(M1, M2, t);
    //    return Vector3.Lerp(B0, B1, t);
    //}

    private void OnValidate()
    {
        _sides = Mathf.Max(3, _sides);
    }

    public void SetPositions(Vector3[] positions)
    {
        _positions = positions;
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        if (_mesh == null || _positions == null || _positions.Length <= 1)
        {
            _mesh = new Mesh();
            return;
        }

        var verticesLength = _sides * _positions.Length;
        if (_vertices == null || _vertices.Length != verticesLength)
        {
            _vertices = new Vector3[verticesLength];

            var indices = GenerateIndices();
            var uvs = GenerateUVs();

            if (verticesLength > _mesh.vertexCount)
            {
                _mesh.vertices = _vertices;
                _mesh.triangles = indices;
                _mesh.uv = uvs;
            }
            else
            {
                _mesh.triangles = indices;
                _mesh.vertices = _vertices;
                _mesh.uv = uvs;
            }
        }

        var currentVertIndex = 0;

        for (int i = 0; i < _positions.Length; i++)
        {
            var circle = CalculateCircle(i);
            foreach (var vertex in circle)
            {
                _vertices[currentVertIndex++] = _useWorldSpace ? transform.InverseTransformPoint(vertex) : vertex;
            }
        }

        _mesh.vertices = _vertices;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        _meshFilter.mesh = _mesh;
    }

    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[_positions.Length * _sides];

        for (int segment = 0; segment < _positions.Length; segment++)
        {
            for (int side = 0; side < _sides; side++)
            {
                var vertIndex = (segment * _sides + side);
                var u = side / (_sides - 1f);
                var v = segment / (_positions.Length - 1f);

                uvs[vertIndex] = new Vector2(u, v);
            }
        }

        return uvs;
    }

    private int[] GenerateIndices()
    {
        // Two triangles and 3 vertices
        var indices = new int[_positions.Length * _sides * 2 * 3];

        var currentIndicesIndex = 0;
        for (int segment = 1; segment < _positions.Length; segment++)
        {
            for (int side = 0; side < _sides; side++)
            {
                var vertIndex = (segment * _sides + side);
                var prevVertIndex = vertIndex - _sides;

                // Triangle one
                indices[currentIndicesIndex++] = prevVertIndex;
                indices[currentIndicesIndex++] = (side == _sides - 1) ? (vertIndex - (_sides - 1)) : (vertIndex + 1);
                indices[currentIndicesIndex++] = vertIndex;


                // Triangle two
                indices[currentIndicesIndex++] = (side == _sides - 1) ? (prevVertIndex - (_sides - 1)) : (prevVertIndex + 1);
                indices[currentIndicesIndex++] = (side == _sides - 1) ? (vertIndex - (_sides - 1)) : (vertIndex + 1);
                indices[currentIndicesIndex++] = prevVertIndex;
            }
        }

        return indices;
    }

    private Vector3[] CalculateCircle(int index)
    {
        var dirCount = 0;
        var forward = Vector3.zero;

        // If not first index
        if (index > 0)
        {
            forward += (_positions[index] - _positions[index - 1]).normalized;
            dirCount++;
        }

        // If not last index
        if (index < _positions.Length - 1)
        {
            forward += (_positions[index + 1] - _positions[index]).normalized;
            dirCount++;
        }

        // Forward is the average of the connecting edges directions
        forward = (forward / dirCount).normalized;
        var side = Vector3.Cross(forward, forward + new Vector3(.123564f, .34675f, .756892f)).normalized;
        var up = Vector3.Cross(forward, side).normalized;

        var circle = new Vector3[_sides];
        var angle = 0f;
        var angleStep = (2 * Mathf.PI) / _sides;

        var t = index / (_positions.Length - 1f);
        var radius = _useTwoRadii ? Mathf.Lerp(_radiusOne, _radiusTwo, t) : _radiusOne;

        for (int i = 0; i < _sides; i++)
        {
            var x = Mathf.Cos(angle);
            var y = Mathf.Sin(angle);

            circle[i] = _positions[index] + side * x * radius + up * y * radius;

            angle += angleStep;
        }

        return circle;
    }
}