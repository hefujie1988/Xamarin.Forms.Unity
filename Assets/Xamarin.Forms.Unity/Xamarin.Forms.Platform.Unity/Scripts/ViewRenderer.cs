﻿using System;
using System.ComponentModel;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	public class ViewRenderer<TElement, TNativeElement> :
		VisualElementRenderer<TElement, TNativeElement>
		where TElement : View
		where TNativeElement : UnityEngine.Component
	{
		/*-----------------------------------------------------------------*/
		#region Field



		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == View.HorizontalOptionsProperty.PropertyName ||
				e.PropertyName == View.VerticalOptionsProperty.PropertyName)
			{
				UpdateNativeControl();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				UpdateBackgroundColor();
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		protected override void UpdateNativeControl()
		{
			base.UpdateNativeControl();

			UpdateLayout();
		}

		protected virtual void UpdateLayout()
		{
			var view = Element;
			if (_rectTransform == null || view == null)
			{
				return;
			}

			var anchorMax = new Vector2();
			var anchorMin = new Vector2();

			switch (view.HorizontalOptions.Alignment)
			{
				case LayoutAlignment.Start:
					{
						anchorMin.x = 1.0f;
						anchorMax.x = 1.0f;
					}
					break;

				case LayoutAlignment.Center:
					{
						anchorMin.x = 0.5f;
						anchorMax.x = 0.5f;
					}
					break;

				case LayoutAlignment.End:
					{
						anchorMin.x = 0.0f;
						anchorMax.x = 0.0f;
					}
					break;

				case LayoutAlignment.Fill:
					{
						anchorMin.x = 0.0f;
						anchorMax.x = 1.0f;
					}
					break;
			}

			switch (view.VerticalOptions.Alignment)
			{
				case LayoutAlignment.Start:
					{
						anchorMin.y = 1.0f;
						anchorMax.y = 1.0f;
					}
					break;

				case LayoutAlignment.Center:
					{
						anchorMin.y = 0.5f;
						anchorMax.y = 0.5f;
					}
					break;

				case LayoutAlignment.End:
					{
						anchorMin.y = 0.0f;
						anchorMax.y = 0.0f;
					}
					break;

				case LayoutAlignment.Fill:
					{
						anchorMin.y = 0.0f;
						anchorMax.y = 1.0f;
					}
					break;
			}

			_rectTransform.anchorMin = anchorMin;
			_rectTransform.anchorMax = anchorMax;
			_rectTransform.position = new Vector3();

			var parent = view.Parent as VisualElement;
			if (parent != null && _rectTransform.parent != null)
			{
				var pivot = _rectTransform.pivot;
				var size = new Vector2((float)view.Width, (float)view.Height);
				_rectTransform.sizeDelta = size;
				_rectTransform.anchoredPosition = new Vector2((float)view.X + size.x * pivot.x, (float)(parent.Height - view.Y) - size.y * pivot.y);
			}
			else
			{
				_rectTransform.sizeDelta = new Vector2();
				_rectTransform.anchoredPosition = new Vector2();
			}
		}

		#endregion

	}
}