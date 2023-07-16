using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// SharedScene
/// </summary>
public class SharedScene : MonoBehaviour
{
#if UNITY_EDITOR
    /// <summary>
    /// SharedScene経由後に開くシーン名のEditorPrefsキー
    /// </summary>
    private const string NEXT_SCENENAME_KEY = "SharedScene.NextSceneName";

    /// <summary>
    /// 再生時開始シーンをセットする
    /// </summary>
    /// <para>
    /// エディタ起動時やスクリプトコンパイル時に呼ばれる
    /// </para>
    [InitializeOnLoadMethod]
    private static void SetPlayModeStartScene()
    {
        //エディタ上でシーン開いたときのイベントをセット
        EditorSceneManager.activeSceneChangedInEditMode += (closedScene, openedScene) =>
        {
            EditorSceneManager.playModeStartScene = null;

            //指定ディレクトリ外のシーンの場合は再生時にSharedSceneを経由したくないのでスルーする
            if (!openedScene.path.Contains("Assets/RAS/Scenes")) return;

            //再生時開始シーンをSharedSceneに設定
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/RAS/Scenes/Shared.unity");

            //SharedScene経由後に開くシーンとして、今開いたシーンの名前を保存
            EditorPrefs.SetString(NEXT_SCENENAME_KEY, openedScene.name);

            Debug.LogFormat("SetPlayModeStartScene: {0}", openedScene.name);
        };
    }
#endif

    /// <summary>
    /// インスタンス
    /// </summary>
    public static SharedScene instance { get; private set; }
    
    #region [Scene Change Animation]
    /// <summary>
    /// シーン遷移時アニメ生成先
    /// </summary>
    [Header("Scene Change Animation")]
    [SerializeField]
    private RectTransform sceneChangeAnimationRoot = null;
    /// <summary>
    /// シーン遷移アニメーションプレハブ
    /// </summary>
    [SerializeField]
    private SceneChangeAnimation sceneChangeAnimationPrefab = null;
    /// <summary>
    /// シーン遷移アニメーション
    /// </summary>
    private SceneChangeAnimation sceneChangeAnimation = null;
    #endregion

    #region [ConnectingIndicator]
    /// <summary>
    /// 通信中アニメ生成先
    /// </summary>
    [Header("Connecting Indicator")]
    [SerializeField]
    private RectTransform connectingRoot = null;
    /// <summary>
    /// 通信中アニメプレハブ
    /// </summary>
    [SerializeField]
    private ConnectingIndicator connectingIndicatorPrefab = null;
    /// <summary>
    /// 通信中アニメ
    /// </summary>
    private ConnectingIndicator connectingIndicator = null;
    #endregion
    
    
    /// <summary>
    /// ビルドデータ
    /// </summary>
    [SerializeField]
    public BuildData buildData = null;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        instance = this;

        //SharedSceneは破棄されないようにする
        DontDestroyOnLoad(this.gameObject);

        //シーン遷移アニメーションイベント登録
        SceneChanger.onShowSceneChangeAnimation = this.ShowSceneChangeAnimation;
        SceneChanger.onHideSceneChangeAnimation = this.HideSceneChangeAnimation;

#if UNITY_EDITOR
        if (EditorPrefs.HasKey(NEXT_SCENENAME_KEY))
        {
            //SharedSceneに来る前のシーンを開く（前のシーンもSharedSceneだったらスルー）
            string nextSceneName = EditorPrefs.GetString(NEXT_SCENENAME_KEY);
            if (nextSceneName != EditorSceneManager.GetActiveScene().name)
            {
                SceneChanger.ChangeSceneAsync(nextSceneName);
                return;
            }
        }
#endif

        //SharedSceneの次のシーンはTitleに遷移
        SceneChanger.ChangeSceneAsync("Title");
    }

    /// <summary>
    /// 通信中マークを表示（タッチブロックされる）
    /// </summary>
    public void ShowConnectingIndicator()
    {
        if (this.connectingIndicator == null)
        {
            this.connectingIndicator = Instantiate(this.connectingIndicatorPrefab, this.connectingRoot, false);
        }

        this.connectingIndicator.Play();
    }

    /// <summary>
    /// 通信中マークを非表示
    /// </summary>
    public void HideConnectingIndicator()
    {
        if (this.connectingIndicator != null)
        {
            this.connectingIndicator.Destroy();
        }
    }

    /// <summary>
    /// シーン遷移アニメーション表示
    /// </summary>
    private void ShowSceneChangeAnimation(Action onFinishedIn)
    {
        if (this.sceneChangeAnimation == null)
        {
            this.sceneChangeAnimation = Instantiate(this.sceneChangeAnimationPrefab, this.sceneChangeAnimationRoot, false);
            this.sceneChangeAnimation.onFinishedIn = onFinishedIn;
        }
        else
        {
            onFinishedIn?.Invoke();
        }
    }

    /// <summary>
    /// シーン遷移アニメーション非表示
    /// </summary>
    private void HideSceneChangeAnimation(Action onFinished = null)
    {
        if (this.sceneChangeAnimation != null)
        {
            this.sceneChangeAnimation.SetOut();
            this.sceneChangeAnimation.onFinishedOut = () =>
            {
                Destroy(this.sceneChangeAnimation.gameObject);
                this.sceneChangeAnimation = null;
                onFinished?.Invoke();
            };
        }
    }
    
    /// <summary>
    /// GUIログ表示
    /// </summary>
    private GUILogViewer guiLogViewer = null;

#if DEBUG
    /// <summary>
    /// OnGUI
    /// </summary>
    private void OnGUI()
    {
        this.guiLogViewer?.DrawGUI();
        SceneChanger.currentScene?.DrawGUI();
    }
#endif
}
