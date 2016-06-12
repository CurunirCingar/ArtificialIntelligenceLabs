using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public enum NodesTypesT
{
    NORMAL,
    START,
    END,
}

public interface INodeFactory
{   
    void CreateNode(Vector3 nodePosition, Vector3 nodeSize);
    void CreateConnection(GameObject FromNode, GameObject ToNode, float nodesScale);
    void SetTargetNode(GameObject NodeObject);
    void SetupNode(GameObject NodeObject);
    void DeleteNode();
    void GetGraph(out int[,] graph);
    void SetSize(float size);
    void StartAlgorithm();
    void SaveGraph();
    void LoadGraph(Vector3 nodesScale);
}

public class MinimaxNodeFactory : MonoBehaviour, INodeFactory
{
    public const int INF = int.MaxValue;
    MinimaxScript Minimax;

    protected GameObject nodePrefab;
    protected GameObject NodesTransformContainer;

    private List<Node> nodesList;
    private List<Connection> connectionsList;
    private Node startNode;
    private Node setupNode;

    Button MinButton, MaxButton;
    Toggle UsePruningToggle;
    Text ResultText;
    Material MinMaterial, MaxMaterial;

    public MinimaxNodeFactory()
    {
        nodePrefab = (GameObject)Resources.Load("NodePrefab");

        nodesList = new List<Node>();
        connectionsList = new List<Connection>();
    }

    public void Awake()
    {
        NodesTransformContainer = new GameObject();
        NodesTransformContainer.name = "NodesContainer";
        Minimax = gameObject.AddComponent<MinimaxScript>();

        ResultText = GameObject.Find("ResultText").GetComponent<Text>();

        MinButton = GameObject.Find("MinButton").GetComponent<Button>();
        MinButton.onClick.AddListener(SetMin);
        MaxButton = GameObject.Find("MaxButton").GetComponent<Button>();
        MaxButton.onClick.AddListener(SetMax);

        UsePruningToggle = GameObject.Find("UsePruningToggle").GetComponent<Toggle>();
        UsePruningToggle.onValueChanged.AddListener(SetPruning);

        MinMaterial = Resources.Load("Materials/Blue") as Material;
        MaxMaterial = Resources.Load("Materials/HighlightedLine") as Material;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            setupNode.value = 0;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            setupNode.value = 1;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            setupNode.value = 2;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            setupNode.value = 3;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            setupNode.value = 4;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            setupNode.value = 5;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            setupNode.value = 6;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            setupNode.value = 7;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            setupNode.value = 8;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            setupNode.value = 9;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            setupNode.value++;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
        else
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            setupNode.value--;
            setupNode.SetGUIName(setupNode.value.ToString());
        }
    }

    public void CreateNode(Vector3 nodePosition, Vector3 nodeSize)
    {
        nodePosition.y += 1.5f;
        GameObject NewNode = (GameObject)Instantiate(nodePrefab, nodePosition, Quaternion.identity);
        NewNode.transform.parent = NodesTransformContainer.transform;
        NewNode.transform.localScale = nodeSize;

        Node NewNodeScript = NewNode.AddComponent<Node>();
        NewNodeScript.id = nodesList.Count;
        NewNode.name = NewNodeScript.id.ToString();
        NewNodeScript.value = Random.Range(0,10);
        NewNodeScript.SetGUIName(NewNode.name);

        nodesList.Add(NewNodeScript);

        if (nodesList.Count == 1)
            SetTargetNode(nodesList[0].gameObject);

        startNode.SetColor(null, MinMaterial, MaxMaterial, Minimax.isMax);
    }

    public void CreateNode(Vector3 nodePosition, Vector3 nodeSize, int value)
    {
        nodePosition.y += 1.5f;
        GameObject NewNode = (GameObject)Instantiate(nodePrefab, nodePosition, Quaternion.identity);
        NewNode.transform.parent = NodesTransformContainer.transform;
        NewNode.transform.localScale = nodeSize;

        Node NewNodeScript = NewNode.AddComponent<Node>();
        NewNodeScript.id = nodesList.Count;
        NewNode.name = NewNodeScript.id.ToString();
        NewNodeScript.value = value;
        NewNodeScript.SetGUIName(NewNode.name);

        nodesList.Add(NewNodeScript);

        if (nodesList.Count == 1)
            SetTargetNode(nodesList[0].gameObject);

        startNode.SetColor(null, MinMaterial, MaxMaterial, Minimax.isMax);
    }

    public void CreateConnection(GameObject FromNodeGameObject, GameObject ToNodeGameObject, float nodesScale)
    {
        Connection connection;
        Node FromNode, ToNode;
        FromNode = FromNodeGameObject.GetComponent<Node>();
        ToNode = ToNodeGameObject.GetComponent<Node>();

        if (Node.BindNodes(FromNode, ToNode, out connection, nodesScale))
        {
            connectionsList.Add(connection);

            if (FromNode.NeighbourNodes.Count == 1)
                FromNode.SetGUIName(FromNode.value.ToString());
            else
                FromNode.SetGUIName("");

            if (ToNode.NeighbourNodes.Count == 1)
                ToNode.SetGUIName(ToNode.value.ToString());
            else
                ToNode.SetGUIName("");

            startNode.SetColor(null, MinMaterial, MaxMaterial, Minimax.isMax);
        }
    }

    public void SetTargetNode(GameObject NodeObject)
    {
        Node Node = NodeObject.GetComponent<Node>();
        if (startNode != null)
        {
            if (Node.Type == NodesTypesT.START)
            {
                startNode.Type = NodesTypesT.NORMAL;
                startNode.SetGUIName(startNode.id.ToString());
                startNode = null;
                return;
            }
        }
        else
        {
            startNode = Node;
            startNode.Type = NodesTypesT.START;
            startNode.SetGUIName("Start " + startNode.id);
            return;
        }
    }

    public void SetupNode(GameObject NodeObject)
    {
        Node nodeBuf = NodeObject.GetComponent<Node>();
        if (setupNode != nodeBuf)
        {
            if (setupNode != null)
                setupNode.IsSetup = false;

            setupNode = nodeBuf;
            setupNode.IsSetup = true;
        }
        else
        {
            setupNode.IsSetup = false;
            setupNode = null;
        }
    }

    public void DeleteNode()
    {
        if (setupNode != null)
        {
            Connection connection;

            foreach (var neighbour in setupNode.NeighbourNodes.Keys)
            {
                neighbour.RemoveNeighbourConnection(setupNode, ref connectionsList);
            }

            nodesList.Remove(setupNode);
            Destroy(setupNode.gameObject);
            RefreshNodesId();
        }
    }

    public void RefreshNodesId()
    {
        int i = 0;
        foreach (var node in nodesList)
        {
            node.id = i++;
            node.SetGUIName(node.id.ToString());
        }
    }

    public void GetGraph(out int[,] graph)
    {
        int N = nodesList.Count;
        graph = new int[N, N];
        Debug.Log(graph + "  s = " + N);

        // Заполняем граф бесконечностями.
        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                graph[i, j] = INF;

        // Заполняем граф значениями рёбер.
        for (int i = 0; i < N; i++)
            foreach (var j in nodesList[i].NeighbourNodes.Keys)
                graph[i, j.id] = (int)nodesList[i].NeighbourNodes[j].Distance;

        // Выставляем 0 по диагонали.
        for (int i = 0; i < N; i++)
            graph[i, i] = 0;
    }

    public void SetSize(float size)
    {
        for (int i = 0; i < nodesList.Count; i++)
            nodesList[i].GameObjectSize = size;
    }

    public void StartAlgorithm()
    {
        foreach (var connection in connectionsList)
            connection.IsHighlighted = false;
        
        if(Minimax.isPruning)
            ResultText.text = Minimax.AlphaBetaPruning(startNode).ToString();
        else
            ResultText.text = Minimax.Minimax(startNode).ToString();
    }

    public void SaveGraph()
    {
        string path = SceneManager.GetActiveScene().name + ".dat";

        BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));

        writer.Write(nodesList.Count);
        foreach (var node in nodesList)
        {
            writer.Write(node.value);
            writer.Write(node.gameObject.transform.position.x);
            writer.Write(node.gameObject.transform.position.y);
            writer.Write(node.gameObject.transform.position.z);
        }

        writer.Write(connectionsList.Count);
        foreach (var connection in connectionsList)
        {
            writer.Write(connection.ConnectedObjects[0].GetComponent<Node>().id);
            writer.Write(connection.ConnectedObjects[1].GetComponent<Node>().id);
        }

        writer.Close();
    }

    public void LoadGraph(Vector3 nodesScale)
    {
        string path = SceneManager.GetActiveScene().name + ".dat";

        foreach (var node in nodesList)
            Destroy(node.gameObject);

        foreach (var connection in connectionsList)
            Destroy(connection.gameObject);

        nodesList.Clear();
        connectionsList.Clear();

        BinaryReader reader = new BinaryReader(File.Open(path, FileMode.OpenOrCreate));

        Vector3 nodePos;
        int value;
        int N = reader.ReadInt32();
        for (int i = 0; i < N; i++)
        {
            value = reader.ReadInt32();
            nodePos.x = reader.ReadSingle();
            nodePos.y = reader.ReadSingle();
            nodePos.z = reader.ReadSingle();

            CreateNode(nodePos, nodesScale, value);
        }

        GameObject from, to;
        int fromId, toId;
        N = reader.ReadInt32();
        for (int i = 0; i < N; i++)
        {
            fromId = reader.ReadInt32();
            toId = reader.ReadInt32();

            from = nodesList.Find(x => x.id == fromId).gameObject;
            to = nodesList.Find(x => x.id == toId).gameObject;

            CreateConnection(from, to, nodesScale.x);
        }

        reader.Close();
    }

    private void SetMin()
    {
        Minimax.isMax = true;
        startNode.SetColor(null, MinMaterial, MaxMaterial, Minimax.isMax);
    }

    private void SetMax()
    {
        Minimax.isMax = false;
        startNode.SetColor(null, MinMaterial, MaxMaterial, Minimax.isMax);
    }

    private void SetPruning(bool value)
    {
        Minimax.isPruning = value;
    }
}

public class PheromoneNodeFactory : MonoBehaviour, INodeFactory
{
    public const int INF = int.MaxValue;
    AntColonyScript AntColony;

    protected GameObject nodePrefab;
    protected GameObject NodesTransformContainer;

    private List<PheromoneNode> nodesList;
    private List<PheromoneConnection> connectionsList;
    private PheromoneNode startNode;
    private PheromoneNode endNode;
    private PheromoneNode setupNode;

    public PheromoneNodeFactory()
    {
        nodePrefab = (GameObject)Resources.Load("NodePrefab");

        nodesList = new List<PheromoneNode>();
        connectionsList = new List<PheromoneConnection>();
    }

    public void Awake()
    {
        AntColony = gameObject.AddComponent<AntColonyScript>();
        NodesTransformContainer = new GameObject();
        NodesTransformContainer.name = "NodesContainer";
    }

    public void CreateNode(Vector3 nodePosition, Vector3 nodeSize)
    {
        nodePosition.y += 1.5f;
        GameObject NewNode = (GameObject)Instantiate(nodePrefab, nodePosition, Quaternion.identity);
        NewNode.transform.parent = NodesTransformContainer.transform;
        NewNode.transform.localScale = nodeSize;

        PheromoneNode NewNodeScript = NewNode.AddComponent<PheromoneNode>();
        NewNodeScript.id = nodesList.Count;
        NewNode.name = NewNodeScript.id.ToString();
        NewNodeScript.SetGUIName(NewNode.name);

        nodesList.Add(NewNodeScript);
    }

    public void CreateConnection(GameObject FromNode, GameObject ToNode, float nodesScale)
    {
        PheromoneConnection connection;
        if (PheromoneNode.BindNodes(FromNode.GetComponent<PheromoneNode>(), ToNode.GetComponent<PheromoneNode>(), out connection, nodesScale))
            connectionsList.Add(connection);
    }

    public void SetTargetNode(GameObject NodeObject)
    {
        PheromoneNode Node = NodeObject.GetComponent<PheromoneNode>();

        if (startNode != null)
        {
            if (Node.Type == NodesTypesT.START)
            {
                startNode.Type = NodesTypesT.NORMAL;
                startNode.SetGUIName(startNode.id.ToString());
                startNode = null;
                return;
            }
        }
        else
        {
            startNode = Node;
            startNode.Type = NodesTypesT.START;
            startNode.SetGUIName("Start " + startNode.id);
            return;
        }

        if (endNode != null)
        {
            if (Node.Type == NodesTypesT.END)
            {
                endNode.Type = NodesTypesT.NORMAL;
                endNode.SetGUIName(endNode.id.ToString());
                endNode = null;
                return;
            }
        }
        else
        {
            endNode = Node;
            endNode.Type = NodesTypesT.END;
            endNode.SetGUIName("End " + endNode.id);
            return;
        }
    }

    public void SetupNode(GameObject NodeObject)
    {
        PheromoneNode nodeBuf = NodeObject.GetComponent<PheromoneNode>();
        if (setupNode != nodeBuf)
        {
            if (setupNode != null)
                setupNode.IsSetup = false;

            setupNode = nodeBuf;
            setupNode.IsSetup = true;
        }
        else
        {
            setupNode.IsSetup = false;
            setupNode = null;
        }
    }

    public void DeleteNode()
    {
        if (setupNode != null)
        {
            PheromoneConnection connection;

            foreach (var neighbour in setupNode.NeighbourNodes.Keys)
            {
                neighbour.RemoveNeighbourConnection(setupNode, ref connectionsList);
            }

            nodesList.Remove(setupNode);
            Destroy(setupNode.gameObject);
            RefreshNodesId();
        }
    }

    public void RefreshNodesId()
    {
        int i = 0;
        foreach (var node in nodesList)
        {
            node.id = i++;
            node.SetGUIName(node.id.ToString());
        }
    }

    public void GetGraph(out int[,] graph)
    {
        int N = nodesList.Count;
        graph = new int[N, N];
        Debug.Log(graph + "  s = " + N);

        // Заполняем граф бесконечностями.
        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                graph[i, j] = INF;

        // Заполняем граф значениями рёбер.
        for (int i = 0; i < N; i++)
            foreach (var j in nodesList[i].NeighbourNodes.Keys)
                graph[i, j.id] = (int)nodesList[i].NeighbourNodes[j].Distance;

        // Выставляем 0 по диагонали.
        for (int i = 0; i < N; i++)
            graph[i, i] = 0;
    }

    public void SetSize(float size)
    {
        foreach(var node in nodesList)
            node.GameObjectSize = size;
    }

    public void StartAlgorithm()
    {
        foreach (var connection in connectionsList)
            connection.IsHighlighted = false;
        AntColony.SetColonyParams(nodesList, connectionsList, startNode, endNode);
    }

    public void SaveGraph()
    {
        string path = SceneManager.GetActiveScene().name + ".dat";

        BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));

        writer.Write(nodesList.Count);
        foreach (var node in nodesList)
        {
            writer.Write(node.gameObject.transform.position.x);
            writer.Write(0f);
            writer.Write(node.gameObject.transform.position.z);
        }

        writer.Write(connectionsList.Count);
        foreach (var connection in connectionsList)
        {
            writer.Write(connection.ConnectedObjects[0].GetComponent<PheromoneNode>().id);
            writer.Write(connection.ConnectedObjects[1].GetComponent<PheromoneNode>().id);
        }

        writer.Close();
    }

    public void LoadGraph(Vector3 nodesScale)
    {
        string path = SceneManager.GetActiveScene().name + ".dat";

        foreach (var node in nodesList)
            Destroy(node.gameObject);

        foreach (var connection in connectionsList)
            Destroy(connection.gameObject);

        nodesList.Clear();
        connectionsList.Clear();

        BinaryReader reader = new BinaryReader(File.Open(path, FileMode.OpenOrCreate));

        Vector3 nodePos;
        int N = reader.ReadInt32();
        for (int i = 0; i < N; i++)
        {
            nodePos.x = reader.ReadSingle();
            nodePos.y = reader.ReadSingle();
            nodePos.z = reader.ReadSingle();

            CreateNode(nodePos, nodesScale);
        }

        GameObject from, to;
        int fromId, toId;
        N = reader.ReadInt32();
        for (int i = 0; i < N; i++)
        {
            fromId = reader.ReadInt32();
            toId = reader.ReadInt32();

            from = nodesList.Find(x => x.id == fromId).gameObject;
            to = nodesList.Find(x => x.id == toId).gameObject;

            CreateConnection(from, to, nodesScale.x);
        }

        reader.Close();
    }
}

public class HeuristicNodeFactory : MonoBehaviour, INodeFactory
{
    public const int INF = 10000;
    private AStarScript AStar;
    
    protected GameObject nodePrefab;
    protected GameObject NodesTransformContainer;
    
    private List<HeuristicNode> nodesList;
    private List<HeuristicConnection> connectionsList;
    private HeuristicNode startNode;
    private HeuristicNode endNode;
    private HeuristicNode setupNode;
    private HeuristicNode setupNode2;

    public HeuristicNodeFactory()
    {
        nodePrefab = (GameObject)Resources.Load("NodePrefab");
        nodesList = new List<HeuristicNode>();
        connectionsList = new List<HeuristicConnection>();
    }

    public void Awake()
    {
        AStar = gameObject.AddComponent<AStarScript>();
        NodesTransformContainer = new GameObject();
        NodesTransformContainer.name = "NodesContainer";
    }

    public void CreateNode(Vector3 nodePosition, Vector3 nodeSize)
    {
        nodePosition.y += 1.5f;
        GameObject NewNode = (GameObject)Instantiate(nodePrefab, nodePosition, Quaternion.identity);
        NewNode.transform.parent = NodesTransformContainer.transform;
        NewNode.transform.localScale = nodeSize;
        HeuristicNode NewNodeScript = NewNode.AddComponent<HeuristicNode>();
        NewNodeScript.id = nodesList.Count;
        NewNode.name = NewNodeScript.id.ToString();
        NewNodeScript.SetGUIName(NewNode.name);

        nodesList.Add(NewNodeScript);
    }

    public void CreateConnection(GameObject FromNode, GameObject ToNode, float nodesScale)
    {
        HeuristicConnection connection;
        //Debug.Log(FromNode + " " + FromNode.GetComponent<HeuristicNode>());
        if (HeuristicNode.BindNodes(FromNode.GetComponent<HeuristicNode>(), ToNode.GetComponent<HeuristicNode>(), out connection, nodesScale))
            connectionsList.Add(connection);
    }

    public void SetTargetNode(GameObject NodeObject)
    {
        HeuristicNode Node = NodeObject.GetComponent<HeuristicNode>();
        if (startNode != null)
        {
            if (Node.Type == NodesTypesT.START)
            {
                startNode.Type = NodesTypesT.NORMAL;
                startNode.SetGUIName(startNode.id.ToString());
                startNode = null;
                return;
            }
        }
        else
        {
            startNode = Node;
            startNode.Type = NodesTypesT.START;
            startNode.SetGUIName("Start " + startNode.id);
            return;
        }

        if (endNode != null)
        {
            if (Node.Type == NodesTypesT.END)
            {
                endNode.Type = NodesTypesT.NORMAL;
                endNode.SetGUIName(endNode.id.ToString());
                endNode = null;
                return;
            }
        }
        else
        {
            endNode = Node;
            endNode.Type = NodesTypesT.END;
            endNode.SetGUIName("End " + endNode.id);
            return;
        }
    }

    public void SetupNode(GameObject NodeObject)
    {
        if (NodeObject == null)
            return;

        HeuristicNode nodeBuf = NodeObject.GetComponent<HeuristicNode>();

        if (setupNode == null)
        {
            setupNode = nodeBuf;
            setupNode.IsSetup = true;
            setupNode.SetGUIName(setupNode.Heuristic.ToString());
        }
        else
            if (setupNode2 == null)
            {
                setupNode2 = nodeBuf;
                setupNode2.IsSetup = true;
                setupNode2.SetGUIName(setupNode.Heuristic.ToString());
            }
    }

    void Update()
    {
        if (setupNode != null && setupNode2 != null)
        {
            if (Input.GetKey(KeyCode.KeypadPlus))
                setupNode.NeighbourNodes[setupNode2].m_distance++;
            if (Input.GetKey(KeyCode.KeypadMinus))
                setupNode.NeighbourNodes[setupNode2].m_distance--;
        }
    }

    public void DeleteNode()
    {
        if (setupNode != null)
        {
            PheromoneConnection connection;

            foreach(var neighbour in setupNode.NeighbourNodes.Keys)
            {
                neighbour.RemoveNeighbourConnection(setupNode, ref connectionsList);
            }

            nodesList.Remove(setupNode);
            Destroy(setupNode.gameObject);
            RefreshNodesId();
        }
    }

    public void RefreshNodesId()
    {
        int i = 0;
        foreach (var node in nodesList)
        {
            node.id = i++;
            node.SetGUIName(node.id.ToString());
        }
    }

    public void GetGraph(out int[,] graph)
    {
        int N = nodesList.Count;
        graph = new int[N, N];
        Debug.Log(graph + "  s = " + N);

        // Заполняем граф бесконечностями.
        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                graph[i, j] = INF;

        // Заполняем граф значениями рёбер.
        for (int i = 0; i < N; i++)
            foreach (var j in nodesList[i].NeighbourNodes.Keys)
                graph[i, j.id] = (int)nodesList[i].NeighbourNodes[j].Distance;

        for (int i = 0; i < N; i++)
            graph[i, i] = 0;
    }

    public void SetSize(float size)
    {
        for (int i = 0; i < nodesList.Count; i++)
            nodesList[i].GameObjectSize = size;
    }

    public void StartAlgorithm()
    {
        int[,] graph = new int[nodesList.Count, nodesList.Count];
        int[,] heuristicGraph;
        GetGraph(out graph);
        GetFloydMatrix(graph, out heuristicGraph);

        foreach (var connection in connectionsList)
            connection.IsHighlighted = false;

        foreach (var node in nodesList)
            node.Heuristic = heuristicGraph[endNode.id, node.id];

        foreach (var node in nodesList)
            Debug.Log(node.id + " " + node.Heuristic);

        AStar.RunAStarAlgorithm(nodesList, startNode, endNode);
    }

    private void GetFloydMatrix(int[,] graph, out int[,] heuristicGraph)
    {
        int N = nodesList.Count;
        heuristicGraph = new int[N, N];

        for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
                heuristicGraph[i, j] = graph[i, j];

        string line = "";
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (heuristicGraph[i, j] == INF)
                    line += "X ";
                else
                    line += heuristicGraph[i, j] + " ";
            }
            line += '\n';
        }
        Debug.Log(line);

        // Алгоритм Флойда-Уоршелла.
        for (int k = 0; k < N; ++k)
            for (int i = 0; i < N; ++i)
                for (int j = 0; j < N; ++j)
                    heuristicGraph[i, j] = heuristicGraph[i, j] < (heuristicGraph[i, k] + heuristicGraph[k, j]) ? heuristicGraph[i, j] : (heuristicGraph[i, k] + heuristicGraph[k, j]);

        for (int k = 0; k < N; ++k)
            for (int i = 0; i < N; ++i)
                for (int j = 0; j < N; ++j)
                    if (graph[i, j] < (graph[i, k] + graph[k, j]))
                        graph[i, j] = graph[i, j];
                    else
                        graph[i, j] = graph[i, k] + graph[k, j];


        line = "";
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (heuristicGraph[i, j] == INF)
                    line += "X ";
                else
                    line += heuristicGraph[i, j] + " ";
            }
            line += '\n';
        }
        Debug.Log(line);
    }

    public void SaveGraph()
    {
        string path = SceneManager.GetActiveScene().name + ".dat";

        BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));

        writer.Write(nodesList.Count);
        foreach(var node in nodesList)
        {
            writer.Write(node.gameObject.transform.position.x);
            writer.Write(0f);
            writer.Write(node.gameObject.transform.position.z);
        }

        writer.Write(connectionsList.Count);
        foreach (var connection in connectionsList)
        {
            writer.Write(connection.ConnectedObjects[0].GetComponent<HeuristicNode>().id);
            writer.Write(connection.ConnectedObjects[1].GetComponent<HeuristicNode>().id);
        }

        writer.Close();
    }

    public void LoadGraph(Vector3 nodesScale)
    {
        string path = SceneManager.GetActiveScene().name + ".dat";

        foreach (var node in nodesList)
            Destroy(node.gameObject);

        foreach (var connection in connectionsList)
            Destroy(connection.gameObject);

        nodesList.Clear();
        connectionsList.Clear();
        
        BinaryReader reader = new BinaryReader(File.Open(path, FileMode.OpenOrCreate));

        Vector3 nodePos;
        int N = reader.ReadInt32();
        for (int i = 0; i < N; i++ )
        {
            nodePos.x = reader.ReadSingle();
            nodePos.y = reader.ReadSingle();
            nodePos.z = reader.ReadSingle();

            CreateNode(nodePos, nodesScale);
        }

        GameObject from, to;
        int fromId, toId;
        N = reader.ReadInt32();
        for (int i = 0; i < N; i++)
        {
            fromId = reader.ReadInt32();
            toId = reader.ReadInt32();

            from = nodesList.Find(x => x.id == fromId).gameObject;
            to = nodesList.Find(x => x.id == toId).gameObject;

            CreateConnection(from, to, nodesScale.x);
        }

        reader.Close();
    }
}
