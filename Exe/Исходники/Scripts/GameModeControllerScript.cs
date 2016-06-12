using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameModeControllerScript : MonoBehaviour {

    public bool SimulateStage
    { set { ChangeStage(value); } }
    Button Simulate;
    Text SimulateText;

    bool isSimulate;

    NodesPlacerScript NodesPlacer;


	// Use this for initialization
	void Start () {
        isSimulate = false;
        NodesPlacer = GameObject.FindObjectOfType<NodesPlacerScript>();
	}

    void ChangeStage(bool newStage)
    {
        isSimulate = newStage;

        if (!isSimulate)
            return;

       
    }
}
