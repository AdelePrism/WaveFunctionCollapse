using UnityEngine;

public class WFCManager : MonoBehaviour
{

    [SerializeField] WaveFunction waveFunction;
    [SerializeField] Map map;
    
    MeshFilter meshFilter;
    ConstraintsActive activeConstraints;


    [SerializeField] CellInfo quickComparisonLeftCell;
    [SerializeField] CellInfo quickComparisonRightCell;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        activeConstraints = new ConstraintsActive();
        activeConstraints.coordinateConstraint = GetComponent<CoordinateConstraint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            meshFilter.sharedMesh = waveFunction.GenerateWFC(map, activeConstraints);
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            Debug.Log(quickComparisonLeftCell.GetPort(1).ComparePort(quickComparisonRightCell.GetPort(3)));
        }
    }
}

public class ConstraintsActive {
    public CoordinateConstraint coordinateConstraint;
}
