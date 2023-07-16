using UnityEngine;

/// <summary>
/// シーン基底
/// </summary>
public class SceneBase : MonoBehaviour
{
    
    /// <summary>
    /// Awake
    /// </summary>
    protected virtual void Awake()
    {
        //TODO:ヘッダーの表示処理とか
        //マルチタッチを許可しない
        Input.multiTouchEnabled = false;
    }
    
    /// <summary>
    /// ①Awake ②OnSceneLoaded ③Startの順で呼ばれます
    /// </summary>
    public virtual void OnSceneLoaded(SceneDataPackBase dataPack)
    {
    }

#if DEBUG
    /// <summary>
    /// GUI描画
    /// </summary>
    public virtual void DrawGUI()
    {
    }
#endif
}
