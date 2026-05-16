using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダンジョンの1マスの種類。
/// </summary>
public enum DungeonTileType
{
    Empty,
    Floor,
    Wall,
    Start,
    Goal
}

/// <summary>
/// 文字列マップを読み取って、3Dダンジョンを生成するクラス。
/// 
/// マップ記号：
/// S = スタート
/// G = ゴール
/// F = 床
/// W = 壁
/// 
/// 例：
/// WWWWWWWW
/// WSFFFFFW
/// WFWWWFFW
/// WFFFWWGW
/// WWWWWWWW
/// </summary>
public class DungeonBuilder : MonoBehaviour
{
    [Header("マップデータ")]
    [Tooltip("ここにDungeonMapDataを入れた場合、そのマップを使います。空なら下のMap Textを使います。")]
    [SerializeField]
    private DungeonMapData mapData;

    [Tooltip("DungeonMapDataを使わない場合、この文字列マップを使います。S=スタート, G=ゴール, F=床, W=壁")]
    [TextArea(5, 30)]
    [SerializeField]
    private string mapText =
@"WWWWWWWW
WSFFFFFW
WFWWWFFW
WFFFWWGW
WWWWWWWW";

    [Header("Prefab")]
    [SerializeField]
    private GameObject floorPrefab;

    [SerializeField]
    private GameObject wallPrefab;

    [SerializeField]
    private GameObject goalPrefab;

    [Header("生成設定")]
    [Tooltip("1マスの大きさ。Prefab側のサイズと合わせてください。")]
    [SerializeField]
    private float cellSize = 4f;

    [Tooltip("生成した床・壁・ゴールをまとめる親Transform。空なら自動で作ります。")]
    [SerializeField]
    private Transform dungeonRoot;

    [Header("Prefabの位置調整")]
    [Tooltip("床Prefabの生成位置補正。")]
    [SerializeField]
    private Vector3 floorOffset = Vector3.zero;

    [Tooltip("壁Prefabの生成位置補正。Cubeを使う場合はYを少し上げると床の上に乗ります。")]
    [SerializeField]
    private Vector3 wallOffset = Vector3.zero;

    [Tooltip("ゴールPrefabの生成位置補正。床にめり込む場合はYを少し上げてください。")]
    [SerializeField]
    private Vector3 goalOffset = Vector3.zero;

    private readonly Dictionary<Vector2Int, DungeonTileType> tileMap = new Dictionary<Vector2Int, DungeonTileType>();

    public Vector2Int StartGridPosition { get; private set; }

    public float CellSize => cellSize;

    /// <summary>
    /// Inspectorの右クリックメニュー、または歯車メニューから実行できます。
    /// 再生前に見た目確認をしたいとき用です。
    /// </summary>
    [ContextMenu("Build Dungeon")]
    public void BuildDungeon()
    {
        EnsureDungeonRoot();
        ClearGeneratedObjects();

        tileMap.Clear();

        List<string> rows = SplitMapText(GetActiveMapText());

        if (rows.Count == 0)
        {
            Debug.LogWarning("マップが空です。DungeonBuilderのMap Textを確認してください。");
            return;
        }

        bool foundStart = false;
        int height = rows.Count;

        for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            string row = rows[rowIndex];

            for (int x = 0; x < row.Length; x++)
            {
                char symbol = row[x];

                // 文字列マップの上の行を、Unity上ではZプラス側に置く。
                // これにより、上方向＝Northとして扱いやすくする。
                int z = height - 1 - rowIndex;

                Vector2Int gridPosition = new Vector2Int(x, z);
                DungeonTileType tileType = CharToTileType(symbol);

                if (tileType == DungeonTileType.Empty)
                {
                    continue;
                }

                tileMap[gridPosition] = tileType;

                if (tileType == DungeonTileType.Wall)
                {
                    SpawnPrefab(wallPrefab, gridPosition, wallOffset, "Wall");
                }
                else
                {
                    // Floor、Start、Goalは歩けるマスなので床を置く。
                    SpawnPrefab(floorPrefab, gridPosition, floorOffset, "Floor");

                    if (tileType == DungeonTileType.Start)
                    {
                        StartGridPosition = gridPosition;
                        foundStart = true;
                    }
                    else if (tileType == DungeonTileType.Goal)
                    {
                        SpawnPrefab(goalPrefab, gridPosition, goalOffset, "Goal");
                    }
                }
            }
        }

        if (!foundStart)
        {
            Debug.LogWarning("マップ内にスタート地点 S がありません。最初に見つかった歩けるマスをスタートにします。");
            StartGridPosition = FindFirstWalkablePosition();
        }
    }

    /// <summary>
    /// 指定したグリッド座標が歩けるかどうか。
    /// </summary>
    public bool IsWalkable(Vector2Int gridPosition)
    {
        DungeonTileType tileType = GetTileType(gridPosition);

        return tileType == DungeonTileType.Floor
            || tileType == DungeonTileType.Start
            || tileType == DungeonTileType.Goal;
    }

    /// <summary>
    /// 指定したグリッド座標がゴールかどうか。
    /// </summary>
    public bool IsGoal(Vector2Int gridPosition)
    {
        return GetTileType(gridPosition) == DungeonTileType.Goal;
    }

    /// <summary>
    /// 指定したグリッド座標のマス種別を返す。
    /// マップ外や未登録の場所はEmptyとして扱います。
    /// </summary>
    public DungeonTileType GetTileType(Vector2Int gridPosition)
    {
        if (tileMap.TryGetValue(gridPosition, out DungeonTileType tileType))
        {
            return tileType;
        }

        return DungeonTileType.Empty;
    }

    /// <summary>
    /// グリッド座標をUnity上のワールド座標に変換する。
    /// </summary>
    public Vector3 GridToWorldPosition(Vector2Int gridPosition, float y = 0f)
    {
        Vector3 localPosition = new Vector3(
            gridPosition.x * cellSize,
            y,
            gridPosition.y * cellSize
        );

        return transform.position + localPosition;
    }

    private string GetActiveMapText()
    {
        if (mapData != null)
        {
            return mapData.MapText;
        }

        return mapText;
    }

    /// <summary>
    /// 文字をタイル種別に変換する。
    /// 
    /// S = スタート
    /// G = ゴール
    /// F = 床
    /// W = 壁
    /// </summary>
    private DungeonTileType CharToTileType(char symbol)
    {
        switch (symbol)
        {
            case 'S':
            case 's':
                return DungeonTileType.Start;

            case 'G':
            case 'g':
                return DungeonTileType.Goal;

            case 'F':
            case 'f':
                return DungeonTileType.Floor;

            case 'W':
            case 'w':
                return DungeonTileType.Wall;

            default:
                return DungeonTileType.Empty;
        }
    }

    private List<string> SplitMapText(string text)
    {
        List<string> rows = new List<string>();

        if (string.IsNullOrEmpty(text))
        {
            return rows;
        }

        text = text.Replace("\r\n", "\n").Replace('\r', '\n');

        string[] rawRows = text.Split('\n');

        foreach (string row in rawRows)
        {
            // 完全な空行は無視する。
            // 行の中にある空白はEmpty扱いになります。
            if (row.Length == 0)
            {
                continue;
            }

            rows.Add(row);
        }

        return rows;
    }

    private void EnsureDungeonRoot()
    {
        if (dungeonRoot != null)
        {
            return;
        }

        GameObject rootObject = new GameObject("Generated Dungeon");
        rootObject.transform.SetParent(transform);
        rootObject.transform.localPosition = Vector3.zero;
        rootObject.transform.localRotation = Quaternion.identity;
        rootObject.transform.localScale = Vector3.one;

        dungeonRoot = rootObject.transform;
    }

    private void ClearGeneratedObjects()
    {
        if (dungeonRoot == null)
        {
            return;
        }

        for (int i = dungeonRoot.childCount - 1; i >= 0; i--)
        {
            GameObject child = dungeonRoot.GetChild(i).gameObject;

            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }
    }

    private GameObject SpawnPrefab(GameObject prefab, Vector2Int gridPosition, Vector3 offset, string label)
    {
        if (prefab == null)
        {
            return null;
        }

        Vector3 position = GridToWorldPosition(gridPosition) + offset;

        GameObject instance = Instantiate(
            prefab,
            position,
            Quaternion.identity,
            dungeonRoot
        );

        instance.name = $"{label}_{gridPosition.x}_{gridPosition.y}";

        return instance;
    }

    private Vector2Int FindFirstWalkablePosition()
    {
        foreach (KeyValuePair<Vector2Int, DungeonTileType> pair in tileMap)
        {
            DungeonTileType tileType = pair.Value;

            if (tileType == DungeonTileType.Floor || tileType == DungeonTileType.Goal)
            {
                return pair.Key;
            }
        }

        return Vector2Int.zero;
    }
}