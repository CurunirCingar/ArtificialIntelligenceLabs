using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class NodesPlacerScript : MonoBehaviour {

    public INodeFactory nodeFactory;

    private SimpleMove Move;
    
    private GameObject FromNode;
    private GameObject ToNode;
    private bool connectionSetup;
    private float nodesScale;

    private float clickTimePassed;
    private float doubleClickInterval;
    private bool isDoubleClicked;

    public INodeFactory NodeFactory
    {
        get { return nodeFactory; }
    }

    public float NodesSize
    {
        set
        {
            nodesScale = value;
            nodeFactory.SetSize(nodesScale);
        }
    }

	void Awake () {
        nodesScale = 3f;
        switch (SceneManager.GetActiveScene().name)
        {
            case "AStar":
                nodeFactory = gameObject.AddComponent<HeuristicNodeFactory>();
                break;

            case "Ants":
                nodeFactory = gameObject.AddComponent<PheromoneNodeFactory>();
                break;

            case "Cutouts":
                nodeFactory = gameObject.AddComponent<MinimaxNodeFactory>();
                break;
        }
        
        Move = FindObjectOfType<SimpleMove>();
        doubleClickInterval = 0.4f;
        clickTimePassed = doubleClickInterval + 1;
        isDoubleClicked = false;
	}
	
	// Update is called once per frame
	void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnLeftMouseDown();

        if (Input.GetMouseButtonUp(0))
            OnLeftMouseUp();

        if (Input.GetMouseButtonUp(1))
            OnRightMouseUp();

        if (Input.GetKey(KeyCode.Delete))
            nodeFactory.DeleteNode();
	}

    private bool GetRaycastPosition(out RaycastHit hit)
    {
        Vector3 placePoint;

        if (Move.lockCursor)
            placePoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        else
            placePoint = new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);

        Ray ray = Camera.main.ScreenPointToRay(placePoint);
        return Physics.Raycast(ray, out hit, 300);
    }

    private void OnLeftMouseDown()
    {
        RaycastHit raycastHit;

        if (!GetRaycastPosition(out raycastHit))
            return;

        if (raycastHit.collider.gameObject.tag == "Node")
        {
            // Double click detection;
            if ( IsDoubleClick() )
            {
                nodeFactory.SetupNode(FromNode);
            }
            
            FromNode = raycastHit.collider.gameObject;
            connectionSetup = true;
        }
        else
            if (Input.GetKey(KeyCode.LeftControl))
                nodeFactory.CreateNode(raycastHit.point, new Vector3(nodesScale, nodesScale, nodesScale));
    }

    private bool IsDoubleClick()
    {
        bool result = (Time.timeSinceLevelLoad - clickTimePassed) < doubleClickInterval;

        if (result)
            clickTimePassed = Time.timeSinceLevelLoad - clickTimePassed;
        else
            clickTimePassed = Time.timeSinceLevelLoad;

        return result;
    }

    private void OnLeftMouseUp()
    {
        RaycastHit raycastHit;

        if (!connectionSetup)
            return;

        if (!GetRaycastPosition(out raycastHit))
            return;

        if (raycastHit.collider.gameObject.tag == "Node" && raycastHit.collider.gameObject != FromNode)
        {
            ToNode = raycastHit.collider.gameObject;

            //Connect both nodes to each other
            nodeFactory.CreateConnection(FromNode, ToNode, nodesScale);
        }
        connectionSetup = false;
    }

    private void OnRightMouseUp()
    {
        RaycastHit raycastHit;

        if (!GetRaycastPosition(out raycastHit))
            return;

        nodeFactory.SetTargetNode(raycastHit.collider.gameObject);
    }

    public void StartAlgorithm()
    {
        nodeFactory.StartAlgorithm();
    }
    
    public void SaveGraph()
    {
        nodeFactory.SaveGraph();
    }

    public void LoadGraph()
    {
        nodeFactory.LoadGraph(new Vector3(nodesScale, nodesScale, nodesScale));
    }
}