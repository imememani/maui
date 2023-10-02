using System;
using System.Diagnostics.CodeAnalysis;
using CoreGraphics;
using UIKit;

namespace Microsoft.Maui.Platform
{
	public class MauiImageView : UIImageView, IUIViewLifeCycleEvents
	{
		bool _isDisposed;
		WeakReference<MauiCAKeyFrameAnimation>? _animation;

		readonly WeakReference<IImageHandler>? _handler;

		public MauiImageView(IImageHandler handler) => _handler = new(handler);

		[Obsolete("Use MauiImageView(IImageHandler handler) instead.")]
		public MauiImageView()
		{
		}

		[Obsolete("Use MauiImageView(IImageHandler handler) instead.")]
		public MauiImageView(CGRect frame)
			: base(frame)
		{
		}

		[UnconditionalSuppressMessage("Memory", "MEM0002", Justification = IUIViewLifeCycleEvents.UnconditionalSuppressMessage)]
		EventHandler? _movedToWindow;
		event EventHandler IUIViewLifeCycleEvents.MovedToWindow
		{
			add => _movedToWindow += value;
			remove => _movedToWindow -= value;
		}

		public override void MovedToWindow()
		{
			if (_handler is not null && _handler.TryGetTarget(out var handler))
			{
				handler.OnWindowChanged();
			}
			_movedToWindow?.Invoke(this, EventArgs.Empty);
		}

		[Obsolete("Use IImageHandler.OnWindowChanged() instead.")]
		public event EventHandler? WindowChanged
		{
			add { }
			remove { }
		}

		public override UIImage? Image
		{
			get
			{
				return base.Image;
			}
			set
			{
				base.Image = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			_isDisposed = true;

			if (disposing && _animation?.TryGetTarget(out var animation) != null)
			{
				AnimationImages = null;
				animation?.Dispose();
				_animation = null;
			}

			base.Dispose(disposing);
		}
	}
}