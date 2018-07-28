﻿using System;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementTextArea : GuiElementEditableTextBase
    {
        double minHeight;
        int highlightTextureId;
        ElementBounds highlightBounds;

        public GuiElementTextArea(ICoreClientAPI capi, ElementBounds bounds, API.Common.Action<string> OnTextChanged, CairoFont font) : base(capi, font, bounds)
        {
            multilineMode = true;
            minHeight = bounds.fixedHeight;
            this.OnTextChanged = OnTextChanged;
        }
        
        internal override void TextChanged()
        {
            Bounds.fixedHeight = Math.Max(minHeight, GetMultilineTextHeight(lines.ToArray(), Bounds.InnerWidth));
            Bounds.CalcWorldBounds();
            base.TextChanged();
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            EmbossRoundRectangleElement(ctx, Bounds, true, 3);
            ctx.SetSourceRGBA(0, 0, 0, 0.2f);
            ElementRoundRectangle(ctx, Bounds, true, 3);
            ctx.Fill();

            GenerateHighlight();

            RecomposeText();
        }

        void GenerateHighlight()
        {
            ImageSurface surfaceHighlight = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            Context ctxHighlight = genContext(surfaceHighlight);

            ctxHighlight.SetSourceRGBA(0, 0, 0, 0);
            ctxHighlight.Paint();

            ctxHighlight.SetSourceRGBA(0.5, 0.5, 0.5, 0.3);
            ctxHighlight.Paint();

            generateTexture(surfaceHighlight, ref highlightTextureId);

            ctxHighlight.Dispose();
            surfaceHighlight.Dispose();

            highlightBounds = Bounds.FlatCopy().FixedGrow(6, 6);
            highlightBounds.CalcWorldBounds();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (HasFocus)
            {
                api.Render.GlToggleBlend(true, EnumBlendMode.Standard);
                api.Render.Render2DTexturePremultipliedAlpha(highlightTextureId, highlightBounds);
                api.Render.GlToggleBlend(true, EnumBlendMode.Standard);
            }

            api.Render.Render2DTexturePremultipliedAlpha(textTextureId, Bounds);

            base.RenderInteractiveElements(deltaTime);
        }

        public void SetMaxLines(int maxlines)
        {
            this.maxlines = maxlines;
        }
    }



    public static partial class GuiComposerHelpers
    {
        public static GuiComposer AddTextArea(this GuiComposer composer, ElementBounds bounds, API.Common.Action<string> OnTextChanged, CairoFont font = null, string key = null)
        {
            if (font == null)
            {
                font = CairoFont.SmallTextInput();
            }

            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementTextArea(composer.Api, bounds, OnTextChanged, font), key);
            }

            return composer;
        }

        public static GuiElementTextArea GetTextArea(this GuiComposer composer, string key)
        {
            return (GuiElementTextArea)composer.GetElement(key);
        }



    }
}
