using UnityEngine;

/// <summary>
/// Home Scene
/// </summary>
public class HomeScene : SceneBase
{
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        var asd = Masters.SymbolDB.FindById(1).frameId;
        Debug.Log($"{asd}");
        Debug.Log($"{UserData.Get().name}");
    }
}
