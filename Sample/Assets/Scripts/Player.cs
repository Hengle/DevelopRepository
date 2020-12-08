using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float power;
    [SerializeField] int boundMax;
    [SerializeField] private DrawMesh drawMesh;
    [SerializeField] LineRenderer direction;
    [SerializeField] GenerateLine drawLine;
    [SerializeField] GameObject polygonList;
    [SerializeField] int maxLife;

    public event Action OnMovwEndListener;
    public event Action OnDeathListener;

    private Vector2 startPos, pos;
    private int boundCount;
    private int life;
    private bool isMoving;
    private bool active;

    Touch touch;
    List<Vector2> wallVertices = new List<Vector2>();           //壁頂点リスト
    List<Vector2> lineVertices = new List<Vector2>();           //線頂点リスト
    List<Vector2[]> lines = new List<Vector2[]>();          //線 (頂点,頂点)
    List<int[]> verticesLineNumbers = new List<int[]>();     //頂点の構成ライン(線インデックス,線インデックス)
    List<int[]> passLine = new List<int[]>();              //除外用ライン
    List<List<Vector3>> drawPolygonList = new List<List<Vector3>>();           //

    private void Awake()
    {
        life = maxLife;
        boundCount = 0;
        wallVertices.Add(transform.position);
        drawLine.SetActive(false);
        touch = GameObject.FindGameObjectWithTag("TouchZone").GetComponentInChildren<Touch>();
        touch.OnBeginDragListener += (position, touchTime) => Grab(position, touchTime);
        touch.OnDragListener += (position, touchTime) => Pull(position, touchTime);
        touch.OnEndDragListener += (position, touchTime) => Release(position, touchTime);
    }

    private void Grab(Vector2 positon, float touchTime)
    {
        if (isMoving) return;
        direction.enabled = true;

        startPos = positon;
        direction.SetPosition(0, rb.position);
        direction.SetPosition(1, rb.position);
    }

    private void Pull(Vector2 positon, float touchTime)
    {
        if (isMoving) return;
        pos = positon;
        var difference = positon - startPos;
        var distance = Vector2.Distance(startPos, positon);
        var radian = Mathf.Atan2(difference.x, difference.y * -1.0f) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, radian);
        direction.SetPosition(0, rb.position);
        direction.SetPosition(1, rb.position + new Vector2(transform.up.x, transform.up.y) * 5);
    }

    private void Release(Vector2 positon, float touchTime)
    {
        if (isMoving) return;
        isMoving = true;
        direction.enabled = false;
        drawLine.SetActive(true);

        var difference = positon - startPos;
        var distance = Vector2.Distance(startPos, positon);
        var radian = Mathf.Atan2(difference.x, difference.y * -1.0f) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, radian);
        rb.AddForce(transform.up * power, ForceMode2D.Impulse);
        //rb.AddForce(transform.up * distance, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        boundCount++;
        
        var target = collision.gameObject;

        switch (target.tag)
        {
            case "Wall":
                WallBound(collision.contacts[0].point);
                break;
            default:
                break;
        }

        //バウンド終了
        if (boundCount >= boundMax)
        {
            rb.velocity = Vector3.zero;
            OnMovwEndListener?.Invoke();
        }
    }

    public void Refresh()
    {
        boundCount = 0;
        isMoving = false;
        drawLine.SetActive(false);
        rb.velocity = Vector3.zero;
        wallVertices.Clear();
        lineVertices.Clear();
        drawPolygonList.Clear();
        lines.Clear();
        verticesLineNumbers.Clear();
        passLine.Clear();
        transform.position = new Vector2(0f, -4.0f);
        wallVertices.Add(transform.position);
        foreach (Transform n in polygonList.transform)
        {
            Destroy(n.gameObject);
        }
    }

    public int MaxLife()
    {
        return maxLife;
    }

    public void DrawPolygon()
    {
        foreach (var drawPolygon in drawPolygonList)
        {
            var polygon = drawMesh.CreateMesh(drawPolygon);
            polygon.transform.parent = polygonList.transform;
            polygon.GetComponent<MeshRenderer>().material.color = transform.GetComponent<SpriteRenderer>().color;
        }
    }

    public void Failure()
    {
        life -= 1;
        if (life <= 0)
        {
            OnDeathListener?.Invoke();
        }
    }

    //壁にバウンド
    private void WallBound(Vector2 position)
    {
        wallVertices.Add(position);

        if (wallVertices.Count < 1) return;

        //一番新しい線を生成
        Vector2[] newLine = {
            wallVertices[wallVertices.Count - 2],
            wallVertices[wallVertices.Count - 1]
        };

        //線を格納
        lines.Add(newLine);

        //線が二本
        if (wallVertices.Count < 2) return;

        //頂点を構成する線の組み合わせを格納
        var newLineNumber = lines.IndexOf(newLine);
        verticesLineNumbers.Add(new int[] { newLineNumber - 1, newLineNumber });

        foreach (Vector2[] line in lines)
        {
            //新規で生成された線に交点が発生するかチェック
            if(LineAndLineIntersection(newLine[0], newLine[1],line[0], line[1]))
            {
                var lineNumber = lines.IndexOf(line);                   //交差した線
                var searchLineNumber = lineNumber;                      //絞込む線
                var origin = lineNumber;                                //原点
                List<List<int[]>> hierarchy = new List<List<int[]>>();  //探索階層
                List<int> hierarchyLinuNumber = new List<int>();        //探索階層で絞り込んだ数字
                var matcheVertices = MatchedVerticesLineNumber(searchLineNumber, new List<int[]>() { new int[] { lineNumber, newLineNumber } });

                verticesLineNumbers.Add(new int[] { lineNumber, newLineNumber });

                //一つ取り出す
                var matcheVertice = matcheVertices[0];
                matcheVertices.RemoveAt(0);

                //階層を保存
                hierarchy.Add(matcheVertices);
                hierarchyLinuNumber.Add(lineNumber);

                var loopCount = 0;

                //全てのルートが検索し終わるまで検索し続ける
                while(hierarchy.Count > 0)
                {
                    loopCount++;

                    //絞り込んだ数字じゃないほうで絞り込む
                    searchLineNumber = matcheVertice[0] == searchLineNumber ? matcheVertice[1] : matcheVertice[0];

                    //一度通ったところを除外する
                    passLine.Add(matcheVertice);
                    matcheVertices = MatchedVerticesLineNumber(searchLineNumber, passLine);

                    var hierarchyBackFlg = false;

                    //最初の交点に戻ってきたその時点でその検索はやめる
                    if (matcheVertices.Find(n => n[0] == lineNumber && n[1] == newLineNumber) != null)
                    {
                        passLine.Add(new int[] { lineNumber, newLineNumber });
                        List<Vector3> vertices = new List<Vector3>();
                        foreach (var drawLine in passLine)
                        {
                            if (lines[drawLine[0]][0] == lines[drawLine[1]][1])
                            {
                                vertices.Add(lines[drawLine[0]][0]);
                            }
                            else if (lines[drawLine[0]][1] == lines[drawLine[1]][0])
                            {
                                vertices.Add(lines[drawLine[0]][1]);
                            }
                            else
                            {
                                vertices.Add(LineAndLineIntersection2(lines[drawLine[0]][0], lines[drawLine[0]][1], lines[drawLine[1]][0], lines[drawLine[1]][1]));
                            }
                        }
                        if (passLine.Count <= 5)
                        {
                            drawPolygonList.Add(vertices);
                        }

                        hierarchyBackFlg = true;

                    }
                    else if (matcheVertices.Find(n => n[0] == origin || n[1] == origin) != null || matcheVertices.Count == 0)
                    {
                        hierarchyBackFlg = true;
                    }
                    else
                    {
                        matcheVertice = matcheVertices[0];
                        matcheVertices.RemoveAt(0);
                        hierarchy.Add(matcheVertices);
                        hierarchyLinuNumber.Add(searchLineNumber);
                    }

                    if (hierarchyBackFlg)
                    {
                        passLine.RemoveAt(passLine.Count - 1);
                        hierarchy.Reverse();
                        foreach (var h in hierarchy)
                        {
                            if (h.Count == 0)
                            {
                                passLine.RemoveAt(passLine.Count - 1);
                                hierarchyLinuNumber.RemoveAt(hierarchyLinuNumber.Count - 1);
                            }
                            else
                            {
                                matcheVertice = h[0];
                                h.RemoveAt(0);
                                searchLineNumber = hierarchyLinuNumber[hierarchyLinuNumber.Count - 1];
                                break;
                            }
                        }
                        hierarchy.RemoveRange(0, hierarchy.Count - hierarchyLinuNumber.Count);
                        hierarchy.Reverse();
                    }
                }
                Debug.Log("検索回数"+loopCount);
                passLine.Clear();
            }
        }
    }

    private bool LineAndLineIntersection(Vector2 p1,Vector2 p2,Vector2 p3,Vector2 p4)
    {
        //同一座標はスルー
        if (p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4) return false;
        Vector2 intersection = Vector2.zero;
        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        //交点の追加
        lineVertices.Add(intersection);

        return true;
    }
    private Vector2 LineAndLineIntersection2(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        //同一座標はスルー
        if (p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4) return new Vector2();
        Vector2 intersection = Vector2.zero;
        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return new Vector2();
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return new Vector2();
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        //交点の追加
        lineVertices.Add(intersection);

        return intersection;
    }

    private List<int[]> MatchedVerticesLineNumber(int index, List<int[]> passLine)
    {
        return verticesLineNumbers.FindAll(n => passLine.Find( m => m[0] == n[0] && m[1] == n[1]) == null && (n[0] == index || n[1] == index));
    }
}
