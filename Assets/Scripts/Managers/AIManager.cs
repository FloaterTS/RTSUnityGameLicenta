using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager instance;

    public Transform enemyWavesSpawnPoint;

    public float firstWaveStartAfter;
    public float minTimeBetweenWaves;
    public float maxTimeBetweenWaves;

    public int maxEnemyUnitsSpawnedPerWave = 5;

    private int waveNumber;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another AI manager present.");
    }

    private void Start()
    {
        StartCoroutine(StartWaves());
    }

    private IEnumerator StartWaves()
    {
        waveNumber = 0;
        yield return new WaitForSeconds(firstWaveStartAfter);
        StartCoroutine(SpawnEnemyWaves());
    }

    private IEnumerator SpawnEnemyWaves()
    {
        waveNumber++;
        Debug.Log("Spawning wave " + waveNumber + "...");

        for (int i = 0; i < Mathf.Min(waveNumber, maxEnemyUnitsSpawnedPerWave); i++)
        {
            GameObject enemyUnit = Instantiate(PrefabManager.instance.villagerEnemyPrefab, enemyWavesSpawnPoint.position, Quaternion.identity, PrefabManager.instance.unitsTransformParentGO.transform);

            Unit closestPlayerUnit = GameManager.instance.GetClosestUnitOfTeamFrom(Team.PLAYER, enemyUnit.transform.position);

            enemyUnit.GetComponent<Fighter>().AttackCommand(closestPlayerUnit.gameObject, true);
        }

        yield return new WaitForSeconds(Random.Range(minTimeBetweenWaves, maxTimeBetweenWaves));

        StartCoroutine(SpawnEnemyWaves());
    }
}
