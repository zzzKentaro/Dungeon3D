using System.Collections;
using UnityEngine;

/// <summary>
/// グリッド式一人称プレイヤー操作。
/// 
/// 自由移動ではなく、
/// ・1マス前進
/// ・1マス後退
/// ・左右に平行移動
/// ・左右に90度回転
/// のみを行います。
/// </summary>
public class GridPlayerController : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("プレイヤー視点の高さ。Camera自体を動かす場合は1.6程度がおすすめです。")]
    [SerializeField]
    private float eyeHeight = 1.6f;

    [Tooltip("1マス移動にかかる時間。")]
    [SerializeField]
    private float moveDuration = 0.18f;

    [Tooltip("90度回転にかかる時間。")]
    [SerializeField]
    private float rotateDuration = 0.15f;

    [Header("参照")]
    [SerializeField]
    private DungeonBuilder dungeonBuilder;

    [SerializeField]
    private DungeonGameManager gameManager;

    public Vector2Int CurrentGridPosition { get; private set; }
    public GridDirection CurrentDirection { get; private set; }

    private bool isInitialized;
    private bool isBusy;
    private bool isControlLocked;

    /// <summary>
    /// GameManagerから呼ばれる初期化処理。
    /// </summary>
    public void Initialize(
        DungeonBuilder builder,
        DungeonGameManager manager,
        Vector2Int startGridPosition,
        GridDirection startDirection
    )
    {
        dungeonBuilder = builder;
        gameManager = manager;

        CurrentGridPosition = startGridPosition;
        CurrentDirection = startDirection;

        transform.position = GetPlayerWorldPosition(CurrentGridPosition);
        transform.rotation = GridDirectionUtility.ToRotation(CurrentDirection);

        isBusy = false;
        isControlLocked = false;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized)
        {
            return;
        }

        if (isBusy)
        {
            return;
        }

        if (isControlLocked)
        {
            return;
        }

        // W：前進
        if (Input.GetKeyDown(KeyCode.W))
        {
            TryMove(GridMoveCommand.Forward);
        }
        // S：後退
        else if (Input.GetKeyDown(KeyCode.S))
        {
            TryMove(GridMoveCommand.Backward);
        }
        // A：左に平行移動
        else if (Input.GetKeyDown(KeyCode.A))
        {
            TryMove(GridMoveCommand.Left);
        }
        // D：右に平行移動
        else if (Input.GetKeyDown(KeyCode.D))
        {
            TryMove(GridMoveCommand.Right);
        }
        // Q：左に90度回転
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            TryRotateLeft();
        }
        // E：右に90度回転
        else if (Input.GetKeyDown(KeyCode.E))
        {
            TryRotateRight();
        }
    }

    /// <summary>
    /// 操作を完全に止める。
    /// ゴール後などに使います。
    /// </summary>
    public void LockControl()
    {
        isControlLocked = true;
    }

    /// <summary>
    /// 操作停止を解除する。
    /// 今回の初期版では基本的には使いません。
    /// </summary>
    public void UnlockControl()
    {
        isControlLocked = false;
    }

    private void TryMove(GridMoveCommand moveCommand)
    {
        if (dungeonBuilder == null)
        {
            Debug.LogError("GridPlayerControllerにDungeonBuilderが設定されていません。");
            return;
        }

        Vector2Int moveOffset = GridDirectionUtility.GetMoveOffset(CurrentDirection, moveCommand);
        Vector2Int nextGridPosition = CurrentGridPosition + moveOffset;

        if (!dungeonBuilder.IsWalkable(nextGridPosition))
        {
            // 壁やマップ外なら移動しない。
            Debug.Log("その方向には進めません。");
            return;
        }

        StartCoroutine(MoveRoutine(nextGridPosition));
    }

    private void TryRotateLeft()
    {
        GridDirection nextDirection = GridDirectionUtility.TurnLeft(CurrentDirection);
        StartCoroutine(RotateRoutine(nextDirection));
    }

    private void TryRotateRight()
    {
        GridDirection nextDirection = GridDirectionUtility.TurnRight(CurrentDirection);
        StartCoroutine(RotateRoutine(nextDirection));
    }

    private IEnumerator MoveRoutine(Vector2Int nextGridPosition)
    {
        isBusy = true;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = GetPlayerWorldPosition(nextGridPosition);

        float timer = 0f;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;

            float t = timer / moveDuration;
            t = Mathf.Clamp01(t);

            // 少しだけ滑らかにする。
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);

            yield return null;
        }

        transform.position = endPosition;
        CurrentGridPosition = nextGridPosition;

        isBusy = false;

        if (dungeonBuilder.IsGoal(CurrentGridPosition))
        {
            if (gameManager != null)
            {
                gameManager.OnPlayerReachedGoal();
            }
        }
    }

    private IEnumerator RotateRoutine(GridDirection nextDirection)
    {
        isBusy = true;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = GridDirectionUtility.ToRotation(nextDirection);

        float timer = 0f;

        while (timer < rotateDuration)
        {
            timer += Time.deltaTime;

            float t = timer / rotateDuration;
            t = Mathf.Clamp01(t);

            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, smoothT);

            yield return null;
        }

        transform.rotation = endRotation;
        CurrentDirection = nextDirection;

        isBusy = false;
    }

    private Vector3 GetPlayerWorldPosition(Vector2Int gridPosition)
    {
        return dungeonBuilder.GridToWorldPosition(gridPosition, eyeHeight);
    }
}