using UnityEditor;
using UnityEngine;

namespace TileGraph.Editors
{
    [CustomNodeEditor(typeof(Nodes.TileMapContFillNode))]
    public class TileMapContFillNodeEditor : BaseNodeEditor<Nodes.TileMapContFillNode>
    {
        private SerializedProperty _TileMapIn, _FillValue, _TileMapOut;

        protected override void OnNodeEnable()
        {
            _TileMapIn  = serializedObject.FindProperty("_TileMapIn");
            _FillValue  = serializedObject.FindProperty("_FillValue");
            _TileMapOut = serializedObject.FindProperty("_TileMapOut");

            AddPreview("_TileMapOut");
        }

        protected override void NodeInputGUI()
        {
            EditorGUILayout.BeginHorizontal();

            graph.CAEditorUtilities.PortFieldMinLabel(_TileMapIn);
            graph.CAEditorUtilities.PortFieldMinLabel(_TileMapOut);

            EditorGUILayout.EndHorizontal();

            graph.CAEditorUtilities.PortFieldMinLabel(_FillValue, new GUIContent("fill value"));
        }

        protected override void NodeBodyGUI()
        {
        }
    }
}