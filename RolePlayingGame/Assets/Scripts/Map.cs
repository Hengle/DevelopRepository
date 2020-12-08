using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text.RegularExpressions;
using System;

namespace RolePlayingGame
{
    public enum ChipType
    {
        Air = -1,
        Block0 = 0,
        Block1 = 1,
        Block2 = 2,
        Block3 = 3,
        Block4 = 4,
        Block5 = 5,
        Block6 = 6,
        Block7 = 7,
        Block8 = 8,
        Block9 = 9,
        Block10 = 10,
        Block11 = 11,
        Block12 = 12,
        Block13 = 13,
        Block14 = 14,
        Block15 = 15,
        Block16 = 16,
        Block17 = 17,
        Block18 = 18,
        Block19 = 19,
        Enemy = 20,
        Player = 21,

        Error = 255,        // 255  エラー
    };

    public class Map : MonoBehaviour
    {
        public struct Pos
        {
            public int x, y;

            public Pos(int x, int y)
            {
                this.x = x; this.y = y;
            }
        }

        [SerializeField] Tilemap backGround;
        [SerializeField] Tilemap foreGround;
        [SerializeField] Grid grid;
        [SerializeField] List<Tile> blockLiast;

        int[,] map = new int[4,4];

        int width = 1280;
        int height = 720;
        int chipSize = 32;

        private Dictionary<int, Tile> mapChip = new Dictionary<int, Tile>();
        private Dictionary<int, bool> mapChipCollision = new Dictionary<int, bool>();
        private List<Map.Pos> temPos;
        Vector3 oldTouchPos;

        private void Awake()
        {
            temPos = new List<Pos>();
            oldTouchPos = new Vector3(width / 2, height / 2, 0);
            Load();
            foreach (var block in blockLiast)
            {
                var name = block.name;
                var chipType = int.Parse(Regex.Replace(name, @"[^0-9]", ""));
                mapChip.Add(chipType, block);
            }

        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 touchPos = Input.mousePosition;

                if(CallMapData(touchPos) == (int)ChipType.Air)
                {
                    ChangeMapData(touchPos, ChipType.Block0);
                }
                else if(CallMapData(touchPos) == (int)ChipType.Block0)
                {
                    ChangeMapData(touchPos, ChipType.Air);
                }
                Debug.Log(CallMapData(touchPos));
                
                //var pos = MapPosUpdate(oldTouchPos, touchPos, ChipType.Block1);
                oldTouchPos = touchPos;
            }
        }

        void Initialize()
        {
            Load();
            temPos.Clear();
        }

        void Load()
        {
            width = width / 32 / 2;
            height = height / 32 / 2;

            map = new int[width * 2, height * 2];

            for (int y = height * -1; y < height; y++)
            {
                for (int x = width * -1; x < width; x++)
                {
                    var position = new Vector3Int(x, y, 0);
                    var tileSprite = foreGround.GetSprite(position);
                    if (tileSprite)
                    {
                        map[x + width, y + height] = int.Parse(Regex.Replace(tileSprite.name, @"[^0-9]", ""));
                    }
                    else
                    {
                        map[x + width, y + height] = -1;
                    }
                }
            }
        }

        public int CallMapData(int posX, int posY)
        {
            return map[posX, posY];
        }

        public int CallMapData(Vector3 pos)
        {
            Vector3Int gridPos = GetGridPos(pos);
            return map[gridPos.x, gridPos.y];
        }

        public int CallMapData(Vector3Int gridPos)
        {
            return map[gridPos.x, gridPos.y];
        }

        public void ChangeMapData(Vector3 pos, ChipType changeChip)
        {
            Vector3Int gridPos = GetGridPos(pos);
            Tile tile = null;
            if (mapChip.ContainsKey((int)changeChip) && ChipType.Air != changeChip)
            {
                tile = mapChip[(int)changeChip];
            }
            foreGround.SetTile(new Vector3Int(gridPos.x - width, gridPos.y - height, 0), tile);

            map[gridPos.x, gridPos.y] = (int)changeChip;

        }

        public void ChangeMapData(int posX, int posY, ChipType changeChip)
        {
            Tile tile = null;
            if (mapChip.ContainsKey((int)changeChip) && ChipType.Air != changeChip)
            {
                tile = mapChip[(int)changeChip];
            }
            foreGround.SetTile(new Vector3Int(posX - width, posY - height, 0), tile);

            map[posX, posY] = (int)changeChip;
        }


        public List<Map.Pos> SearchMapData(ChipType searchChip)
        {
            // 一時的な座標保存変数を初期化
            temPos.Clear();
            if (!Enum.IsDefined(typeof(ChipType), searchChip))
            {
                return temPos;
            }

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    Search(x, y, searchChip);
                }
            }
            return temPos;
        }

        private void Search(int x, int y, ChipType searchChip)
        {
            if (map[y, x] == (int)searchChip)
            {
                temPos.Add(new Pos((byte)x, (byte)y));
            }
        }

        public Vector3Int GetGridPos(Vector3 pos)
        {
            Vector3Int gridPos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(pos));
            gridPos.x = gridPos.x + width;
            gridPos.y = gridPos.y + height;
            return gridPos;
        }

        public Vector3 CellToWorld(Vector3Int pos)
        {
            var gridPos = new Vector3Int(pos.x - width, pos.y - height, pos.z);
            var worldPos = grid.CellToWorld(gridPos);
            worldPos.x += 0.5f;
            worldPos.y += 0.5f;
            return worldPos;
        }

        public static string IntConvStr(int value)
        {
            // stringからTexNamesに変換
            var str = Enum.GetName(typeof(ChipType), value);
            // TexNamesの中に変換したものが含まれているかの確認
            if (Enum.IsDefined(typeof(ChipType), str))
            {
                return str;
            }
            return null;
        }

        /// <summary>
        /// マップ更新
        /// </summary>
        /// <param name="befhoreMapPos">過去の位置を通路</param>
        /// <param name="aftereMapPos">今の位置を対象のチップタイプ</param>
        /// <param name="chipType"></param>
        /// <returns></returns>
        public Vector3Int MapPosUpdate(Vector3Int befhoreMapPos, Vector3 aftereMapPos, ChipType chipType)
        {
            if (!Enum.IsDefined(typeof(ChipType), chipType))
            {
                //return new Vector3();
            }
            Vector3Int newMapPos = GetGridPos(aftereMapPos);
            
            // マップ座標値の更新
            ChangeMapData(befhoreMapPos.x, befhoreMapPos.y, ChipType.Air);
            ChangeMapData(aftereMapPos, chipType);
            // 新しいマップ座標を返す
            return newMapPos;
        }

        public int MaxWidth
        {
            get { return map.GetLength(0); }
        }
        public int MaxHeight
        {
            get { return map.GetLength(1); }
        }
    }
}
