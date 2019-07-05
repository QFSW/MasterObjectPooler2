using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFSW.MOP2.Editor
{
    public static class EditorHelpers
    {
        public static void DrawBanner(Texture2D banner, float sizeMultiplier = 1f)
        {
            if (banner)
            {
                sizeMultiplier = Mathf.Clamp01(sizeMultiplier);

                Rect bannerRect = GUILayoutUtility.GetRect(0.0f, 0.0f);
                bannerRect.height = Screen.width * banner.height / banner.width;
                bannerRect.x += bannerRect.width * (1 - sizeMultiplier) / 2;
                bannerRect.width *= sizeMultiplier;
                bannerRect.height *= sizeMultiplier;

                GUILayout.Space(bannerRect.height);
                GUI.Label(bannerRect, banner);
            }
        }
    }
}
