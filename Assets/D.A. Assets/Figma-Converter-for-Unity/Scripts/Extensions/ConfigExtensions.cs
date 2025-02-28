﻿using System;

namespace DA_Assets.FCU.Extensions
{
    public static class ConfigExtensions
    {
        public static bool UsingUnityButton(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ButtonComponent == ButtonComponent.UnityButton;

        public static bool UsingFcuButton(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ButtonComponent == ButtonComponent.FcuButton;

        public static bool UsingDaButton(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ButtonComponent == ButtonComponent.DAButton;

        public static bool UsingTrueShadow(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ShadowComponent == ShadowComponent.TrueShadow;

        public static bool UsingUnityText(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.TextComponent == TextComponent.UnityText;

        public static bool UsingTextMesh(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.TextComponent == TextComponent.TextMeshPro || fcu.Settings.ComponentSettings.TextComponent == TextComponent.RTLTextMeshPro;

        public static bool UsingSpriteRenderer(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ImageComponent == ImageComponent.SpriteRenderer;

        public static bool UsingShapes2D(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ImageComponent == ImageComponent.SubcShape;

        public static bool UsingShapes(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ImageComponent == ImageComponent.ShapesAsset;

        public static bool UsingUnityImage(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ImageComponent == ImageComponent.UnityImage;

        public static bool UsingRawImage(this FigmaConverterUnity fcu) =>
             fcu.Settings.ComponentSettings.ImageComponent == ImageComponent.RawImage;

        public static bool UsingJoshPui(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ImageComponent == ImageComponent.ProceduralImage;

        public static bool UsingDttPui(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ImageComponent == ImageComponent.RoundedImage;

        public static bool UsingMPUIKit(this FigmaConverterUnity fcu) =>
            fcu.Settings.ComponentSettings.ImageComponent == ImageComponent.MPImage;

        public static Type GetCurrentImageType(this FigmaConverterUnity fcu)
        {
            switch (fcu.Settings.ComponentSettings.ImageComponent)
            {
                case ImageComponent.UnityImage:
                    return typeof(UnityEngine.UI.Image);
                case ImageComponent.RawImage:
                    return typeof(UnityEngine.UI.RawImage);
#if SUBC_SHAPES_EXISTS
                case ImageComponent.SubcShape:
                    return typeof(Shapes2D.Shape);
#endif
#if MPUIKIT_EXISTS
                case ImageComponent.MPImage:
                    return typeof(MPUIKIT.MPImage);
#endif
#if JOSH_PUI_EXISTS
                case ImageComponent.ProceduralImage:
                    return typeof(UnityEngine.UI.ProceduralImage.ProceduralImage);
#endif
            }

            return null;
        }

        public static Type GetCurrentTextType(this FigmaConverterUnity fcu)
        {
            switch (fcu.Settings.ComponentSettings.TextComponent)
            {
                case TextComponent.UnityText:
                    return typeof(UnityEngine.UI.Text);
#if TextMeshPro
                case TextComponent.TextMeshPro:
                    return typeof(TMPro.TextMeshProUGUI);
#endif
            }

            return null;
        }
    }
}