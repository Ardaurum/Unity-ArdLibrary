using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace ArdEditor.EditorTools
{
    public static class EditorRectUtilities
    {
        public static readonly Rect InfinityRect = new Rect(
            float.NegativeInfinity, 
            float.NegativeInfinity,
            float.PositiveInfinity,
            float.PositiveInfinity);

        public static Rect FitToLine(this Rect rect)
        {
            rect.height = EditorGUIUtility.singleLineHeight - 2.0f;
            rect.y += 1.0f;
            return rect;
        }

        public static Rect MoveBelow(this Rect rect)
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            return rect;
        }

        public static Rect MoveBelow(this Rect rect, Rect otherRect)
        {
            rect.y = otherRect.y + otherRect.height + EditorGUIUtility.standardVerticalSpacing;
            return rect;
        }

        public static Rect[] DivideIntoColumns(this Rect rect, int amount)
        {
            return rect.DivideIntoColumns(amount, Array.Empty<float>());
        }

        public static Rect[] DivideIntoColumns(this Rect rect, int amount, params float[] relativeSizes)
        {
            Assert.Greater(amount, 0);
            Assert.LessOrEqual(relativeSizes.Length, amount);

            float standardRelativeSize = 1.0f / amount;
            float sum = standardRelativeSize * (amount - relativeSizes.Length);
            for (var i = 0; i < relativeSizes.Length; ++i)
            {
                sum += relativeSizes[i];
            }

            var rects = new Rect[amount];
            float currentX = rect.x;
            for (var i = 0; i < amount; i++)
            {
                float relativeWidth = (i < relativeSizes.Length ? relativeSizes[i] : standardRelativeSize) / sum;
                rects[i] = rect;
                rects[i].width = rect.width * relativeWidth;
                rects[i].x = currentX;
                currentX += rects[i].width;
            }

            return rects;
        }
    }
}