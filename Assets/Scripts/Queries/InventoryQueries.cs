using Models;
using QFramework;
using SOScripts;
using UnityEngine;

namespace Queries
{
    public class HasGameItemQuery : AbstractQuery<bool>
    {
        private int id;

        public HasGameItemQuery(int id)
        {
            this.id = id;
        }
        protected override bool OnDo()
        {
            return this.GetModel<InventoryModel>().gameItemDictPool.ContainsKey(id);
        }
    }
    public class GetGameItemByIdQuery:AbstractQuery<GameItem>
    {
        private int id;
        public GetGameItemByIdQuery(int id)
        {
            this.id = id;
        }
        protected override GameItem OnDo()
        {
            bool isNotEmpty=this.GetModel<InventoryModel>().gameItemDictPool.TryGetValue(id, out var item);
            if (isNotEmpty) return item;
            else
            {
                Debug.LogError($"ID:{id}不存在于GameItem配置池中");
                return null;
            }
        }
    }

    public class ChangePlayerCoinsCommand : AbstractCommand
    {
        private int changeValue;

        public ChangePlayerCoinsCommand(int changeValue)
        {
            this.changeValue = changeValue;
        }
        protected override void OnExecute()
        {
            this.GetModel<InventoryModel>().playerCoins.Value += changeValue;
        }
    }
}