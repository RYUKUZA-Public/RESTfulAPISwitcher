using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : SceneBase
{
    protected override void Awake()
    {
        base.Awake();

        UserData.Set(new UserData());
        UserData.Get().Load();
    }

    public void Start()
    {
    }

    public void OnClickButton()
    {
        //TODO.
        this.Login();
    }
    
    private void Login()
    {
        var queue = new Queue<Action>();

        //ユーザーデータがある
        if (UserData.Get().userId > 0)
        {
            //ログイン
            queue.Enqueue(() => LoginApi.CallLoginApi(
                UserData.Get(),
                queue.Dequeue()
            ));
        }
        //ユーザーデータがない
        else
        {
            //ユーザーデータ作成
            queue.Enqueue(() => UserApi.CallCreateApi(
                queue.Dequeue()
            ));
        }

        //ユーザー情報取得
        queue.Enqueue(() => FirstApi.CallFirstUserApi(
            UserData.Get(),
            queue.Dequeue()
        ));

        //マスター分割取得その１
        queue.Enqueue(() => MasterApi.CallGetMasterApi(
            queue.Dequeue(),
            Masters.LocalizeTextDB,
            Masters.SymbolBgDB,
            Masters.SymbolFrameDB,
            Masters.SymbolDB
        ));
        
        //HOMEシーンへ
        queue.Enqueue(() =>
            SceneChanger.ChangeSceneAsync("Home")
        );

        //Queue実行
        queue.Dequeue().Invoke();
    }


    #if DEBUG
    /// <summary>
    /// GUIボタンスタイル
    /// </summary>
    private GUIStyle buttonStyle = null;

    /// <summary>
    /// GUI描画
    /// </summary>
    public override void DrawGUI()
    {
        if (this.buttonStyle == null)
        {
            this.buttonStyle = UIUtility.CreateButtonStyle(600, 50);
        }

        GUILayout.Button("BuildNo." + SharedScene.instance.buildData.buildNumber, this.buttonStyle);

        var userData = UserData.Get();
        int userId = userData == null ? 0 : userData.userId;
        GUILayout.Button("UserID:" + userId, this.buttonStyle);

        if (GUILayout.Button("Delete User Data", this.buttonStyle))
        {
            PlayerPrefs.DeleteAll();
            UserData.Set(new UserData());
        }
    }
#endif
}
