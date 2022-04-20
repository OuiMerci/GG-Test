using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour {

    [System.Serializable]
    public struct EnemySpawn
    {
        // The type of enemy we want to spawn.
        public int enemyDatabaseId;

        // The position we want it to have.
        public float xPosition;

        // Or do we want a random position ? This will overwrite the field "xPosition". (Eventually, I would like to make a custom editor to hide xPosition if randomPosition is checked.)
        public bool randomPosition;
    }

    [System.Serializable]
    public struct EnemySpawnPattern
    {
        public int weight;
        public List<EnemySpawn> enemies;
    }

    #region Fields
    private const float SCREEN_REFERENCE_WIDTH = 420;

    [SerializeField] private float _delayBetweenWaves = 0.0f;
    [SerializeField] private float _minDelayBetweenWaves = 0.0f;
    [SerializeField] private float _delayReductionSpeed = 0.0f;
    [SerializeField] private List<EnemySpawnPattern> _spawnPatterns = null;

    private EnemyManager _enemyManager = null;
    private int _totalPatternWeight = 0;
    private float _lastSpawnTime = 0.0f;
    #endregion Fields

    #region Private Methods
    private void Start()
    {
        _lastSpawnTime = -_delayBetweenWaves;
        _spawnPatterns.OrderBy(p => p.weight);
        _totalPatternWeight = _spawnPatterns.Sum(c => c.weight);
        _enemyManager = EnemyManager.Instance;
    }

    private void Update()
    {
        if(_lastSpawnTime + _delayBetweenWaves < Time.time)
        {
            SpawnRandomPattern();
            _lastSpawnTime = Time.time;

            _delayBetweenWaves -= _delayReductionSpeed;
            _delayBetweenWaves = _delayBetweenWaves < _minDelayBetweenWaves ? _minDelayBetweenWaves : _delayBetweenWaves;
        }
    }

    private void SpawnRandomPattern()
    {
        if(_spawnPatterns.Count == 0)
        {
            Debug.Log("EnemySpawner : No pattern was found, please add at least one.");
            return;
        }

        int randomNumber = Random.Range(0, _totalPatternWeight);

        EnemySpawnPattern chosenPattern = default(EnemySpawnPattern);

        foreach (EnemySpawnPattern pattern in _spawnPatterns)
        {
            if (randomNumber < pattern.weight)
            {
                chosenPattern = pattern;
                break;
            }

            randomNumber = randomNumber - pattern.weight;
        }

        chosenPattern.enemies.ForEach(p => SpawnEnemy(p));
    }

    private void SpawnEnemy(EnemySpawn spawn)
    {
        float posX = 0;

        if(spawn.randomPosition == true)
        {
            posX = Random.Range(GameCamera.LeftLimit, GameCamera.RightLimit);
        }
        else
        {
            // Multiply the position by the ratio "current width" / "reference width"
            posX = spawn.xPosition * (Screen.width / SCREEN_REFERENCE_WIDTH);
        }

        EnemyBehaviour enemy = _enemyManager.GetEnemy(spawn.enemyDatabaseId);
        enemy.transform.position = new Vector3(posX, GameCamera.UpperLimit + enemy.SafeDistance, transform.position.z);
        enemy.transform.eulerAngles = transform.eulerAngles;
    }
    #endregion Private Methods
}
