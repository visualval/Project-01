﻿using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

#if JOSH_PUI_EXISTS
using UnityEngine.UI.ProceduralImage;
#endif

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class JoshPuiDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, Sprite sprite, GameObject target)
        {
#if JOSH_PUI_EXISTS
            target.TryAddGraphic(out ProceduralImage img);

            img.sprite = sprite;
            img.type = monoBeh.Settings.JoshPuiSettings.Type;
            img.raycastTarget = monoBeh.Settings.JoshPuiSettings.RaycastTarget;
            img.preserveAspect = monoBeh.Settings.JoshPuiSettings.PreserveAspect;
            img.FalloffDistance = monoBeh.Settings.JoshPuiSettings.FalloffDistance;
#if UNITY_2020_1_OR_NEWER
            img.raycastPadding = monoBeh.Settings.JoshPuiSettings.RaycastPadding;
#endif
            if (fobject.Type == NodeType.ELLIPSE)
            {
                target.TryAddComponent(out RoundModifier roundModifier);
            }
            else
            {
                if (fobject.CornerRadiuses != null)
                {
                    target.TryAddComponent(out FreeModifier freeModifier);
                    freeModifier.Radius = fobject.GetCornerRadius(ImageComponent.ProceduralImage);
                }
                else
                {
                    target.TryAddComponent(out UniformModifier uniformModifier);
                    uniformModifier.Radius = fobject.CornerRadius.ToFloat();
                }
            }

            SetColor(fobject, img);
#endif
        }

        public void SetColor(FObject fobject, Image img)
        {
            FGraphic graphic = fobject.GetGraphic();

            monoBeh.Log($"SetUnityImageColor | {fobject.Data.NameHierarchy} | {fobject.Data.FcuImageType} | hasFills: {graphic.HasFill} | hasStroke: {graphic.HasStroke}", FcuLogType.ComponentDrawer);

            if (fobject.IsDrawableType())
            {
                if (graphic.HasFill)
                {
                    if (graphic.SolidFill.IsDefault() == false)
                    {
                        img.color = graphic.SolidFill.Color;
                    }
                    else
                    {
                        img.color = Color.white;
                    }

                    if (graphic.GradientFill.IsDefault() == false)
                    {
                        monoBeh.CanvasDrawer.ImageDrawer.UnityImageDrawer.AddGradient(graphic.GradientFill, img.gameObject);
                    }
                }
                else
                {
                    Color c = Color.white;
                    c.a = 0;
                    img.color = c;
                }

                if (graphic.HasStroke)
                {
                    if (fobject.StrokeAlign == StrokeAlign.OUTSIDE)
                    {
                        monoBeh.CanvasDrawer.ImageDrawer.UnityImageDrawer.AddUnityOutline(fobject, img.gameObject, graphic.SolidStroke, graphic.GradientStroke);
                    }
                }
            }
            else
            {
                monoBeh.CanvasDrawer.ImageDrawer.UnityImageDrawer.SetColor(fobject, img);
            }
        }
    }
}
