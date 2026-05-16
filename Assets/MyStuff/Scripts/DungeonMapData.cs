using UnityEngine;

/// <summary>
/// ダンジョンの文字列マップを保存するためのScriptableObject。
/// 
/// マップ記号：
/// S = スタート
/// G = ゴール
/// F = 床
/// W = 壁
/// </summary>
[CreateAssetMenu(fileName = "NewDungeonMap", menuName = "Dungeon/Grid Dungeon Map")]
public class DungeonMapData : ScriptableObject
{
    [Header("文字列マップ")]
    [Tooltip("S=スタート, G=ゴール, F=床, W=壁")]
    [TextArea(5, 30)]
    [SerializeField]
    private string mapText =
@"WWWWWWWW
WSFFFFFW
WFWWWFFW
WFFFWWGW
WWWWWWWW";

    public string MapText => mapText;
}