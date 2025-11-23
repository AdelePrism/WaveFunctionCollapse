using UnityEngine;

public class WFCManager : MonoBehaviour
{

    [SerializeField] WaveFunction waveFunction;
    [SerializeField] Map map;
    
    MeshFilter meshFilter;


    [SerializeField] CellInfo leftCell;
    [SerializeField] CellInfo rightCell;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            meshFilter.sharedMesh = waveFunction.GenerateWFC(map);
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            Debug.Log(leftCell.GetPort(1).ComparePort(rightCell.GetPort(3)));
        }
    }
}
