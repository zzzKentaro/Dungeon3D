using UnityEngine;

/// <summary>
/// ダンジョン探索ゲーム全体を管理するクラス。
/// 
/// 今回の初期版では、
/// ・ダンジョン生成
/// ・プレイヤー初期配置
/// ・ゴール到達時の処理
/// を担当します。
/// </summary>
public class DungeonGameManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField]
    private DungeonBuilder dungeonBuilder;

    [SerializeField]
    private GridPlayerController playerController;

    [SerializeField]
    private DungeonUIManager uiManager;

    [Header("プレイヤー初期設定")]
    [SerializeField]
    private GridDirection playerStartDirection = GridDirection.North;

    [Header("メッセージ")]
    [SerializeField]
    private string startMessage = "WASDで移動、Q/Eで回転します。";

    [SerializeField]
    private string goalMessage = "迷宮を脱出した！";

    public bool IsGameCleared { get; private set; }

    private void Start()
    {
        ResolveReferences();

        if (dungeonBuilder == null)
        {
            Debug.LogError("DungeonGameManagerにDungeonBuilderが設定されていません。");
            return;
        }

        if (playerController == null)
        {
            Debug.LogError("DungeonGameManagerにGridPlayerControllerが設定されていません。");
            return;
        }

        // 1. 文字列マップからダンジョンを生成する。
        dungeonBuilder.BuildDungeon();

        // 2. スタート地点にプレイヤーを配置する。
        playerController.Initialize(
            dungeonBuilder,
            this,
            dungeonBuilder.StartGridPosition,
            playerStartDirection
        );

        // 3. 操作説明を表示する。
        if (uiManager != null)
        {
            uiManager.ShowMessage(startMessage);
        }
    }

    /// <summary>
    /// プレイヤーがゴールマスに到達したときに呼ばれる。
    /// </summary>
    public void OnPlayerReachedGoal()
    {
        if (IsGameCleared)
        {
            return;
        }

        IsGameCleared = true;

        if (playerController != null)
        {
            playerController.LockControl();
        }

        if (uiManager != null)
        {
            uiManager.ShowMessage(goalMessage);
        }

        Debug.Log(goalMessage);
    }

    private void ResolveReferences()
    {
        if (dungeonBuilder == null)
        {
            dungeonBuilder = FindFirstObjectByType<DungeonBuilder>();
        }

        if (playerController == null)
        {
            playerController = FindFirstObjectByType<GridPlayerController>();
        }

        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<DungeonUIManager>();
        }
    }
}