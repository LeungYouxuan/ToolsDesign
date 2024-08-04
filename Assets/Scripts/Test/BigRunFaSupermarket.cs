using System.Collections.Generic;
using Models;
using QFramework;
using Queries;
using UnityEngine;
namespace Test
{
    public class BigRunFaSupermarket:AbstractInventory,ICanSendEvent
    {
        private BindableProperty<int> playerBuyCount=new BindableProperty<int>();//玩家购买的总次数
        private static Dictionary<int, int> buyingRecordDict = new Dictionary<int, int>();//玩家购买商品的记录
        public BigRunFaSupermarket(int length, int width, int gridSize) : base(length, width, gridSize)
        {
            playerBuyCount.Value = 0;
            playerBuyCount.Register(value =>
            {
                switch (value)
                {
                    case 1:
                        Debug.Log("购物萌新");
                        break;
                    case 10:
                        Debug.Log("购物达人");
                        break;
                    case 15:
                        Debug.Log("购物狂人");
                        break;
                }
            });
        }
        /// <summary>
        /// 上架若干件指定商品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stockCount"></param>
        public void StockProduct(int id,int stockCount)
        {
            //获取物品的基准价格
            int basePrice=this.SendQuery(new GetGameItemByIdQuery(id)).price;
            for (int i = 1; i <= stockCount; i++)
            {
                AddGameItemById(id,(idParameter,count)=>
                    new BigRunFaSupermarketGridInfo(id, count, basePrice));
            }
        }
        /// <summary>
        /// 根据位置出售某件物品
        /// </summary>
        /// <param name="pos"></param>
        public void SellProductByPos(int pos)
        {
            
        }
        /// <summary>
        /// 根据Id出售某件物品
        /// </summary>
        /// <param name="id"></param>
        public void SellProductById(int id)
        {
            if (!FindGameItemPosAtFirstById(id, out var pos)) return;
            var gridInfo = (BigRunFaSupermarketGridInfo)gridInfoList[pos];
            this.SendCommand(new ChangePlayerCoinsCommand(-gridInfo.price));
            RemoveGameItemById(id);
            if (!buyingRecordDict.TryAdd(id, 1))
            {
                buyingRecordDict[id] += 1;
            }
            playerBuyCount.Value += 1;
            //判断是否要打折降价
            ShouldDiscount(id);
        }
        /// <summary>
        /// 给某件物品打折
        /// </summary>
        public void DiscountProductById(int id,float value)
        {
            if (FindGameItemPosById(id, out var posList))
            {
                foreach (var pos in posList)
                {
                    if (gridInfoList[pos] == null) continue;
                    if(gridInfoList[pos].GetType()==typeof(BigRunFaSupermarketGridInfo))
                    {
                        var gridInfo = (BigRunFaSupermarketGridInfo)gridInfoList[pos];
                        var temp=gridInfo.price * (1-value);
                        gridInfo.price = (int)temp;
                        gridInfoList[pos] = gridInfo;
                    }
                }
            }
        }
        public void PrintAllProductInfo()
        {
            foreach (var t in gridInfoList)
            {
                if(t==null)continue;
                var item = this.SendQuery(new GetGameItemByIdQuery(t.id));
                if(item==null)continue;
                var gridInfo = (BigRunFaSupermarketGridInfo)t;
                Debug.Log($"ID:{item.id}  Name:{item.name}  Des:{item.des}  Count:{t.count}  Price:{gridInfo.price}");
            }
        }
        private bool ShouldDiscount(int id)
        {
            if (!buyingRecordDict.ContainsKey(id)) return false;
            if (buyingRecordDict[id] < 5) return false;
            DiscountProductById(id,0.3f);
            return true;
        }
        public class BigRunFaSupermarketGridInfo : InventoryGridInfo
        {
            public int price;
            public BigRunFaSupermarketGridInfo(int id, int count,int price):base(id, count)
            {
                this.id = id;
                this.count = count;
                this.price = price;
            }
        }
    }
}