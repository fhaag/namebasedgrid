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
	/// <summary>
	/// Defines a virtual column or a row in a <see cref="T:NameBasedGrid"/>.
	/// </summary>
	/// <remarks>
	/// <para>A virtual column or row does not occupy any space of its own.
	///   Instead, it is defined based on other columns or rows in the same list.
	///   The virtual column or row then serves as an alias for another column or row, or for a range of columns or rows.
	///   That alias can then be used when placing visual elements, or also when defining further virtual columns or rows.</para>
	/// <para>Use the <see cref="StartAt"/> property to indicate where the virtual column or row starts by assigning the name of another column or row.
	///   If the virtual column or row needs to span several columns or rows, use the <see cref="ExtendTo"/> property to indicate its extent.</para>
	/// </remarks>
	public sealed class VirtualColumnOrRow : ColumnOrRowBase
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public VirtualColumnOrRow()
		{
		}
		
		/// <summary>
		/// Identifies the <see cref="P:StartAt"/> property.
		/// </summary>
		/// <seealso cref="P:StartAt"/>
		public static readonly DependencyProperty StartAtProperty = DependencyProperty.Register("StartAt",
		                                                                                        typeof(string),
		                                                                                        typeof(VirtualColumnOrRow),
		                                                                                        new FrameworkPropertyMetadata(OnStartAtChanged));
		
		/// <summary>
		/// Processes a change of the <see cref="P:StartAt"/> property.
		/// </summary>
		/// <param name="source">The instance whose <see cref="P:StartAt"/> property was changed.</param>
		/// <param name="e">An object providing some information about the change.</param>
		private static void OnStartAtChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((VirtualColumnOrRow)source).OnStartAtChanged(e);
		}
		
		/// <summary>
		/// Processes a change of the <see cref="P:StartAt"/> property.
		/// </summary>
		/// <param name="e">An object providing some information about the change.</param>
		private void OnStartAtChanged(DependencyPropertyChangedEventArgs e)
		{
			OnPropertyChanged(ColumnOrRowProperty.StartAt);
		}
		
		/// <summary>
		/// A column or row that the virtual column or row is based on.
		/// </summary>
		/// <value>
		/// <para>Gets or sets the name of the column or row that defines one edge of the virtual column or row.
		///   If <see cref="ExtendTo"/> is <see langword="null"/>, the virtual column or row will be equal to the column or row indicated for <see cref="StartAt"/>.
		///   Otherwise, it will span the whole area between <see cref="StartAt"/> and <see cref="ExtendTo"/>.
		///   In the latter case, the relative order of the two columns or rows is irrelevant.</para>
		/// </value>
		/// <seealso cref="ExtendTo"/>
		public string StartAt {
			get {
				return (string)GetValue(StartAtProperty);
			}
			set {
				SetValue(StartAtProperty, value);
			}
		}
		
		/// <summary>
		/// Identifies the <see cref="P:ExtendTo"/> property.
		/// </summary>
		/// <seealso cref="P:ExtendTo"/>
		public static readonly DependencyProperty ExtendToProperty = DependencyProperty.Register("ExtendTo",
		                                                                                         typeof(string),
		                                                                                         typeof(VirtualColumnOrRow),
		                                                                                         new FrameworkPropertyMetadata(OnExtendToChanged));
		
		/// <summary>
		/// Processes a change of the <see cref="P:ExtendTo"/> property.
		/// </summary>
		/// <param name="source">The instance whose <see cref="P:ExtendTo"/> property was changed.</param>
		/// <param name="e">An object providing some information about the change.</param>
		private static void OnExtendToChanged(object source, DependencyPropertyChangedEventArgs e)
		{
			((VirtualColumnOrRow)source).OnExtendToChanged(e);
		}
		
		/// <summary>
		/// Processes a change of the <see cref="P:ExtendTo"/> property.
		/// </summary>
		/// <param name="e">An object providing some information about the change.</param>
		private void OnExtendToChanged(DependencyPropertyChangedEventArgs e)
		{
			OnPropertyChanged(ColumnOrRowProperty.ExtendTo);
		}
		
		/// <summary>
		/// A column or row that the virtual column or row extends to.
		/// </summary>
		/// <value>
		/// <para>Gets or sets the name of the column or row that defines the second edge of the virtual column or row.
		///   If <see cref="ExtendTo"/> is <see langword="null"/>, the virtual column or row will be equal to the column or row indicated for <see cref="StartAt"/>.
		///   Otherwise, it will span the whole area between <see cref="StartAt"/> and <see cref="ExtendTo"/>.
		///   In the latter case, the relative order of the two columns or rows is irrelevant.</para>
		/// </value>
		/// <seealso cref="StartAt"/>
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
