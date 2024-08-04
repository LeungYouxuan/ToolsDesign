
using System.Collections.Generic;
using UnityEngine;
using XNode;
using XNodeEditor;
namespace CustomGraphWindow
{
    [CreateAssetMenu(fileName = "DialogGraph")]
    public class DialogGraph : NodeGraph
    {
        
    }

    public class SimpleNode : Node
    {
        public int a;
        public int b;
        [Output] public int sum;
        public override object GetValue(NodePort port)
        {
            return Add();
        }
        public int Add()
        {
            return a + b;
        }
    }
    [CustomNodeEditor(typeof(SimpleNode))]
    public class SimpleNodeEditor : NodeEditor
    {
        private SimpleNode _simpleNode;
        public override void OnBodyGUI()
        {
            if (_simpleNode == null) _simpleNode=target as SimpleNode;
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("a"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("b"));
            UnityEditor.EditorGUILayout.LabelField("The Value is "+_simpleNode.Add());
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("sum"));
            serializedObject.ApplyModifiedProperties();
        }
    }
    public class DialogNode : Node
    {
        public int dialogId;
        public string dialogName;
        public TextAsset contentText;
        [Output(dynamicPortList = true)] [TextArea]public string[] optionArray;
        [Output]public string outputOption;
        [Input]public string inputOption;
    }
}
