using UnityEngine;
using System.Collections.Generic;
using System;
using NUnit.Framework;

[Serializable]
public class WaveFunction
{
    [SerializeField] List<Cell> allCells;
    List<CellInfo> allTrueCells;
    Map map;

    List<Vector2Int> queue;
    HashSet<Vector2Int> queueHash;

    public void NewMap() {
        if (map != null) {
            map = new Map();
        }

        RefreshCells();

        map.NewMap(allTrueCells);

    }

    [ContextMenu("Generate WFC")]
    public Mesh GenerateWFC() {
        NewMap();
        
        while (true) {
            queue = new List<Vector2Int>(); //Queue for what tiles to check

            Vector2Int minEntropyCoords = map.LeastEntropy(); 
            if (minEntropyCoords == Vector2Int.left) { //Vector2Int.left means that all cells are collapsed, and map generation cannot continue
                break;
            }
            CellInfo currentCellInfo = map.CollapseCell(minEntropyCoords);
            queue.Add(minEntropyCoords);
            queueHash.Add(minEntropyCoords);

            while (queue.Count > 0) {
                CheckEdges(queue[0]);
                queueHash.Remove(queue[0]);
                queue.RemoveAt(0);
            }

        }  

        return map.PlaceMap();
    }

    /// <summary>
    /// Checks all valid edges of the current cell to remove impossible states
    /// </summary>
    /// <param name="coords"></param>
    public void CheckEdges(Vector2Int coords) {
        
        if (coords.y != 0) { //Check top if not at the top edge of the map
            CheckEdge(coords, new Vector2Int(0, -1));
        }

        if (coords.x != map.x - 1) { //Check right if not at the right edge of the map
            CheckEdge(coords, new Vector2Int(1, 0));
        }

        if (coords.y != map.y - 1) { //Check bottom if not at the bottom edge of the map
            CheckEdge(coords, new Vector2Int(0, 1));
        }

        if (coords.x != 0) { //Check left if not at the left edge of the map
            CheckEdge(coords, new Vector2Int(-1, 0));
        } 

        //I need to remove any possibility that can no longer occur in *all* possible scenarios, not *any* scenario.
        //Obviously when a cell is collapsed, that is the only possibility
    }

    /// <summary>
    /// Checks all the states of the current and target cell and compares all the ports, and removes all states that are now impossible to happen
    /// </summary>
    /// <param name="coords"></param>
    /// <param name="targetOffset"></param>
    private void CheckEdge(Vector2Int coords, Vector2Int targetOffset) {

        HashSet<CellInfo> impossibleCells = new HashSet<CellInfo>();

        //Translates an offset in Vector2 format into a direction integer where north is 0, east is 1, south is 2, and west is 3.
        int directionComponentX = (targetOffset.x - targetOffset.x * 2) + (Math.Abs(targetOffset.x) * 2); //An offset.x of 0 returns 0, -1 returns 3, 1 returns 1
        int directionComponentY = targetOffset.y + Math.Abs(targetOffset.y); //An offset.y of -1 and 0 returns 0, and 1 returns 2
        int dir = directionComponentX + directionComponentY;

        //Calculates the opposite direction for the target cell to find its Port
        int targetDirComponentX = (-targetOffset.x + targetOffset.x * 2) + (Math.Abs(targetOffset.x) * 2); //An offset.x of 0 returns 0, -1 returns 1, 1 returns 3
        int targetDirComponentY = -targetOffset.y + Math.Abs(targetOffset.y); //An offset.y of 1 and 0 returns 0, and -1 returns 2
        int targetDir = directionComponentX + directionComponentY;

        Vector2Int targetCoords = coords + targetOffset; //Target is the cell adjacent to the current, and is the one being checked

        for (int i = 0; i < map.slots[coords.x, coords.y].states.Count; i++) { //For each state in the currently viewed cell

            CellInfo self = map.slots[coords.x, coords.y].states[i]; //Self is the state of the current cell

            for (int j = 0; j < map.slots[targetCoords.x, targetCoords.y].states.Count; j++) { //For each state in the target cell
                CellInfo target = map.slots[targetCoords.x, targetCoords.y].states[j];
                if (!self.GetPort(dir).ComparePort(target.GetPort(targetDir))) { //If the ports are not compatible
                    if (i == 0) {
                        impossibleCells.Add(target); //Add to the hashSet if its the first cellstate (of the current cell) being checked
                    }

                } else if (impossibleCells.Contains(target)) { //If the hashSet contains the target cell already, while being compatible after all
                    impossibleCells.Remove(target); //Remove from the hashSet, as the cell is now possible again
                }

            }
            //After all cell states have been checked in the current cell, the impossibleCells hashSet should contain ONLY states in the target cell
            //that are entirely impossible to occur due to Port Compatibility, and therefore should be removed from the target's available states
        }

        if (impossibleCells.Count > 0) {
            bool hasRemovedState = false;
            for (int i = map.slots[targetCoords.x, targetCoords.y].states.Count; i >= 0; i--) {
                if (impossibleCells.Contains(map.slots[targetCoords.x, targetCoords.y].states[i])) { 
                    map.slots[targetCoords.x, targetCoords.y].states.RemoveAt(i); //Remove all states in the target cell that is in the hashSet
                    hasRemovedState = true;
                }
            }
            if (hasRemovedState && !queueHash.Contains(targetCoords)) { //If any state of the target cell was removed, it must be added to queue to check if it propogates
                queue.Add(targetCoords);
                queueHash.Add(targetCoords);
            }
        }
    }


    public void RefreshCells() {

        for (int i = 0; i < allCells.Count; i++) {

            if (allCells[i].canRotate) { //Cell can rotate

                if (allCells[i].canMirror) { //Cell can be mirrored
                    for (int j = 0; j < 4; j++) {
                        CellInfo info = new CellInfo(allCells[i], j, false); //Get all unmirrored rotations
                        allTrueCells.Add(info);

                        info = new CellInfo(allCells[i], j, true); //Get all mirrored rotations
                        allTrueCells.Add(info);
                    }
                } else { //Cell cant be mirrored
                    for (int j = 0; j < 4; j++) {
                        CellInfo info = new CellInfo(allCells[i], j, false); //Get all rotations
                        allTrueCells.Add(info);
                    }
                }
            } else { //Cell cant rotate
                CellInfo info = new CellInfo(allCells[i], 0, false); //Theres only 1 rotation
                allTrueCells.Add(info);

                if (allCells[i].canMirror) {
                    info = new CellInfo(allCells[i], 0, true); //Get a mirrored version
                    allTrueCells.Add(info);
                }
            }

        }
    }



}
