using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;

public class Levels : MonoBehaviour
{
    public void StartScreen()
    {
        SceneManager.LoadScene("Start");
    }
    public void Level1()
    {
        SceneManager.LoadScene("_Scene_0");
    }
    public void Level2Load()
    {
        SceneManager.LoadScene("_Scene_1");
    }
    public void Level2()
    {
        SceneManager.LoadScene("_Scene_2");
    }
    public void Level3Load()
    {
        SceneManager.LoadScene("_Scene_3");
    }
    public void Level3()
    {
        SceneManager.LoadScene("_Scene_4");
    }

}
