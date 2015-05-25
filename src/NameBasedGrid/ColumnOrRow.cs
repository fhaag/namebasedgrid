/*
------------------------------------------------------------------------------
This source file is a part of Name-Based Grid.

Copyright (c) 2015 Florian Haag

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
------------------------------------------------------------------------------
 */
using System;
using System.Windows;

namespace NameBasedGrid
{
	public sealed class ColumnOrRow : ColumnOrRowBase
	{
		public ColumnOrRow()
		{
		}
		
		public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size",
		                                                                                     typeof(GridLength),
		                                                                                     typeof(ColumnOrRow),
		                                                                                     new FrameworkPropertyMetadata(new GridLength(1.0, GridUnitType.Star), OnSizeChanged));
		
		private static void OnSizeChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((ColumnOrRow)source).OnSizeChanged(e);
		}
		
		private void OnSizeChanged(DependencyPropertyChangedEventArgs e)
		{
			OnPropertyChanged(ColumnOrRowProperty.Size);
		}
		
		public GridLength Size {
			get {
				return (GridLength)GetValue(SizeProperty);
			}
			set {
				SetValue(SizeProperty, value);
			}
		}
		
		public static readonly DependencyProperty SharedSizeGroupProperty = DependencyProperty.Register("SharedSizeGroup",
		                                                                                                typeof(string),
		                                                                                                typeof(ColumnOrRow),
		                                                                                                new FrameworkPropertyMetadata(OnSharedSizeGroupChanged));
		
		private static void OnSharedSizeGroupChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((ColumnOrRow)source).OnSharedSizeGroupChanged(e);
		}
		
		private void OnSharedSizeGroupChanged(DependencyPropertyChangedEventArgs e)
		{
			OnPropertyChanged(ColumnOrRowProperty.SharedSizeGroup);
		}
		
		public string SharedSizeGroup {
			get {
				return (string)GetValue(SharedSizeGroupProperty);
			}
			set {
				SetValue(SharedSizeGroupProperty, value);
			}
		}
	}
}
