using Models;
using QFramework;
using Queries;
using UnityEngine;

namespace Test
{
    [System.Serializable]
    public class VendingMachine:AbstractInventory,ICanSendEvent
    {
        public VendingMachine(int length, int width, int gridSize) : base(length, width, gridSize)
        {
        }
        /// <summary>
        /// 上架物品
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
                    new VendingMachineGridInfo(id, count, basePrice));
            }
        }
        /// <summary>
        /// 根据Id出售某件物品
        /// </summary>
        /// <param name="id"></param>
        public void SellProductById(int id)
        {
            if (!FindGameItemPosAtFirstById(id, out var pos)) return;
            //Debug.Log($"SellProductPos:{pos}");
            var gridInfo = (VendingMachineGridInfo)gridInfoList[pos];
            this.SendCommand(new ChangePlayerCoinsCommand(-gridInfo.price));
            RemoveGameItemById(id);
        }
        private class VendingMachineGridInfo : InventoryGridInfo
        {
            public int price;
            public VendingMachineGridInfo(int id, int count, int price) : base(id, count)
            {
                this.price = price;
            }
        }
    }
}