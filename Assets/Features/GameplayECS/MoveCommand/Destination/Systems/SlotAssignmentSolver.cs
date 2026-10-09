using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.GameplayECS.MoveCommand.Destination.Systems
{
    public sealed class SlotAssignmentSolver
    {
        private const int UNASSIGNED = -1;

        private double[,] _cost = new double[1, 1];
        private double[] _rowPotential = new double[1];
        private double[] _columnPotential = new double[1];
        private double[] _minSlack = new double[1];
        private int[] _rowOfColumn = new int[1];
        private int[] _previousColumn = new int[1];
        private bool[] _usedColumn = new bool[1];

        public void Solve(IReadOnlyList<Vector3> units, IReadOnlyList<Vector3> slots, List<int> slotIndexOfUnit)
        {
            slotIndexOfUnit.Clear();
            for (int i = 0; i < units.Count; i++)
                slotIndexOfUnit.Add(UNASSIGNED);

            if (units.Count == 0 || slots.Count == 0) return;

            bool unitsAreRows = units.Count <= slots.Count;
            int rows = unitsAreRows ? units.Count : slots.Count;
            int columns = unitsAreRows ? slots.Count : units.Count;

            EnsureCapacity(rows, columns);

            for (int row = 1; row <= rows; row++)
            for (int column = 1; column <= columns; column++)
            {
                int unit = unitsAreRows ? row - 1 : column - 1;
                int slot = unitsAreRows ? column - 1 : row - 1;
                _cost[row, column] = FlatSqrDistance(units[unit], slots[slot]);
            }

            RunHungarian(rows, columns);

            for (int column = 1; column <= columns; column++)
            {
                int row = _rowOfColumn[column];
                if (row == 0) continue;

                int unit = unitsAreRows ? row - 1 : column - 1;
                int slot = unitsAreRows ? column - 1 : row - 1;
                slotIndexOfUnit[unit] = slot;
            }
        }

        private void RunHungarian(int rows, int columns)
        {
            Array.Clear(_rowPotential, 0, rows + 1);
            Array.Clear(_columnPotential, 0, columns + 1);
            Array.Clear(_rowOfColumn, 0, columns + 1);
            Array.Clear(_previousColumn, 0, columns + 1);

            for (int row = 1; row <= rows; row++)
            {
                _rowOfColumn[0] = row;
                int currentColumn = 0;

                for (int column = 0; column <= columns; column++)
                {
                    _minSlack[column] = double.PositiveInfinity;
                    _usedColumn[column] = false;
                }

                do
                {
                    _usedColumn[currentColumn] = true;
                    int currentRow = _rowOfColumn[currentColumn];
                    double delta = double.PositiveInfinity;
                    int nextColumn = 0;

                    for (int column = 1; column <= columns; column++)
                    {
                        if (_usedColumn[column]) continue;

                        double slack = _cost[currentRow, column] - _rowPotential[currentRow] - _columnPotential[column];
                        if (slack < _minSlack[column])
                        {
                            _minSlack[column] = slack;
                            _previousColumn[column] = currentColumn;
                        }

                        if (_minSlack[column] < delta)
                        {
                            delta = _minSlack[column];
                            nextColumn = column;
                        }
                    }

                    for (int column = 0; column <= columns; column++)
                    {
                        if (_usedColumn[column])
                        {
                            _rowPotential[_rowOfColumn[column]] += delta;
                            _columnPotential[column] -= delta;
                        }
                        else
                        {
                            _minSlack[column] -= delta;
                        }
                    }

                    currentColumn = nextColumn;
                } while (_rowOfColumn[currentColumn] != 0);

                do
                {
                    int previous = _previousColumn[currentColumn];
                    _rowOfColumn[currentColumn] = _rowOfColumn[previous];
                    currentColumn = previous;
                } while (currentColumn != 0);
            }
        }

        private void EnsureCapacity(int rows, int columns)
        {
            if (_cost.GetLength(0) < rows + 1 || _cost.GetLength(1) < columns + 1)
                _cost = new double[rows + 1, columns + 1];

            if (_rowPotential.Length < rows + 1)
                _rowPotential = new double[rows + 1];

            if (_columnPotential.Length < columns + 1)
            {
                _columnPotential = new double[columns + 1];
                _minSlack = new double[columns + 1];
                _rowOfColumn = new int[columns + 1];
                _previousColumn = new int[columns + 1];
                _usedColumn = new bool[columns + 1];
            }
        }

        private static double FlatSqrDistance(Vector3 a, Vector3 b)
        {
            double dx = a.x - b.x;
            double dz = a.z - b.z;
            return dx * dx + dz * dz;
        }
    }
}