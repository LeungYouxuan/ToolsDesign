using System.Collections.Generic;
using System.Text;
using QFramework;
using UnityEngine;
namespace SOScripts
{
    [CreateAssetMenu(fileName = "游戏道具配置池",menuName = "手动创建游戏道具配置池")]
    public class GameItemPool : ScriptableObject
    {
        public List<GameItem> gameItemListPool = new List<GameItem>();
    }
    [System.Serializable]
    public partial class GameItem
    {
        public int id;
        [TextArea] public string name;
        [TextArea] public string des;
        public int price;//道具的基准价格
        public Sprite sprite;//道具的ICON的Sprite
        public new string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"道具ID:{id}  ");
            builder.AppendLine($"道具名字:{name}  ");
            builder.AppendLine($"道具描述:{des}");
            return builder.ToString();
        }
        public GameItem()
        {
            
        }
        public GameItem(int id, string name, string des)
        {
            this.id = id;
            this.name = name;
            this.des = des;
        }
        public GameItem(int id, string name, string des, int price)
        {
            
            this.id = id;
            this.name = name;
            this.des = des;
            this.price = price;
        }
    }
}