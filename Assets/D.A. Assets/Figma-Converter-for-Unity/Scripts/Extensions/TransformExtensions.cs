﻿using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using UnityEngine;

namespace DA_Assets.FCU.Extensions
{
    public static class TransformExtensions
    {
        public static bool IsNeedRotate(this FObject fobject, FigmaConverterUnity fcu)
        {
            float angle = fobject.Data.Angle;

            bool? needRotate = null;

            if (fobject.ContainsTag(FcuTag.Text) && angle != 0)
            {
                needRotate = true;
            }
            else if (fobject.Data.ChildIndexes.IsEmpty())
            {
                if (fobject.GetRenderSize(out Vector2 size))
                {
                    //No need rotation for squares/circles.
                    if (size.x == size.y)
                    {
                        needRotate = false;
                    }
                    //No need rotation - at 0 and 180 degrees a shape without children looks the same.
                    else if (angle == 180f)
                    {
                        needRotate = false;
                    }
                }
            }

            if (needRotate == null)
            {
                switch (fobject.Data.FcuImageType)
                {
                    case FcuImageType.Downloadable:
                        {
                            needRotate = false;
                        }
                        break;
                    default:
                        {
                            if (fobject.Type == NodeType.GROUP)
                            {
                                bool hasrot = false;    
                                foreach (var cindex in fobject.Data.ChildIndexes)
                                {
                                    if (fcu.CurrentProject.TryGetByIndex(cindex, out FObject child))
                                    {
                                        float a = child.Data.Angle;

                                        if (a != angle)
                                        {
                                            hasrot = true;
                                            break;
                                        }
                                    }
                                }

                                if (!hasrot)
                                {
                                    needRotate = false;
                                }
                                else
                                {
                                    needRotate = true;
                                }
                               
                            }
                            else
                            {
                                needRotate = angle != 0;
                            }
                    
                        }
                        break;
                }

                          

            }

            fcu.Log($"IsNeedRotate | {fobject.Data.NameHierarchy} | needRotate: {needRotate} | angle: {angle} | {fobject.Data.FcuImageType}", FcuLogType.Transform);
            return needRotate.ToBoolNullFalse();
        }

        public static void SetFigmaRotation(this FObject fobject, GameObject target, FigmaConverterUnity fcu)
        {
            if (fobject.IsNeedRotate(fcu))
            {
                float rotationAngle;

                Transform rect = target.GetComponent<Transform>();

                if (fobject.IsNeedRotate(fcu))
                {
                    rotationAngle = fobject.Data.Angle;
                }
                else
                {
                    rotationAngle = 0;
                }

                rect.SetRotation(rotationAngle);
            }
        }

        public static float GetAngleFromMatrix(this FObject fobject, FigmaConverterUnity fcu)
        {
            float rotRad;
            float rotDeg;
            float a;
            float b;

            if (fobject.RelativeTransform.IsEmpty() ||
                fobject.RelativeTransform.Count < 2 ||
                fobject.RelativeTransform[1].Count < 2 ||
                fobject.RelativeTransform[1][0] == null ||
                fobject.RelativeTransform[1][1] == null)
            {
                fcu.Log($"{nameof(GetAngleFromMatrix)} | {fobject.Data.NameHierarchy} | wrong relative transform.", FcuLogType.Error);
                return 0;
            }
            else
            {
                a = (float)fobject.RelativeTransform[1][0];
                b = (float)fobject.RelativeTransform[1][1];
                rotRad = Mathf.Atan2(a, b).Round(5);
            }

            rotRad = -1 * rotRad;
            rotDeg = rotRad * (180f / (float)Mathf.PI);

            fcu.Log($"{nameof(GetAngleFromMatrix)} | {fobject.Data.NameHierarchy} | rotDeg: {rotDeg}\nb: {b}\na: {a}\nrotRad: {rotRad}");

            return rotDeg;
        }

        public static float GetAngleFromField(this FObject fobject, FigmaConverterUnity fcu)
        {
            float rotRad;
            float rotDeg;
            float a = 0f;
            float b = 0f;

            rotRad = fobject.Rotation.HasValue ? fobject.Rotation.Value.Round(5) : 0;
            rotRad = -1 * rotRad;
            rotDeg = rotRad * (180f / (float)Mathf.PI);

            fcu.Log($"{nameof(GetAngleFromField)} | {fobject.Data.NameHierarchy} | hasValue: {fobject.Rotation.HasValue} | rotDeg: {rotDeg}\nb: {b}\na: {a}\nrotRad: {rotRad}");

            return rotDeg;
        }

        public static float GetFigmaRotationAngle(this FObject fobject, FigmaConverterUnity fcu)
        {
            float angle = fobject.GetAngleFromField(fcu);

            if (angle == 0)
                angle = fobject.GetAngleFromMatrix(fcu);

            return angle;
        }

        public static AnchorType GetFigmaAnchor(this FObject fobject)
        {
            string anchor = fobject.Constraints.Vertical + " " + fobject.Constraints.Horizontal;

            AnchorType anchorPreset;

            switch (anchor)
            {
                ////////////////LEFT////////////////
                case "TOP LEFT":
                    anchorPreset = AnchorType.TopLeft;
                    break;
                case "BOTTOM LEFT":
                    anchorPreset = AnchorType.BottomLeft;
                    break;
                case "TOP_BOTTOM LEFT":
                    anchorPreset = AnchorType.VertStretchLeft;
                    break;
                case "CENTER LEFT":
                    anchorPreset = AnchorType.MiddleLeft;
                    break;
                case "SCALE LEFT":
                    anchorPreset = AnchorType.VertStretchLeft;
                    break;
                ////////////////RIGHT////////////////
                case "TOP RIGHT":
                    anchorPreset = AnchorType.TopRight;
                    break;
                case "BOTTOM RIGHT":
                    anchorPreset = AnchorType.BottomRight;
                    break;
                case "TOP_BOTTOM RIGHT":
                    anchorPreset = AnchorType.VertStretchRight;
                    break;
                case "CENTER RIGHT":
                    anchorPreset = AnchorType.MiddleRight;
                    break;
                case "SCALE RIGHT":
                    anchorPreset = AnchorType.VertStretchRight;
                    break;
                ////////////////LEFT_RIGHT////////////////
                case "TOP LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchTop;
                    break;
                case "BOTTOM LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchBottom;
                    break;
                case "TOP_BOTTOM LEFT_RIGHT":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                case "CENTER LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchMiddle;
                    break;
                case "SCALE LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchMiddle;
                    break;
                ////////////////CENTER////////////////
                case "TOP CENTER":
                    anchorPreset = AnchorType.TopCenter;
                    break;
                case "BOTTOM CENTER":
                    anchorPreset = AnchorType.BottomCenter;
                    break;
                case "TOP_BOTTOM CENTER":
                    anchorPreset = AnchorType.VertStretchCenter;
                    break;
                case "CENTER CENTER":
                    anchorPreset = AnchorType.MiddleCenter;
                    break;
                case "SCALE CENTER":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                ////////////////SCALE////////////////
                case "TOP SCALE":
                    anchorPreset = AnchorType.HorStretchTop;
                    break;
                case "BOTTOM SCALE":
                    anchorPreset = AnchorType.HorStretchBottom;
                    break;
                case "TOP_BOTTOM SCALE":
                    anchorPreset = AnchorType.VertStretchCenter;
                    break;
                case "CENTER SCALE":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                case "SCALE SCALE":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                ////////////////DEFAULT////////////////
                default:
                    anchorPreset = AnchorType.MiddleCenter;
                    break;
            }

            return anchorPreset;
        }
    }
}