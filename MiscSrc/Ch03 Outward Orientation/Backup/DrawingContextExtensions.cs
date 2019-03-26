using System.Windows;
using System.Windows.Media;

using System.Linq;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace Interlocked
{
    public enum VertAlignment
    {
        Top,
        Middle,
        Bottom,
    }

    public static class DrawingContextExtensions
    {
        // Draw text at the indicated location.
        public static void DrawString(this DrawingContext drawing_context,
            string text, string font_name, double em_size, Brush brush,
            Point origin, VertAlignment valign, TextAlignment halign)
        {
            Typeface typeface = new Typeface(font_name);
            FormattedText formatted_text = new FormattedText(
                text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                typeface, em_size, brush);
            formatted_text.TextAlignment = halign;

            if (valign == VertAlignment.Middle) origin.Y -= formatted_text.Height / 2;
            else if (valign == VertAlignment.Bottom) origin.Y -= formatted_text.Height;

            drawing_context.DrawText(formatted_text, origin);
        }

        // Draw rotated text at the indicated location.
        public static void DrawRotatedString(this DrawingContext drawing_context,
            string text, double angle, string font_name, double em_size, Brush brush,
            Point origin, TextAlignment text_align, VertAlignment valign, TextAlignment halign)
        {
            Typeface typeface = new Typeface(font_name);
            FormattedText formatted_text = new FormattedText(
                text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                typeface, em_size, brush);
            formatted_text.TextAlignment = text_align;

            // Make a transformation to center the text.
            double width = formatted_text.Width - formatted_text.OverhangLeading;
            double height = formatted_text.Height;
            TranslateTransform translate1 = new TranslateTransform();
            translate1.Y = -height / 2;
            if ((text_align == TextAlignment.Left) ||
                (text_align == TextAlignment.Justify))
                translate1.X = -width / 2;
            else if (text_align == TextAlignment.Right) translate1.X = width / 2;
            else translate1.X = 0;

            // Make a transformation to rotate the text.
            RotateTransform rotate = new RotateTransform(angle);

            // Get the text's bounding rectangle.
            Rect rect = new Rect(0, 0, width, height);
            if (text_align == TextAlignment.Center) rect.X -= width / 2;
            else if (text_align == TextAlignment.Right) rect.X -= width;

            // Get the rotated bounding rectangle.
            Rect rotated_rect = rotate.TransformBounds(rect);

            // Make a transformation to center the
            // bounding rectangle at the destination.
            TranslateTransform translate2 = new TranslateTransform(origin.X, origin.Y);

            // Adjust the translation for the desired alignment.
            if (halign == TextAlignment.Left)
                translate2.X += rotated_rect.Width / 2;
            else if (halign == TextAlignment.Right)
                translate2.X -= rotated_rect.Width / 2;
            if (valign == VertAlignment.Top)
                translate2.Y += rotated_rect.Height / 2;
            else if (valign == VertAlignment.Bottom)
                translate2.Y -= rotated_rect.Height / 2;

            // Push transformations in reverse order.
            drawing_context.PushTransform(translate2);
            drawing_context.PushTransform(rotate);
            drawing_context.PushTransform(translate1);

            // Draw.
            drawing_context.DrawText(formatted_text, new Point(0, 0));

            // Draw a rectangle around the text. (For debugging.)
            drawing_context.DrawRectangle(null, new Pen(Brushes.Red, 1), rect);

            // Remove the transformations.
            drawing_context.Pop();
            drawing_context.Pop();
            drawing_context.Pop();

            // Draw the rotated bounding rectangle. (For debugging.)
            Rect transformed_rect =
                translate2.TransformBounds(
                    rotate.TransformBounds(
                        translate1.TransformBounds(rect)));
            Pen custom_pen = new Pen(Brushes.Blue, 1);
            custom_pen.DashStyle = new DashStyle(
                new double[] { 5, 5 }, 0);
            drawing_context.DrawRectangle(null, custom_pen, transformed_rect);
        }

        // Draw a polygon or polyline.
        private static void DrawPolygonOrPolyline(
            this DrawingContext drawingContext,
            Brush brush, Pen pen, Point[] points, FillRule fill_rule,
            bool draw_polygon)
        {
            // Make a StreamGeometry to hold the drawing objects.
            StreamGeometry geo = new StreamGeometry();
            geo.FillRule = fill_rule;

            // Open the context to use for drawing.
            using (StreamGeometryContext context = geo.Open())
            {
                // Start at the first point.
                context.BeginFigure(points[0], true, draw_polygon);

                // Add the points after the first one.
                context.PolyLineTo(points.Skip(1).ToArray(), true, false);
            }

            // Draw.
            drawingContext.DrawGeometry(brush, pen, geo);
        }

        // Draw a polygon.
        public static void DrawPolygon(this DrawingContext drawingContext,
            Brush brush, Pen pen, Point[] points, FillRule fill_rule)
        {
            drawingContext.DrawPolygonOrPolyline(brush, pen, points, fill_rule, true);
        }

        // Draw a polyline.
        public static void DrawPolyline(this DrawingContext drawingContext,
            Brush brush, Pen pen, Point[] points, FillRule fill_rule)
        {
            drawingContext.DrawPolygonOrPolyline(brush, pen, points, fill_rule, false);
        }

        // Render text onto a bitmap.
        public static RenderTargetBitmap RenderTextOntoBitmap(
            string text, double dpiX, double dpiY,
            string font_name, double em_size, TextAlignment text_align,
            Brush bg_brush, Pen rect_pen, Brush text_brush,
            Thickness margin)
        {
            // Make the Typeface and FormattedText.
            Typeface typeface = new Typeface(font_name);
            FormattedText formatted_text = new FormattedText(
                text, CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, typeface,
                em_size, text_brush);
            formatted_text.TextAlignment = text_align;

            // See how big we need the result to be.
            double width = formatted_text.Width - formatted_text.OverhangLeading;
            double height = formatted_text.Height;

            // See where we need to draw the text to
            // position it with upper left corner at (0, 0).
            Point text_origin = new Point(0, 0);
            if (text_align == TextAlignment.Center) text_origin.X = width / 2;
            else if (text_align == TextAlignment.Right) text_origin.X = width;

            // Add room for the bounding rectangle.
            if (rect_pen != null)
            {
                width += rect_pen.Thickness;
                height += rect_pen.Thickness;
                text_origin.X += rect_pen.Thickness / 2;
                text_origin.Y += rect_pen.Thickness / 2;
            }

            // Add the margin.
            width += margin.Left + margin.Right;
            height += margin.Top + margin.Bottom;
            text_origin.X += margin.Left;
            text_origin.Y += margin.Top;

            // Add 1 to width and height to the bottom and
            // right edges of the rectangle aren't clipped.
            int width_int = (int)(width + 1);
            int height_int = (int)(height + 1);

            // Make a DrawingVisual and draw on it.
            DrawingVisual drawing_visual = new DrawingVisual();
            DrawingContext drawing_context = drawing_visual.RenderOpen();
            drawing_context.DrawRectangle(bg_brush,
                rect_pen, new Rect(0, 0, width_int, height_int));
            drawing_context.DrawText(formatted_text, text_origin);
            drawing_context.Close();

            // Copy the result onto a RenderTargetBitmap.
            RenderTargetBitmap bm = new RenderTargetBitmap(
                width_int, height_int, dpiX, dpiY, PixelFormats.Pbgra32);
            bm.Render(drawing_visual);

            return bm;
        }
    }
}
