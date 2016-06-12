using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AntColonyScript : MonoBehaviour {

    PheromoneNodeFactory nodeFactory;

    float m_alpha = 0.5f;
    float m_beta = 0.5f;
    public float Alpha
    {
        get { return m_alpha; }
        set { EqualizeSliders(value, 1 - value); }
    }
    public float Beta
    {
        get { return m_beta; }
        set { EqualizeSliders(1 - value, value); }
    }

    float m_pheromoneIncreaseValue = 1f;
    float m_pheromoneDecreaseValue = 0.002f;
    public float PheromoneIncreaseValue { get { return m_pheromoneIncreaseValue; } set { m_pheromoneIncreaseValue = value;} }
    public float PheromoneDecreaseValue { get { return m_pheromoneDecreaseValue; } set { m_pheromoneDecreaseValue = value;} }

    float m_antsSpeed = 0.5f;

    int m_iterationsAmount = 0;
    int m_iterationsPassed = 0;
    int antsAmount;
    public int IterationsAmount
    {
        get { return m_iterationsAmount; }
        set { m_iterationsAmount = value; }
    }

    List<AntBehaviorScript> AntColony;

    Text AntsAmountText, IterationAmountText, PheromoneInfluenceText, DistanceInfluenceText, EvaporationStrenghtText, PheromoneStrenghtText;
    Slider PheromoneInfluenceSlider, DistanceInfluenceSlider, EvaporationStrenghtSlider, PheromoneStrenghtSlider, AntsSpeedSlider;
    InputField AddAntsField, IterationsAmountField;
    Button SetAntsButton, SetIterationsButton, ResetPheromoneButton, StopButton, PlayButton;
    Toggle VisualSimulationToggle;

    int m_nodesAmount;

    int antsIgnited;
    bool m_isSimulating = false;

    private bool m_showVisualSimulation;
    public bool ShowVisualSimulation
    {
        get { return m_showVisualSimulation; }
    }

    float m_speedBeforePause;

    List<PheromoneNode> nodesList;
    List<PheromoneConnection> connectionsList;
    PheromoneNode StartNode;
    PheromoneNode EndNode;

    // Use this for initialization
    void Awake ()
    {
        AntsAmountText = GameObject.Find("AntsAmountText").GetComponent<Text>();
        IterationAmountText = GameObject.Find("IterationAmountText").GetComponent<Text>();
        PheromoneInfluenceText = GameObject.Find("PheromoneInfluenceText").GetComponent<Text>();
        DistanceInfluenceText = GameObject.Find("DistanceInfluenceText").GetComponent<Text>();
        EvaporationStrenghtText = GameObject.Find("EvaporationStrenghtText").GetComponent<Text>();
        PheromoneStrenghtText = GameObject.Find("PheromoneStrenghtText").GetComponent<Text>();

        PheromoneInfluenceSlider = GameObject.Find("PheromoneInfluenceSlider").GetComponent<Slider>();
        PheromoneInfluenceSlider.onValueChanged.AddListener(PheromoneInfluenceChange);
        DistanceInfluenceSlider = GameObject.Find("DistanceInfluenceSlider").GetComponent<Slider>();
        DistanceInfluenceSlider.onValueChanged.AddListener(DistanceInfluenceChange);
        EvaporationStrenghtSlider = GameObject.Find("EvaporationStrenghtSlider").GetComponent<Slider>();
        EvaporationStrenghtSlider.onValueChanged.AddListener(EvaporationStrenghtChange);
        PheromoneStrenghtSlider = GameObject.Find("PheromoneStrenghtSlider").GetComponent<Slider>();
        PheromoneStrenghtSlider.onValueChanged.AddListener(PheromoneStrenghtChange);
        AntsSpeedSlider = GameObject.Find("AntsSpeedSlider").GetComponent<Slider>();
        AntsSpeedSlider.onValueChanged.AddListener(RefreshAntsParams);

        AddAntsField = GameObject.Find("AddAntsField").GetComponent<InputField>(); 
        IterationsAmountField = GameObject.Find("IterationsAmountField").GetComponent<InputField>();

        SetAntsButton = GameObject.Find("SetAntsButton").GetComponent<Button>();
        SetAntsButton.onClick.AddListener(SetAnts);
        SetIterationsButton = GameObject.Find("SetIterationsButton").GetComponent<Button>();
        SetIterationsButton.onClick.AddListener(SetIterations);
        ResetPheromoneButton = GameObject.Find("ResetPheromoneButton").GetComponent<Button>();
        ResetPheromoneButton.onClick.AddListener(ResetPheromoneValues);
        StopButton = GameObject.Find("StopButton").GetComponent<Button>();
        StopButton.onClick.AddListener(StopSimulation);
        PlayButton = GameObject.Find("PlayButton").GetComponent<Button>();
        PlayButton.onClick.AddListener(StartSimulation);

        VisualSimulationToggle = GameObject.Find("VisualSimulationToggle").GetComponent<Toggle>();
        VisualSimulationToggle.onValueChanged.AddListener(UseVisualSimulation);

        GameObject SimulatePanel = GameObject.Find("SimulatePanel");
        SimulatePanel.SetActive(false);

        AntColony = new List<AntBehaviorScript>();
        AntsAmountText.text = string.Format("{0} ants", AntColony.Count);
        IterationAmountText.text = string.Format("{0} iterations", m_iterationsAmount);
        PheromoneInfluenceText.text = string.Format("Pheromone influence: {0}", m_alpha);
        DistanceInfluenceText.text = string.Format("Distance influence: {0}", m_beta);
        EvaporationStrenghtText.text = string.Format("Evaporation strenght: {0}", m_pheromoneDecreaseValue);
        PheromoneStrenghtText.text = string.Format("Pheromone strenght: {0}", m_pheromoneIncreaseValue);
    }

    private void PheromoneInfluenceChange(float value)
    {
        Alpha = value;
        PheromoneInfluenceText.text = string.Format("Pheromone influence: {0}", m_alpha);
    }

    private void DistanceInfluenceChange(float value)
    {
        Beta = value;
        DistanceInfluenceText.text = string.Format("Distance influence: {0}", m_beta);
    }

    private void EvaporationStrenghtChange(float value)
    {
        PheromoneDecreaseValue = value;
        EvaporationStrenghtText.text = string.Format("Evaporation strenght: {0}", m_pheromoneDecreaseValue);
    }

    private void PheromoneStrenghtChange(float value)
    {
        PheromoneIncreaseValue = value;
        PheromoneStrenghtText.text = string.Format("Pheromone strenght: {0}", m_pheromoneIncreaseValue);
    }

    private void UseVisualSimulation(bool value)
    {
        m_showVisualSimulation = value;
    }

    private void RefreshAntsParams(float value)
    {
        m_antsSpeed = value;
        foreach(var ant in AntColony)
            ant.MovementSpeed = m_antsSpeed;
    }

    public void SetColonyParams(List<PheromoneNode> _nodesList, List<PheromoneConnection> _connectionsList, PheromoneNode From, PheromoneNode To)
    {
        nodesList = _nodesList;
        connectionsList = _connectionsList;
        StartNode = From;
        EndNode = To;

        m_nodesAmount = nodesList.Count;
        m_isSimulating = true;
        m_showVisualSimulation = VisualSimulationToggle.isOn;

        GameObject antPrefab = (GameObject)Resources.Load("AntPrefab");
        AntColony.Clear();
        for (int i = 0; i < antsAmount; i++)
        {
            //Debug.Log(StartNode);
            GameObject Ant = (GameObject)Instantiate(antPrefab, StartNode.gameObject.transform.position, Quaternion.identity);
            AntBehaviorScript AntBehaviour = Ant.GetComponent<AntBehaviorScript>();
            AntBehaviour.SetAnt(StartNode, EndNode, m_nodesAmount, m_antsSpeed);
            AntColony.Add(AntBehaviour);
        }
    }

    public void RefreshParams()
    {
        foreach (var node in nodesList)
            node.isVisited = false;
        foreach (var connection in connectionsList)
            connection.IsHighlighted = false;
    }

    public void SetAnts()
    {
        if (!int.TryParse(AddAntsField.text, out antsAmount))
            return;

        AddAntsField.text = "";
        AntsAmountText.text = string.Format("{0} ants", antsAmount);
    }

    public void IgniteWave()
    {
        foreach (var connection in connectionsList)
            connection.UpdatePheromone();
        StartCoroutine(SendAnt());
        antsIgnited = AntColony.Count;
        m_iterationsPassed++;
        IterationAmountText.text = string.Format("Iteration: {0}/{1}", m_iterationsPassed, m_iterationsAmount);
        //Debug.Log("antsIgnited = " + antsIgnited);   
    }

    IEnumerator SendAnt()
    {
        AntBehaviorScript[] CurrentAntColony = new AntBehaviorScript[AntColony.Count];
        AntColony.CopyTo(CurrentAntColony);
        for(int i = 0; i < CurrentAntColony.Length; i++)
        {
            CurrentAntColony[i].SendAnt();
            if(m_showVisualSimulation)
                yield return new WaitForSeconds(1f - m_antsSpeed);
        }
    }

    public void AntFinished()
    {
        antsIgnited--;
    }

    void FixedUpdate()
    {
        if (m_isSimulating && antsIgnited == 0)
        {
            HighlightFinalPath();
            if (m_iterationsAmount != m_iterationsPassed)
                IgniteWave();
            else
            {
                m_isSimulating = false;

            }
        }
        
    }

    public void RefreshText()
    {
        PheromoneInfluenceText.text = string.Format("Pheromone influence: {0}", m_alpha);
        DistanceInfluenceText.text = string.Format("Distance influence: {0}", m_beta);
        EvaporationStrenghtText.text = string.Format("Evaporation strenght: {0}", m_pheromoneDecreaseValue);
        PheromoneStrenghtText.text = string.Format("Pheromone strenght: {0}", m_pheromoneIncreaseValue);
    }

    public void StartSimulation()
    {
        m_isSimulating = true;
    }

    public void StopSimulation()
    {
        m_isSimulating = false;
        ResetPheromoneValues();
        m_iterationsPassed = 0;
    }

    public void PauseSimulation()
    {
        m_isSimulating = false;
    }

    public int HighlightFinalPath()
    {
        foreach (var node in nodesList)
            node.isVisited = false;
        foreach (var connection in connectionsList)
            connection.IsHighlighted = false;
        
        return StartNode.HighlightBestPath(null);
    }

    public void SetIterations()
    {
        int.TryParse(IterationsAmountField.text, out m_iterationsAmount);
        IterationsAmountField.text = "";
        IterationAmountText.text = string.Format("{0} iterations", m_iterationsAmount);
    }

    IEnumerator AntsSpeedFade(float speed)
    {
        while (m_antsSpeed >= 0.0001f)
        {
            m_antsSpeed = Mathf.Lerp(m_antsSpeed, 0f, 0.7f);
            yield return null;
        }
        m_antsSpeed = 0f;
    }

    public void ResetPheromoneValues()
    {
        foreach (var connection in connectionsList)
            connection.ResetPheromoneValue();
    }

    private void EqualizeSliders(float alpha, float beta)
    {
        m_alpha = alpha;
        m_beta = beta;

        PheromoneInfluenceSlider.value = m_alpha;
        DistanceInfluenceSlider.value = m_beta;
    }
}
