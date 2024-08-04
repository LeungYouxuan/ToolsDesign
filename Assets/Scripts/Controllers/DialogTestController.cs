using System.Collections;
using System.Collections.Generic;
using CustomGraphWindow;
using UnityEngine;

namespace Controllers
{
    public class DialogTestController : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]private DialogGraph dialogGraph;
        void Start()
        {
            // if (dialogGraph == null)
            // {
            //     Debug.LogError("DialogGraph is not assigned!");
            //     return;
            // }
            //
            // // 示例：遍历图中的所有节点并打印对话ID和名称
            // foreach (var node in dialogGraph.nodes)
            // {
            //     if (node is DialogNode dialogNode)
            //     {
            //         Debug.Log($"Dialog ID: {dialogNode.dialogId}, Name: {dialogNode.dialogName}");
            //     }
            // }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
