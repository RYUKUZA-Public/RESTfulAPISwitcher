using UnityEngine;

public static class UIUtility
{
    public static GUIStyle CreateButtonStyle(float width, int fontSize)
    {
        float w = width / RASDefine.SCREEN_WIDTH;
        float h = (float)fontSize / RASDefine.SCREEN_HEIGHT;
#if UNITY_EDITOR
        var res = UnityEditor.UnityStats.screenRes.Split('x');
        width = int.Parse(res[0]) * w;
        fontSize = (int)(int.Parse(res[1]) * h);
#else
        width = Screen.width * w;
        fontSize = (int)(Screen.height * h);
#endif
        var style = new GUIStyle(GUI.skin.button);
        style.fixedWidth = width;
        style.stretchWidth = false;
        style.fontSize = fontSize;

        return style;
    }
}
