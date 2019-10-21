using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Wave

{
    public int EnemiesPerWave;
    public GameObject Enemy;
}
 
public class SpawnManager : MonoBehaviour
{
    static public SpawnManager instance;
    public Wave[] Waves; // class to hold information per wave
    public Transform[] SpawnPoints;
    public float TimeBetweenEnemies = 2f;
 
    private int _totalEnemiesInCurrentWave;
    private int _spawnedEnemies;
 
    private int _currentWave;
    private int _totalWaves;
    private float timeMult;
 
    void Start()
    {
        instance = this;
        _currentWave = -1; // avoid off by 1
        _totalWaves = Waves.Length - 1; // adjust, because we're using 0 index
 
        StartNextWave();
    }

    void StartNextWave()
    {
        if (SceneManager.GetActiveScene().name == "_Scene_2")
        {
            timeMult = Random.Range(0.1f, 1f);
            TimeBetweenEnemies = timeMult * 3.0f;
        }

        _currentWave++;

        // win
        if (_currentWave > _totalWaves)
        {
            SceneManager.LoadScene("_Scene_3");
        }
 
        _totalEnemiesInCurrentWave = Waves[_currentWave].EnemiesPerWave;
        _spawnedEnemies = 0;
 
        StartCoroutine(SpawnEnemies());

    }
 
    // Coroutine to spawn all of our enemies
    IEnumerator SpawnEnemies()
    {
        GameObject enemy = Waves[_currentWave].Enemy;

        _spawnedEnemies++;

        for(int i = 0; i<SpawnPoints.Length; i++)
        {
            Instantiate(enemy, SpawnPoints[i].position, SpawnPoints[i].rotation);
        }
            
        yield return new WaitForSeconds(TimeBetweenEnemies);

        StartNextWave();
        
        yield return null;
    }

}
