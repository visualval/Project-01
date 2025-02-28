﻿using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using UnityEngine;
using DA_Assets.FCU.Extensions;


#if UITK_LINKER_EXISTS
using DA_Assets.UEL;
#endif

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class MainSettings : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] UIFramework uiFramework = UIFramework.UGUI;
        public UIFramework UIFramework
        {
            get => uiFramework;
            set
            {
                switch (value)
                {
                    case UIFramework.UITK:
#if FCU_UITK_EXT_EXISTS == false
                        DALogger.LogError(FcuLocKey.log_asset_not_imported.Localize(nameof(UIFramework.UITK)));
                        uiFramework = UIFramework.UGUI;
                        return;
#endif
                        break;

                    case UIFramework.NOVA:
#if NOVA_UI_EXISTS == false
                        DALogger.LogError(FcuLocKey.log_asset_not_imported.Localize(nameof(UIFramework.NOVA)));
                        uiFramework = UIFramework.UGUI;
                        return;
#endif
                        break;
                }

                SetValue(ref uiFramework, value);
            }
        }

#if UITK_LINKER_EXISTS
        [SerializeField] UitkLinkingMode uitkLinkingMode = UitkLinkingMode.IndexNames;
        public UitkLinkingMode UitkLinkingMode
        {
            get => uitkLinkingMode;
            set
            {
                if (value != uitkLinkingMode)
                {
                    switch (value)
                    {
                        case UitkLinkingMode.Name:
                            DALogger.LogError(FcuLocKey.log_name_linking_not_recommended.Localize(FcuLocKey.label_uitk_linking_mode.Localize(), nameof(UitkLinkingMode.Name)));
                            break;
                    }
                }

                SetValue(ref uitkLinkingMode, value);
            }
        }

#endif

        [SerializeField] PositioningMode positioningMode = PositioningMode.Absolute;
        public PositioningMode PositioningMode { get => positioningMode; set => SetValue(ref positioningMode, value); }

        [SerializeField] PivotType pivotType = PivotType.MiddleCenter;
        public PivotType PivotType { get => pivotType; set => SetValue(ref pivotType, value); }

        [SerializeField] ImageFormat imageFormat = ImageFormat.PNG;
        public ImageFormat ImageFormat { get => imageFormat; set => SetValue(ref imageFormat, value); }

        [SerializeField] float imageScale = 4.0f;
        public float ImageScale { get => imageScale; set => SetValue(ref imageScale, value); }

        [SerializeField] PreserveRatioMode preserveRatioMode = PreserveRatioMode.None;
        public PreserveRatioMode PreserveRatioMode { get => preserveRatioMode; set => SetValue(ref preserveRatioMode, value); }

        [SerializeField] float pixelsPerUnit = 100;
        public float PixelsPerUnit { get => pixelsPerUnit; set => SetValue(ref pixelsPerUnit, value); }

        [SerializeField] int goLayer = 5;
        public int GameObjectLayer { get => goLayer; set => SetValue(ref goLayer, value); }

        [SerializeField] string spritesPath = "Assets\\Sprites";
        public string SpritesPath { get => spritesPath; set => SetValue(ref spritesPath, value); }

        [SerializeField] string uitkOutputPath = "Assets\\UITK Output";
        public string UitkOutputPath { get => uitkOutputPath; set => SetValue(ref uitkOutputPath, value); }

        [SerializeField] bool redownloadSprites = false;
        public bool RedownloadSprites { get => redownloadSprites; set => SetValue(ref redownloadSprites, value); }

        [SerializeField] bool rawImport = false;
        public bool RawImport
        {
            get => rawImport;
            set
            {
                if (value && value != rawImport)
                {
                    DALogger.LogError(FcuLocKey.log_dev_function_enabled.Localize(FcuLocKey.label_raw_import.Localize()));
                }

                SetValue(ref rawImport, value);
            }
        }

        [SerializeField] bool windowMode = false;
        public bool WindowMode { get => windowMode; set => SetValue(ref windowMode, value); }

        [SerializeField] string projectUrl;
        public string ProjectUrl
        {
            get => projectUrl;
            set
            {
                string _value = value;

                try
                {
                    string fileTag = "/file/";
                    char del = '/';

                    if (_value.IsEmpty())
                    {

                    }
                    else if (_value.Contains(fileTag))
                    {
                        _value = _value.GetBetween(fileTag, del.ToString());
                    }
                    else if (_value.Contains(del.ToString()))
                    {
                        string[] splited = value.Split(del);
                        _value = splited[4];
                    }
                }
                catch
                {
                    Debug.LogError(FcuLocKey.log_incorrent_project_url.Localize());
                }

                SetValue(ref projectUrl, _value);
            }
        }

        [SerializeField] string[] componentsUrls = new string[5];
        public string[] ComponentsUrls
        {
            get
            {
                if (componentsUrls.IsEmpty())
                {
                    componentsUrls = new string[5];
                }

                return componentsUrls;
            }
            set
            {
                SetValue(ref componentsUrls, value);
            }
        }
    }
}
