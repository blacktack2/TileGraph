using UnityEngine;
using XNode;

namespace TileGraph.Nodes
{
    /// <summary> Operation node for filling a <paramref name="TileMapBool" />
    /// with a single value. </summary>
    [CreateNodeMenu("Operations/TileMap/Bool/Fill", 10)]
    public class TileMapBoolFillNode : BaseNode
    {
        [SerializeField, Input] private Types.TileMapBool _TileMapIn;
        /// <summary> Value to fill the TileMap with. </summary>
        [SerializeField, Input, Range(0, 1)] private int _FillValue = 0;
        [SerializeField, Output] private Types.TileMapBool _TileMapOut;

        private int _CurrentFillValue = 0;

        private long _TileMapInIDBuffer = 0L;
        private Types.TileMapBool _TileMapOutBuffer;

        private void Reset()
        {
            name = "Fill Tile-Map Bool";
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "_TileMapOut")
            {
                GetTileMapInput(
                    "_TileMapIn", "_TileMapOut",
                    ref _TileMapOutBuffer, ref _TileMapInIDBuffer,
                    GetFillValue() != _CurrentFillValue
                );
                return _TileMapOutBuffer;
            }
            return null;
        }
        
        protected override void UpdateTileMapOutput(string portName)
        {
            if (portName == "_TileMapOut")
            {
                _CurrentFillValue = GetFillValue();
                _Graph.functionLibrary.tileMapOperations.FillTileMap(_TileMapOutBuffer, _CurrentFillValue, GPUEnabled);
            }
        }

        private int GetFillValue()
        {
            return GetInputValue<int>("_FillValue", _FillValue);
        }
    }
}
