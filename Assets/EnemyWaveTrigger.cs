using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // 引入Cinemachine命名空间

public class EnemyWaveTrigger : MonoBehaviour
{
  
    [Header("Cinemachine相机")]
    public CinemachineVirtualCamera vcFollow; // 拖入跟随相机VC_Follow
    public CinemachineVirtualCamera vcFixed; // 拖入固定相机VC_Fixed
    public Transform player; // 拖入玩家对象

    [Header("敌人生成设置")] // 保持你之前的敌人配置（略）
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int enemyCount = 5;
    public float spawnInterval = 0.5f;
    public float waveDelay = 1f;

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            // 1. 切换到固定相机：提高固定相机优先级（覆盖跟随相机）
            vcFixed.Priority = 20;
            // 2. 计算固定相机的可视范围，传递给玩家（限制移动）
            SetPlayerMoveBounds();
            // 3. 生成敌人（保持之前的协程逻辑）
            StartCoroutine(SpawnEnemyWave());
        }
    }

    // 计算固定相机的可视范围，传递给玩家移动脚本
    private void SetPlayerMoveBounds()
    {
        // 获取固定相机的正交相机组件
        Camera fixedCam = vcFixed.GetComponentInChildren<Camera>();
        if (fixedCam == null || player == null) return;

        // 计算2D正交相机的可视范围：
        float orthoSize = fixedCam.orthographicSize; // 相机正交大小
        float aspect = fixedCam.aspect; // 屏幕宽高比
        float viewWidth = orthoSize * 2 * aspect; // 可视宽度
        float viewHeight = orthoSize * 2; // 可视高度

        // 镜头范围：以固定相机位置为中心的矩形
        Rect bounds = new Rect(
            vcFixed.transform.position.x - viewWidth / 2, // 左边界
            vcFixed.transform.position.y - viewHeight / 2, // 下边界
            viewWidth, // 宽度
            viewHeight // 高度
        );

        // 把范围传递给玩家的移动脚本
        NewBehaviourScript playerMove = player.GetComponent<NewBehaviourScript>();
        if (playerMove != null)
        {
            playerMove.isInFixedCameraArea = true;
            playerMove.fixedCameraBounds = bounds;
        }
    }

    // 敌人生成协程（保持你之前的代码，略）
    private IEnumerator SpawnEnemyWave()
    {
        yield return new WaitForSeconds(waveDelay);

        int spawnedCount = 0;
        while (spawnedCount < enemyCount)
        {
            // 随机选一个生成点
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            // 生成敌人
            Instantiate(enemyPrefab, randomSpawnPoint.position, Quaternion.identity);
            spawnedCount++;
            // 间隔生成
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // 可选：敌人全灭后恢复跟随相机+解除移动限制
    public void ResetCameraAndBounds()
    {
        vcFixed.Priority = 0; // 降低固定相机优先级，恢复跟随相机
        NewBehaviourScript  playerMove = player.GetComponent<NewBehaviourScript>();
        if (playerMove != null)
        {
            playerMove.isInFixedCameraArea = false;
        }
    }
}

