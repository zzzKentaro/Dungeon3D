using UnityEngine;

/// <summary>
/// プレイヤーが向く方向。
/// 
/// Unity上では、
/// North = Zプラス方向
/// East  = Xプラス方向
/// South = Zマイナス方向
/// West  = Xマイナス方向
/// として扱います。
/// </summary>
public enum GridDirection
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}

/// <summary>
/// プレイヤーが行う移動の種類。
/// 
/// Forward  : 前進
/// Backward : 後退
/// Left     : 左へ平行移動
/// Right    : 右へ平行移動
/// </summary>
public enum GridMoveCommand
{
    Forward,
    Backward,
    Left,
    Right
}

/// <summary>
/// GridDirectionに関する便利関数をまとめたクラス。
/// 
/// static classなので、シーン上のGameObjectにアタッチする必要はありません。
/// </summary>
public static class GridDirectionUtility
{
    /// <summary>
    /// 向きをグリッド上の移動量に変換する。
    /// </summary>
    public static Vector2Int ToVector2Int(GridDirection direction)
    {
        switch (direction)
        {
            case GridDirection.North:
                return new Vector2Int(0, 1);

            case GridDirection.East:
                return new Vector2Int(1, 0);

            case GridDirection.South:
                return new Vector2Int(0, -1);

            case GridDirection.West:
                return new Vector2Int(-1, 0);

            default:
                return Vector2Int.zero;
        }
    }

    /// <summary>
    /// 向きをUnity上のY軸回転角度に変換する。
    /// 
    /// Cameraの正面方向はZプラス方向なので、
    /// NorthはY回転0度にしています。
    /// </summary>
    public static float ToYAngle(GridDirection direction)
    {
        switch (direction)
        {
            case GridDirection.North:
                return 0f;

            case GridDirection.East:
                return 90f;

            case GridDirection.South:
                return 180f;

            case GridDirection.West:
                return 270f;

            default:
                return 0f;
        }
    }

    /// <summary>
    /// 向きをUnityのQuaternionに変換する。
    /// </summary>
    public static Quaternion ToRotation(GridDirection direction)
    {
        return Quaternion.Euler(0f, ToYAngle(direction), 0f);
    }

    /// <summary>
    /// 左に90度回転した後の向きを返す。
    /// </summary>
    public static GridDirection TurnLeft(GridDirection direction)
    {
        int value = (int)direction;
        value = (value + 3) % 4;
        return (GridDirection)value;
    }

    /// <summary>
    /// 右に90度回転した後の向きを返す。
    /// </summary>
    public static GridDirection TurnRight(GridDirection direction)
    {
        int value = (int)direction;
        value = (value + 1) % 4;
        return (GridDirection)value;
    }

    /// <summary>
    /// 後ろを向いた方向を返す。
    /// </summary>
    public static GridDirection TurnBack(GridDirection direction)
    {
        int value = (int)direction;
        value = (value + 2) % 4;
        return (GridDirection)value;
    }

    /// <summary>
    /// 現在向いている方向と移動コマンドから、
    /// 実際に移動するグリッド上の方向を求める。
    /// 
    /// 例：
    /// Northを向いているとき
    /// Forward  -> Zプラス
    /// Backward -> Zマイナス
    /// Left     -> Xマイナス
    /// Right    -> Xプラス
    /// </summary>
    public static Vector2Int GetMoveOffset(GridDirection facingDirection, GridMoveCommand moveCommand)
    {
        switch (moveCommand)
        {
            case GridMoveCommand.Forward:
                return ToVector2Int(facingDirection);

            case GridMoveCommand.Backward:
                return -ToVector2Int(facingDirection);

            case GridMoveCommand.Left:
                return ToVector2Int(TurnLeft(facingDirection));

            case GridMoveCommand.Right:
                return ToVector2Int(TurnRight(facingDirection));

            default:
                return Vector2Int.zero;
        }
    }
}