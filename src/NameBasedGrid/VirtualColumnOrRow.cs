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
	public sealed class VirtualColumnOrRow : ColumnOrRowBase
	{
		public VirtualColumnOrRow()
		{
		}
		
		public static readonly DependencyProperty StartAtProperty = DependencyProperty.Register("StartAt",
		                                                                                        typeof(string),
		                                                                                        typeof(VirtualColumnOrRow),
		                                                                                        new FrameworkPropertyMetadata(OnStartAtChanged));
		
		private static void OnStartAtChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((VirtualColumnOrRow)source).OnStartAtChanged(e);
		}
		
		private void OnStartAtChanged(DependencyPropertyChangedEventArgs e)
		{
			
		}
		
		public string StartAt {
			get {
				return (string)GetValue(StartAtProperty);
			}
			set {
				SetValue(StartAtProperty, value);
			}
		}
		
		public static readonly DependencyProperty ExtendToProperty = DependencyProperty.Register("ExtendTo",
		                                                                                         typeof(string),
		                                                                                         typeof(VirtualColumnOrRow),
		                                                                                         new FrameworkPropertyMetadata(OnExtendToChanged));
		
		private static void OnExtendToChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((VirtualColumnOrRow)source).OnExtendToChanged(e);
		}
		
		private void OnExtendToChanged(DependencyPropertyChangedEventArgs e)
		{
			
		}
		
		public string ExtendTo {
			get {
				return (string)GetValue(ExtendToProperty);
			}
			set {
				SetValue(ExtendToProperty, value);
			}
		}
	}
}
