using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RolePlayingGame
{
    //キャラステータス種別
    enum CharaStatusType
    {
        HP,
        MaxHP,
        MP,
        MaxMP,
        PP,
        AP,
        Level,
        EXP,    
    }

    //向き
    public enum Direction
    {
        Up,
        Left,
        Right,
        Down,
    }

    //行動状態
    enum CharaActionState
    {
        Idol = 0,
        Move = 1,
        Attack = 2,
        Damage = 3,
        Happy = 4,
        Down = 5,
        Dead = 6,
    }

    /// <summary>
    /// キャラステータス管理
    /// </summary>
    class CharaStatus
    {
        private Dictionary<CharaStatusType, int> status;
        public CharaActionState Action { get; set; }
        public Map Map { get; private set; }
        public Vector3Int GridPos { get; set; }
        public Direction Dir { get; set; }

        public CharaStatus(int hp, int mp, int pp, int ap, int lv, int exp, Map map, Vector3Int gridPos)
        {
            status = new Dictionary<CharaStatusType, int>();

            status[CharaStatusType.HP] = hp;
            status[CharaStatusType.MaxHP] = hp;
            status[CharaStatusType.MP] = mp;
            status[CharaStatusType.MaxMP] = mp;
            status[CharaStatusType.PP] = pp;
            status[CharaStatusType.AP] = ap;
            status[CharaStatusType.Level] = lv;

            this.Map = map;
            this.GridPos = gridPos;
            this.Dir = Direction.Down;
            this.Action = CharaActionState.Idol;
        }


        //ステータス取得
        public int GetStatus(CharaStatusType statusType)
        {
            return status[statusType];
        }

        //ステータス加算
        private void StateAdd(CharaStatusType statusType, int value)
        {
            status[statusType] += value;

            // EXP以外が255以上なら255に補正する
            if (statusType != CharaStatusType.EXP)
            {
                if (status[statusType] > 255)
                {
                    status[statusType] = 255;
                }
            }
        }

        //ステータス減産
        private void StateSub(CharaStatusType statusType, int value)
        {
            status[statusType] -= value;

            // 0以下状態なら0に補正する
            if (status[statusType] < 0)
            {
                status[statusType] = 0;
            }
        }

        
        public void DamageHP(int value)
        {
            Debug.Log(value+"ダメージ");
            this.StateSub(CharaStatusType.HP, (int)value);
        }

        public Vector3Int MapPosFront()
        {
            switch (Dir)
            {
                case Direction.Up:
                    return new Vector3Int(GridPos.x, GridPos.y + 1, GridPos.z);
                case Direction.Down:
                    return new Vector3Int(GridPos.x, GridPos.y - 1, GridPos.z);
                case Direction.Left:
                    return new Vector3Int(GridPos.x - 1, GridPos.y, GridPos.z);
                default:    // Right
                    return new Vector3Int(GridPos.x + 1, GridPos.y, GridPos.z);
            }
        }

        public Vector3Int MapPosBehind()
        {
            switch (Dir)
            {
                case Direction.Up:
                    return new Vector3Int(GridPos.x, GridPos.y - 1, GridPos.z);
                case Direction.Down:
                    return new Vector3Int(GridPos.x, GridPos.y + 1, GridPos.z);
                case Direction.Left:
                    return new Vector3Int(GridPos.x + 1, GridPos.y, GridPos.z);
                default:    // Right
                    return new Vector3Int(GridPos.x - 1, GridPos.y, GridPos.z);
            }
        }

        public Vector3Int MapPosLeft()
        {
            switch (Dir)
            {
                case Direction.Up:
                    return new Vector3Int(GridPos.x - 1, GridPos.y, GridPos.z);
                case Direction.Down:
                    return new Vector3Int(GridPos.x + 1, GridPos.y, GridPos.z);
                case Direction.Left:
                    return new Vector3Int(GridPos.x, GridPos.y - 1, GridPos.z);
                default:    // Right
                    return new Vector3Int(GridPos.x, GridPos.y + 1, GridPos.z);
            }
        }

        public Vector3Int MapPosRight()
        {
            switch (Dir)
            {
                case Direction.Up:
                    return new Vector3Int(GridPos.x + 1, GridPos.y, GridPos.z);
                case Direction.Down:
                    return new Vector3Int(GridPos.x - 1, GridPos.y, GridPos.z);
                case Direction.Left:
                    return new Vector3Int(GridPos.x, GridPos.y + 1, GridPos.z);
                default:    // Right
                    return new Vector3Int(GridPos.x, GridPos.y - 1, GridPos.z);
            }
        }

        public Direction DirIfTurnLeft()
        {
            switch (Dir)
            {
                case Direction.Up:
                    return Direction.Left;
                case Direction.Down:
                    return Direction.Right;
                case Direction.Left:
                    return Direction.Down;
                default:    // Right
                    return Direction.Up;
            }
        }

        public Direction DirIfTurnRight()
        {
            switch (Dir)
            {
                case Direction.Up:
                    return Direction.Right;
                case Direction.Down:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Up;
                default:    // Right
                    return Direction.Down;
            }
        }

        public Direction DirIfTurnReverse()
        {
            switch (Dir)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                default:    // Right
                    return Direction.Left;
            }
        }
    }
}
