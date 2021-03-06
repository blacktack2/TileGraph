using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace TileGraph.Utilities
{
    /// <summary> Utility class for general Node Editor operations. </summary>
    public class EditorUtilities
    {
        private Texture2D _NullPreview;
        /// <summary> Default preview to show if no other image can be
        /// obtained/generated </summary>
        public Texture2D nullPreview {get {return _NullPreview;}}

        /// <summary> Image size of all node previews. </summary>
        public static readonly int previewWidth = 150;

        public void Enable()
        {
            InitNullPreview();
        }

        /// <summary> Create a <c>NodeEditorGUILayout.PropertyField</c> with
        /// the label width set to match the size of the label text </summary>
        /// <seealso cref="NodeEditorGUILayout.PropertyField(SerializedProperty port, GUIContent label)" />
        public void PortFieldMinLabel(SerializedProperty port, GUIContent label = null, bool includeChildren = true, params GUILayoutOption[] options)
        {
            if (label == null)
            {
                EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent(port.name)).x;
                NodeEditorGUILayout.PropertyField(port, includeChildren, options);
            }
            else
            {
                EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(label).x;
                NodeEditorGUILayout.PropertyField(port, label, includeChildren, options);
            }
            EditorGUIUtility.labelWidth = 0;
        }
        /// <summary> Create a <c>NodeEditorGUILayout.PropertyField</c> with
        /// the label width set to match the size of the label text </summary>
        /// <seealso cref="NodeEditorGUILayout.PortField(GUIContent label, SerializedProperty port)" />
        public float PortFieldMinLabel(NodePort port, GUIContent label = null, params GUILayoutOption[] options)
        {
            float width = 0;
            if (label == null)
            {
                width = GUI.skin.label.CalcSize(new GUIContent(port.fieldName)).x;
                EditorGUIUtility.labelWidth = width;
                NodeEditorGUILayout.PortField(port, options);
            }
            else
            {
                width = GUI.skin.label.CalcSize(label).x;
                EditorGUIUtility.labelWidth = width;
                NodeEditorGUILayout.PortField(label, port, options);
            }
            EditorGUIUtility.labelWidth = 0;
            return width;
        }

        /// <summary> Create a <c>EditorGUILayout.PropertyField</c> with
        /// the label width set to match the size of the label text </summary>
        /// <seealso cref="EditorGUILayout.PropertyField(SerializedProperty port, GUIContent label)" />
        public void PropertyFieldMinLabel(SerializedProperty property, GUIContent label = null, bool includeChildren = true, params GUILayoutOption[] options)
        {
            if (label == null)
            {
                EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent(property.name)).x;
                EditorGUILayout.PropertyField(property, includeChildren, options);
            }
            else
            {
                EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(label).x;
                EditorGUILayout.PropertyField(property, label, includeChildren, options);
            }
            EditorGUIUtility.labelWidth = 0;
        }

        /// <summary> Set label width to match the width of the text in
        /// <paramref name="label" /> </summary>
        /// <returns> Width value being set. </returns>
        public float SetLabelWidthToText(string label)
        {
            return SetLabelWidthToText(new GUIContent(label));
        }
        /// <summary> Set label width to match the width of the text in
        /// <paramref name="label" /> </summary>
        /// <returns> Width value being set. </returns>
        public float SetLabelWidthToText(GUIContent label)
        {
            float width = GUI.skin.label.CalcSize(label).x;
            EditorGUIUtility.labelWidth = width;
            return width;
        }

        private void InitNullPreview()
        {
            _NullPreview = new Texture2D(previewWidth, previewWidth);
            Color[] pixels = _NullPreview.GetPixels();
            for (int px = 0; px < pixels.Length; px++)
                pixels[px] = Color.magenta;
            _NullPreview.SetPixels(pixels);
            _NullPreview.Apply();
        }
    }
}