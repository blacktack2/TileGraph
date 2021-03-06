using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using XNode;

namespace TileGraph
{
    [CreateAssetMenu]
    public class TileGraph : NodeGraph
    {
        private static int _IDCounter = 0;

        [SerializeField]
        private ComputeShader _ComputeShader;
        public ComputeShader computeShader {get {return _ComputeShader;}}

        private Utilities.FunctionLibrary _FunctionLibrary;
        public Utilities.FunctionLibrary functionLibrary {get {return _FunctionLibrary;}}

        private Utilities.EditorUtilities _EditorUtilities;
        public Utilities.EditorUtilities editorUtilities {get {return _EditorUtilities;}}

        [SerializeField, HideInInspector]
        private bool _GPUEnabledGlobal = true;
        public bool GPUEnabledGlobal {get {return _GPUEnabledGlobal;}}

        private int _ID;
        public int id {get {return _ID;}}

        [ContextMenu("Toggle Default GPU")]
        public void ToggleGPUDefault() => _GPUEnabledGlobal = ! _GPUEnabledGlobal;

        void OnEnable()
        {
            _ID = _IDCounter++;

            _FunctionLibrary = new Utilities.FunctionLibrary(_ComputeShader);
            _EditorUtilities = new Utilities.EditorUtilities();
            _FunctionLibrary.Enable();
            _EditorUtilities.Enable();
        }

        void OnDisable()
        {
            _FunctionLibrary.Disable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _FunctionLibrary.Disable();
        }

        public string CheckInOutName(string outputName, Nodes.IInputOutputNode node)
        {
            if (outputName == "" || outputName == null)
            {
                if (node is Nodes.IInputNode)
                    return GenerateInOutName("Input");
                else if (node is Nodes.IOutputNode)
                    return GenerateInOutName("Output");
            }
            
            bool alreadyExists = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (!ReferenceEquals(nodes[i], node) && nodes[i] is Nodes.IInputOutputNode && ((Nodes.IInputOutputNode) nodes[i]).GetName() == outputName)
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (!alreadyExists)
                return outputName;

            string prefix = outputName;
            while (prefix.Length > 0 && char.IsNumber(prefix, prefix.Length - 1))
                prefix = prefix.Substring(0, prefix.Length - 1);
            
            return GenerateInOutName(prefix);
        }

        public string GenerateInOutName(string prefix)
        {
            Regex formatRegex = new Regex("^" + prefix + "([0-9]+)$");
            List<int> existing = new List<int>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is Nodes.IOutputNode)
                {
                    Match m = formatRegex.Match(((Nodes.IOutputNode) nodes[i]).GetName());
                    if (m.Success)
                        existing.Add(int.Parse(m.Groups[1].Value));
                }
            }
            int value = 1;
            while (existing.Contains(value))
                value++;
            return prefix + value;
        }

        public T GetOutputValue<T>(string name)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is Nodes.IOutputNode)
                {
                    Nodes.IOutputNode node = nodes[i] as Nodes.IOutputNode;
                    string nodeName = node.GetName();
                    if (nodeName == name)
                        return node.GetOutput<T>();
                }
            }
            throw new System.ArgumentException(
                string.Format("No output port with name '{0}' and type '{1}' found.", name, typeof(T).Name));
        }

        public void SetInputValue<T>(string name, T value)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is Nodes.IInputNode)
                {
                    Nodes.IInputNode node = nodes[i] as Nodes.IInputNode;
                    string nodeName = node.GetName();
                    if (nodeName == name)
                    {
                        node.SetInput(value);
                        return;
                    }
                }
            }
            throw new System.ArgumentException(
                string.Format("No input port with the name '{0}' and type '{1}' found.", name, typeof(T).Name));
        }

        public List<GraphPort> GetInputNodes()
        {
            List<GraphPort> ports = new List<GraphPort>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is Nodes.IInputNode)
                {
                    Nodes.IInputNode node = nodes[i] as Nodes.IInputNode;
                    ports.Add(new GraphPort(node.GetName(), node.GetValueType()));
                }
            }
            ports.Sort(GraphPort.SortOrder);
            return ports;
        }

        public List<GraphPort> GetOutputNodes()
        {
            List<GraphPort> ports = new List<GraphPort>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is Nodes.IOutputNode)
                {
                    Nodes.IOutputNode node = nodes[i] as Nodes.IOutputNode;
                    ports.Add(new GraphPort(node.GetName(), node.GetValueType()));
                }
            }
            ports.Sort(GraphPort.SortOrder);
            return ports;
        }
    }

    public class GraphPort
    {
        private static Type[] typeOrder = new Type[] {
            typeof(int), typeof(float), typeof(string),
            typeof(Types.TileMap), typeof(Types.TileMapBool), typeof(Types.TileMapCont), typeof(Types.TileMapUint)
        };

        public string portName;
        public Type portType;

        public GraphPort(string portName, Type portType)
        {
            this.portName = portName;
            this.portType = portType;
        }

        /// <summary> Sorted in order of type, then name. Types not in
        /// <paramref name="typeOrder" /> are ordered erroneously. </summary>
        public static int SortOrder(GraphPort left, GraphPort right)
        {
            int leftTypeIndex  = Array.IndexOf(typeOrder, left.GetType());
            int rightTypeIndex = Array.IndexOf(typeOrder, right.GetType());
            int nameOrder = left.portName.CompareTo(right.portName);
            if (leftTypeIndex != -1)
            {
                if (rightTypeIndex != -1)
                {
                    if (leftTypeIndex == rightTypeIndex)
                        return nameOrder;
                    else
                        return leftTypeIndex > rightTypeIndex ? 1 : -1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (rightTypeIndex != -1)
                {
                    return -1;
                }
                else
                {
                    if (leftTypeIndex == rightTypeIndex)
                        return nameOrder;
                    else
                        return leftTypeIndex > rightTypeIndex ? 1 : -1;
                }
            }
        }
    }
}
