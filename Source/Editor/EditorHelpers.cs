using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFSW.MOP2.Editor
{
    public static class EditorHelpers
    {
        public static void DrawBanner(Texture2D banner)
        {
            if (banner)
            {
                Rect bannerRect = GUILayoutUtility.GetRect(0.0f, 0.0f);
                bannerRect.height = Screen.width * banner.height / banner.width;
                GUILayout.Space(bannerRect.height);
                GUI.Label(bannerRect, banner);
            }
        }
    }
}
