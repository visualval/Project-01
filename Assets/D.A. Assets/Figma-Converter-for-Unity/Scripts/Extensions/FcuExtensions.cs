﻿using DA_Assets.FCU.Drawers;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU.Extensions
{
    public static class FcuExtensions
    {
        public static bool IsJsonNetExists(this FigmaConverterUnity fcu)
        {
#pragma warning disable CS0162
#if JSONNET_EXISTS
            return true;
#endif
            return false;
#pragma warning restore CS0162
        }

        public static IEnumerator ReEnableRectTransform(this FigmaConverterUnity fcu)
        {
            fcu.gameObject.SetActive(false);
            yield return WaitFor.Delay01();
            fcu.gameObject.SetActive(true);
        }

        public static bool TryParseSpriteName(this string spriteName, out float scale, out System.Numerics.BigInteger hash)
        {
            try
            {
                if (spriteName.IsEmpty())
                {
                    throw new Exception($"Sprite name is empty.");
                }

                char delimiter = ' ';

                string withoutEx = Path.GetFileNameWithoutExtension(spriteName);
                List<string> nameParts = withoutEx.Split(delimiter).ToList();

                if (nameParts.Count < 2)
                {
                    throw new Exception($"nameParts.Count < 2: {spriteName}");
                }

                string _hash = nameParts[nameParts.Count() - 1];
                string _scale = nameParts[nameParts.Count() - 2].Replace("x", "");

                bool scaleParsed = _scale.TryParseWithDot(out scale);
                bool hashParsed = System.Numerics.BigInteger.TryParse(_hash, out hash);

                if (scaleParsed == false)
                {
                    throw new Exception($"Cant parse scale from name: {spriteName}");
                }

                if (hashParsed == false)
                {
                    throw new Exception($"Cant parse hash from name: {spriteName}");
                }

                return true;
            }
            catch (Exception ex)
            {
                DALogger.LogException(ex);
                scale = 1;
                hash = -1;
                return false;
            }
        }

        public static bool TryParseWithDot(this string str, out float value) =>
            float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

        public static string ToDotString(this float value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public static IEnumerator WriteLog(this DARequest request, string text, string add = null)
        {
            FileInfo[] fileInfos = new DirectoryInfo(FcuConfig.LogPath).GetFiles($"*.*");

            if (fileInfos.Length >= FcuConfig.Instance.LogFilesLimit)
            {
                foreach (FileInfo file in fileInfos)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {

                    }
                }
            }

            string logFileName = $"{DateTime.Now.ToString(FcuConfig.Instance.DateTimeFormat1)}_{add}{FcuConfig.Instance.WebLogFileName}";
            string logFilePath = Path.Combine(FcuConfig.LogPath, logFileName);

            string result;

            JFResult jfr = DAFormatter.Format<string>(text);

            if (jfr.IsValid)
            {
                result = jfr.Json;
            }
            else
            {
                result = text;
            }

            result = $"{request.Query}\n{result}";

            File.WriteAllText(logFilePath, result);

            yield return null;
        }

        public static bool IsProjectEmpty(this SelectableFObject sf)
        {
            if (sf == null)
                return true;

            if (sf.Id.IsEmpty())
                return true;

            return false;
        }

        public static string GetImageFormat(this ImageFormat imageFormat) =>
            imageFormat.ToString().ToLower();

        public static bool IsNova(this FigmaConverterUnity fcu) => fcu.Settings.MainSettings.UIFramework == UIFramework.NOVA;
        public static bool IsUITK(this FigmaConverterUnity fcu) => fcu.Settings.MainSettings.UIFramework == UIFramework.UITK;
        public static bool IsUGUI(this FigmaConverterUnity fcu) => fcu.Settings.MainSettings.UIFramework == UIFramework.UGUI;

        public static bool IsDebug(this FigmaConverterUnity fcu) => fcu.Settings.DebugSettings.DebugMode;

        public static void Log(this FigmaConverterUnity fcu, object log, FcuLogType logType = FcuLogType.Default)
        {
            if (fcu.Settings.DebugSettings.DebugMode == false)
            {
                return;
            }

            switch (logType)
            {
                case FcuLogType.Default:
                    if (fcu.Settings.DebugSettings.LogDefault == false)
                        return;
                    break;
                case FcuLogType.SetTag:
                    if (fcu.Settings.DebugSettings.LogSetTag == false)
                        return;
                    break;
                case FcuLogType.IsDownloadable:
                    if (fcu.Settings.DebugSettings.LogIsDownloadable == false)
                        return;
                    break;
                case FcuLogType.Transform:
                    if (fcu.Settings.DebugSettings.LogTransform == false)
                        return;
                    break;
                case FcuLogType.GameObjectDrawer:
                    if (fcu.Settings.DebugSettings.LogGameObjectDrawer == false)
                        return;
                    break;
                case FcuLogType.HashGenerator:
                    if (fcu.Settings.DebugSettings.LogHashGenerator == false)
                        return;
                    break;
                case FcuLogType.ComponentDrawer:
                    if (fcu.Settings.DebugSettings.LogComponentDrawer == false)
                        return;
                    break;
                case FcuLogType.Error:
                    Debug.LogError($"FCU {fcu.Guid}: {log}");
                    return;
            }

            Debug.Log($"FCU {fcu.Guid}: {log}");
        }
    }
}