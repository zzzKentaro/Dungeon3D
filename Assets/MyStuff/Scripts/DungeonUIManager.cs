using TMPro;
using UnityEngine;

/// <summary>
/// ダンジョン探索用のUI管理クラス。
/// 
/// 今回はメッセージ表示だけを担当します。
/// 後からミニマップ、現在座標表示、向き表示などを追加できます。
/// </summary>
public class DungeonUIManager : MonoBehaviour
{
    [Header("メッセージ表示")]
    [SerializeField]
    private GameObject messageRoot;

    [SerializeField]
    private TextMeshProUGUI messageText;

    /// <summary>
    /// メッセージを表示する。
    /// </summary>
    public void ShowMessage(string message)
    {
        if (messageRoot != null)
        {
            messageRoot.SetActive(true);
        }

        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    /// <summary>
    /// メッセージを消す。
    /// </summary>
    public void HideMessage()
    {
        if (messageRoot != null)
        {
            messageRoot.SetActive(false);
        }
    }

    /// <summary>
    /// 表示中の文字だけを空にする。
    /// </summary>
    public void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }
}