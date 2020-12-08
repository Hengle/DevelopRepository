using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace RolePlayingGame
{
    public class Enemy : MonoBehaviour, IDamageable<int>
    {
        [SerializeField] int hp;
        [SerializeField] int mp;
        [SerializeField] Map map;
        [SerializeField] Animator animator;

        CharaStatus charaStatus;

        // 壁判定のゆるさ
        [SerializeField] int about = 15;

        private void Start()
        {
            //ステータスの設定
            charaStatus = new CharaStatus(150, 5, 1, 5, 1, 0, map, map.GetGridPos(Camera.main.WorldToScreenPoint(transform.position)));
            charaStatus.Map.ChangeMapData(charaStatus.GridPos.x, charaStatus.GridPos.y,ChipType.Enemy);
        }

        /// <summary>
        /// 移動アルゴリズム
        /// DG.Tweening
        /// </summary>
        public IEnumerator MainRoutine()
        {
            var beforePos = transform.position;
            var beforeTime = Time.time;
            while (true)
            {
                switch (charaStatus.Action)
                {
                    case CharaActionState.Idol:
                        ObjectCheck();
                        yield return null;
                        break;
                    case CharaActionState.Move:
                        //Debug.Log("MOVE");
                        yield return StartCoroutine(Move(beforePos, beforeTime));
                        yield break;
                    case CharaActionState.Attack:
                        //Debug.Log("ATTACK");
                        //yield return StartCoroutine(Attack(beforePos, beforeTime));
                        yield break;
                }
                //ここキャラクターごとにアクションごとのクールタイム設定出来たら面白い
                beforePos = transform.position;
            }
        }

        public void ObjectCheck()
        {
            if ((ChipType)charaStatus.Map.CallMapData(charaStatus.MapPosFront()) == ChipType.Player)
            {
                charaStatus.Action = CharaActionState.Attack;
                RayTest();

            }
            else
            {
                charaStatus.Action = CharaActionState.Move;
            }
        }

        void RayTest()
        {
            Debug.Log("テスト");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 10);
            Debug.Log(hit.collider);
        }

        IEnumerator Move(Vector3 beforePos, float beforeTime)
        {

            if ((ChipType)charaStatus.Map.CallMapData(charaStatus.MapPosRight()) == ChipType.Air)
            {
                charaStatus.Dir = charaStatus.DirIfTurnRight();
                animator.SetFloat("Dir", (float)charaStatus.Dir);
            }
            else if ((ChipType)charaStatus.Map.CallMapData(charaStatus.MapPosFront()) == ChipType.Air)
            {
                animator.SetFloat("Dir", (float)charaStatus.Dir);

            }
            else if ((ChipType)charaStatus.Map.CallMapData(charaStatus.MapPosLeft()) == ChipType.Air)
            {
                charaStatus.Dir = charaStatus.DirIfTurnLeft();
                animator.SetFloat("Dir", (float)charaStatus.Dir);
            }
            else if ((ChipType)charaStatus.Map.CallMapData(charaStatus.MapPosBehind()) == ChipType.Air)
            {
                charaStatus.Dir = charaStatus.DirIfTurnReverse();
                animator.SetFloat("Dir", (float)charaStatus.Dir);
            }
            else
            {
                yield break;
            }

            var pos = charaStatus.MapPosFront();
            while (Vector2.Distance(transform.position, charaStatus.Map.CellToWorld(pos)) > 0.1f)
            {
                transform.position = Vector2.Lerp(beforePos, charaStatus.Map.CellToWorld(pos), (Time.time - beforeTime) * 5.0f);
                yield return null;
            }

            charaStatus.GridPos = charaStatus.Map.MapPosUpdate(charaStatus.GridPos, Camera.main.WorldToScreenPoint(transform.position), ChipType.Enemy);
            charaStatus.Action = CharaActionState.Idol;
        }

        private Vector2 ChengeVector(Direction dir)
        {
            if (dir == Direction.Up)
            {
                return new Vector2(0, 0.1f);
            }
            if (dir == Direction.Down)
            {
                return new Vector2(0, -0.1f);
            }
            if (dir == Direction.Left)
            {
                return new Vector2(-0.1f, 0);
            }
            if (dir == Direction.Right)
            {
                return new Vector2(0.1f, 0);
            }
            return new Vector2(0, 0);
        }

        private Direction ChangeDir(Direction dir)
        {

            if (dir == Direction.Up)
            {
                return Direction.Left;
            }
            if (dir == Direction.Down)
            {
                return Direction.Right;
            }
            if (dir == Direction.Left)
            {
                return Direction.Down;
            }
            if (dir == Direction.Right)
            {
                return Direction.Up;
            }
            return Direction.Up;
        }

        public bool Check(Map map, Vector2 charaPos, float size, Direction dir, ChipType myChipType)
        {
            // 上ブロック判定
            if (dir == Direction.Up)
                return Up(map, charaPos, size, myChipType);
            // 左ブロック判定
            if (dir == Direction.Left)
                return Left(map, charaPos, size, myChipType);
            // 右ブロック判定
            if (dir == Direction.Right)
                return Right(map, charaPos, size, myChipType);
            // 下ブロック判定
            if (dir == Direction.Down)
                return Down(map, charaPos, size, myChipType);

            return false;
        }

        // 上ブロック判定
        private bool Up(Map map, Vector2 pos, float size, ChipType myChipType)
        {
            Vector2 temp = new Vector2(pos.x, pos.y);
            for (int i = 0; i < size - about; i++)
            {
                if (CollisionAfter(map, temp, myChipType))
                {
                    return true;
                }
                temp.y++;
            }
            return false;
        }

        // 左ブロック判定
        private bool Left(Map map, Vector2 pos, float size, ChipType myChipType)
        {
            Vector2 temp = new Vector2(pos.x, pos.y);
            for (int i = 0; i < size - about; i++)
            {
                if (CollisionAfter(map, temp, myChipType))
                {
                    return true;
                }
                temp.x--;
            }
            return false;
        }

        // 右ブロック判定
        private bool Right(Map map, Vector2 pos, float size, ChipType myChipType)
        {
            Vector2 temp = new Vector2(pos.x + 1, pos.y);
            for (int i = 0; i < size - about; i++)
            {
                if (CollisionAfter(map, temp, myChipType))
                {
                    return true;
                }
                temp.x++;
            }
            return false;
        }

        // 下ブロック判定
        private bool Down(Map map, Vector2 pos, float size, ChipType myChipType)
        {
            Vector2 temp = new Vector2(pos.x, pos.y - 1);
            for (int i = 0; i < size - about; i++)
            {
                if (CollisionAfter(map, temp, myChipType))
                {
                    return true;
                }
                temp.y--;
            }
            return false;
        }

        /// <summary>
        /// 衝突判定
        /// </summary>
        /// <param name="map"></param>
        /// <param name="playerPos"></param>
        /// <returns></returns>
        private bool CollisionAfter(Map map, Vector2 pos, ChipType myChipType)
        {
            // ウインドウ内座標をマップ座標に変換
            var gridPos = map.GetGridPos(pos);

            // マップ外へ行かないようにする
            if (gridPos.y < 0 || gridPos.y >= map.MaxHeight) return true;

            if (gridPos.x < 0 || gridPos.x >= map.MaxWidth) return true;

            // マップチップが同タイプであれば衝突しない
            if ((ChipType)map.CallMapData(gridPos.x, gridPos.y) == myChipType) return false;

            // マップチップが空気以外であれば衝突
            return !(map.CallMapData(gridPos.x, gridPos.y) == (int)ChipType.Air);
        }

        public void Damage(int dmg)
        {
            //animator.SetTrigger("Damage");
            var sprite = animator.GetComponent<SpriteRenderer>();
            Sequence sequence = DOTween.Sequence();
            sequence.OnStart(() => animator.enabled = false);
            sequence.Append(sprite.DOFade(1.0f, 0.1f));
            sequence.Join(sprite.DOColor(new Color(1, 0, 0, 0), 0.1f));
            sequence.SetLoops(3, LoopType.Yoyo);
            sequence.OnComplete(() => animator.enabled = true);
            sequence.Play();
            charaStatus.DamageHP(dmg);
            Debug.Log("残り"+charaStatus.GetStatus(CharaStatusType.HP));
            if (charaStatus.GetStatus(CharaStatusType.HP) <= 0)
            {
                charaStatus.Map.ChangeMapData(charaStatus.GridPos.x, charaStatus.GridPos.y, ChipType.Air);
                Destroy(gameObject);
            }
        }

        public Vector3Int GridPosition
        {
            get { return charaStatus.GridPos; }
        }
    }
}
