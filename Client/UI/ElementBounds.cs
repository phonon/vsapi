﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Box sizing model comparable to the box sizing model of cascading style sheets using "position:relative;"
    /// Each element has a position, size, padding and margin
    /// Padding is counted towards the size of the box, whereas margin is not
    /// </summary>
    public class ElementBounds
    {
        public ElementBounds parentBounds;
        public List<ElementBounds> childBounds;

        public ElementAlignment alignment;

        public ElementSizing bothSizing
        {
            set { verticalSizing = value; horizontalSizing = value; }
        }

        public ElementSizing verticalSizing;
        public ElementSizing horizontalSizing;

        // Sizing = Percentual
        //public double percentMarginX;
        //public double percentMarginY;
        public double percentPaddingX;
        public double percentPaddingY;
        public double percentX;
        public double percentY;
        public double percentWidth;
        public double percentHeight;

        // Sizing = Fixed
        public double fixedMarginX;
        public double fixedMarginY;
        public double fixedPaddingX;
        public double fixedPaddingY;
        public double fixedX;
        public double fixedY;
        public double fixedWidth;
        public double fixedHeight;
        public double fixedOffsetX;
        public double fixedOffsetY;


        // Sizing = Fit to text
        // Takes values from element


        public double absPaddingX;
        public double absPaddingY;
        public double absMarginX;
        public double absMarginY;
        public double absOffsetX;
        public double absOffsetY;

        public double absFixedX;
        public double absFixedY;
        public double absInnerWidth;
        public double absInnerHeight;

        public string Name;


        public bool Initialized;
        public bool IsDrawingSurface;
        private bool requiresrelculation = true;
        public virtual bool RequiresRecalculation { get { return requiresrelculation; } }

        /// <summary>
        /// Position relative to it's parent element plus margin
        /// </summary>
        public virtual double relX { get { return absFixedX + absMarginX + absOffsetX; } }
        public virtual double relY { get { return absFixedY + absMarginY + absOffsetY; } }


        /// <summary>
        /// Absolute position of the element plus margin. Same as renderX but without padding
        /// </summary>
        public virtual double absX { get { return absFixedX + absMarginX + absOffsetX + parentBounds.absPaddingX + parentBounds.absX; } }
        public virtual double absY { get { return absFixedY + absMarginY + absOffsetY + parentBounds.absPaddingY + parentBounds.absY; } }

        /// <summary>
        /// Width including padding
        /// </summary>
        public virtual double OuterWidth { get { return absInnerWidth + 2 * absPaddingX; } }
        public virtual double OuterHeight { get { return absInnerHeight + 2 * absPaddingY; } }

        public virtual int OuterWidthInt { get { return (int)OuterWidth; } }
        public virtual int OuterHeightInt { get { return (int)OuterHeight; } }

        public virtual double InnerWidth { get { return absInnerWidth; } }
        public virtual double InnerHeight { get { return absInnerHeight; } }


        /// <summary>
        /// Position where the element has to be drawn. This is a position relative to it's parent element plus margin plus padding. 
        /// </summary>
        public virtual double drawX
        {
            get { return bgDrawX + absPaddingX; }
        }
        public virtual double drawY
        {
            get { return bgDrawY + absPaddingY; }
        }

        /// <summary>
        /// Position where the background has to be drawn, this encompasses the elements padding
        /// </summary>
        public virtual double bgDrawX
        {
            get
            {
                return absFixedX + absMarginX + absOffsetX + (parentBounds.IsDrawingSurface ? parentBounds.absPaddingX : parentBounds.drawX);
            }
        }
        public virtual double bgDrawY
        {
            get
            {
                return absFixedY + absMarginY + absOffsetY + (parentBounds.IsDrawingSurface ? parentBounds.absPaddingY : parentBounds.drawY);
            }
        }


        public virtual double renderX { get { return absFixedX + absMarginX + absOffsetX + parentBounds.absPaddingX + parentBounds.renderX; } }
        public virtual double renderY { get { return absFixedY + absMarginY + absOffsetY + parentBounds.absPaddingY + parentBounds.renderY; } }




        public ElementBounds()
        {

        }


        public void MarkDirtyRecursive()
        {
            Initialized = false;
            if (childBounds != null)
            {
                foreach (ElementBounds child in childBounds)
                {
                    child.MarkDirtyRecursive();
                }
            }
        }


        public virtual void CalcWorldBounds()
        {
            requiresrelculation = false;

            absOffsetX = scaled(fixedOffsetX);
            absOffsetY = scaled(fixedOffsetY);

            if (horizontalSizing == ElementSizing.FitToChildren && verticalSizing == ElementSizing.FitToChildren)
            {
                absFixedX = scaled(fixedX);
                absFixedY = scaled(fixedY);

                absPaddingX = scaled(fixedPaddingX);
                absPaddingY = scaled(fixedPaddingY);

                buildBoundsFromChildren();
            }
            else
            {
                switch (horizontalSizing)
                {
                    case ElementSizing.Fixed:
                        absFixedX = scaled(fixedX);
                        absInnerWidth = scaled(fixedWidth);
                        absPaddingX = scaled(fixedPaddingX);
                        break;

                    case ElementSizing.Percentual:
                    case ElementSizing.PercentualSubstractFixed:
                        absFixedX = percentX * parentBounds.OuterWidth;
                        absInnerWidth = percentWidth * parentBounds.OuterWidth;
                        absPaddingX = scaled(fixedPaddingX) + percentPaddingX * parentBounds.OuterWidth;

                        if (horizontalSizing == ElementSizing.PercentualSubstractFixed)
                        {
                            absInnerWidth -= scaled(fixedWidth);
                        }
                        break;
                }

                switch (verticalSizing)
                {
                    case ElementSizing.Fixed:
                        absFixedY = scaled(fixedY);
                        absInnerHeight = scaled(fixedHeight);
                        absPaddingY = scaled(fixedPaddingY);
                        break;

                    case ElementSizing.Percentual:
                    case ElementSizing.PercentualSubstractFixed:
                        absFixedY = percentY * parentBounds.OuterHeight;
                        absInnerHeight = percentHeight * parentBounds.OuterHeight;
                        absPaddingY = scaled(fixedPaddingY) + percentPaddingY * parentBounds.OuterHeight;

                        if (horizontalSizing == ElementSizing.PercentualSubstractFixed)
                        {
                            absInnerHeight -= scaled(fixedHeight);
                        }

                        break;
                }
            }


            // Only if the parent element has been initialized already
            if (parentBounds.Initialized)
            {
                calcMarginFromAlignment(parentBounds.InnerWidth, parentBounds.InnerHeight);
            }

            Initialized = true;

            if (childBounds != null)
            {

                foreach (ElementBounds child in childBounds)
                {
                    if (!child.Initialized)
                    {
                        child.CalcWorldBounds();
                    }

                }
            }
        }

        void calcMarginFromAlignment(double dialogWidth, double dialogHeight)
        {
            switch (alignment)
            {
                case ElementAlignment.FixedTop:
                    break;
                case ElementAlignment.FixedMiddle:
                    absMarginY = dialogHeight / 2 - OuterHeight / 2;
                    break;
                case ElementAlignment.FixedBottom:
                    absMarginY = dialogHeight - OuterHeight;
                    break;
                case ElementAlignment.CenterFixed:
                    absMarginX = dialogWidth / 2 - OuterWidth / 2;
                    break;

                case ElementAlignment.CenterBottom:
                    absMarginX = dialogWidth / 2 - OuterWidth / 2;
                    absMarginY = dialogHeight - OuterHeight;
                    break;
                case ElementAlignment.CenterMiddle:
                    absMarginX = dialogWidth / 2 - OuterWidth / 2;
                    absMarginY = dialogHeight / 2 - OuterHeight / 2;
                    break;

                case ElementAlignment.CenterTop:
                    absMarginX = dialogWidth / 2 - OuterWidth / 2;
                    break;

                case ElementAlignment.LeftBottom:
                    absMarginX = 0;
                    absMarginY = dialogHeight - OuterHeight;
                    break;
                case ElementAlignment.LeftMiddle:
                    absMarginX = 0;
                    absMarginY = dialogHeight / 2 - absInnerHeight / 2;
                    break;
                case ElementAlignment.LeftTop:
                    absMarginX = 0;
                    absMarginY = 0;
                    break;
                case ElementAlignment.LeftFixed:
                    absMarginX = 0;
                    break;


                case ElementAlignment.RightBottom:
                    absMarginX = dialogWidth - OuterWidth;
                    absMarginY = dialogHeight - OuterHeight;
                    break;
                case ElementAlignment.RightMiddle:
                    absMarginX = dialogWidth - OuterWidth;
                    absMarginY = dialogHeight / 2 - OuterHeight / 2;
                    break;
                case ElementAlignment.RightTop:
                    absMarginX = dialogWidth - OuterWidth;
                    absMarginY = 0;
                    break;

                case ElementAlignment.RightFixed:
                    absMarginX = dialogWidth - OuterWidth - 0;
                    break;

            }
        }

        void buildBoundsFromChildren()
        {
            if (childBounds == null)
            {
                throw new Exception("Cant build bounds from children elements, there are no children!");
            }

            double width = 0;
            double height = 0;

            foreach (ElementBounds bounds in childBounds)
            {
                // Alignment can only happen once the max size is known, so ignore it for now
                ElementAlignment prevAlign = bounds.alignment;
                bounds.alignment = ElementAlignment.None;

                bounds.CalcWorldBounds();

                if (bounds.horizontalSizing != ElementSizing.Percentual)
                {
                    width = Math.Max(width, bounds.OuterWidth + bounds.relX);
                }
                if (bounds.verticalSizing != ElementSizing.Percentual)
                {
                    height = Math.Max(height, bounds.OuterHeight + bounds.relY);
                }

                // Reassign actual alignment, now as we can calculate the alignment
                bounds.alignment = prevAlign;
            }

            if (width == 0 || height == 0)
            {
                throw new Exception("Couldn't build bounds from children, there were probably no child elements using fixed sizing! (or they were size 0)");
            }

            this.absInnerWidth = width;
            this.absInnerHeight = height;
        }


        public static double scaled(double value)
        {
            return value * ClientSettingsApi.GUIScale;
        }


        public ElementBounds WithScale(double factor)
        {
            fixedX *= factor;
            fixedY *= factor;
            fixedWidth *= factor;
            fixedHeight *= factor;
            absPaddingX *= factor;
            absPaddingY *= factor;
            absMarginX *= factor;
            absMarginY *= factor;

            percentPaddingX *= factor;
            percentPaddingY *= factor;
            percentX *= factor;
            percentY *= factor;
            percentWidth *= factor;
            percentHeight *= factor;


            return this;
        }

        public ElementBounds WithChildren(params ElementBounds[] bounds)
        {
            foreach (ElementBounds bound in bounds)
            {
                WithChild(bound);
            }
            return this;
        }

        public ElementBounds WithChild(ElementBounds bounds)
        {
            if (childBounds == null)
            {
                childBounds = new List<ElementBounds>();
            }

            if (!childBounds.Contains(bounds))
            {
                childBounds.Add(bounds);
            }


            if (bounds.parentBounds == null)
            {
                bounds.parentBounds = this;
            }

            return this;
        }


        /// <summary>
        /// Returns the relative coordinate if supplied coordinate is inside the bounds, otherwise null
        /// </summary>
        /// <param name="absPointX"></param>
        /// <param name="absPointY"></param>
        /// <returns></returns>
        public Vec2d PositionInside(int absPointX, int absPointY)
        {
            if (PointInside(absPointX, absPointY))
            {
                return new Vec2d(absPointX - absX, absPointY - absY);
            }

            return null;
        }

        /// <summary>
        /// Returns true if supplied coordinate is inside the bounds
        /// </summary>
        /// <param name="absPointX"></param>
        /// <param name="absPointY"></param>
        /// <returns></returns>
        public bool PointInside(int absPointX, int absPointY)
        {
            return
                absPointX >= absX &&
                absPointX <= absX + OuterWidth &&
                absPointY >= absY &&
                absPointY <= absY + OuterHeight
            ;
        }


        /// <summary>
        /// Returns true if supplied coordinate is inside the bounds
        /// </summary>
        /// <param name="absPointX"></param>
        /// <param name="absPointY"></param>
        /// <returns></returns>
        public bool PointInside(double absPointX, double absPointY)
        {
            return
                absPointX >= absX &&
                absPointX <= absX + OuterWidth &&
                absPointY >= absY &&
                absPointY <= absY + OuterHeight
            ;
        }


        /// <summary>
        /// Checks if the bounds is at least partially inside it's parent bounds by checking if any of the 4 corner points is inside
        /// </summary>
        /// <returns></returns>
        public bool PartiallyInside(ElementBounds boundingBounds)
        {
            return
                boundingBounds.PointInside(absX, absY) ||
                boundingBounds.PointInside(absX + OuterWidth, absY) ||
                boundingBounds.PointInside(absX, absY + OuterHeight) ||
                boundingBounds.PointInside(absX + OuterWidth, absY + OuterHeight)
            ;
        }



        /// <summary>
        /// Makes a copy of the current bounds but leaves the position and padding at 0. Sets the parent to the calling bounds
        /// </summary>
        /// <returns></returns>
        public ElementBounds CopyOnlySize()
        {
            return new ElementBounds()
            {
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                fixedHeight = fixedHeight,
                fixedWidth = fixedWidth,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                parentBounds = Empty.WithSizing(ElementSizing.FitToChildren)
            };
        }

        /// <summary>
        /// Makes a copy of the current bounds but leaves the position and padding at 0. Sets the same parent as the current one.
        /// </summary>
        /// <returns></returns>
        public ElementBounds CopyOffsetedSibling(double fixedDeltaX = 0, double fixedDeltaY = 0, double fixedDeltaWidth = 0, double fixedDeltaHeight = 0)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedX = fixedX + fixedDeltaX,
                fixedY = fixedY + fixedDeltaY,
                fixedWidth = fixedWidth + fixedDeltaWidth,
                fixedHeight = fixedHeight + fixedDeltaHeight,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                fixedMarginX = fixedMarginX,
                fixedMarginY = fixedMarginY,
                percentPaddingX = percentPaddingX,
                percentPaddingY = percentPaddingY,
                parentBounds = parentBounds
            };
        }

        /// <summary>
        /// Makes a copy of the current bounds but leaves the position and padding at 0. Sets the same parent as the current one.
        /// </summary>
        /// <returns></returns>
        public ElementBounds BelowCopy(double fixedDeltaX = 0, double fixedDeltaY = 0, double fixedDeltaWidth = 0, double fixedDeltaHeight = 0)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                percentX = percentX,
                percentY = percentY = percentHeight,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedX = fixedX + fixedDeltaX,
                fixedY = fixedY + fixedDeltaY + fixedHeight + fixedPaddingY * 2,
                fixedWidth = fixedWidth + fixedDeltaWidth,
                fixedHeight = fixedHeight + fixedDeltaHeight,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                fixedMarginX = fixedMarginX,
                fixedMarginY = fixedMarginY,
                percentPaddingX = percentPaddingX,
                percentPaddingY = percentPaddingY,
                parentBounds = parentBounds,
            };
        }

        public ElementBounds RightCopy(double fixedDeltaX = 0, double fixedDeltaY = 0, double fixedDeltaWidth = 0, double fixedDeltaHeight = 0)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                percentX = percentX,
                percentY = percentY = percentHeight,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedX = fixedX + fixedDeltaX + fixedWidth + fixedPaddingX * 2,
                fixedY = fixedY + fixedDeltaY,
                fixedWidth = fixedWidth + fixedDeltaWidth,
                fixedHeight = fixedHeight + fixedDeltaHeight,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                fixedMarginX = fixedMarginX,
                fixedMarginY = fixedMarginY,
                percentPaddingX = percentPaddingX,
                percentPaddingY = percentPaddingY,
                parentBounds = parentBounds,
            };
        }



        /// <summary>
        /// Creates a clone of the bounds but without child elements
        /// </summary>
        /// <returns></returns>
        public ElementBounds FlatCopy()
        {
            return new ElementBounds()
            {
                alignment = alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedX = fixedX,
                fixedY = fixedY,
                fixedWidth = fixedWidth,
                fixedHeight = fixedHeight,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                fixedMarginX = fixedMarginX,
                fixedMarginY = fixedMarginY,
                percentPaddingX = percentPaddingX,
                percentPaddingY = percentPaddingY,
                parentBounds = parentBounds
            };
        }





        public ElementBounds ForkChild()
        {
            return ForkChildOffseted();
        }

        public ElementBounds ForkChildOffseted(double fixedDeltaX = 0, double fixedDeltaY = 0, double fixedDeltaWidth = 0, double fixedDeltaHeight = 0)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                percentHeight = percentHeight,
                percentWidth = percentHeight,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedX = fixedX + fixedDeltaX,
                fixedY = fixedY + fixedDeltaY,
                fixedWidth = fixedWidth + fixedDeltaWidth,
                fixedHeight = fixedHeight + fixedDeltaHeight,
                fixedPaddingX = fixedPaddingX,
                fixedPaddingY = fixedPaddingY,
                percentPaddingX = percentPaddingX,
                percentPaddingY = percentPaddingY,
                parentBounds = this
            };

        }

        /// <summary>
        /// Creates a new elements bounds which acts as the parent bounds of the current bounds. It will also arrange the fixedX/Y and Width/Height coords of both bounds so that the parent bounds surrounds the child bounds with given spacings. Uses fixed coords only!
        /// </summary>
        /// <param name="leftSpacing"></param>
        /// <param name="topSpacing"></param>
        /// <param name="rightSpacing"></param>
        /// <param name="bottomSpacing"></param>
        /// <returns></returns>
        public ElementBounds ForkBoundingParent(double leftSpacing = 0, double topSpacing = 0, double rightSpacing = 0, double bottomSpacing = 0)
        {
            ElementBounds bounds = new ElementBounds()
            {
                alignment = alignment,
                verticalSizing = verticalSizing,
                horizontalSizing = horizontalSizing,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedWidth = fixedWidth + 2 * fixedPaddingX + leftSpacing + rightSpacing,
                fixedHeight = fixedHeight + 2 * fixedPaddingY + topSpacing + bottomSpacing,
                fixedX = fixedX,
                fixedY = fixedY,
                percentHeight = percentHeight,
                percentWidth = percentWidth
            };

            fixedX = leftSpacing;
            fixedY = topSpacing;
            percentWidth = 1;
            percentHeight = 1;

            parentBounds = bounds;

            return bounds;
        }



        public override string ToString()
        {
            return absX + "/" + absY + " -> " + (absX + OuterWidth) + " / " + (absY + OuterHeight);
        }

        public ElementBounds FixedUnder(ElementBounds bounds, double spacing)
        {
            fixedY += bounds.fixedY + bounds.fixedHeight + spacing;
            return this;
        }

        public ElementBounds FixedRightOf(ElementBounds leftBounds, double leftSpacing)
        {
            fixedX = leftBounds.fixedX + leftBounds.fixedWidth + leftSpacing;
            return this;
        }

        public ElementBounds FixedLeftOf(ElementBounds leftBounds, double rightSpacing)
        {
            fixedX = leftBounds.fixedX - leftBounds.fixedWidth - rightSpacing;
            return this;
        }

        public ElementBounds WithFixedSize(double width, double height)
        {
            fixedWidth = width;
            fixedHeight = height;
            return this;
        }


        public ElementBounds WithFixedWidth(double width)
        {
            fixedWidth = width;
            return this;
        }

        public ElementBounds WithFixedHeight(double height)
        {
            fixedHeight = height;
            return this;
        }

        public ElementBounds WithAlignment(ElementAlignment alignment)
        {
            this.alignment = alignment;
            return this;
        }

        public ElementBounds WithSizing(ElementSizing sizing)
        {
            this.verticalSizing = sizing;
            this.horizontalSizing = sizing;
            return this;
        }

        public ElementBounds WithSizing(ElementSizing horizontalSizing, ElementSizing verticalSizing)
        {
            this.verticalSizing = verticalSizing;
            this.horizontalSizing = horizontalSizing;
            return this;
        }

        /// <summary>
        /// Sets a new fixed margin (pad = top/right/down/left margin)
        /// </summary>
        /// <param name="pad"></param>
        /// <returns></returns>
        public ElementBounds WithFixedMargin(double pad)
        {
            this.fixedMarginX = pad;
            this.fixedMarginY = pad;
            return this;
        }

        /// <summary>
        /// Sets a new fixed margin (pad = top/right/down/left margin)
        /// </summary>
        /// <param name="pad"></param>
        /// <returns></returns>
        public ElementBounds WithFixedMargin(double padH, double padV)
        {
            this.fixedMarginX = padH;
            this.fixedMarginY = padV;
            return this;
        }



        /// <summary>
        /// Sets a new fixed padding (pad = top/right/down/left padding)
        /// </summary>
        /// <param name="pad"></param>
        /// <returns></returns>
        public ElementBounds WithFixedPadding(double pad)
        {
            this.fixedPaddingX = pad;
            this.fixedPaddingY = pad;
            return this;
        }

        /// <summary>
        /// Sets a new fixed padding (x = left/right, y = top/down padding)
        /// </summary>
        /// <param name="leftRight"></param>
        /// <param name="upDown"></param>
        /// <returns></returns>
        public ElementBounds WithFixedPadding(double leftRight, double upDown)
        {
            this.fixedPaddingX = leftRight;
            this.fixedPaddingY = upDown;
            return this;
        }


        /// <summary>
        /// Sets a new fixed offset that is applied after element alignment. So you could i.e. horizontally center an element and then offset in x direction  from there using this method.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public ElementBounds WithFixedAlignmentOffset(double x, double y)
        {
            this.fixedOffsetX = x;
            this.fixedOffsetY = y;
            return this;
        }


        /// <summary>
        /// Sets a new fixed offset that is used during element alignment.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public ElementBounds WithFixedPosition(double x, double y)
        {
            this.fixedX = x;
            this.fixedY = y;
            return this;
        }

        /// <summary>
        /// Sets a new fixed offset that is used during element alignment.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public ElementBounds WithAddedFixedPosition(double offx, double offy)
        {
            this.fixedX += offx;
            this.fixedY += offy;
            return this;
        }


        /// <summary>
        /// Shrinks the current width/height by a fixed value
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ElementBounds FixedShrink(double amount)
        {
            fixedWidth -= amount;
            fixedHeight -= amount;

            return this;
        }

        /// <summary>
        /// Grows the current width/height by a fixed value
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ElementBounds FixedGrow(double amount)
        {
            fixedWidth += amount;
            fixedHeight += amount;

            return this;
        }

        /// <summary>
        /// Grows the current width/height by a fixed value
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ElementBounds FixedGrow(double width, double height)
        {
            fixedWidth += width;
            fixedHeight += height;

            return this;
        }


        /// <summary>
        /// Sets the parent of the bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public ElementBounds WithParent(ElementBounds bounds)
        {
            parentBounds = bounds;
            return this;
        }

        /// <summary>
        /// Creates a new bounds using FitToChildren and sets that as bound parent. This is usefull if you want to draw elements that are not part of the dialog
        /// </summary>
        /// <returns></returns>
        public ElementBounds WithEmptyParent()
        {
            parentBounds = Empty;
            return this;
        }


        public static ElementBounds Fixed(int fixedX, int fixedY)
        {
            return Fixed(fixedX, fixedY, 0, 0);
        }

        /// <summary>
        /// Quick Method to create a new ElementBounds instance that fills 100% of its parent bounds. Useful for backgrounds.
        /// </summary>
        public static ElementBounds Fill
        {
            get
            {
                return new ElementBounds() { alignment = ElementAlignment.None, bothSizing = ElementSizing.Percentual, percentWidth = 1, percentHeight = 1 };
            }
        }


        public static ElementBounds FixedPos(ElementAlignment alignment, double fixedX, double fixedY)
        {
            return new ElementBounds() { alignment = alignment, bothSizing = ElementSizing.Fixed, fixedX = fixedX, fixedY = fixedY };
        }





        /// <summary>
        /// Quick method to create a new ElementBounds instance that uses fixed element sizing. The X/Y Coordinates are left at 0. 
        /// </summary>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds FixedSize(double fixedWidth, double fixedHeight)
        {
            return new ElementBounds() { alignment = ElementAlignment.None, fixedWidth = fixedWidth, fixedHeight = fixedHeight, bothSizing = ElementSizing.Fixed };
        }

        /// <summary>
        /// Quick method to create a new ElementBounds instance that uses fixed element sizing. The X/Y Coordinates are left at 0. 
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds FixedSize(ElementAlignment alignment, double fixedWidth, double fixedHeight)
        {
            return new ElementBounds() { alignment = alignment, fixedWidth = fixedWidth, fixedHeight = fixedHeight, bothSizing = ElementSizing.Fixed };
        }

        /// <summary>
        /// Quick method to create new ElementsBounds instance that uses fixed element sizing.
        /// </summary>
        /// <param name="fixedX"></param>
        /// <param name="fixedY"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds Fixed(double fixedX, double fixedY, double fixedWidth, double fixedHeight)
        {
            return new ElementBounds() { fixedX = fixedX, fixedY = fixedY, fixedWidth = fixedWidth, fixedHeight = fixedHeight, bothSizing = ElementSizing.Fixed };
        }


        /// <summary>
        /// Quick method to create new ElementsBounds instance that uses fixed element sizing.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="fixedX"></param>
        /// <param name="fixedY"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds FixedOffseted(ElementAlignment alignment, double fixedOffsetX, double fixedOffsetY, double fixedWidth, double fixedHeight)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                fixedOffsetX = fixedOffsetX,
                fixedOffsetY = fixedOffsetY,
                fixedWidth = fixedWidth,
                fixedHeight = fixedHeight,
                bothSizing = ElementSizing.Fixed
            };
        }

        /// <summary>
        /// Quick method to create new ElementsBounds instance that uses fixed element sizing.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="fixedX"></param>
        /// <param name="fixedY"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        public static ElementBounds Fixed(ElementAlignment alignment, double fixedX, double fixedY, double fixedWidth, double fixedHeight)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                fixedX = fixedX,
                fixedY = fixedY,
                fixedWidth = fixedWidth,
                fixedHeight = fixedHeight,
                bothSizing = ElementSizing.Fixed
            };
        }

        /// <summary>
        /// Quick method to create new ElementsBounds instance that uses percentual element sizing, e.g. setting percentWidth to 0.5 will set the width of the bounds to 50% of its parent width
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="percentWidth"></param>
        /// <param name="percentHeight"></param>
        /// <returns></returns>
        public static ElementBounds Percentual(ElementAlignment alignment, double percentWidth, double percentHeight)
        {
            return new ElementBounds()
            {
                alignment = alignment,
                percentWidth = percentWidth,
                percentHeight = percentHeight,
                bothSizing = ElementSizing.Percentual
            };
        }

        /// <summary>
        /// Quick method to create new ElementsBounds instance that uses percentual element sizing, e.g. setting percentWidth to 0.5 will set the width of the bounds to 50% of its parent width
        /// </summary>
        /// <param name="percentX"></param>
        /// <param name="percentY"></param>
        /// <param name="percentWidth"></param>
        /// <param name="percentHeight"></param>
        /// <returns></returns>
        public static ElementBounds Percentual(double percentX, double percentY, double percentWidth, double percentHeight)
        {
            return new ElementBounds()
            {
                percentX = percentX,
                percentY = percentY,
                percentWidth = percentWidth,
                percentHeight = percentHeight,
                bothSizing = ElementSizing.Percentual
            };
        }


        public static ElementBounds Empty
        {
            get
            {
                return new ElementEmptyBounds();
            }
        }

    }
}