using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MinimaxScript : MonoBehaviour {

    public bool isMax;
    public bool isPruning;


    public int AlphaBetaPruning(Node node)
    {
        if (isMax)
            return node.AlphaPruning(null, -1, 1000);
        else
            return node.BetaPruning(null, -1, 1000);
    }

    public int Minimax(Node node)
    {
        if (isMax)
            return node.GetChildMax(null);
        else
            return node.GetChildMin(null);
    }
}
