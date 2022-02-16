using System.Collections.Generic;
using UnityEngine;

namespace CAGraph.Utilities
{
    public class MatrixOperations
    {
        public static void RandomizeMatrix(Types.Matrix matrix, float chance, int seed)
        {
            Random.State state = Random.state;
            Random.InitState(seed);

            int[] cells = matrix.GetCells();
            for (int c = 0; c < cells.Length; c++)
                cells[c] = Random.value < chance ? 1 : 0;
            matrix.SetCells(cells);

            Random.state = state;
        }

        public static void ClearMatrix(Types.Matrix matrix)
        {
            FillMatrix(matrix, 0);
        }

        public static void FillMatrix(Types.Matrix matrix, int value)
        {
            int[] cells = matrix.GetCells();
            for (int c = 0; c < cells.Length; c++)
                cells[c] = value;
            matrix.SetCells(cells);
        }

        public static void ReplaceMatrixValues(Types.Matrix matrix, List<int> toReplace, int replaceWith)
        {
            int[] cells = matrix.GetCells();
            for (int c = 0; c < cells.Length; c++)
                if (toReplace.Contains(cells[c]))
                    cells[c] = replaceWith;
            matrix.SetCells(cells);
        }
    }
}
