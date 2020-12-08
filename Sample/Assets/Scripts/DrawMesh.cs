using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Draw mesh by clicked points.
/// </summary>
public class DrawMesh : MonoBehaviour
{
    [SerializeField] Material material;
    private List<int> _triangles = new List<int>();
    private List<Vector3> _vertices = new List<Vector3>();
    private Dictionary<int, bool> _verticesBuffer = new Dictionary<int, bool>();

    private Vector3 _prevDirection = Vector3.zero;

    private bool _isIncluding = false;
    private int _curIndex;
    private int _nextIndex;
    private int _prevIndex;

    private Vector3 CurrentPoint
    {
        get { return _vertices[_curIndex]; }
    }
    private Vector3 PreviousPoiont
    {
        get { return _vertices[_prevIndex]; }
    }
    private Vector3 NextPoint
    {
        get { return _vertices[_nextIndex]; }
    }

    /// <summary>
    /// Clear buffers.
    /// </summary>
    private void Clear()
    {
        _vertices.Clear();
        _verticesBuffer.Clear();
        _triangles.Clear();
    }

    private void Initialize(List<Vector3> vertices)
    {
        Clear();

        // 設定された頂点を保持しておく
        _vertices.AddRange(vertices);

        // 全頂点のインデックスを保持、使用済みフラグをfalseで初期化
        for (int i = 0; i < vertices.Count; i++)
        {
            _verticesBuffer.Add(i, false);
        }
    }

    /// <summary>
    /// Create mesh by vertices.
    /// </summary>
    public GameObject CreateMesh(List<Vector3> vertices)
    {
        Initialize(vertices);

        while (true)
        {
            KeyValuePair<int, bool>[] left = _verticesBuffer.Where(buf => !buf.Value).ToArray();
            if (left.Length <= 3)
            {
                break;
            }
            DetecteTriangle();
        }

        int[] keys = _verticesBuffer.Keys.ToArray();
        foreach (int key in keys)
        {
            if (!_verticesBuffer[key])
            {
                _verticesBuffer[key] = true;
                _triangles.Add(key);
            }
        }

        Debug.Log("Done chekcing.");

        Mesh mesh = new Mesh();
        mesh.vertices = _vertices.ToArray();

        mesh.triangles = _triangles.ToArray();
        mesh.RecalculateNormals();

        GameObject polygon = new GameObject("polygon");
        MeshRenderer renderer = polygon.AddComponent<MeshRenderer>();
        polygon.AddComponent<MeshFilter>().sharedMesh = mesh;
        polygon.AddComponent<MeshCollider>().sharedMesh = mesh;
        renderer.material.shader = Shader.Find("Sprites/Default");
        renderer.material.color = new Color(253.0f/ 255.0f, 215.0f / 255.0f, 55.0f / 255.0f);

        return polygon;
    }

    /// <summary>
    /// Detect triangle from far point.
    /// </summary>
    private void DetecteTriangle()
    {
        if (!_isIncluding)
        {
            FindFarPoint();
        }

        Vector3 a = CurrentPoint;
        Vector3 b = NextPoint;
        Vector3 c = PreviousPoiont;

        Vector3 edge1 = b - a;
        Vector3 edge2 = c - a;

        float angle = Vector3.Angle(edge1, edge2);
        if (angle >= 180)
        {
            Debug.LogError("Something was wrong.");
            return;
        }

        if (IsIncludePoint())
        {
            Debug.Log("Point is including.");

            // try to find other point.
            _isIncluding = true;

            // Store current triangle dicretion.
            _prevDirection = GetCurrentDirection();

            MoveToNext();

            return;
        }

        _isIncluding = false;

        _triangles.Add(_curIndex);
        _triangles.Add(_nextIndex);
        _triangles.Add(_prevIndex);

        bool isDtected = true;
        _verticesBuffer[_curIndex] = isDtected;
    }

    /// <summary>
    /// Check to include point in the triangle.
    /// </summary>
    /// <returns></returns>
    private bool IsIncludePoint()
    {
        foreach (var key in _verticesBuffer.Keys)
        {
            int index = key;

            if (_verticesBuffer[key])
            {
                continue;
            }

            // skip if index in detected three points.
            if (index == _curIndex || index == _nextIndex || index == _prevIndex)
            {
                continue;
            }

            if (CheckInPoint(_vertices[index]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get current triangle direction.
    /// </summary>
    /// <returns>Triagnel direction normal.</returns>
    private Vector3 GetCurrentDirection()
    {
        Vector3 edge1 = (NextPoint - CurrentPoint).normalized;
        Vector3 edge2 = (PreviousPoiont - CurrentPoint).normalized;

        return Vector3.Cross(edge1, edge2);
    }

    /// <summary>
    /// Check including point.
    /// </summary>
    /// <param name="target">Target point.</param>
    /// <returns>return true if point is including.</returns>
    private bool CheckInPoint(Vector3 target)
    {
        // Triangle points.
        Vector3[] tp =
        {
            CurrentPoint,
            NextPoint,
            PreviousPoiont,
        };

        Vector3 prevNormal = default(Vector3);
        for (int i = 0; i < tp.Length; i++)
        {
            Vector3 edge1 = (target - tp[i]);
            Vector3 edge2 = (target - tp[(i + 1) % tp.Length]);

            Vector3 normal = Vector3.Cross(edge1, edge2).normalized;

            if (prevNormal == default(Vector3))
            {
                prevNormal = normal;
                continue;
            }

            // If not same direction, the point out of a triangle.
            if (Vector3.Dot(prevNormal, normal) <= 0.99f)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Poition reference move to next.
    /// </summary>
    private void MoveToNext()
    {
        _curIndex = FindNextIndex(_curIndex);
        _nextIndex = FindNextIndex(_curIndex);
        _prevIndex = FindPrevIndex(_curIndex);
    }

    /// <summary>
    /// 原点から最も遠い点を探す
    /// </summary>
    private void FindFarPoint()
    {
        int farIndex = -1;
        float maxDist = float.MinValue;

        foreach (var key in _verticesBuffer.Keys)
        {
            if (_verticesBuffer[key])
            {
                continue;
            }

            float dist = Vector3.Distance(Vector3.zero, _vertices[key]);
            if (dist > maxDist)
            {
                maxDist = dist;
                farIndex = key;
            }
        }

        _curIndex = farIndex;
        _nextIndex = FindNextIndex(_curIndex);
        _prevIndex = FindPrevIndex(_curIndex);
    }

    /// <summary>
    /// 指定インデックスから調べて次の有効頂点インデックスを探す
    /// </summary>
    private int FindNextIndex(int start)
    {
        int i = start;
        while (true)
        {
            i = (i + 1) % _vertices.Count;
            if (!_verticesBuffer[i])
            {
                return i;
            }
        }
    }

    /// <summary>
    /// 指定インデックスから調べて前の有効頂点インデックスを探す
    /// </summary>
    private int FindPrevIndex(int start)
    {
        int i = start;
        while (true)
        {
            i = (i - 1) >= 0 ? i - 1 : _vertices.Count - 1;
            if (!_verticesBuffer[i])
            {
                return i;
            }
        }
    }
}