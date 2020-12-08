using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] Transform[] ranking;
    [SerializeField] Slider progressBar;
    Transform[] warpPointList;
    GameObject player;
    CharacterAI[] enemies;

    Vector3 startPos, goalPos;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        var rings = GameObject.FindGameObjectsWithTag("Ring");
        var jumpBoards = GameObject.FindGameObjectsWithTag("JumpBoard");

        warpPointList = new Transform[rings.Length + jumpBoards.Length];
        int index = 0;
        for (int i = 0; i < rings.Length; ++i)
        {
            warpPointList[index] = rings[i].GetComponent<Transform>();
            index++;
        }

        for (int i = 0; i < jumpBoards.Length; ++i)
        {
            warpPointList[index] = jumpBoards[i].GetComponent<Transform>();
            index++;
        }

        var enemiesObj = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = new CharacterAI[enemiesObj.Length];
        Object[] materials = Resources.LoadAll("CharacterColor", typeof(Material));
        for (int i = 0; i < enemies.Length; ++i)
        {
            enemies[i] = enemiesObj[i].GetComponent<CharacterAI>();
            enemies[i].SetGliderColor((Material)materials[Random.Range(0, materials.Length)]);
        }

        startPos = player.transform.position;
        goalPos = new Vector3(0f, 0f, 1000f);
    }

    private void Update()
    {
        RankingUpdate();
        ProgressBarUpdate();

    }
    void ProgressBarUpdate()
    {
        var distance = goalPos.z - startPos.z;
        var progress = (Mathf.Abs(startPos.z) + player.transform.position.z) / distance;
        progressBar.value = progress;
    }

    void RankingUpdate()
    {
        ranking = ranking.OrderByDescending(value => value.GetComponent<TargetMarker>().GetZPosition()).ToArray();
        for (int i = 0; i < ranking.Length; i++)
        {
            string rankText = "";
            switch (i + 1)
            {
                case 1:
                    rankText = (i + 1).ToString() + "st";
                    break;
                case 2:
                    rankText = (i + 1).ToString() + "nd";
                    break;
                case 3:
                    rankText = (i + 1).ToString() + "rd";
                    break;
                default:
                    rankText = (i + 1).ToString() + "th";
                    break;

            }
            var tm = ranking[i].GetComponent<TargetMarker>();
            if (i == 0)
            {
                tm.SetCrown(true);
            }
            else
            {
                tm.SetCrown(false);
            }
            tm.SetText(rankText);
        }
    }

    public void ActiveCharqacters()
    {
        player.transform.GetComponent<Character>().SetActive(true);
        for (int i = 0; i < enemies.Length; ++i)
        {
            enemies[i].transform.GetComponent<Character>().SetActive(true);
        }
        StartCoroutine(CharacterAIRoutine());
    }

    IEnumerator CharacterAIRoutine()
    {
        while (true)
        {
            var frontEnemyCount = 0;
            foreach (var enemy in enemies)
            {
                if (enemy.isMove && enemy.transform.position.z >= player.transform.position.z)
                {
                    frontEnemyCount++;
                }
            }

            var moreShortDistance = 100f;
            var warpPointPos = Vector3.zero;
            if (frontEnemyCount <= 0)
            {
                foreach (var warpPoint in warpPointList)
                {
                    var distance = (player.transform.position - warpPoint.position).magnitude;
                    if (warpPoint.position.z < player.transform.position.z && moreShortDistance > distance)
                    {
                        warpPointPos = warpPoint.position;
                        moreShortDistance = distance;
                    }
                }
            }

            CharacterAI warpEnemy = null;
            if (warpPointPos != Vector3.zero)
            {
                foreach (var enemy in enemies)
                {
                    if (!enemy.isMove)
                    {
                        warpEnemy = enemy;
                    }
                }
            }

            

            if (warpEnemy != null)
            {
                if ((player.transform.position - warpPointPos).magnitude >= 30f)
                {
                    warpPointPos = player.transform.position.AddY(15f);
                    warpEnemy.Warp(warpPointPos, false);
                }
                else
                {
                    warpEnemy.Warp(warpPointPos,true);
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}
