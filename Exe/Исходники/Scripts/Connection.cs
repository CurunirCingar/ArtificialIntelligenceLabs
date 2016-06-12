using UnityEngine;
using System.Collections;

public class Connection : MonoBehaviour {

    protected LineRenderer connectionLine;
    protected GameObject[] connectedObjects;

    public GameObject[] ConnectedObjects
    {
        get { return connectedObjects; }
        set { connectedObjects = value; }
    }

    protected Material LineMaterial;
    protected Material HighlightedLineMaterial;

    protected bool isHighlighted;
    public float m_distance;

    public bool IsHighlighted
    {
        get { return isHighlighted; }
        set
        {
            isHighlighted = value;
            connectionLine.material = isHighlighted ? HighlightedLineMaterial : LineMaterial;
        }
    }
    public float Distance
    {
        get { return m_distance; }
    }
    public float LineSize 
    { 
        set{ connectionLine.SetWidth(value, value); } 
    }

    // Use this for initialization
    protected void Awake()
    {
        LineMaterial = Resources.Load("Materials/Line") as Material;
        HighlightedLineMaterial = Resources.Load("Materials/HighlightedLine") as Material;

        connectedObjects = new GameObject[2];
        connectionLine = GetComponent<LineRenderer>();
        connectionLine.material = LineMaterial;

        
    }

    public void SetConnection(GameObject From, GameObject To, float currentScale)
    {
        connectedObjects[0] = From;
        connectedObjects[1] = To;
        connectionLine.SetPosition(0, From.transform.position);
        connectionLine.SetPosition(1, To.transform.position);
        connectionLine.SetWidth(currentScale, currentScale);

        m_distance = Distance;

        Vector3 dotBetween = (To.transform.position - From.transform.position) / 2;
        transform.position = From.transform.position + dotBetween;

        Destroy(gameObject.GetComponent<GUIName>());
    }

    public void SetGUIName(string name)
    {
        GetComponent<GUIName>().objectName = name;
    }
}

public class PheromoneConnection : Connection
{
    AntColonyScript colony;

    float m_pheromone;
    float m_pheromoneBuf;
    public float Pheromone { get { return m_pheromone; } }

    public float Distance
    {
        get { return m_distance; }
    }

    // Use this for initialization
    void Start()
    {
        colony = FindObjectOfType<AntColonyScript>();
        m_pheromone = 1f;
    }

    public void SprayPheromone(float passedDistance)
    {
        //Debug.Log(passedDistance + " " + colony.PheromoneIncreaseValue);
        m_pheromoneBuf += colony.PheromoneIncreaseValue / passedDistance;
    }

    public void SetConnection(GameObject From, GameObject To, float currentScale)
    {
        connectedObjects[0] = From;
        connectedObjects[1] = To;
        connectionLine.SetPosition(0, From.transform.position);
        connectionLine.SetPosition(1, To.transform.position);
        connectionLine.SetWidth(currentScale, currentScale);

        m_distance = Mathf.Round(Vector3.Distance(connectedObjects[0].transform.position, connectedObjects[1].transform.position));
        SetGUIName(m_distance.ToString());

        Vector3 dotBetween = (To.transform.position - From.transform.position) / 2;
        transform.position = From.transform.position + dotBetween;
    }

    public void UpdatePheromone()
    {
        if (m_pheromone >= 0.0000000001f)
        {
            m_pheromone = ((m_pheromone - colony.PheromoneDecreaseValue) < 0) ? 0.00001f : (m_pheromone - colony.PheromoneDecreaseValue);
            //Debug.Log(colony.PheromoneDecreaseValue);
        }

        m_pheromone += m_pheromoneBuf;
        //Debug.Log("Buf = " + m_pheromoneBuf);
        m_pheromoneBuf = 0f;

        SetGUIName(string.Format("{0}\n{1}", m_distance, m_pheromone));
    }

    public void ResetPheromoneValue()
    {
        m_pheromone = 1f;
        SetGUIName(string.Format("{0}\n{1}", m_distance, m_pheromone));
    }
}

public class HeuristicConnection : Connection
{
    public void SetConnection(GameObject From, GameObject To, float currentScale)
    {
        connectedObjects[0] = From;
        connectedObjects[1] = To;
        connectionLine.SetPosition(0, From.transform.position);
        connectionLine.SetPosition(1, To.transform.position);
        connectionLine.SetWidth(currentScale, currentScale);

        m_distance = Distance;

        Vector3 dotBetween = (To.transform.position - From.transform.position) / 2;
        transform.position = From.transform.position + dotBetween;

        m_distance = Mathf.Round(Vector3.Distance(connectedObjects[0].transform.position, connectedObjects[1].transform.position));
    }

    void FixedUpdate()
    {
        SetGUIName(string.Format("{0}", m_distance));
    }
}
