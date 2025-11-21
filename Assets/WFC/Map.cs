using UnityEngine;
using System.Collections.Generic;
using System.Xml.Schema;

public class QuantumCell {
    public bool collapsed = false;
    public List<CellInfo> states;

    public QuantumCell(List<CellInfo> allCells) {
        states = allCells;
    }
}

public class Map
{
    [SerializeField] public int x = 10;
    [SerializeField] public int y = 10;
    [SerializeField] public Vector2 size = new Vector2(5, 5);


    public QuantumCell[,] slots;

    public void NewMap(List<CellInfo> allCells) {
        slots = new QuantumCell[x, y];

        for (int i = 0; i < slots.GetLength(0); i++) {
            for (int j = 0; j < slots.GetLength(1); j++) {
                slots[i, j] = new QuantumCell(allCells);
            }
        }

    }

    public Mesh PlaceMap() {
        CombineInstance[] instances = new CombineInstance[x * y];

        for (int i = 0; i < slots.GetLength(0); i++) {
            for (int j = 0; j < slots.GetLength(1); j++) {
                
                int index = i * x + j;
                Matrix4x4 pos = Matrix4x4.Translate(new Vector3(size.x * i, 0, size.y * j));

                instances[index] = new CombineInstance {
                    mesh = slots[i, j].states[0].GetMesh(),
                    transform = pos
                };
            }
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(instances);

        return finalMesh;

    }

    /// <summary>
    /// Finds and returns the cell that has the lowest entropy
    /// </summary>
    /// <returns>
    /// Coordinates of the cell with smallest entropy, or Vector2Int.left if all cells are collapsed
    /// </returns>
    public Vector2Int LeastEntropy() {
        List<Vector2Int> min = new List<Vector2Int>();
        int size = int.MaxValue;

        for (int i = 0; i < slots.GetLength(0); i++) { //For every element in the 2D array
            for (int j = 0; j < slots.GetLength(1); j++) {
                if (slots[i, j].collapsed) { //If cell is collapsed, ignore
                    continue;
                }
                if (slots[i, j].states.Count < size) { //If smaller than the current size, overwrite the previous list and set size to be equal to its length
                    min.Clear();
                    size = slots[i, j].states.Count;
                    min.Add(new Vector2Int(i, j));
                } else if (slots[i, j].states.Count == size) { //If equal to current size, add itself to the list
                    min.Add(new Vector2Int(i, j));
                }
            }
        }

        if (size == int.MaxValue) { //If size hasnt changed, all cells must have collapsed
            return Vector2Int.left;
        }

        Vector2Int chosenCoords = min[Random.Range(0, size)]; //Pick a random element of those that are smallest

        return chosenCoords;
    }

    /// <summary>
    /// Collapses 1 cell at the given coordinates
    /// </summary>
    /// <param name="coords"></param>
    /// <returns>
    /// CellInfo about the collapsed cell
    /// </returns>
    public CellInfo CollapseCell(Vector2Int coords) {

        List<CellInfo> allStates = slots[coords.x, coords.y].states;
        slots[coords.x, coords.y].states.Clear();
        slots[coords.x, coords.y].states.Add(allStates[Random.Range(0, allStates.Count)]);
        slots[coords.x, coords.y].collapsed = true;

        return slots[coords.x, coords.y].states[0];
    }
}


