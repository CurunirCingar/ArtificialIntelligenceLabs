using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AntBehaviorScript : MonoBehaviour {

    bool cyclePassed;

    AntColonyScript colony;

    bool[] visitedNodes;
    PheromoneNode m_startNode, m_endNode;

    float m_movementSpeed;
    public float MovementSpeed { get { return m_movementSpeed; } set { m_movementSpeed = value;} }

    PheromoneNode targetNode;
    Vector3 targetNodePosition;

    List<PheromoneConnection> passedConnections;
    float passedDistance;
    Dictionary<PheromoneNode, float> nodesChances;

    void Awake()
    {
        colony = FindObjectOfType<AntColonyScript>();
        passedConnections = new List<PheromoneConnection>();
        nodesChances = new Dictionary<PheromoneNode, float>();
    }

    public void SetAnt(PheromoneNode startNode, PheromoneNode endNode, int nodesAmount, float movementSpeed)
    {
        //Debug.Log("SetAnt");
        visitedNodes = new bool[nodesAmount];
        m_startNode = startNode;
        m_endNode = endNode;
        m_movementSpeed = movementSpeed;
        
        passedConnections.Clear();
        passedDistance = 0f;

        for (int i = 0; i < visitedNodes.Length; i++)
            visitedNodes[i] = false;
        cyclePassed = true;

        transform.position = m_startNode.gameObject.transform.position;
        targetNode = m_startNode;
        targetNodePosition = m_startNode.gameObject.transform.position;
    }

    public void SendAnt()
    {
        //Debug.Log("SendAnt");
        cyclePassed = false;
    }

    private void ReturnToStart()
    {
        cyclePassed = true;
        passedConnections.Clear();
        passedDistance = 0f;

        for (int i = 0; i < visitedNodes.Length; i++)
            visitedNodes[i] = false;

        transform.position = m_startNode.gameObject.transform.position;
        targetNode = m_startNode;
        targetNodePosition = m_startNode.gameObject.transform.position;
        colony.AntFinished();
    }

    private void ChooseNextNode()
    {
        if (targetNode.Type == NodesTypesT.END)
        {
            SprayPheromone();
            ReturnToStart();
            return;
        }

        float[] chancesBuf = new float[targetNode.NeighbourNodes.Count];
        float allChances = 0;
        visitedNodes[targetNode.id] = true;
        nodesChances.Clear();
        
        PheromoneConnection conection;
        int i = 0;
        foreach (var node in targetNode.NeighbourNodes.Keys)
        {
            if (visitedNodes[node.id])
                continue;

            conection = targetNode.NeighbourNodes[node];
            
            chancesBuf[i] = Mathf.Pow(conection.Pheromone, colony.Alpha) * Mathf.Pow( (1/conection.Distance), colony.Beta);
            //Debug.Log(node.id + " P = " + Mathf.Pow(conection.Pheromone, colony.Alpha) + " a = " + colony.Alpha + " D = " + Mathf.Pow((1 / conection.Distance), colony.Beta) + " b = " + colony.Beta + " C = " + chancesBuf[i]);
            allChances += chancesBuf[i];
            i++;
        }
        //Debug.Log(allChances);
        if (i == 0)
        {
            ReturnToStart();
            return;
        }

        i = 0;
        foreach (var node in targetNode.NeighbourNodes.Keys)
        {
            if (visitedNodes[node.id])
                continue;

            chancesBuf[i] /= allChances;
            nodesChances.Add(node, chancesBuf[i]);
            //Debug.Log(node.id + " " + nodesChances[node]);
            i++;
        }

        //Debug.Log(i + " " + chancesBuf[0]);
        for (i = 1; i < chancesBuf.Length; i++)
        {
            chancesBuf[i] += chancesBuf[i - 1];
            //Debug.Log(i + " " + chancesBuf[i]);
        }
        
        float rndChoice = Random.Range(0f, 1f);
        i = 0;
        foreach (var node in nodesChances.Keys)
        {
            if (rndChoice <= chancesBuf[i])
            {
                //Debug.Log("Choice: " + i + " " + rndChoice);
                passedConnections.Add(targetNode.NeighbourNodes[node]);
                passedDistance += targetNode.NeighbourNodes[node].Distance;
                targetNode = node;
                targetNodePosition = node.gameObject.transform.position;
                break; 
            }
            i++;
        }
        
    }

    private void SprayPheromone()
    {
        foreach (var connection in passedConnections)
            connection.SprayPheromone(passedDistance);
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("Update " + cyclePassed);
        if (colony.ShowVisualSimulation && Vector3.Distance(transform.position, targetNodePosition) >= 0.8f)
        {
            transform.LookAt(targetNodePosition);
            transform.Translate(0f, 0f, m_movementSpeed, Space.Self);
        }
        else
            if (!cyclePassed)
            ChooseNextNode();
    }
}