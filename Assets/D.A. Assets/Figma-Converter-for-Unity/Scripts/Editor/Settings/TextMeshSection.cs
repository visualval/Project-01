﻿using DA_Assets.FCU.Extensions;
using DA_Assets.Shared;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class TextMeshSection : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        private string[] shaderNames = new string[] { };

        private int shaderSelectedIndex = -1;

        public override void Init()
        {
            SetShaderNames();
        }

        private void SetShaderNames()
        {
            shaderNames = ShaderUtil.GetAllShaderInfo().Select(info => info.name).ToArray();
        }

        public void Draw()
        {
#if TextMeshPro
            gui.SectionHeader(FcuLocKey.label_textmeshpro_settings.Localize());
            gui.Space15();

            monoBeh.Settings.TextMeshSettings.OverrideLetterSpacing = gui.Toggle(new GUIContent(FcuLocKey.label_override_letter_spacing.Localize(), FcuLocKey.tooltip_override_letter_spacing.Localize()),
                monoBeh.Settings.TextMeshSettings.OverrideLetterSpacing);

            //monoBeh.Settings.TextMeshSettings.OverrideLineSpacing = gui.Toggle(new GUIContent(FcuLocKey.label_override_line_spacing.Localize(), FcuLocKey.tooltip_override_line_spacing.Localize()),
                //monoBeh.Settings.TextMeshSettings.OverrideLineSpacing);

            gui.Space15();

            monoBeh.Settings.TextMeshSettings.AutoSize = gui.Toggle(new GUIContent(FcuLocKey.label_auto_size.Localize(), FcuLocKey.tooltip_auto_size.Localize()),
                monoBeh.Settings.TextMeshSettings.AutoSize);

            monoBeh.Settings.TextMeshSettings.OverrideTags = gui.Toggle(new GUIContent(FcuLocKey.label_override_tags.Localize(), FcuLocKey.tooltip_override_tags.Localize()),
                monoBeh.Settings.TextMeshSettings.OverrideTags);

            monoBeh.Settings.TextMeshSettings.Wrapping = gui.Toggle(new GUIContent(FcuLocKey.label_wrapping.Localize(), FcuLocKey.tooltip_wrapping.Localize()),
                monoBeh.Settings.TextMeshSettings.Wrapping);

            if (monoBeh.IsNova())
            {
                monoBeh.Settings.TextMeshSettings.OrthographicMode = gui.Toggle(new GUIContent(FcuLocKey.label_orthographic_mode.Localize(), FcuLocKey.tooltip_orthographic_mode.Localize()),
                    monoBeh.Settings.TextMeshSettings.OrthographicMode);
            }

            monoBeh.Settings.TextMeshSettings.RichText = gui.Toggle(new GUIContent(FcuLocKey.label_rich_text.Localize(), FcuLocKey.tooltip_rich_text.Localize()),
                monoBeh.Settings.TextMeshSettings.RichText);

            monoBeh.Settings.TextMeshSettings.RaycastTarget = gui.Toggle(new GUIContent(FcuLocKey.label_raycast_target.Localize(), FcuLocKey.tooltip_raycast_target.Localize()),
               monoBeh.Settings.TextMeshSettings.RaycastTarget);

            monoBeh.Settings.TextMeshSettings.ParseEscapeCharacters = gui.Toggle(new GUIContent(FcuLocKey.label_parse_escape_characters.Localize(), FcuLocKey.tooltip_parse_escape_characters.Localize()),
               monoBeh.Settings.TextMeshSettings.ParseEscapeCharacters);

            monoBeh.Settings.TextMeshSettings.VisibleDescender = gui.Toggle(new GUIContent(FcuLocKey.label_visible_descender.Localize(), FcuLocKey.tooltip_visible_descender.Localize()),
               monoBeh.Settings.TextMeshSettings.VisibleDescender);

            monoBeh.Settings.TextMeshSettings.Kerning = gui.Toggle(new GUIContent(FcuLocKey.label_kerning.Localize(), FcuLocKey.tooltip_kerning.Localize()),
               monoBeh.Settings.TextMeshSettings.Kerning);

            monoBeh.Settings.TextMeshSettings.ExtraPadding = gui.Toggle(new GUIContent(FcuLocKey.label_extra_padding.Localize(), FcuLocKey.tooltip_extra_padding.Localize()),
               monoBeh.Settings.TextMeshSettings.ExtraPadding);

            monoBeh.Settings.TextMeshSettings.Overflow = gui.EnumField(new GUIContent(FcuLocKey.label_overflow.Localize(), FcuLocKey.tooltip_overflow.Localize()),
                monoBeh.Settings.TextMeshSettings.Overflow);

            if (monoBeh.Settings.TextMeshSettings.AutoSize && monoBeh.Settings.TextMeshSettings.Overflow == TMPro.TextOverflowModes.Overflow)
            {
                DALogger.LogError(FcuLocKey.log_cant_enable_autosize_with_overflow.Localize());
                monoBeh.Settings.TextMeshSettings.AutoSize = false;
            }

            monoBeh.Settings.TextMeshSettings.HorizontalMapping = gui.EnumField(new GUIContent(FcuLocKey.label_horizontal_mapping.Localize(), FcuLocKey.tooltip_horizontal_mapping.Localize()),
                monoBeh.Settings.TextMeshSettings.HorizontalMapping);

            monoBeh.Settings.TextMeshSettings.VerticalMapping = gui.EnumField(new GUIContent(FcuLocKey.label_vertical_mapping.Localize(), FcuLocKey.tooltip_vertical_mapping.Localize()),
                monoBeh.Settings.TextMeshSettings.VerticalMapping);

            monoBeh.Settings.TextMeshSettings.GeometrySorting = gui.EnumField(new GUIContent(FcuLocKey.label_geometry_sorting.Localize(), FcuLocKey.tooltip_geometry_sorting.Localize()),
                monoBeh.Settings.TextMeshSettings.GeometrySorting);

            shaderSelectedIndex = gui.ShaderDropdown(new GUIContent(FcuLocKey.label_shader.Localize()), shaderSelectedIndex, shaderNames, (option) =>
            {
                monoBeh.Settings.TextMeshSettings.Shader = Shader.Find(shaderNames[option]);
            });

#if RTLTMP_EXISTS
            if (monoBeh.Settings.ComponentSettings.TextComponent == TextComponent.RTLTextMeshPro)
            {
                gui.Space10();

                monoBeh.Settings.TextMeshSettings.Farsi = gui.Toggle(new GUIContent(FcuLocKey.label_farsi.Localize(), FcuLocKey.tooltip_farsi.Localize()),
                    monoBeh.Settings.TextMeshSettings.Farsi);

                monoBeh.Settings.TextMeshSettings.ForceFix = gui.Toggle(new GUIContent(FcuLocKey.label_force_fix.Localize(), FcuLocKey.tooltip_force_fix.Localize()),
                   monoBeh.Settings.TextMeshSettings.ForceFix);

                monoBeh.Settings.TextMeshSettings.PreserveNumbers = gui.Toggle(new GUIContent(FcuLocKey.label_preserve_numbers.Localize(), FcuLocKey.tooltip_preserve_numbers.Localize()),
                   monoBeh.Settings.TextMeshSettings.PreserveNumbers);

                monoBeh.Settings.TextMeshSettings.FixTags = gui.Toggle(new GUIContent(FcuLocKey.label_fix_tags.Localize(), FcuLocKey.tooltip_fix_tags.Localize()),
                   monoBeh.Settings.TextMeshSettings.FixTags);
            }
#endif
#endif
        }
    }
}