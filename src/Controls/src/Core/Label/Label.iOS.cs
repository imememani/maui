#nullable disable
using System;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Graphics;
using UIKit;

namespace Microsoft.Maui.Controls
{
	public partial class Label
	{
		protected override Size ArrangeOverride(Rect bounds)
		{
			var size = base.ArrangeOverride(bounds);

			RecalculateSpanPositions();

			return size;
		}

		public static void MapText(LabelHandler handler, Label label) => MapText((ILabelHandler)handler, label);

		public static void MapTextDecorations(ILabelHandler handler, Label label) =>
			MapTextDecorations(handler, label, (h, v) => LabelHandler.MapTextDecorations(handler, label));

		public static void MapCharacterSpacing(ILabelHandler handler, Label label) =>
			MapCharacterSpacing(handler, label, (h, v) => LabelHandler.MapCharacterSpacing(handler, label));

		public static void MapLineHeight(ILabelHandler handler, Label label) =>
			MapLineHeight(handler, label, (h, v) => LabelHandler.MapLineHeight(handler, label));

		public static void MapFont(ILabelHandler handler, Label label) =>
			MapFont(handler, label, (h, v) => LabelHandler.MapFont(handler, label));

		public static void MapTextColor(ILabelHandler handler, Label label) =>
			MapTextColor(handler, label, (h, v) => LabelHandler.MapTextColor(handler, label));

		public static void MapTextType(ILabelHandler handler, Label label)
		{
			handler.UpdateValue(nameof(ILabel.Text));
		}

		public static void MapText(ILabelHandler handler, Label label)
		{
			Platform.LabelExtensions.UpdateText(handler.PlatformView, label);

			MapFormatting(handler, label);
		}

		public static void MapLineBreakMode(ILabelHandler handler, Label label)
		{
			handler.PlatformView?.UpdateLineBreakMode(label);
		}

		public static void MapMaxLines(ILabelHandler handler, Label label)
		{
			handler.PlatformView?.UpdateMaxLines(label);
		}

		static void MapFormatting(ILabelHandler handler, Label label)
		{
			// we need to re-apply color and font for HTML labels
			if (!label.HasFormattedTextSpans && label.TextType == TextType.Html)
			{
				handler.UpdateValue(nameof(ILabel.TextColor));
				handler.UpdateValue(nameof(ILabel.Font));
			}

			if (!IsPlainText(label))
				return;

			LabelHandler.MapFormatting(handler, label);
		}

		void RecalculateSpanPositions()
		{
			if (Handler is LabelHandler labelHandler)
			{
				if (labelHandler.PlatformView is not UILabel platformView || labelHandler.VirtualView is not Label virtualView)
					return;

				platformView.RecalculateSpanPositions(virtualView);
			}
		}
	}
}