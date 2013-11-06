namespace Assets.Scripts.UI
{
    using UnityEngine;

    /// <summary>
    /// Provides operations for drawing GUI-Objects with automatic font-resizing etc.
    /// </summary>
    public class GUIOperations
    {
        /// <summary>
        /// Draws Label with content centered on the Screen.
        /// The label size fits around the content with the specified font size.
        /// </summary>
        /// <param name="fontSize">the fontSize</param>
        /// <param name="content">the string to be displayed</param>
        public static void DrawLabelCentered(int fontSize, string content)
        {
            DrawLabelCenteredAt(Screen.width / 2, Screen.height / 2, fontSize, content);
        }

        /// <summary>
        /// Draws Label with content at the specified position.
        /// The label size fits around the content with the specified font size.
        /// </summary>
        /// <param name="x">x-coordinate of the label-center</param>
        /// <param name="y">y-coordinate of the label-center</param>
        /// <param name="fontSize">the fontSize</param>
        /// <param name="content">the string to be displayed</param>
        public static void DrawLabelCenteredAt(int x, int y, int fontSize, string content)
        {
            DrawLabelCenteredAt(x, y, fontSize, content, null);
        }

        /// <summary>
        /// Draws Label with content at the specified position.
        /// The label size fits around the content with the specified font size.
        /// The label is drawn in the specified style.
        /// </summary>
        /// <param name="x">x-coordinate of the label-center</param>
        /// <param name="y">y-coordinate of the label-center</param>
        /// <param name="fontSize">the fontSize</param>
        /// <param name="content">the string to be displayed</param>
        /// <param name="style">the style used to draw</param>
        public static void DrawLabelCenteredAt(int x, int y, int fontSize, string content, GUIStyle style)
        {
            int prevFontSize = GUI.skin.label.fontSize;
            GUI.skin.label.fontSize = fontSize;
            int width = (int)GUI.skin.label.CalcSize(new GUIContent(content)).x;
            int height = (int)GUI.skin.label.CalcSize(new GUIContent(content)).y;
            if (style == null)
            {
                GUI.Label(new Rect(x - (width / 2), y - (height / 2), width, height), content);
            }
            else
            {
                GUI.Label(new Rect(x - (width / 2), y - (height / 2), width, height), content, style);
            }

            GUI.skin.label.fontSize = prevFontSize;
        }

        /// <summary>
        /// Draws Button with content at the specified position.
        /// The button size fits around the content with the specified font size.
        /// </summary>
        /// <param name="x">x-coordinate of the label-center</param>
        /// <param name="y">y-coordinate of the label-center</param>
        /// <param name="fontSize">the fontSize</param>
        /// <param name="content">the string to be displayed</param>
        /// <returns>button pressed</returns>
        public static bool DrawButtonCenteredAt(int x, int y, int fontSize, string content)
        {
            int prevFontSize = GUI.skin.button.fontSize;
            GUI.skin.button.fontSize = fontSize;
            int width = (int)GUI.skin.button.CalcSize(new GUIContent(content)).x;
            int height = (int)GUI.skin.button.CalcSize(new GUIContent(content)).y;
            bool pressed = GUI.Button(new Rect(x - (width / 2), y - (height / 2), width, height), content);
            GUI.skin.button.fontSize = prevFontSize;
            return pressed;
        }

        /// <summary>
        /// Draws Button with content and size at the specified position.
        /// The font size fits the button size.
        /// </summary>
        /// <param name="x">x-coordinate of the label-center</param>
        /// <param name="y">y-coordinate of the label-center</param>
        /// <param name="width">the buttonwidth</param>
        /// <param name="height">the buttonheight</param>
        /// <param name="content">the string to be displayed</param>
        /// <returns>Button Pressed</returns>
        public static bool DrawButtonCenteredAt(int x, int y, int width, int height, string content)
        {
            int prevFontSize = GUI.skin.button.fontSize;
            GUI.skin.button.fontSize = GetButtonFontSize(width, height, content);
            bool pressed = GUI.Button(new Rect(x - (width / 2), y - (height / 2), width, height), content);
            GUI.skin.button.fontSize = prevFontSize;
            return pressed;
        }

        /// <summary>
        /// Draws Button with content and size at the specified position.
        /// The font size fits Ratio*Size.
        /// </summary>
        /// <param name="x">x-coordinate of the button-center</param>
        /// <param name="y">y-coordinate of the button-center</param>
        /// <param name="width">the buttonwidth</param>
        /// <param name="height">the buttonheight</param>
        /// <param name="textWidthRatio">textWidthRatio*width = textspacewidth</param>
        /// <param name="textHeightRatio">textWidthRatio*height = textspaceheight</param>
        /// <param name="content">the string to be displayed</param>
        /// <returns>Button pressed</returns>
        public static bool DrawButtonCenteredAt(int x, int y, int width, int height, float textWidthRatio, float textHeightRatio, string content)
        {
            int prevFontSize = GUI.skin.button.fontSize;
            GUI.skin.button.fontSize = GetButtonFontSize((int)(textWidthRatio * width), (int)(textHeightRatio * height), content);
            bool pressed = GUI.Button(new Rect(x - (width / 2), y - (height / 2), width, height), content);
            GUI.skin.button.fontSize = prevFontSize;
            return pressed;
        }

        /// <summary>
        /// Draws a Horizontal Slider in a Box with a label
        /// </summary>
        /// <param name="window">the Rect that defines the BoxSize</param>
        /// <param name="name">the string to be displayed</param>
        /// <param name="sliderValue">the current slider value</param>
        /// <param name="sliderMin">the minimal Slider Value</param>
        /// <param name="sliderMax">the maximal Slider Value</param>
        /// <returns>the slider value</returns>
        public static float LabeledSlider(Rect window, string name, float sliderValue, float sliderMin, float sliderMax)
        {
            GUI.skin.box.alignment = TextAnchor.UpperCenter;
            GUI.Box(window, name);
            float offset = window.width / 10;
            float sliderWidth = window.width - (2 * offset);
            float sliderHeight = window.height / 5;
            sliderValue = GUI.HorizontalSlider(new Rect(window.xMin + offset, window.yMin + (window.height / 2), sliderWidth, sliderHeight), sliderValue, sliderMin, sliderMax);
            return sliderValue;
        }

        /// <summary>
        /// Calculates fontsize for specified width and height.
        /// </summary>
        /// <param name="width">the buttonwidth</param>
        /// <param name="height">the buttonheight</param>
        /// <param name="content">the string to be displayed</param>
        /// <returns>the fitting fontsize</returns>
        private static int GetButtonFontSize(int width, int height, string content)
        {
            int prevFontSize = GUI.skin.button.fontSize;
            for (int size = 1;; size++)
            {
                GUI.skin.button.fontSize = size;
                if ((int)GUI.skin.button.CalcSize(new GUIContent(content)).x > width ||
                    (int)GUI.skin.button.CalcSize(new GUIContent(content)).y > height)
                {
                    GUI.skin.button.fontSize = prevFontSize;
                    return size - 1;
                }
            }
        }
    }
}
