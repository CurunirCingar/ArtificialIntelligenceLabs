using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManagerScript : MonoBehaviour {

    static string MenuLevel = "Menu";
    static string AntsLevel = "Ants";
    static string AStarLevel = "AStar";
    static string CutoutsLevel = "Cutouts";

    public void LoadMenu()
    {
        SceneManager.LoadScene(MenuLevel);
    }

	public void LoadAnts()
    {
        SceneManager.LoadScene(AntsLevel);
    }

    public void LoadAStar()
    {
        SceneManager.LoadScene(AStarLevel);
    }

    public void LoadCutouts()
    {
        SceneManager.LoadScene(CutoutsLevel);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
