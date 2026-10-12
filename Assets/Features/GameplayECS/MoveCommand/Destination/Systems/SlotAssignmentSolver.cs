using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.GameplayECS.MoveCommand.Destination.Systems
{
    public sealed class SlotAssignmentSolver
    {
        private const int NONE = -1;

        private double[,] _cost = new double[0, 0];
        private double[] _rowPotential = Array.Empty<double>();
        private double[] _columnPotential = Array.Empty<double>();
        private int[] _rowInColumn = Array.Empty<int>();
        private double[] _cheapestReach = Array.Empty<double>();
        private int[] _reachedFromColumn = Array.Empty<int>();
        private bool[] _isColumnInChain = Array.Empty<bool>();

        public void Solve(IReadOnlyList<Vector3> units, IReadOnlyList<Vector3> slots, List<int> slotIndexOfUnit)
        {
            slotIndexOfUnit.Clear();
            for (int i = 0; i < units.Count; i++)
                slotIndexOfUnit.Add(NONE);

            if (units.Count == 0 || slots.Count == 0) return;

            bool unitsAreRows = units.Count <= slots.Count;
            int rows = unitsAreRows ? units.Count : slots.Count;
            int columns = unitsAreRows ? slots.Count : units.Count;

            Prepare(rows, columns);
            FillCosts(units, slots, unitsAreRows, rows, columns);

            for (int row = 0; row < rows; row++)
                AddRow(row, columns);

            for (int column = 0; column < columns; column++)
            {
                int row = _rowInColumn[column];
                if (row == NONE) continue;

                int unit = unitsAreRows ? row : column;
                int slot = unitsAreRows ? column : row;
                slotIndexOfUnit[unit] = slot;
            }
        }

        private void AddRow(int newRow, int columns)
        {
            ResetChain(columns);

            int searchingRow = newRow;
            int fromColumn = NONE;

            while (true)
            {
                int nextColumn = FindCheapestColumn(searchingRow, fromColumn, columns, out double extraCost);
                ShiftPotentials(newRow, extraCost, columns);
                _isColumnInChain[nextColumn] = true;

                int owner = _rowInColumn[nextColumn];
                if (owner == NONE)
                {
                    MoveChain(newRow, nextColumn);
                    return;
                }

                searchingRow = owner;
                fromColumn = nextColumn;
            }
        }

        private int FindCheapestColumn(int searchingRow, int fromColumn, int columns, out double extraCost)
        {
            extraCost = double.PositiveInfinity;
            int cheapestColumn = NONE;

            for (int column = 0; column < columns; column++)
            {
                if (_isColumnInChain[column]) continue;

                double reach = ReducedCost(searchingRow, column);
                if (reach < _cheapestReach[column])
                {
                    _cheapestReach[column] = reach;
                    _reachedFromColumn[column] = fromColumn;
                }

                if (_cheapestReach[column] < extraCost)
                {
                    extraCost = _cheapestReach[column];
                    cheapestColumn = column;
                }
            }
            
            
            
            return cheapestColumn;
        }

        private void ShiftPotentials(int newRow, double extraCost, int columns)
        {
            _rowPotential[newRow] += extraCost;

            for (int column = 0; column < columns; column++)
            {
                if (_isColumnInChain[column])
                {
                    _rowPotential[_rowInColumn[column]] += extraCost;
                    _columnPotential[column] -= extraCost;
                }
                else
                {
                    _cheapestReach[column] -= extraCost;
                }
            }
        }

        private void MoveChain(int newRow, int freeColumn)
        {
            int column = freeColumn;
            while (column != NONE)
            {
                int previousColumn = _reachedFromColumn[column];
                _rowInColumn[column] = previousColumn == NONE ? newRow : _rowInColumn[previousColumn];
                column = previousColumn;
            }
        }

        private double ReducedCost(int row, int column)
            => _cost[row, column] - _rowPotential[row] - _columnPotential[column];

        private void ResetChain(int columns)
        {
            for (int column = 0; column < columns; column++)
            {
                _cheapestReach[column] = double.PositiveInfinity;
                _reachedFromColumn[column] = NONE;
                _isColumnInChain[column] = false;
            }
        }

        private void FillCosts(IReadOnlyList<Vector3> units, IReadOnlyList<Vector3> slots, bool unitsAreRows,
            int rows, int columns)
        {
            for (int row = 0; row < rows; row++)
            for (int column = 0; column < columns; column++)
            {
                int unit = unitsAreRows ? row : column;
                int slot = unitsAreRows ? column : row;
                _cost[row, column] = FlatSqrDistance(units[unit], slots[slot]);
            }
        }

        private void Prepare(int rows, int columns)
        {
            if (_cost.GetLength(0) < rows || _cost.GetLength(1) < columns)
                _cost = new double[rows, columns];

            if (_rowPotential.Length < rows)
                _rowPotential = new double[rows];

            if (_columnPotential.Length < columns)
            {
                _columnPotential = new double[columns];
                _rowInColumn = new int[columns];
                _cheapestReach = new double[columns];
                _reachedFromColumn = new int[columns];
                _isColumnInChain = new bool[columns];
            }

            Array.Clear(_rowPotential, 0, rows);
            Array.Clear(_columnPotential, 0, columns);
            for (int column = 0; column < columns; column++)
                _rowInColumn[column] = NONE;
        }

        private static double FlatSqrDistance(Vector3 a, Vector3 b)
        {
            double dx = a.x - b.x;
            double dz = a.z - b.z;
            return dx * dx + dz * dz;
        }
    }
}
