using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class CoordinateConstraint : MonoBehaviour
{
    [SerializeField] List<CellConstraint> constraints = new List<CellConstraint>();
    HashSet<ListStruct> localAllowList = new HashSet<ListStruct>();
    HashSet<ListStruct> localDenyList = new HashSet<ListStruct>();
    HashSet<ListStruct> globalAllowList = new HashSet<ListStruct>(); //To process logic with OR
    HashSet<ListStruct> globalDenyList = new HashSet<ListStruct>(); //To have a final list of cells to remove

    //TODO
    //Add a final allow and deny list to be able to utilize the AND and the OR functionality of the component
    //Current version works on a local scope, only allows or denies based on if all constraints inside a cellconstraint align (AND)
    //Future version needs a global scope, using what is there currently but also with cell constraints together (OR)

    public HashSet<ListStruct> Run(Map map) {
        CompareCoordinates(map);
        return globalDenyList;
    }

    private void CompareCoordinates(Map map) {
        globalAllowList = new HashSet<ListStruct>();
        globalDenyList = new HashSet<ListStruct>();

        for (int y = 0; y < map.y; y++) {
            for (int x = 0; x < map.x; x++) {
                foreach (CellConstraint constraint in constraints) {

                    //Check for if the coordinates fit within the constraints, to avoid more loops if it isnt necessary
                    //if (constraint.Constraints.)
                    GetLogic(constraint, map.slots[x, y], x, y);
                    
                }
            }
        }
    }

    private void GetLogic(CellConstraint con, QuantumCell states, int x, int y) {
        localAllowList = new HashSet<ListStruct>();
        localDenyList = new HashSet<ListStruct>();

        foreach (Constraint constraint in con.Constraints) {
            int coord;
            if (constraint.coordinate == Coordinate.X) {
                coord = x;
            } else {
                coord = y;
            }


            switch (constraint.comparison) {
                case ComparisonType.Equal:
                    if (coord == constraint.value) {
                        CheckStates(con, states, x, y, true);
                    } else {
                        CheckStates(con, states, x, y, false);
                    }
                break;

                case ComparisonType.Not:
                    if (coord != constraint.value) {
                        CheckStates(con, states, x, y, true);
                    } else {
                        CheckStates(con, states, x, y, false);
                    }
                break;

                case ComparisonType.Over:
                    if (coord > constraint.value) {
                        CheckStates(con, states, x, y, true);
                    } else {
                        CheckStates(con, states, x, y, false);
                    }
                break;

                case ComparisonType.Under:
                    if (coord < constraint.value) {
                        CheckStates(con, states, x, y, true);
                    } else {
                        CheckStates(con, states, x, y, false);
                    }
                break;
            }
        }

        //Add local lists to global ones
        GlobalListsAdd();
    }

    private void CheckStates(CellConstraint constraint, QuantumCell quantum, int x, int y, bool allowed) {
        if (allowed) {
            foreach (Cell cell in constraint.ConstrainedCell) { //For each cell that is constrained
                foreach (CellInfo info in quantum.states) { //And for each state in a quantum cell
                    if (info.cell == cell) {
                        AllowListAdd(new ListStruct(info, x, y));

                    }
                }
            }
        } else { //If not allowed
            foreach (Cell cell in constraint.ConstrainedCell) { //For each cell that is constrained
                foreach (CellInfo info in quantum.states) { //And for each state in a quantum cell
                    if (info.cell == cell) {
                        DenyListAdd(new ListStruct(info, x, y));

                    }
                }
            }
        }
    }

    private void AllowListAdd(ListStruct cell) {
        if (localDenyList.Contains(cell)) {
            localDenyList.Remove(cell);
        }
        if (!localAllowList.Contains(cell)) {
            localAllowList.Add(cell);
        }
    }

    private void DenyListAdd(ListStruct cell) {
        //if (localAllowList.Contains(cell)) {
        //    //localAllowList.Remove(cell);
        //    return;
        //}
        if (!localAllowList.Contains(cell) && !globalAllowList.Contains(cell) && !localDenyList.Contains(cell)) {
            localDenyList.Add(cell);
        }
    }

    private void GlobalListsAdd() {
        foreach (ListStruct cell in localAllowList) {
            if (globalDenyList.Contains(cell)) {
                globalDenyList.Remove(cell);
            }
            if (!globalAllowList.Contains(cell)) {
                globalAllowList.Add(cell);
            }
        }

        foreach (ListStruct cell in localDenyList) {
            if (!globalAllowList.Contains(cell) && !globalDenyList.Contains(cell)) {
                globalDenyList.Add(cell);
            }
        }
    }
}

public enum ComparisonType {
    Over,
    Under,
    Equal,
    Not
}

public enum Coordinate {
    X,
    Y
}

[Serializable]
public class Constraint {
    [SerializeField] public Coordinate coordinate = Coordinate.X;
    [SerializeField] public ComparisonType comparison = ComparisonType.Over;
    [SerializeField] public int value = 0;
}

[Serializable]
public class CellConstraint {
    [SerializeField] public List<Cell> ConstrainedCell = new List<Cell>();
    [SerializeField] public List<Constraint> Constraints = new List<Constraint>();
}

public class ListStruct {
    public CellInfo cellinfo;
    public int x, y;
    public ListStruct(CellInfo cellInput, int xInput, int yInput) {
        cellinfo = cellInput;
        x = xInput;
        y = yInput;
    }
}