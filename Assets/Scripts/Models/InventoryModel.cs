using System;
using System.Collections.Generic;
using QFramework;
using Queries;
using SOScripts;
using UnityEngine;
namespace Models
{
    public class InventoryModel : AbstractModel
    {
        public Dictionary<int, GameItem> gameItemDictPool = new Dictionary<int, GameItem>();
        public BindableProperty<int> playerCoins = new BindableProperty<int>();
        protected override void OnInit()
        {
            playerCoins.Value = 10000;
            playerCoins.Register(value =>
            {
                if (value < 0)
                {
                    Debug.LogWarning($"你欠债了！：{value}");
                }
            });
        }
    }
    public class InventoryGridInfo
    {
        public int id;
        public int count;

        protected InventoryGridInfo(int id,int count)
        {
            this.id = id;
            this.count = count;
        }
    }
    [Serializable]
    public abstract class AbstractInventory:ICanSendCommand,ICanSendQuery
    {
        private int length;//库存网格长度
        private int width;//库存网格宽度
        private int size;//库存网格格子总数
        private int gridSize;//单个库存格子能放置的数量
        [SerializeField]protected List<InventoryGridInfo> gridInfoList;//库存网格
        protected AbstractInventory(int length,int width,int gridSize)
        {
            this.length = length;
            this.width = width;
            size = length * width;
            this.gridSize = gridSize;
            gridInfoList = new List<InventoryGridInfo>(size);
            for (int i = 0; i < size; i++)
            {
                gridInfoList.Add(null);
            }
            Debug.Log("Inventory初始化成功");
        }
        /// <summary>
        /// 添加一个网格信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createInstance">网格信息类的构造委托</param>
        /// <typeparam name="T">网格信息类的构造函数</typeparam>
        protected void AddGameItemById<T>(int id,Func<int,int,T>createInstance) where T:InventoryGridInfo
        {
            bool hasGameItem = this.SendQuery(new HasGameItemQuery(id));
            if(!hasGameItem)
            {
                Debug.LogError($"ID:{id}不存在于GameItem配置池中");
                return;
            }
            if (TryGetLegalGrid(id, out var pos))
            {
                if (gridInfoList[pos] == null)
                {
                    gridInfoList[pos] = createInstance(id, 1);
                    Debug.Log($"成功在{pos}号位<color=red>放置了物品</color>；ID：{id}");
                    return;
                }
                if (gridInfoList[pos].count + 1 > gridSize)
                {
                    Debug.LogWarning($"无法在{pos}号位放置物品；该格子已满");
                    return;
                }
                gridInfoList[pos].count++;
                Debug.Log($"成功在{pos}号位<color=green>新增</color>了物品；ID：{id}");
            }
        }

        protected void RemoveGameItemById(int id)
        {
            for (int i = 0; i < gridInfoList.Count; i++)
            {
                if(gridInfoList[i]==null)return;
                if (gridInfoList[i].id == id)
                {
                    gridInfoList[i].count--;
                    if (gridInfoList[i].count is <= 0)
                    {
                        gridInfoList[i] = null;
                        Debug.Log($"清除了ID:{id}的物品");
                        return;
                    }
                    Debug.Log($"ID:{id}的物品数量减少，剩余数量：{gridInfoList[i].count}");
                    return;
                }
            }
        }
        public void ExchangeGameItems(int pos1,int pos2)
        {
            if(pos1>=gridSize||pos2>=gridSize)return;
            (gridInfoList[pos1], gridInfoList[pos2]) = (gridInfoList[pos2], gridInfoList[pos1]);
        }
        /// <summary>
        /// 根据库存网格，返回对应物品和对应物品的数量
        /// </summary>
        /// <param name="perGameItemCount"></param>
        /// <returns></returns>
        public List<GameItem> GetAllGameItemWithCountFromInventory(out List<int>perGameItemCount)
        {
            List<GameItem> result = new List<GameItem>();
            List<int> countList = new List<int>();
            foreach (var t in gridInfoList)
            {
                result.Add(this.SendQuery(new GetGameItemByIdQuery(t.id)));
                countList.Add(t.count);
            }
            perGameItemCount = countList;
            return result;
        }
        /// <summary>
        /// 获得当前库存中的所有GameItem，并返回它们的位置列表
        /// </summary>
        /// <param name="posList"></param>
        /// <returns></returns>
        public List<GameItem> GetAllGameItemWithPosFromInventory(out List<int> posList)
        {
            List<GameItem> result = new List<GameItem>();
            posList = new List<int>();
            for (int i = 0; i < gridInfoList.Count; i++)
            {
                if(gridInfoList[i]==null)continue;
                result.Add(this.SendQuery(new GetGameItemByIdQuery(gridInfoList[i].id)));
                posList.Add(i);
            }
            return result;
        }
        /// <summary>
        /// 打印当前库存的情况
        /// </summary>
        public void PrintAllGameItemFromInventoryToLog()
        {
            int index = 0;
            foreach (var t in gridInfoList)
            {
                if(t==null)
                {
                    index++;
                    continue;
                }
                var item = this.SendQuery(new GetGameItemByIdQuery(t.id));
                if(item==null)
                {
                    index++;
                    continue;
                }
                Debug.Log($"ID:{item.id}  Name:{item.name}  Des:{item.des}  Count:{t.count}  Pos:{index}");
                index++;
            }
        }
        /// <summary>
        /// 插入一件物品到库存时，找一个合法的放置位置
        /// 优先找空位置
        /// 其次找放置了同类物品且格子容量足够的格子
        /// </summary>
        /// <param name="id">插入物品ID</param>
        /// <param name="pos">返回的位置；若没找到，则返回-1</param>
        /// <returns></returns>
        private bool TryGetLegalGrid(int id,out int pos)
        {
            for (int i = 0; i < gridInfoList.Count; i++)
            {
                if (gridInfoList[i] == null)
                {
                    pos = i;
                    return true;
                }
                if (gridInfoList[i].id == id && gridInfoList[i].count < gridSize)
                {
                    pos = i;
                    return true;
                }
            }
            pos = -1;
            return false;
        }
        /// <summary>
        /// 找到第一个存储了符合ID号的物品的格子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected bool FindGameItemPosAtFirstById(int id,out int pos)
        {
            for (int i = 0; i < gridInfoList.Count; i++)
            {
                if (gridInfoList[i] != null&&gridInfoList[i].id==id)
                {
                    pos = i;
                    return true;
                }
            }
            pos = -1;
            return false;
        }
        /// <summary>
        /// 找出所有符合传入ID的物品的位置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected bool FindGameItemPosById(int id,out List<int> pos)
        {
            pos = new List<int>();
            int index = 0;
            foreach (var t in gridInfoList)
            {
                if(t==null)continue;
                if (t.id == id)
                {
                    pos.Add(index);
                }
                index++;
            }
            return pos.Count != 0;
        }

        public bool FindGameItemByPos(int pos,out GameItem gameItem,out InventoryGridInfo gridInfo)
        {
            if (pos >= size)
            {
                Debug.LogError("查询位置越界");
                gameItem = null;
                gridInfo = null;
                return false;
            }
            if (gridInfoList[pos] == null)
            {
                Debug.LogError("查询位置没有物品");
                gameItem = null;
                gridInfo = null;
                return false;
            }
            gridInfo = gridInfoList[pos];
            gameItem = this.SendQuery(new GetGameItemByIdQuery(gridInfo.id));
            return true;
        }
        /// <summary>
        /// 获取库存网格的信息
        /// </summary>
        /// <returns></returns>
        public List<InventoryGridInfo> GetInventoryGridInfoList()
        {
            List<InventoryGridInfo> result = new List<InventoryGridInfo>(gridInfoList);
            return result;
        }

        public int GetGridSize()
        {
            return gridSize;
        }
        public IArchitecture GetArchitecture()
        {
            return GameArchitecture.Interface;
        }
    }
}
