using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Port", menuName = "Scriptable Objects/Port")]
public class Port : ScriptableObject
{
    [SerializeField] public bool canMirror;
    [SerializeField] public Port mirroredPort; 
    //    { set { 
    //        if (value != null) 
    //            canMirror = true;
    //        else 
    //            canMirror = false; 
    //    } get {  return mirroredPort; }
    //}

    public List<Port> connections = new List<Port>();
    public HashSet<Port> connectionsHash = new HashSet<Port>();

    public bool ComparePort(Port port) {
        return connectionsHash.Contains(port);
        
        //for (int i = 0; i < connections.Count; i++) {
        //    if (connections[i] == port) return true;
        //}
        //return false;
    }

    public Port Mirror() {
        if (canMirror) {
            return mirroredPort;
        } else {
            return this;
        }
    }

    public virtual void Refresh() {
        if (mirroredPort != null) {
            canMirror = true;
        } else {
            canMirror = false;
        }

        for (int i = 0; i < connections.Count; i++) { //Put everything in the list into the HashSet
            connectionsHash.Add(connections[i]);
        }
    }

    private void OnValidate() {
        Refresh();
    }
}
