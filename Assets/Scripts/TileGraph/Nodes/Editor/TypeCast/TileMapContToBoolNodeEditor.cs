using UnityEditor;
using UnityEngine;

namespace TileGraph.Editors
{
    [CustomNodeEditor(typeof(Nodes.TileMapContToBoolNode))]
    public class TileMapContToBoolNodeEditor : BaseNodeEditor<Nodes.TileMapContToBoolNode>
    {
        private SerializedProperty _TileMapIn, _Threshold, _TileMapOut;

        protected override bool GPUToggleable => true;

        protected override void OnNodeEnable()
        {
            _TileMapIn  = serializedObject.FindProperty("_TileMapIn");
            _Threshold  = serializedObject.FindProperty("_Threshold");
            _TileMapOut = serializedObject.FindProperty("_TileMapOut");

            AddPreview("_TileMapOut");
        }

        protected override void NodeInputGUI()
        {
            EditorGUILayout.BeginHorizontal();

            graph.editorUtilities.PortFieldMinLabel(_TileMapIn);
            graph.editorUtilities.PortFieldMinLabel(_TileMapOut);

            EditorGUILayout.EndHorizontal();
            
            graph.editorUtilities.PortFieldMinLabel(_Threshold);
        }

        protected override void NodeBodyGUI()
        {
        }
    }
}
