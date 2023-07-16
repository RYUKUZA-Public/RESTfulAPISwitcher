using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// WebRequest
/// </summary>
public class RASWebRequest<T>
{
    /// <summary>
    /// 共通レスポンスデータ
    /// </summary>
    public class ResponseData
    {
        public string date;
        public string version;
        public string resourceVersion;
        public int errorCode;
        public T data;
    }

    /// <summary>
    /// パス
    /// </summary>
    private string path = null;
    /// <summary>
    /// URL
    /// </summary>
    private string url = null;
    /// <summary>
    /// ヘッダー
    /// </summary>
    private List<(string name, string value)> header = new List<(string name, string value)>();
    /// <summary>
    /// リクエストJson
    /// </summary>
    private string requestJson = "";
    /// <summary>
    /// リソースバージョン
    /// </summary>
    public string resourceVersion { get; protected set; }
    /// <summary>
    /// HTTPコンテンツ
    /// </summary>
    private StringContent content = null;
    /// <summary>
    /// スレッドを元に戻すため
    /// </summary>
    private SynchronizationContext mainThread = null;
    /// <summary>
    /// 成功時コールバック
    /// </summary>
    public Action<T> onSuccess = null;
    /// <summary>
    /// エラー時コールバック
    /// </summary>
    public Action<int> onError = null;

    /// <summary>
    /// construct
    /// </summary>
    public RASWebRequest(string path)
    {
        this.path = path;
        this.url = GetURL(this.path);
        this.SetRequestHeader("Content-Type", "application/json");
    }

    /// <summary>
    /// URL取得
    /// </summary>
    protected static string GetURL(string path)
    {
        //TODO: 環境別に変更出来るようにする
        return "http://testapiserver.com/api/" + path;
    }

    /// <summary>
    /// リクエストヘッダーをセットする
    /// </summary>
    public void SetRequestHeader(string name, string value)
    {
        this.header.Add((name, value));
    }

    /// <summary>
    /// リクエストパラメータをセットする
    /// </summary>
    public void SetRequestParameter(object requestParameter)
    {
        if (requestParameter != null)
        {
            this.requestJson = JsonConvert.SerializeObject(requestParameter);
        }
    }

    /// <summary>
    /// 送信
    /// </summary>
    public void Send()
    {
        Debug.LogFormat("Send URL:{0}\nRequest:{1}",
            this.url,
            this.requestJson
        );

        //通信中ローディング表示
        this.ShowLoading();

#if RAS_OFFLINE
        // オフライン時処理
        Resources.LoadAsync<OfflineResponseData>(string.Format("ScriptableObject/OfflineResponceData/{0}", path)).completed += (op) =>
        {
            var resourceRequest = op as ResourceRequest;
            var asset = resourceRequest.asset as OfflineResponseData;
            var data = JsonConvert.DeserializeObject<T>(asset.data);

            Debug.LogFormat("Success URL:{0}\nResponse:{1}",
                this.url,
                asset.data
            );

            this.onSuccess?.Invoke(data);

            // 通信中ローディング非表示
            this.HideLoading();
        };
#else
        // リクエストJsonセット
        this.content = new StringContent(this.requestJson, Encoding.UTF8, "application/json");
        this.content.Headers.Clear();

        // ヘッダーセット
        for (int i = 0; i < this.header.Count; i++)
        {
            this.content.Headers.Add(this.header[i].name, this.header[i].value);
        }

        // 通信開始
        this.mainThread = SynchronizationContext.Current;
        TaskUpdator.Run(async () =>
        {
            try
            {
                //タイムアウト設定
                RASHttpClient.instance.Timeout = TimeSpan.FromSeconds(30);
                //通信開始
                var response = await RASHttpClient.instance.PostAsync(this.url, this.content);
                //通信完了
                this.mainThread.Post(x => this.OnCompleted(response), null);
            }
            catch (Exception e)
            {
                //エラー
                this.mainThread.Post(x => this.OnCompleted(new HttpResponseMessage{
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = e.Message,
                }), null);
            }
        });
#endif
    }

    /// <summary>
    /// 通信完了時
    /// </summary>
    private void OnCompleted(HttpResponseMessage response)
    {
        // システムエラー
        if (response.StatusCode != HttpStatusCode.OK)
        {
            // 通信中ローディング非表示
            this.HideLoading();

            // リトライさせるしかない
            Debug.LogErrorFormat("Error URL:{0}\nError:{1}\nReason:{2}",
                this.url,
                response.StatusCode,
                response.ReasonPhrase
            );
            this.OnSystemError();
            return;
        }

        this.mainThread = SynchronizationContext.Current;
        TaskUpdator.Run(async () =>
        {
            // Json受け取り
            var json = await response.Content.ReadAsStringAsync();
            // 通信中ローディング非表示
            this.HideLoading();
            // Json受け取り完了
            this.mainThread.Post(x => this.OnReceivedJson(json), null);
        });
    }

    /// <summary>
    /// Json受け取り完了時
    /// </summary>
    protected void OnReceivedJson(string json)
    {
        // レスポンスJsonを共通レスポンスデータに変換
        ResponseData response = null;
        try { response = JsonConvert.DeserializeObject<ResponseData>(json); }
        catch { }

        // レスポンスエラー
        if (response == null || response.errorCode > 0)
        {
            Debug.Log("response" + response);
            Debug.Log("response code" + response.errorCode);
            
            //TODO:エラーコードで処理を分岐させる必要がある
            //・タイトル戻す（システム）
            //・リトライさせる（システム）
            //・アイテムリスト再取得させる、商品リスト再取得させる等（クライアント）
            Debug.LogErrorFormat("Error URL:{0}\nResponse:{1}",
                GetURL(this.path),
                json
            );

            if (this.onError != null)
            {
                this.onError(response == null ? 0 : response.errorCode);
            }
            else
            {
                // エラーコールバックが仕込まれていなかったらシステム的に処理する
                this.OnSystemError();
            }
            return;
        }

        Debug.Log($"response.version : {response.version}");
        Debug.Log($"Application.version : {Application.version}");

#if !UNITY_EDITOR
        //アプリのバージョンが変わった
        if (response.version != Application.version)
        {
            //エラーメッセージを表示してストアに誘導
            this.OnVersionError();
            return;
        }
#endif

        Debug.LogFormat("Success URL:{0}\nResponse:{1}",
            GetURL(this.path),
            json
        );

        // 完了通知
        this.resourceVersion = response.resourceVersion;
        this.onSuccess?.Invoke(response.data);
    }

    /// <summary>
    /// リトライ
    /// </summary>
    private void Retry()
    {
        this.Send();
    }

    /// <summary>
    /// システムエラー時
    /// </summary>
    protected virtual void OnSystemError()
    {
        Debug.LogError($"{Masters.LocalizeTextDB.Get("ConnectErrorMessage")}");
    }

    /// <summary>
    /// アプリのバージョンが変わった時
    /// </summary>
    private void OnVersionError()
    {
        Debug.LogError($"{Masters.LocalizeTextDB.Get("BundleVersionErrorMessage")}");
    }

    /// <summary>
    /// 通信中ローディング表示
    /// </summary>
    protected virtual void ShowLoading()
    {
        SharedScene.instance.ShowConnectingIndicator();
    }

    /// <summary>
    /// 通信中ローディング非表示
    /// </summary>
    protected virtual void HideLoading()
    {
        SharedScene.instance.HideConnectingIndicator();
    }
}
