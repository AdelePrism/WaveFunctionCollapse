using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

[Serializable]
public class CellInfo {

    public Cell cell;
    public int rot;
    public bool mir;

    public CellInfo(Cell c, int r, bool m) {
        cell = c;
        rot = cell.canRotate ? r : 0;
        mir = cell.canMirror ? m : false;
    }

    public Port GetPort(int direction) {
        return cell.GetPort(direction, rot, mir); ;
    }

    public Mesh GetMesh() {
        
        Mesh mesh = DeepCopy();
        Matrix4x4 mirMatrix = Matrix4x4.Scale(mir ? new Vector3(-1, 1, 1) : Vector3.one);
        Matrix4x4 rotMatrix = Matrix4x4.Rotate(Quaternion.Euler(-90, 180 + rot * 90, 0));
        Matrix4x4 finalMatrix = mirMatrix * rotMatrix; //Ensure its done in the correct order

        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < verts.Length;  i++) {
            verts[i] = finalMatrix.MultiplyPoint3x4(verts[i]); //Transform vertices
        }
        mesh.vertices = verts;

        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++) {
            normals[i] = finalMatrix.MultiplyVector(normals[i]).normalized; //Transform normals
        }
        mesh.normals = normals;

        if (mir) { //If mirrored, swap the winding order of each triangle to correct it
            int[] tris = mesh.triangles;
            for (int i = 0; i <  tris.Length; i += 3) {
                int temp = tris[i];
                tris[i] = tris[i + 1];
                tris[i + 1] = temp;
            }
            mesh.triangles = tris;
        }

        mesh.RecalculateBounds();

        return mesh;
    }

    private Mesh DeepCopy() {
        Mesh mesh = new Mesh();
        Mesh src = cell.GetMesh();

        mesh.vertices = src.vertices;
        mesh.normals = src.normals;
        mesh.tangents = src.tangents;
        mesh.colors = src.colors;
        mesh.colors32 = src.colors32;
        mesh.uv = src.uv;
        mesh.uv2 = src.uv2;
        mesh.uv3 = src.uv3;
        mesh.uv4 = src.uv4;
        mesh.triangles = src.triangles;

        mesh.subMeshCount = src.subMeshCount;
        for (int i = 0; i < src.subMeshCount; i++)
            mesh.SetTriangles(src.GetTriangles(i), i);

        mesh.RecalculateBounds();


        return mesh;
    }

}

[CreateAssetMenu(fileName = "Cell", menuName = "Scriptable Objects/Cell")]
public class Cell : ScriptableObject
{
    [SerializeField] Mesh mesh;
    public bool canMirror;
    public bool canRotate;

    [SerializeField] Port[] ports = new Port[4];

    public Port GetPort(int direction, int rotation = 0, bool mirrored = false) {


        if (mirrored) {
            if (direction == 1 || direction == 3) {
                direction += 2;
                direction = direction % 4;
            }
        }
        int rotatedPortID = (direction - rotation + 4) % 4;

        return mirrored ? ports[rotatedPortID].Mirror() : ports[rotatedPortID];
    }

    public Mesh GetMesh() {
        return mesh;
    }
}
