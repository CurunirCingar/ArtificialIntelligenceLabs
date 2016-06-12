using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public int id;
    public bool isVisited;
    public int value;

    protected NodesTypesT type;
    protected ParticleSystem[] particles;
    protected Material normalMaterial;
    protected Material setupMaterial;
    protected Material prevMaterial;
    private bool isSetup;
    private Dictionary<Node, Connection> neighbourNodes;

    public Dictionary<Node, Connection> NeighbourNodes
    {
        get { return neighbourNodes; }
    }

    public bool IsSetup
    {
        get { return isSetup; }
        set 
        {
            isSetup = value; 
            if (isSetup)
                prevMaterial = gameObject.GetComponent<MeshRenderer>().material;
            gameObject.GetComponent<MeshRenderer>().material = isSetup ? setupMaterial : prevMaterial;
        }
    }

    public NodesTypesT Type
    {
        get { return type; }
        set
        {
            switch (value)
            {
                case NodesTypesT.START:
                    particles[0].enableEmission = true;
                    particles[1].enableEmission = false;
                    break;
                case NodesTypesT.END:
                    particles[0].enableEmission = false;
                    particles[1].enableEmission = true;
                    break;
                case NodesTypesT.NORMAL:
                    particles[0].enableEmission = false;
                    particles[1].enableEmission = false;
                    break;
                default:
                    break;
            }
            type = value;
        }
    }

    public float GameObjectSize {
        get { return gameObject.transform.localScale.x; }
        set
        {
            gameObject.transform.localScale = new Vector3(value, value, value);

            foreach (var line in NeighbourNodes.Values)
                line.LineSize = value * 0.1f;
        }
    }

	// Use this for initialization
	void Awake () {
        neighbourNodes = new Dictionary<Node, Connection>();
        particles = GetComponentsInChildren<ParticleSystem>();
        Type = NodesTypesT.NORMAL;

        normalMaterial = Resources.Load("Materials/BlackNode") as Material;
        setupMaterial = Resources.Load("Materials/HighlightMaterial") as Material;
    }

    public void SetGUIName(string name)
    {
        GetComponent<GUIName>().objectName = name;
    }

    public void HighlightConnection(Node to)
    {
        NeighbourNodes[to].IsHighlighted = true;
    }

    public void RemoveNeighbourConnection(Node Neighbour, ref List<Connection> connectionsList)
    {
        GameObject toDelete = neighbourNodes[Neighbour].gameObject;
        connectionsList.Remove(neighbourNodes[Neighbour]);
        neighbourNodes.Remove(Neighbour);
        Destroy(toDelete);
    }

    public static bool BindNodes(Node From, Node To, out Connection connection, float connectionScale)
    {
        //If they already binded
        if (From.NeighbourNodes.ContainsKey(To))
        {
            connection = null;
            return false;
        }

        //Create new connection prefab
        GameObject connectionPrefab = (GameObject)Resources.Load("ConnectionPrefab");
        GameObject NewConnection = (GameObject)Instantiate(connectionPrefab, From.gameObject.transform.position, Quaternion.identity);
        NewConnection.transform.parent = From.gameObject.transform.parent;

        //Setup new connection script
        connection = NewConnection.AddComponent<Connection>();
        connection.SetConnection(From.gameObject, To.gameObject, connectionScale*0.1f);

        From.NeighbourNodes.Add(To, connection);
        To.NeighbourNodes.Add(From, connection);

        return true;
    }

    public int GetChildMin(Node from)
    {
        int result, buf;

        result = 1000;
        foreach (var node in neighbourNodes.Keys)
            if (node != from)
            {
                node.HighlightConnection(this);
                buf = node.GetChildMax(this);
                if (buf < result)
                    result = buf;
            }

        if (result == 1000)
            result = value;

        SetGUIName(result.ToString());
        return result;     
    }

    public int GetChildMax(Node from)
    {
        int result, buf;

        result = -1;
        foreach (var node in neighbourNodes.Keys)
            if (node != from)
            {
                node.HighlightConnection(this);
                buf = node.GetChildMin(this);
                if (buf > result)
                    result = buf;
            }

        if (result == -1)
            result = value;

        SetGUIName(result.ToString());
        return result;
    }

    public int AlphaPruning(Node from, int alpha, int beta)
    {
        int result = -1, buf;

        //result = beta;
        foreach (var node in neighbourNodes.Keys)
            if (node != from)
            {
                buf = node.BetaPruning(this, alpha, beta);
                node.HighlightConnection(this);
                Debug.Log(beta + " a=" + alpha + " b=" + beta);
                if (result < buf)
                    result = buf;
                if (buf >= alpha)
                {
                    alpha = buf;
                    if (alpha >= beta)
                        break; 
                }
            }

        if (neighbourNodes.Count == 1)
            result = value;

        SetGUIName(result.ToString());
        return result;     
    }

    public int BetaPruning(Node from, int alpha, int beta)
    {
        int result = 1000, buf;

        foreach (var node in neighbourNodes.Keys)
            if (node != from)
            {
                buf = node.AlphaPruning(this, alpha, beta);
                node.HighlightConnection(this);
                Debug.Log(buf + " a=" + alpha + " b=" + beta);
                if (result > buf)
                    result = buf;
                if (buf <= beta)
                {
                    beta = buf;
                    if (beta <= alpha)
                        break;
                }
            }

        if (neighbourNodes.Count == 1)
            result = value;
            
        SetGUIName(result.ToString());
        return result;
    }

    public void SetColor(Node from, Material Min, Material Max, bool isMax)
    {
        gameObject.GetComponent<MeshRenderer>().material = isMax ? Max : Min;

        foreach (var node in neighbourNodes.Keys)
            if (node != from)
                node.SetColor(this, Min, Max, !isMax);
    }
}

public class PheromoneNode : Node
{
    private Dictionary<PheromoneNode, PheromoneConnection> neighbourNodes;

    public Dictionary<PheromoneNode, PheromoneConnection> NeighbourNodes
    {
        get { return neighbourNodes; }
    }

    public float GameObjectSize
    {
        get { return gameObject.transform.localScale.x; }
        set
        {
            gameObject.transform.localScale = new Vector3(value, value, value);

            foreach (var line in NeighbourNodes.Values)
                line.LineSize = value * 0.1f;
        }
    }

    void Awake()
    {
        neighbourNodes = new Dictionary<PheromoneNode, PheromoneConnection>();
        particles = GetComponentsInChildren<ParticleSystem>();
        Type = NodesTypesT.NORMAL;

        normalMaterial = Resources.Load("Materials/BlackNode") as Material;
        setupMaterial = Resources.Load("Materials/HighlightMaterial") as Material;
    }
    public void HighlightConnection(int to)
    {
        foreach (var node in NeighbourNodes.Keys)
        {
            if (node.id == to)
            {
                NeighbourNodes[node].IsHighlighted = true;
                Debug.Log(to);
            }
        }
    }

    public void RemoveNeighbourConnection(PheromoneNode Neighbour, ref List<PheromoneConnection> connectionsList)
    {
        GameObject toDelete = neighbourNodes[Neighbour].gameObject;
        connectionsList.Remove(neighbourNodes[Neighbour]);
        neighbourNodes.Remove(Neighbour);
        Destroy(toDelete);
    }

    public static bool BindNodes(PheromoneNode From, PheromoneNode To, out PheromoneConnection connection, float connectionScale)
    {
        //If they already binded
        if (From.NeighbourNodes.ContainsKey(To))
        {
            connection = null;
            return false;
        }

        //Create new connection prefab
        GameObject connectionPrefab = (GameObject)Resources.Load("ConnectionPrefab");
        GameObject NewConnection = (GameObject)Instantiate(connectionPrefab, From.gameObject.transform.position, Quaternion.identity);
        NewConnection.transform.parent = From.gameObject.transform.parent;

        //Setup new connection script
        connection = NewConnection.AddComponent<PheromoneConnection>();
        connection.SetConnection(From.gameObject, To.gameObject, connectionScale * 0.1f);

        From.NeighbourNodes.Add(To, connection);
        To.NeighbourNodes.Add(From, connection);

        return true;
    }

    public int HighlightBestPath(Node CameFrom)
    {
        //Debug.Log("My node: " + id);
        isVisited = true;
        PheromoneNode BestPath = null;
        foreach (var node in neighbourNodes.Keys)
        {
            if (node == CameFrom || node.isVisited)
                continue;

            if (BestPath == null)
                BestPath = node;
            else
            {
                if ((neighbourNodes[BestPath] as PheromoneConnection).Pheromone < (neighbourNodes[node] as PheromoneConnection).Pheromone)
                    BestPath = node;
            }
        }

        if (BestPath == null)
            return 0;


        neighbourNodes[BestPath].IsHighlighted = true;
        if (BestPath.type != NodesTypesT.END)
            return (int)neighbourNodes[BestPath].Distance + BestPath.HighlightBestPath(this);
        else
            return (int)neighbourNodes[BestPath].Distance;
    }
}

public class HeuristicNode : Node
{
    private Dictionary<HeuristicNode, HeuristicConnection> neighbourNodes;
    protected Material BlackNode;
    protected Material GrayNode;
    protected Material WhiteNode;

    public int heuristic;
    public int Heuristic
    {
        get { return heuristic; }
        set { heuristic = value; }
    }

    public enum HeuristicNodeT
    {
        OPEN,
        CLOSED,
        UNSEEN,
    }

    HeuristicNodeT heuristicNodeType;

    public HeuristicNodeT HeuristicNodeType
    {
        get { return heuristicNodeType; }
        set
        {
            switch (value)
            {
                case HeuristicNodeT.OPEN:
                    gameObject.GetComponent<MeshRenderer>().material = WhiteNode;
                    break;
                case HeuristicNodeT.CLOSED:
                    gameObject.GetComponent<MeshRenderer>().material = BlackNode;
                    break;
                case HeuristicNodeT.UNSEEN:
                    gameObject.GetComponent<MeshRenderer>().material = GrayNode;
                    break;
                default:
                    break;
            }
            heuristicNodeType = value;
        }
    }

    public Dictionary<HeuristicNode, HeuristicConnection> NeighbourNodes
    {
        get { return neighbourNodes; }
    }

    public float GameObjectSize
    {
        get { return gameObject.transform.localScale.x; }
        set
        {
            gameObject.transform.localScale = new Vector3(value, value, value);

            foreach (var line in NeighbourNodes.Values)
                line.LineSize = value * 0.1f;
        }
    }

    void Awake()
    {
        neighbourNodes = new Dictionary<HeuristicNode, HeuristicConnection>();
        particles = GetComponentsInChildren<ParticleSystem>();
        Type = NodesTypesT.NORMAL;

        normalMaterial = Resources.Load("Materials/BlackNode") as Material;
        setupMaterial = Resources.Load("Materials/HighlightMaterial") as Material;
        BlackNode = Resources.Load("Materials/BlackNode") as Material;
        GrayNode = Resources.Load("Materials/GrayNode") as Material;
        WhiteNode = Resources.Load("Materials/WhiteNode") as Material;
    }

    public void HighlightConnection(HeuristicNode to)
    {
        if (NeighbourNodes.ContainsKey(to))
            NeighbourNodes[to].IsHighlighted = true;
    }

    public void RemoveNeighbourConnection(HeuristicNode Neighbour, ref List<HeuristicConnection> connectionsList)
    {
        GameObject toDelete = neighbourNodes[Neighbour].gameObject;
        connectionsList.Remove(neighbourNodes[Neighbour]);
        neighbourNodes.Remove(Neighbour);
        Destroy(toDelete);
    }

    void FixedUpdate()
    {
        SetGUIName(id + "\n" + heuristic);
    }

    public static bool BindNodes(HeuristicNode From, HeuristicNode To, out HeuristicConnection connection, float connectionScale)
    {
        //If they already binded

        //Debug.Log(From + " " + From.neighbourNodes);

        if (From.neighbourNodes.ContainsKey(To))
        {
            connection = null;
            return false;
        }

        //Create new connection prefab
        GameObject connectionPrefab = (GameObject)Resources.Load("ConnectionPrefab");
        GameObject NewConnection = (GameObject)Instantiate(connectionPrefab, From.gameObject.transform.position, Quaternion.identity);
        NewConnection.transform.parent = From.gameObject.transform.parent;

        //Setup new connection script
        connection = NewConnection.AddComponent<HeuristicConnection>();
        connection.SetConnection(From.gameObject, To.gameObject, connectionScale * 0.1f);

        From.NeighbourNodes.Add(To, connection);
        To.NeighbourNodes.Add(From, connection);

        return true;
    }
}
