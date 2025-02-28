﻿using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Extensions
{
    public static class FObjectExtensions
    {
        public static Color32 GetRectTransformColor(this FObject fobject)
        {
            FGraphic graphic = fobject.GetGraphic();

            Color32? color = null;

            if (graphic.HasFill)
            {
                if (!graphic.SolidFill.IsDefault())
                {
                    color = graphic.SolidFill.Color;
                }
                else if (!graphic.GradientFill.IsDefault())
                {
                    color = graphic.GradientFill.GradientToSolid();
                }
            }
            else if (graphic.HasStroke)
            {
                if (!graphic.SolidStroke.IsDefault())
                {
                    color = graphic.SolidStroke.Color;
                }
                else if (!graphic.GradientStroke.IsDefault())
                {
                    color = graphic.GradientStroke.GradientToSolid();
                }
            }

            if (color == null)
            {
                color = new Color32(255, 255, 255, 0);
            }
            else
            {
                Color32 tempColor = color.Value;
                tempColor.a = 128;
                color = tempColor;
            }

            return color.Value;
        }

        public static void ExecuteWithTemporaryParent(this FObject fobject, Transform tempChildsParent, Func<FObject, GameObject> targetSelector, Action action)
        {
            GameObject target = targetSelector(fobject);
            List<Transform> children = new List<Transform>();
            List<int> siblingIndices = new List<int>();

            foreach (Transform child in target.transform)
            {
                children.Add(child);
                siblingIndices.Add(child.GetSiblingIndex());
            }

            foreach (Transform child in children)
            {
                child.SetParent(tempChildsParent);
            }

            action.Invoke();

            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetParent(target.transform);
                children[i].SetSiblingIndex(siblingIndices[i]);
            }
        }

        public static bool IsSprite(this SyncData data)
        {
            return data.FcuImageType == FcuImageType.Downloadable || data.FcuImageType == FcuImageType.Generative;
        }

        public static bool IsSprite(this FObject fobject)
        {
            return fobject.Data.FcuImageType == FcuImageType.Downloadable || fobject.Data.FcuImageType == FcuImageType.Generative;
        }

        public static bool IsCircle(this FObject fobject)
        {
            if (fobject.Type != NodeType.ELLIPSE)
                return false;

            if (fobject.Size.x.Round(1) == fobject.Size.y.Round(1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsRectangle(this FObject fobject)
        {
            if (fobject.Type == NodeType.RECTANGLE)
                return true;

            if (fobject.Type != NodeType.FRAME)
                return false;

            if (!fobject.Children.IsEmpty())
                return false;

            return true;
        }

        public static bool TryFixSizeWithStroke(this FObject fobject, float currentY, out float newY)
        {
            newY = 0;

            if (currentY > 0)
                return false;

            if (fobject.Strokes.IsEmpty())
                return false;

            if (!fobject.Strokes.Any(x => x.Visible.ToBoolNullTrue()))
                return false;

            newY = fobject.StrokeWeight;
            return true;
        }

        public static bool IsInsideAutoLayout(this FObject fobject, out FObject parent, out HorizontalOrVerticalLayoutGroup layoutGroup, FigmaConverterUnity fcu)
        {
            layoutGroup = null;

            if (!fcu.CurrentProject.TryGetByIndex(fobject.Data.ParentIndex, out parent))
                return false;

            if (!parent.ContainsTag(FcuTag.AutoLayoutGroup))
                return false;

            if (parent.Data?.GameObject == null)
                return false;

            if (!parent.Data.GameObject.TryGetComponent(out layoutGroup))
                return false;

            return true;
        }

        public static void SetFlagToAllChilds(this FObject parent, Action<FObject> action)
        {
            if (parent.IsDefault() || parent.Children.IsEmpty())
                return;

            //  action(parent);

            foreach (FObject child in parent.Children)
            {
                action(child);
                SetFlagToAllChilds(child, action);
            }
        }

        public static List<GradientAlphaKey> ToGradientAlphaKeys(this Paint gradient)
        {
            List<GradientAlphaKey> gradientColorKeys = new List<GradientAlphaKey>();

            foreach (GradientStop gradientStop in gradient.GradientStops)
            {
                gradientColorKeys.Add(new GradientAlphaKey
                {
                    alpha = gradientStop.Color.a,
                    time = gradientStop.Position
                });
            }

            return gradientColorKeys;
        }
        public static List<GradientColorKey> ToGradientColorKeys(this Paint gradient)
        {
            List<GradientColorKey> gradientColorKeys = new List<GradientColorKey>();

            foreach (GradientStop gradientStop in gradient.GradientStops)
            {
                gradientColorKeys.Add(new GradientColorKey
                {
                    color = gradientStop.Color,
                    time = gradientStop.Position
                });
            }

            return gradientColorKeys;
        }

        public static float ToAngle(this List<Vector2> gradientHandlePositions)
        {
            if (gradientHandlePositions.Count < 3)
            {
                DALogger.LogError($"Can't calculate angle.");
                return 0f;
            }

            float radians = Mathf.Atan2(
                gradientHandlePositions[2].y - gradientHandlePositions[0].y,
                gradientHandlePositions[2].x - gradientHandlePositions[0].x
            );

            float degrees = radians * (180 / Mathf.PI);
            degrees = (degrees + 360) % 360; // Normalize angle between 0 and 360 degrees

            return degrees;
        }

        public static string GetText(this FObject fobject)
        {
            return fobject.Characters.Replace("\\r", " ").Replace("\\n", Environment.NewLine);
        }

        public static bool IsInsideDownloadable(this FObject fobject, FigmaConverterUnity fcu, out FObject downloadableFObject)
        {
            downloadableFObject = default;

            bool get = fcu.CurrentProject.TryGetByIndex(fobject.Data.ParentIndex, out FObject parent);

            if (get == false)
                return false;

            if (fcu.ImageTypeSetter.DownloadableIds.Contains(parent.Id))
            {
                downloadableFObject = fobject;
                return true;
            }

            return IsInsideDownloadable(parent, fcu, out downloadableFObject);
        }

        public static bool IsSupportedLine(this FObject fobject)
        {
            if (fobject.StrokeCap == StrokeCap.SQUARE)
                return true;

            if (fobject.StrokeCap == StrokeCap.ROUND && fobject.StrokeWeight >= 2f)
                return true;

            return false;
        }

        public static bool HasActiveProperty<T>(this FObject fobject, Expression<Func<FObject, IEnumerable<T>>> propertySelector) where T : IVisible
        {
            var func = propertySelector.Compile();
            IEnumerable<T> collection = func(fobject);
            return !collection.IsEmpty() && collection.Any(item => item.Visible.ToBoolNullTrue());
        }

        public static bool IsShadowType(this Effect effect) => effect.Type.ToString().Contains("SHADOW");
        public static bool IsBlurType(this Effect effect) => effect.Type.ToString().Contains("BLUR");

        public static bool IsAnyMask(this FObject fobject) => fobject.IsObjectMask() || fobject.IsClipMask() || fobject.IsFrameMask();
        public static bool IsFrameMask(this FObject fobject) => fobject.ContainsTag(FcuTag.Frame);
        public static bool IsClipMask(this FObject fobject) => fobject.ClipsContent.ToBoolNullFalse();
        public static bool IsObjectMask(this FObject fobject) => fobject.IsMask.ToBoolNullFalse();
        public static bool IsGenerativeType(this FObject fobject) => fobject.Data.FcuImageType == FcuImageType.Generative;
        public static bool IsDrawableType(this FObject fobject) => fobject.Data.FcuImageType == FcuImageType.Drawable;
        public static bool IsDownloadableType(this FObject fobject) => fobject.Data.FcuImageType == FcuImageType.Downloadable;
        public static bool IsMaskType(this FObject fobject) => fobject.Data.FcuImageType == FcuImageType.Mask;

        public static bool IsSupportedRenderSize(this Vector2 sourceSize, FigmaConverterUnity fcu, out Vector2Int spriteSize, out Vector2Int renderSize)
        {
            spriteSize = (sourceSize * fcu.Settings.MainSettings.ImageScale).ToVector2Int();

            int maxRenderSize = FcuConfig.Instance.MaxRenderSize;
            int renderUpscaleFactor = FcuConfig.Instance.RenderUpscaleFactor;

            renderSize = spriteSize * renderUpscaleFactor;

            if (renderSize.x <= maxRenderSize && renderSize.y <= maxRenderSize)
            {
                return true;
            }

            return false;
        }

        public static bool IsGenerativeInstance(this FObject fobject)
        {
            bool v1 = fobject.PrimaryAxisSizingMode.IsEmpty();
            bool v2 = fobject.CounterAxisSizingMode.IsEmpty();
            bool v3 = fobject.PrimaryAxisAlignItems == PrimaryAxisAlignItem.NONE;
            bool v4 = fobject.CounterAxisAlignItems == CounterAxisAlignItem.NONE;

            if (v1 == false || v2 == false || v3 == false || v4 == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsAllPaintsDisabled(this FObject fobject)
        {
            bool allFillsDisabled = fobject.Fills
                .Where(x => x.Visible.ToBoolNullTrue() == false)
                .Count() == fobject.Fills.Count();

            bool allStrokesDisabled = fobject.Strokes
                .Where(x => x.Visible.ToBoolNullTrue() == false)
                .Count() == fobject.Strokes.Count();

            if (allFillsDisabled && allStrokesDisabled)
                return true;
            else
                return false;
        }

        public static bool TryGetLocalPosition(this FObject fobject, out Vector3 rtPos)
        {
            try
            {
                rtPos = new Vector3(fobject.RelativeTransform[0][2].ToFloat(), -fobject.RelativeTransform[1][2].ToFloat(), 0);
                return true;
            }
            catch
            {
                rtPos = new Vector3(0, 0, 0);
                return false;
            }
        }
    }
}
