using Models;
using QFramework;
using SOScripts;
using Systems;
using Test;
using UnityEngine;
namespace Commands
{
    public class InitInventoryModelCommand:AbstractCommand
    {
        private GameItemPool gameItemPool;

        public InitInventoryModelCommand(GameItemPool gameItemPool)
        {
            this.gameItemPool = gameItemPool;
        }
        protected override void OnExecute()
        {
            var dict = this.GetModel<InventoryModel>().gameItemDictPool;
            foreach (var t in gameItemPool.gameItemListPool)
            {
                dict.TryAdd(t.id, t);
            }
            if (dict.Count == gameItemPool.gameItemListPool.Count)
            {
                Debug.Log($"<color=green>InventoryModel初始化成功!</color>");
            }
            else
            {
                Debug.LogError($"<color=green>InventoryModel初始化失败!</color>");
            }
        }
    }

    public class InitInventorySystemCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            //新建一个超市对象
            this.GetSystem<InventorySystem>().bigRunFaSupermarket = new BigRunFaSupermarket(9, 4, 64);
            //测试，给超市上架货品
            this.GetSystem<InventorySystem>().bigRunFaSupermarket.StockProduct(1,64);
            this.GetSystem<InventorySystem>().bigRunFaSupermarket.StockProduct(2,64);
            this.GetSystem<InventorySystem>().bigRunFaSupermarket.StockProduct(3,64);
        }
    }
}