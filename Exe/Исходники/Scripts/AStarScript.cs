using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AStarScript : MonoBehaviour {

    const float INF = float.MaxValue;
    [SerializeField]
    Text AlgorithmLog;
    Toggle stepExecutionToggle;
    Button nextStepButton;
    string logText, resultPath;

    HeuristicNode start, end, current, previous;
    List<HeuristicNode> nodesList;
    List<HeuristicNode> Open = new List<HeuristicNode>();
    List<HeuristicNode> Closed = new List<HeuristicNode>();
    float heuristicCoefficient;

    private bool isStepRun;
    private bool stepExecution;
    float passedDistance = 0f;

    public bool IsStepRun
    {
        get { return isStepRun; }
        set { isStepRun = value; }
    }

    public void RunAStarAlgorithm(List<HeuristicNode> _nodesList, HeuristicNode from, HeuristicNode to)
    {
        Open.Clear();
        Closed.Clear();
        Debug.Log("Enter!");

        AlgorithmLog = GameObject.Find("AlgorithmLogText").GetComponent<Text>();
        AlgorithmLog.text = "";
        nextStepButton = GameObject.Find("NextStepButton").GetComponent<Button>();
        nextStepButton.onClick.AddListener(NextStep);
        stepExecutionToggle = GameObject.Find("StepExecutionToggle").GetComponent<Toggle>();
        stepExecutionToggle.onValueChanged.AddListener(StepExecution);

        nodesList = _nodesList;
        start = from;
        end = to;

        foreach (var node in nodesList)
            node.HeuristicNodeType = HeuristicNode.HeuristicNodeT.UNSEEN;

        StartCoroutine(AlgorithmCycle());
	}

    private void NextStep()
    {
        isStepRun = false;
    }

    private void StepExecution(bool value)
    {
        stepExecution = value;
    }

    IEnumerator AlgorithmCycle()
    {
        yield return new WaitForSeconds(0.1f);

        stepExecution = stepExecutionToggle.isOn;
        passedDistance = 0f;
        bool isStart = true;
        resultPath += start.id + " => ";
        current = start;

        Open.Add(start);
        start.HeuristicNodeType = HeuristicNode.HeuristicNodeT.OPEN;
        while (Open.Count != 0)
        {
            if (!isStart)
            {
                heuristicCoefficient = INF;
                foreach (var node in Open)
                {
                    if (previous.NeighbourNodes.ContainsKey(node))
                    {
                        if (Heuristic(node) < heuristicCoefficient)
                        {
                            heuristicCoefficient = Heuristic(node);
                            current = node;

                        }
                        logText += "Can go to: " + node.id + ". Heuristic = " + node.Heuristic + ". Distance = " + previous.NeighbourNodes[node].Distance + "\n";
                    }
                }

                logText += "Current node: " + current.id + ". Heuristic = " + current.Heuristic + ". Distance = " + previous.NeighbourNodes[current].Distance + "\n";

                passedDistance += previous.NeighbourNodes[current].Distance;
                logText += "Passed distance: " + passedDistance + "\n\n";

                previous.HighlightConnection(current);
                if (current == end)
                {
                    current.HeuristicNodeType = HeuristicNode.HeuristicNodeT.CLOSED;
                    logText += "Algorithm succeed: The end reached!\n";
                    resultPath += current.id;
                    logText += resultPath + "\n";
                    AlgorithmLog.text += logText;
                    resultPath = "";
                    logText = "";
                    break;
                }
                else
                    resultPath += current.id + " => ";
            }

            isStart = false;
            
            Open.Remove(current);
            Closed.Add(current);
            current.HeuristicNodeType = HeuristicNode.HeuristicNodeT.CLOSED;

            previous = current;

            foreach (var neighbour in GetUnclosedNeighbours(current))
                if (!Open.Contains(neighbour))
                {
                    Open.Add(neighbour);
                    neighbour.HeuristicNodeType = HeuristicNode.HeuristicNodeT.OPEN;
                    logText += "Open node: " + neighbour.id + "\n";
                }

            AlgorithmLog.text += logText;
            logText = "";

            if (stepExecution)
            {
                isStepRun = true;
                while (isStepRun)
                    yield return new WaitForSeconds(0.1f);
            }
        }
        yield return null;
    }

    float Heuristic(HeuristicNode node)
    {
        //Debug.Log("Node " + node + " = " + passedDistance + " + " + previous.NeighbourNodes[node].Distance + " + " + node.Heuristic);
        return passedDistance + previous.NeighbourNodes[node].Distance + node.Heuristic;
    }

    HeuristicNode[] GetUnclosedNeighbours(HeuristicNode currentNode)
    {
        List<HeuristicNode> unclosedNeighbours = new List<HeuristicNode>();
        int N = nodesList.Count;
        foreach (var node in currentNode.NeighbourNodes.Keys)
        {
            if ( !Closed.Contains(node) )
                unclosedNeighbours.Add(node);
        }

        return unclosedNeighbours.ToArray();
    }
}
