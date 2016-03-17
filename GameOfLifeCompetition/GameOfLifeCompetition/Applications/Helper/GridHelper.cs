using System.Windows;
using System.Windows.Media;

namespace GameOfLiveCompetition.Applications.Helper
{
    /// <summary>
    /// Helper to Handle the Grid Class 
    /// </summary>
    /// <author>Christian Hahn</author>
    public static class GridHelper
    {
        /// <summary>
        /// Helper to Change the Color of an UIElement
        /// </summary>
        /// <param name="cellElement">the Element</param>
        /// <param name="color">the new Color in Hex e.g."#ff0000"</param>
        public static void SetColorOfCell(this UIElement cellElement, string color)
        {
            BrushConverter bc = new BrushConverter();
            cellElement.GetType().GetProperty("Background").SetValue(cellElement, bc.ConvertFrom(color));
        }

        /// <summary>
        /// Helper to Change the Color of an UIElement
        /// </summary>
        /// <param name="cellElement">the Element</param>
        public static string GetColorOfCell(this UIElement cellElement)
        {
            return cellElement.GetType().GetProperty("Background").GetValue(cellElement).ToString().ToLower();
        }
    }
}
