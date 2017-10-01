﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using UnityEngine;
using UniRx;

namespace Xamarin.Forms.Platform.Unity
{
	public class ScrollViewRenderer : ViewRenderer<ScrollView, UnityEngine.UI.ScrollRect>
	{
		/*-----------------------------------------------------------------*/
		#region Field

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		protected override void Awake()
		{
			base.Awake();

			var scrollRect = UnityComponent;
			if (scrollRect != null)
			{
				var vbar = scrollRect.verticalScrollbar;
				var hbar = scrollRect.horizontalScrollbar;

				vbar?.OnValueChangedAsObservable()
					.BlockReenter()
					.Subscribe(value =>
					{
						var element = Element;
						if (element != null)
						{
							var y = (1.0 - value) * element.ContentSize.Height;
							element.SetScrolledPosition(element.ScrollX, y);

							Debug.Log(string.Format("Unity: vbar={0} -> XF: ScollView.SetScrolledPosition({1}, {2})", value, element.ScrollX, y));
						}
					});

				hbar?.OnValueChangedAsObservable()
					.BlockReenter()
					.Subscribe(value =>
					{
						var element = Element;
						if (element != null)
						{
							var x = (1.0 - value) * element.ContentSize.Width;
							element.SetScrolledPosition(x, element.ScrollY);

							Debug.Log(string.Format("Unity: hbar={0} -> XF: ScollView.SetScrolledPosition({1}, {2})", value, x, element.ScrollY));
						}
					});
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region IVisualElementRenderer

		public override Transform UnityContainerTransform => UnityComponent?.content;

		public override Vector2 GetChildAnchorPoint(IVisualElementRenderer child)
		{
			if (child == null)
			{
				return new Vector2();
			}

			//	ScrollView の content に入れる子は親からの補正は不要
			return child.GetAnchorPoint();
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<ScrollView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				UpdateOrientation();
				UpdateContentSize();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == ScrollView.OrientationProperty.PropertyName)
			{
				UpdateOrientation();
			}
			else if (e.PropertyName == ScrollView.ScrollXProperty.PropertyName)
			{
				UpdateScrollXPosition();
			}
			else if (e.PropertyName == ScrollView.ScrollYProperty.PropertyName)
			{
				UpdateScrollYPosition();
			}
			else if (e.PropertyName == ScrollView.ContentSizeProperty.PropertyName)
			{
				UpdateContentSize();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateOrientation()
		{
			var element = Element;
			var scroll = UnityComponent;
			if (element != null)
			{
				switch (element.Orientation)
				{
					case ScrollOrientation.Vertical:
						{
							scroll.horizontal = false;
							scroll.vertical = true;
						}
						break;

					case ScrollOrientation.Horizontal:
						{
							scroll.horizontal = true;
							scroll.vertical = false;
						}
						break;

					case ScrollOrientation.Both:
						{
							scroll.horizontal = true;
							scroll.vertical = true;
						}
						break;
				}
			}
		}

		void UpdateScrollXPosition()
		{
			var element = Element;
			var hbar = UnityComponent?.horizontalScrollbar;

			if (element != null && hbar != null)
			{
				var size = element.ContentSize;
				if (size.Width > 0.0)
				{
					hbar.value = (float)(1.0f - element.ScrollX / size.Width);
					Debug.Log(string.Format("XF: {0} -> Unity: hbar.value = {1}", element.ScrollX, hbar.value));
				}
			}
		}

		void UpdateScrollYPosition()
		{
			var element = Element;
			var vbar = UnityComponent?.verticalScrollbar;

			if (element != null && vbar != null)
			{
				var size = element.ContentSize;
				if (size.Height > 0.0)
				{
					vbar.value = (float)(1.0f - element.ScrollY / size.Height);
					Debug.Log(string.Format("XF: {0} -> Unity: vbar.value = {1}", element.ScrollY, vbar.value));
				}
			}
		}

		void UpdateContentSize()
		{
			var element = Element;
			var content = UnityComponent?.content;
			if (element != null && content != null)
			{
				var size = element.ContentSize;

				var pivot = content.pivot;
				content.anchorMin = new Vector2();
				content.anchorMax = new Vector2();
				content.anchoredPosition = new Vector2();
				content.pivot = new Vector2();
				content.sizeDelta = new Vector2((float)size.Width, (float)size.Height);
			}
			UpdateScrollXPosition();
			UpdateScrollYPosition();
		}

		#endregion
	}
}
