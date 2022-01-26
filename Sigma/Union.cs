using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Sigma
{
	/// <summary>
	/// A generalized type converter for unions.
	/// </summary>
	public class UnionTypeConverter : TypeConverter
	{
		public UnionTypeConverter(Type pointType)
		{
			this.pointType = pointType;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return
				sourceType == pointType ||
				sourceType == typeof(string) ||
				sourceType == typeof(Symbol);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var sourceType = value.GetType();
			if (sourceType == pointType) return value;
			if (sourceType == typeof(string)) return ConvertFrom(context, culture, Symbol.FromString((string)value));
			if (sourceType == typeof(Symbol))
			{
				var symbol = (Symbol)value;
				try
				{
					// convert from symbol to union
					var fields = symbol.ToSymbols;
					var valType = symbol.GetType().GetGenericArguments()[symbol.Tag];
					var valConverter = new SymbolicConverter(valType);
					var data = fields[1].Match(
						atom => valConverter.ConvertFromString(atom),
						number => valConverter.ConvertFromString(number),
						str => valConverter.ConvertFromString(SymbolParser.Explicitize(str)),
						_ => fields[1],
						_ => valConverter.ConvertFrom(fields[1]));
					return Activator.CreateInstance(pointType, new object[] { symbol.Tag, data });
				}
				catch (Exception exn)
				{
					throw new ArgumentException($"Invalid form '{symbol}' for {value.GetType().FullName}. Required form is '[Enum Value]'.", exn);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return
				destinationType == pointType ||
				destinationType == typeof(string) ||
				destinationType == typeof(Symbol);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == pointType) return value;
			if (destinationType == typeof(string)) return ConvertTo(context, culture, value, typeof(Symbol)).ToString();
			if (destinationType == typeof(Symbol))
			{
				// convert from union to symbol
				var tagField = pointType.GetField(nameof(Union<Unit>.Tag));
				var tag = (Enum)tagField.GetValue(value);
				var tagConverter = new SymbolicConverter(tagField.FieldType);
				var tagSymbol = new Symbol(tagConverter.ConvertToString(tag));
				var dataField = pointType.GetField(nameof(Union<Unit>.ValueObj));
				var data = dataField.GetValue(value);
				var dataConverter = new SymbolicConverter(tag.TryGetAttributeOfType<UnionAttribute>().TryThen(attr => attr.Type) ?? dataField.FieldType);
				var dataSymbol = dataConverter.CanConvertTo(typeof(Symbol)) ? (Symbol)dataConverter.ConvertTo(data, typeof(Symbol)) : new Symbol(dataConverter.ConvertToString(data));
				return new Symbol(new List<Symbol>() { tagSymbol, dataSymbol });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		private readonly Type pointType;
	}

	public class Union<T1>
	{
		protected internal int Tag => tag;
		protected internal object ValueObj => tag == 0 ? value1 : throw new InvalidOperationException();
		protected internal bool IsT1 => tag == 0;
		protected internal T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected internal Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();

		public Union(int tag, object valueObj)
		{
			switch (tag)
			{
				case 0: value1 = (T1)valueObj; return;
				default: throw new ArgumentException();
			}
		}

		public Union(T1 value1, Overload1 _ = default)
		{
			tag = 0;
			this.value1 = value1;
		}

		[DebuggerStepThrough, DebuggerHidden] public void Switch(Action<T1> case1)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR Match<TR>(Func<T1, TR> case1)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public void PartialSwitch(Action<T1> case1 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR PartialMatch<TR>(Func<T1, TR> case1 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				default: throw new ArgumentException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
	}

	public class Union<T1, T2>
	{
		protected internal int Tag => tag;

		protected internal object ValueObj
		{
			get
			{
				switch (tag)
				{
					case 0: return value1;
					case 1: return value2;
					default: throw new InvalidOperationException();
				}
			}
		}

		protected internal bool IsT1 => tag == 0;
		protected internal bool IsT2 => tag == 1;

		protected internal T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected internal T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();

		protected internal Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected internal Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();

		public Union(int tag, object valueObj)
		{
			switch (tag)
			{
				case 0: value1 = (T1)valueObj; return;
				case 1: value2 = (T2)valueObj; return;
				default: throw new ArgumentException();
			}
		}

		public Union(T1 value1, Overload1 _ = default)
		{
			tag = 0;
			this.value1 = value1;
		}

		public Union(T2 value2, Overload2 _ = default)
		{
			tag = 1;
			this.value2 = value2;
		}

		[DebuggerStepThrough, DebuggerHidden] public void Switch(
			Action<T1> case1,
			Action<T2> case2)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				case 1: case2(value2); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR Match<TR>(
			Func<T1, TR> case1,
			Func<T2, TR> case2)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				case 1: return case2(value2);
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public void PartialSwitch(
			Action<T1> case1 = null,
			Action<T2> case2 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				case 1: case2?.Invoke(value2); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null,
			Func<T2, TR> case2 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				case 1: return (case2 ?? (_ => default)).Invoke(value2);
				default: throw new ArgumentException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
	}

	public class Union<T1, T2, T3>
	{
		protected internal int Tag => tag;

		protected internal object ValueObj
		{
			get
			{
				switch (tag)
				{
					case 0: return value1;
					case 1: return value2;
					case 2: return value3;
					default: throw new InvalidOperationException();
				}
			}
		}

		protected internal bool IsT1 => tag == 0;
		protected internal bool IsT2 => tag == 1;
		protected internal bool IsT3 => tag == 2;

		protected internal T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected internal T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected internal T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();

		protected internal Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected internal Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected internal Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();

		public Union(int tag, object valueObj)
		{
			switch (tag)
			{
				case 0: value1 = (T1)valueObj; return;
				case 1: value2 = (T2)valueObj; return;
				case 2: value3 = (T3)valueObj; return;
				default: throw new ArgumentException();
			}
		}

		public Union(T1 value1, Overload1 _ = default)
		{
			tag = 0;
			this.value1 = value1;
		}

		public Union(T2 value2, Overload2 _ = default)
		{
			tag = 1;
			this.value2 = value2;
		}

		public Union(T3 value3, Overload3 _ = default)
		{
			tag = 2;
			this.value3 = value3;
		}

		[DebuggerStepThrough, DebuggerHidden] public void Switch(
			Action<T1> case1,
			Action<T2> case2,
			Action<T3> case3)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				case 1: case2(value2); break;
				case 2: case3(value3); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR Match<TR>(
			Func<T1, TR> case1,
			Func<T2, TR> case2,
			Func<T3, TR> case3)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				case 1: return case2(value2);
				case 2: return case3(value3);
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public void PartialSwitch(
			Action<T1> case1 = null,
			Action<T2> case2 = null,
			Action<T3> case3 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				case 1: case2?.Invoke(value2); break;
				case 2: case3?.Invoke(value3); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null,
			Func<T2, TR> case2 = null,
			Func<T3, TR> case3 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				case 1: return (case2 ?? (_ => default)).Invoke(value2);
				case 2: return (case3 ?? (_ => default)).Invoke(value3);
				default: throw new ArgumentException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
		private readonly T3 value3;
	}

	public class Union<T1, T2, T3, T4>
	{
		protected internal int Tag => tag;

		protected internal object ValueObj
		{
			get
			{
				switch (tag)
				{
					case 0: return value1;
					case 1: return value2;
					case 2: return value3;
					case 3: return value4;
					default: throw new InvalidOperationException();
				}
			}
		}

		protected internal bool IsT1 => tag == 0;
		protected internal bool IsT2 => tag == 1;
		protected internal bool IsT3 => tag == 2;
		protected internal bool IsT4 => tag == 3;

		protected internal T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected internal T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected internal T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected internal T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();

		protected internal Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected internal Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected internal Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected internal Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();

		public Union(int tag, object valueObj)
		{
			switch (tag)
			{
				case 0: value1 = (T1)valueObj; return;
				case 1: value2 = (T2)valueObj; return;
				case 2: value3 = (T3)valueObj; return;
				case 3: value4 = (T4)valueObj; return;
				default: throw new ArgumentException();
			}
		}

		public Union(T1 value1, Overload1 _ = default)
		{
			tag = 0;
			this.value1 = value1;
		}

		public Union(T2 value2, Overload2 _ = default)
		{
			tag = 1;
			this.value2 = value2;
		}

		public Union(T3 value3, Overload3 _ = default)
		{
			tag = 2;
			this.value3 = value3;
		}

		public Union(T4 value4, Overload4 _ = default)
		{
			tag = 3;
			this.value4 = value4;
		}

		[DebuggerStepThrough, DebuggerHidden] public void Switch(
			Action<T1> case1,
			Action<T2> case2,
			Action<T3> case3,
			Action<T4> case4)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				case 1: case2(value2); break;
				case 2: case3(value3); break;
				case 3: case4(value4); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR Match<TR>(
			Func<T1, TR> case1,
			Func<T2, TR> case2,
			Func<T3, TR> case3,
			Func<T4, TR> case4)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				case 1: return case2(value2);
				case 2: return case3(value3);
				case 3: return case4(value4);
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public void PartialSwitch(
			Action<T1> case1 = null,
			Action<T2> case2 = null,
			Action<T3> case3 = null,
			Action<T4> case4 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				case 1: case2?.Invoke(value2); break;
				case 2: case3?.Invoke(value3); break;
				case 3: case4?.Invoke(value4); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null,
			Func<T2, TR> case2 = null,
			Func<T3, TR> case3 = null,
			Func<T4, TR> case4 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				case 1: return (case2 ?? (_ => default)).Invoke(value2);
				case 2: return (case3 ?? (_ => default)).Invoke(value3);
				case 3: return (case4 ?? (_ => default)).Invoke(value4);
				default: throw new ArgumentException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
		private readonly T3 value3;
		private readonly T4 value4;
	}

	public class Union<T1, T2, T3, T4, T5>
	{
		protected internal int Tag => tag;

		protected internal object ValueObj
		{
			get
			{
				switch (tag)
				{
					case 0: return value1;
					case 1: return value2;
					case 2: return value3;
					case 3: return value4;
					case 4: return value5;
					default: throw new InvalidOperationException();
				}
			}
		}

		protected internal bool IsT1 => tag == 0;
		protected internal bool IsT2 => tag == 1;
		protected internal bool IsT3 => tag == 2;
		protected internal bool IsT4 => tag == 3;
		protected internal bool IsT5 => tag == 4;

		protected internal T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected internal T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected internal T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected internal T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();
		protected internal T5 ToT5 => tag == 4 ? value5 : throw new InvalidOperationException();

		protected internal Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected internal Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected internal Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected internal Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		protected internal Option<T5> TryT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();

		public Union(int tag, object valueObj)
		{
			switch (tag)
			{
				case 0: value1 = (T1)valueObj; return;
				case 1: value2 = (T2)valueObj; return;
				case 2: value3 = (T3)valueObj; return;
				case 3: value4 = (T4)valueObj; return;
				case 4: value5 = (T5)valueObj; return;
				default: throw new ArgumentException();
			}
		}

		public Union(T1 value1, Overload1 _ = default)
		{
			tag = 0;
			this.value1 = value1;
		}

		public Union(T2 value2, Overload2 _ = default)
		{
			tag = 1;
			this.value2 = value2;
		}

		public Union(T3 value3, Overload3 _ = default)
		{
			tag = 2;
			this.value3 = value3;
		}

		public Union(T4 value4, Overload4 _ = default)
		{
			tag = 3;
			this.value4 = value4;
		}

		public Union(T5 value5, Overload5 _ = default)
		{
			tag = 4;
			this.value5 = value5;
		}

		[DebuggerStepThrough, DebuggerHidden] public void Switch(
			Action<T1> case1,
			Action<T2> case2,
			Action<T3> case3,
			Action<T4> case4,
			Action<T5> case5)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				case 1: case2(value2); break;
				case 2: case3(value3); break;
				case 3: case4(value4); break;
				case 4: case5(value5); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR Match<TR>(
			Func<T1, TR> case1,
			Func<T2, TR> case2,
			Func<T3, TR> case3,
			Func<T4, TR> case4,
			Func<T5, TR> case5)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				case 1: return case2(value2);
				case 2: return case3(value3);
				case 3: return case4(value4);
				case 4: return case5(value5);
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public void PartialSwitch(
			Action<T1> case1 = null,
			Action<T2> case2 = null,
			Action<T3> case3 = null,
			Action<T4> case4 = null,
			Action<T5> case5 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				case 1: case2?.Invoke(value2); break;
				case 2: case3?.Invoke(value3); break;
				case 3: case4?.Invoke(value4); break;
				case 4: case5?.Invoke(value5); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null,
			Func<T2, TR> case2 = null,
			Func<T3, TR> case3 = null,
			Func<T4, TR> case4 = null,
			Func<T5, TR> case5 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				case 1: return (case2 ?? (_ => default)).Invoke(value2);
				case 2: return (case3 ?? (_ => default)).Invoke(value3);
				case 3: return (case4 ?? (_ => default)).Invoke(value4);
				case 4: return (case5 ?? (_ => default)).Invoke(value5);
				default: throw new ArgumentException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
		private readonly T3 value3;
		private readonly T4 value4;
		private readonly T5 value5;
	}

	public class Union<T1, T2, T3, T4, T5, T6>
	{
		protected internal int Tag => tag;

		protected internal object ValueObj
		{
			get
			{
				switch (tag)
				{
					case 0: return value1;
					case 1: return value2;
					case 2: return value3;
					case 3: return value4;
					case 4: return value5;
					case 5: return value6;
					default: throw new InvalidOperationException();
				}
			}
		}

		protected internal bool IsT1 => tag == 0;
		protected internal bool IsT2 => tag == 1;
		protected internal bool IsT3 => tag == 2;
		protected internal bool IsT4 => tag == 3;
		protected internal bool IsT5 => tag == 4;
		protected internal bool IsT6 => tag == 5;

		protected internal T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected internal T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected internal T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected internal T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();
		protected internal T5 ToT5 => tag == 4 ? value5 : throw new InvalidOperationException();
		protected internal T6 ToT6 => tag == 5 ? value6 : throw new InvalidOperationException();

		protected internal Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected internal Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected internal Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected internal Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		protected internal Option<T5> TryT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();
		protected internal Option<T6> TryT6 => tag == 5 ? Option.Some(value6) : Option.None<T6>();

		public Union(int tag, object valueObj)
		{
			switch (tag)
			{
				case 0: value1 = (T1)valueObj; return;
				case 1: value2 = (T2)valueObj; return;
				case 2: value3 = (T3)valueObj; return;
				case 3: value4 = (T4)valueObj; return;
				case 4: value5 = (T5)valueObj; return;
				case 5: value6 = (T6)valueObj; return;
				default: throw new ArgumentException();
			}
		}

		public Union(T1 value1, Overload1 _ = default)
		{
			tag = 0;
			this.value1 = value1;
		}

		public Union(T2 value2, Overload2 _ = default)
		{
			tag = 1;
			this.value2 = value2;
		}

		public Union(T3 value3, Overload3 _ = default)
		{
			tag = 2;
			this.value3 = value3;
		}

		public Union(T4 value4, Overload4 _ = default)
		{
			tag = 3;
			this.value4 = value4;
		}

		public Union(T5 value5, Overload5 _ = default)
		{
			tag = 4;
			this.value5 = value5;
		}

		public Union(T6 value6, Overload6 _ = default)
		{
			tag = 5;
			this.value6 = value6;
		}

		[DebuggerStepThrough, DebuggerHidden] public void Switch(
			Action<T1> case1,
			Action<T2> case2,
			Action<T3> case3,
			Action<T4> case4,
			Action<T5> case5,
			Action<T6> case6)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				case 1: case2(value2); break;
				case 2: case3(value3); break;
				case 3: case4(value4); break;
				case 4: case5(value5); break;
				case 5: case6(value6); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR Match<TR>(
			Func<T1, TR> case1,
			Func<T2, TR> case2,
			Func<T3, TR> case3,
			Func<T4, TR> case4,
			Func<T5, TR> case5,
			Func<T6, TR> case6)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				case 1: return case2(value2);
				case 2: return case3(value3);
				case 3: return case4(value4);
				case 4: return case5(value5);
				case 5: return case6(value6);
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public void PartialSwitch(
			Action<T1> case1 = null,
			Action<T2> case2 = null,
			Action<T3> case3 = null,
			Action<T4> case4 = null,
			Action<T5> case5 = null,
			Action<T6> case6 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				case 1: case2?.Invoke(value2); break;
				case 2: case3?.Invoke(value3); break;
				case 3: case4?.Invoke(value4); break;
				case 4: case5?.Invoke(value5); break;
				case 5: case6?.Invoke(value6); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null,
			Func<T2, TR> case2 = null,
			Func<T3, TR> case3 = null,
			Func<T4, TR> case4 = null,
			Func<T5, TR> case5 = null,
			Func<T6, TR> case6 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				case 1: return (case2 ?? (_ => default)).Invoke(value2);
				case 2: return (case3 ?? (_ => default)).Invoke(value3);
				case 3: return (case4 ?? (_ => default)).Invoke(value4);
				case 4: return (case5 ?? (_ => default)).Invoke(value5);
				case 5: return (case6 ?? (_ => default)).Invoke(value6);
				default: throw new ArgumentException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
		private readonly T3 value3;
		private readonly T4 value4;
		private readonly T5 value5;
		private readonly T6 value6;
	}

	public class Union<T1, T2, T3, T4, T5, T6, T7>
	{
		protected internal int Tag => tag;

		protected internal object ValueObj
		{
			get
			{
				switch (tag)
				{
					case 0: return value1;
					case 1: return value2;
					case 2: return value3;
					case 3: return value4;
					case 4: return value5;
					case 5: return value6;
					case 6: return value7;
					default: throw new InvalidOperationException();
				}
			}
		}

		protected internal bool IsT1 => tag == 0;
		protected internal bool IsT2 => tag == 1;
		protected internal bool IsT3 => tag == 2;
		protected internal bool IsT4 => tag == 3;
		protected internal bool IsT5 => tag == 4;
		protected internal bool IsT6 => tag == 5;
		protected internal bool IsT7 => tag == 6;

		protected internal T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected internal T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected internal T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected internal T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();
		protected internal T5 ToT5 => tag == 4 ? value5 : throw new InvalidOperationException();
		protected internal T6 ToT6 => tag == 5 ? value6 : throw new InvalidOperationException();
		protected internal T7 ToT7 => tag == 6 ? value7 : throw new InvalidOperationException();

		protected internal Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected internal Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected internal Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected internal Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		protected internal Option<T5> TryT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();
		protected internal Option<T6> TryT6 => tag == 5 ? Option.Some(value6) : Option.None<T6>();
		protected internal Option<T7> TryT7 => tag == 6 ? Option.Some(value7) : Option.None<T7>();

		public Union(int tag, object valueObj)
		{
			switch (tag)
			{
				case 0: value1 = (T1)valueObj; return;
				case 1: value2 = (T2)valueObj; return;
				case 2: value3 = (T3)valueObj; return;
				case 3: value4 = (T4)valueObj; return;
				case 4: value5 = (T5)valueObj; return;
				case 5: value6 = (T6)valueObj; return;
				case 6: value7 = (T7)valueObj; return;
				default: throw new ArgumentException();
			}
		}

		public Union(T1 value1, Overload1 _ = default)
		{
			tag = 0;
			this.value1 = value1;
		}

		public Union(T2 value2, Overload2 _ = default)
		{
			tag = 1;
			this.value2 = value2;
		}

		public Union(T3 value3, Overload3 _ = default)
		{
			tag = 2;
			this.value3 = value3;
		}

		public Union(T4 value4, Overload4 _ = default)
		{
			tag = 3;
			this.value4 = value4;
		}

		public Union(T5 value5, Overload5 _ = default)
		{
			tag = 4;
			this.value5 = value5;
		}

		public Union(T6 value6, Overload6 _ = default)
		{
			tag = 5;
			this.value6 = value6;
		}

		public Union(T7 value7, Overload7 _ = default)
		{
			tag = 6;
			this.value7 = value7;
		}

		[DebuggerStepThrough, DebuggerHidden] public void Switch(
			Action<T1> case1,
			Action<T2> case2,
			Action<T3> case3,
			Action<T4> case4,
			Action<T5> case5,
			Action<T6> case6,
			Action<T7> case7)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				case 1: case2(value2); break;
				case 2: case3(value3); break;
				case 3: case4(value4); break;
				case 4: case5(value5); break;
				case 5: case6(value6); break;
				case 6: case7(value7); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR Match<TR>(
			Func<T1, TR> case1,
			Func<T2, TR> case2,
			Func<T3, TR> case3,
			Func<T4, TR> case4,
			Func<T5, TR> case5,
			Func<T6, TR> case6,
			Func<T7, TR> case7)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				case 1: return case2(value2);
				case 2: return case3(value3);
				case 3: return case4(value4);
				case 4: return case5(value5);
				case 5: return case6(value6);
				case 6: return case7(value7);
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public void PartialSwitch(
			Action<T1> case1 = null,
			Action<T2> case2 = null,
			Action<T3> case3 = null,
			Action<T4> case4 = null,
			Action<T5> case5 = null,
			Action<T6> case6 = null,
			Action<T7> case7 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				case 1: case2?.Invoke(value2); break;
				case 2: case3?.Invoke(value3); break;
				case 3: case4?.Invoke(value4); break;
				case 4: case5?.Invoke(value5); break;
				case 5: case6?.Invoke(value6); break;
				case 6: case7?.Invoke(value7); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null,
			Func<T2, TR> case2 = null,
			Func<T3, TR> case3 = null,
			Func<T4, TR> case4 = null,
			Func<T5, TR> case5 = null,
			Func<T6, TR> case6 = null,
			Func<T7, TR> case7 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				case 1: return (case2 ?? (_ => default)).Invoke(value2);
				case 2: return (case3 ?? (_ => default)).Invoke(value3);
				case 3: return (case4 ?? (_ => default)).Invoke(value4);
				case 4: return (case5 ?? (_ => default)).Invoke(value5);
				case 5: return (case6 ?? (_ => default)).Invoke(value6);
				case 6: return (case7 ?? (_ => default)).Invoke(value7);
				default: throw new ArgumentException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
		private readonly T3 value3;
		private readonly T4 value4;
		private readonly T5 value5;
		private readonly T6 value6;
		private readonly T7 value7;
	}

	public class Union<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		protected internal int Tag => tag;

		protected internal object ValueObj
		{
			get
			{
				switch (tag)
				{
					case 0: return value1;
					case 1: return value2;
					case 2: return value3;
					case 3: return value4;
					case 4: return value5;
					case 5: return value6;
					case 6: return value7;
					case 7: return value8;
					default: throw new InvalidOperationException();
				}
			}
		}

		protected internal bool IsT1 => tag == 0;
		protected internal bool IsT2 => tag == 1;
		protected internal bool IsT3 => tag == 2;
		protected internal bool IsT4 => tag == 3;
		protected internal bool IsT5 => tag == 4;
		protected internal bool IsT6 => tag == 5;
		protected internal bool IsT7 => tag == 6;
		protected internal bool IsT8 => tag == 7;

		protected internal T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected internal T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected internal T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected internal T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();
		protected internal T5 ToT5 => tag == 4 ? value5 : throw new InvalidOperationException();
		protected internal T6 ToT6 => tag == 5 ? value6 : throw new InvalidOperationException();
		protected internal T7 ToT7 => tag == 6 ? value7 : throw new InvalidOperationException();
		protected internal T8 ToT8 => tag == 7 ? value8 : throw new InvalidOperationException();

		protected internal Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected internal Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected internal Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected internal Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		protected internal Option<T5> TryT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();
		protected internal Option<T6> TryT6 => tag == 5 ? Option.Some(value6) : Option.None<T6>();
		protected internal Option<T7> TryT7 => tag == 6 ? Option.Some(value7) : Option.None<T7>();
		protected internal Option<T8> TryT8 => tag == 7 ? Option.Some(value8) : Option.None<T8>();

		public Union(int tag, object valueObj)
		{
			switch (tag)
			{
				case 0: value1 = (T1)valueObj; return;
				case 1: value2 = (T2)valueObj; return;
				case 2: value3 = (T3)valueObj; return;
				case 3: value4 = (T4)valueObj; return;
				case 4: value5 = (T5)valueObj; return;
				case 5: value6 = (T6)valueObj; return;
				case 6: value7 = (T7)valueObj; return;
				case 7: value8 = (T8)valueObj; return;
				default: throw new ArgumentException();
			}
		}

		public Union(T1 value1, Overload1 _ = default)
		{
			tag = 0;
			this.value1 = value1;
		}

		public Union(T2 value2, Overload2 _ = default)
		{
			tag = 1;
			this.value2 = value2;
		}

		public Union(T3 value3, Overload3 _ = default)
		{
			tag = 2;
			this.value3 = value3;
		}

		public Union(T4 value4, Overload4 _ = default)
		{
			tag = 3;
			this.value4 = value4;
		}

		public Union(T5 value5, Overload5 _ = default)
		{
			tag = 4;
			this.value5 = value5;
		}

		public Union(T6 value6, Overload6 _ = default)
		{
			tag = 5;
			this.value6 = value6;
		}

		public Union(T7 value7, Overload7 _ = default)
		{
			tag = 6;
			this.value7 = value7;
		}

		public Union(T8 value8, Overload8 _ = default)
		{
			tag = 7;
			this.value8 = value8;
		}

		[DebuggerStepThrough, DebuggerHidden] public void Switch(
			Action<T1> case1,
			Action<T2> case2,
			Action<T3> case3,
			Action<T4> case4,
			Action<T5> case5,
			Action<T6> case6,
			Action<T7> case7,
			Action<T8> case8)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				case 1: case2(value2); break;
				case 2: case3(value3); break;
				case 3: case4(value4); break;
				case 4: case5(value5); break;
				case 5: case6(value6); break;
				case 6: case7(value7); break;
				case 7: case8(value8); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR Match<TR>(
			Func<T1, TR> case1,
			Func<T2, TR> case2,
			Func<T3, TR> case3,
			Func<T4, TR> case4,
			Func<T5, TR> case5,
			Func<T6, TR> case6,
			Func<T7, TR> case7,
			Func<T8, TR> case8)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				case 1: return case2(value2);
				case 2: return case3(value3);
				case 3: return case4(value4);
				case 4: return case5(value5);
				case 5: return case6(value6);
				case 6: return case7(value7);
				case 7: return case8(value8);
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public void PartialSwitch(
			Action<T1> case1 = null,
			Action<T2> case2 = null,
			Action<T3> case3 = null,
			Action<T4> case4 = null,
			Action<T5> case5 = null,
			Action<T6> case6 = null,
			Action<T7> case7 = null,
			Action<T8> case8 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				case 1: case2?.Invoke(value2); break;
				case 2: case3?.Invoke(value3); break;
				case 3: case4?.Invoke(value4); break;
				case 4: case5?.Invoke(value5); break;
				case 5: case6?.Invoke(value6); break;
				case 6: case7?.Invoke(value7); break;
				case 7: case8?.Invoke(value8); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null,
			Func<T2, TR> case2 = null,
			Func<T3, TR> case3 = null,
			Func<T4, TR> case4 = null,
			Func<T5, TR> case5 = null,
			Func<T6, TR> case6 = null,
			Func<T7, TR> case7 = null,
			Func<T8, TR> case8 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				case 1: return (case2 ?? (_ => default)).Invoke(value2);
				case 2: return (case3 ?? (_ => default)).Invoke(value3);
				case 3: return (case4 ?? (_ => default)).Invoke(value4);
				case 4: return (case5 ?? (_ => default)).Invoke(value5);
				case 5: return (case6 ?? (_ => default)).Invoke(value6);
				case 6: return (case7 ?? (_ => default)).Invoke(value7);
				case 7: return (case8 ?? (_ => default)).Invoke(value8);
				default: throw new ArgumentException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
		private readonly T3 value3;
		private readonly T4 value4;
		private readonly T5 value5;
		private readonly T6 value6;
		private readonly T7 value7;
		private readonly T8 value8;
	}

	public class Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
		protected internal int Tag => tag;

		protected internal object ValueObj
		{
			get
			{
				switch (tag)
				{
					case 0: return value1;
					case 1: return value2;
					case 2: return value3;
					case 3: return value4;
					case 4: return value5;
					case 5: return value6;
					case 6: return value7;
					case 7: return value8;
					case 8: return value9;
					default: throw new InvalidOperationException();
				}
			}
		}

		protected internal bool IsT1 => tag == 0;
		protected internal bool IsT2 => tag == 1;
		protected internal bool IsT3 => tag == 2;
		protected internal bool IsT4 => tag == 3;
		protected internal bool IsT5 => tag == 4;
		protected internal bool IsT6 => tag == 5;
		protected internal bool IsT7 => tag == 6;
		protected internal bool IsT8 => tag == 7;
		protected internal bool IsT9 => tag == 8;

		protected internal T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected internal T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected internal T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected internal T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();
		protected internal T5 ToT5 => tag == 4 ? value5 : throw new InvalidOperationException();
		protected internal T6 ToT6 => tag == 5 ? value6 : throw new InvalidOperationException();
		protected internal T7 ToT7 => tag == 6 ? value7 : throw new InvalidOperationException();
		protected internal T8 ToT8 => tag == 7 ? value8 : throw new InvalidOperationException();
		protected internal T9 ToT9 => tag == 8 ? value9 : throw new InvalidOperationException();

		protected internal Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected internal Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected internal Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected internal Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		protected internal Option<T5> TryT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();
		protected internal Option<T6> TryT6 => tag == 5 ? Option.Some(value6) : Option.None<T6>();
		protected internal Option<T7> TryT7 => tag == 6 ? Option.Some(value7) : Option.None<T7>();
		protected internal Option<T8> TryT8 => tag == 7 ? Option.Some(value8) : Option.None<T8>();
		protected internal Option<T9> TryT9 => tag == 8 ? Option.Some(value9) : Option.None<T9>();

		public Union(int tag, object valueObj)
		{
			switch (tag)
			{
				case 0: value1 = (T1)valueObj; return;
				case 1: value2 = (T2)valueObj; return;
				case 2: value3 = (T3)valueObj; return;
				case 3: value4 = (T4)valueObj; return;
				case 4: value5 = (T5)valueObj; return;
				case 5: value6 = (T6)valueObj; return;
				case 6: value7 = (T7)valueObj; return;
				case 7: value8 = (T8)valueObj; return;
				case 8: value9 = (T9)valueObj; return;
				default: throw new ArgumentException();
			}
		}

		public Union(T1 value1, Overload1 _ = default)
		{
			tag = 0;
			this.value1 = value1;
		}

		public Union(T2 value2, Overload2 _ = default)
		{
			tag = 1;
			this.value2 = value2;
		}

		public Union(T3 value3, Overload3 _ = default)
		{
			tag = 2;
			this.value3 = value3;
		}

		public Union(T4 value4, Overload4 _ = default)
		{
			tag = 3;
			this.value4 = value4;
		}

		public Union(T5 value5, Overload5 _ = default)
		{
			tag = 4;
			this.value5 = value5;
		}

		public Union(T6 value6, Overload6 _ = default)
		{
			tag = 5;
			this.value6 = value6;
		}

		public Union(T7 value7, Overload7 _ = default)
		{
			tag = 6;
			this.value7 = value7;
		}

		public Union(T8 value8, Overload8 _ = default)
		{
			tag = 7;
			this.value8 = value8;
		}

		public Union(T9 value9, Overload9 _ = default)
		{
			tag = 8;
			this.value9 = value9;
		}

		[DebuggerStepThrough, DebuggerHidden] public void Switch(
			Action<T1> case1,
			Action<T2> case2,
			Action<T3> case3,
			Action<T4> case4,
			Action<T5> case5,
			Action<T6> case6,
			Action<T7> case7,
			Action<T8> case8,
			Action<T9> case9)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				case 1: case2(value2); break;
				case 2: case3(value3); break;
				case 3: case4(value4); break;
				case 4: case5(value5); break;
				case 5: case6(value6); break;
				case 6: case7(value7); break;
				case 7: case8(value8); break;
				case 8: case9(value9); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR Match<TR>(
			Func<T1, TR> case1,
			Func<T2, TR> case2,
			Func<T3, TR> case3,
			Func<T4, TR> case4,
			Func<T5, TR> case5,
			Func<T6, TR> case6,
			Func<T7, TR> case7,
			Func<T8, TR> case8,
			Func<T9, TR> case9)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				case 1: return case2(value2);
				case 2: return case3(value3);
				case 3: return case4(value4);
				case 4: return case5(value5);
				case 5: return case6(value6);
				case 6: return case7(value7);
				case 7: return case8(value8);
				case 8: return case9(value9);
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public void PartialSwitch(
			Action<T1> case1 = null,
			Action<T2> case2 = null,
			Action<T3> case3 = null,
			Action<T4> case4 = null,
			Action<T5> case5 = null,
			Action<T6> case6 = null,
			Action<T7> case7 = null,
			Action<T8> case8 = null,
			Action<T9> case9 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				case 1: case2?.Invoke(value2); break;
				case 2: case3?.Invoke(value3); break;
				case 3: case4?.Invoke(value4); break;
				case 4: case5?.Invoke(value5); break;
				case 5: case6?.Invoke(value6); break;
				case 6: case7?.Invoke(value7); break;
				case 7: case8?.Invoke(value8); break;
				case 8: case9?.Invoke(value9); break;
				default: throw new ArgumentException();
			}
		}

		[DebuggerStepThrough, DebuggerHidden] public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null,
			Func<T2, TR> case2 = null,
			Func<T3, TR> case3 = null,
			Func<T4, TR> case4 = null,
			Func<T5, TR> case5 = null,
			Func<T6, TR> case6 = null,
			Func<T7, TR> case7 = null,
			Func<T8, TR> case8 = null,
			Func<T9, TR> case9 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				case 1: return (case2 ?? (_ => default)).Invoke(value2);
				case 2: return (case3 ?? (_ => default)).Invoke(value3);
				case 3: return (case4 ?? (_ => default)).Invoke(value4);
				case 4: return (case5 ?? (_ => default)).Invoke(value5);
				case 5: return (case6 ?? (_ => default)).Invoke(value6);
				case 6: return (case7 ?? (_ => default)).Invoke(value7);
				case 7: return (case8 ?? (_ => default)).Invoke(value8);
				case 8: return (case9 ?? (_ => default)).Invoke(value9);
				default: throw new ArgumentException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
		private readonly T3 value3;
		private readonly T4 value4;
		private readonly T5 value5;
		private readonly T6 value6;
		private readonly T7 value7;
		private readonly T8 value8;
		private readonly T9 value9;
	}

	public static class Message
	{
		public static Message<TS> Success<TS>(TS success) => new Message<TS>(success);
		public static Message<TS> Failure<TS>(string failure) => new Message<TS>(failure);
		public static Message<TR> Select<TS, TR>(this Message<TS> message, Func<TS, TR> selector) =>
			message.Match(
				success => Success(selector(success)),
				failure => Failure<TR>(failure));
	}

	public class Message<TS> : Result<TS, string>
	{
		public Message(TS success) : base(success, Overload1.Value) { }
		public Message(string message) : base(message, Overload2.Value) { }
	}

	public static class Result
	{
		public static Result<TS, TF> Success<TS, TF>(TS success) => new Result<TS, TF>(success);
		public static Result<TS, TF> Failure<TS, TF>(TF failure) => new Result<TS, TF>(failure, Overload2.Value);
		public static Result<TR, TF> Select<TS, TF, TR>(this Result<TS, TF> result, Func<TS, TR> selector) =>
			result.Match(
				success => Success<TR, TF>(selector(success)),
				failure => Failure<TR, TF>(failure));
	}

	public class Result<TS, TF> : Union<TS, TF>
	{
		public bool IsSuccess => IsT1;
		public bool IsFailure => IsT2;
		public TS ToSuccess => ToT1;
		public TF ToFailure => ToT2;
		public Option<TS> TrySuccess => TryT1;
		public Option<TF> TryFailure => TryT2;
		public Result(TS success, Overload1 o = default) : base(success, o) { }
		public Result(TF failure, Overload2 o = default) : base(failure, o) { }
	}

	public static class Option
	{
		public static Option<T> Some<T>(T value) => new Option<T>(value);
		public static Option<T> None<T>() => new Option<T>();
		public static Option<T> FromNullable<T>(T? nullable) where T : struct => nullable.HasValue ? Some(nullable.Value) : None<T>();
		public static Option<T> FromReference<T>(T nullable) where T : class => nullable != null ? Some(nullable) : None<T>();
		public static T? ToNullable<T>(this Option<T> opt) where T : struct => opt.Match(v => (T?)v, _ => null);
		public static T ToReference<T>(this Option<T> opt) where T : class => opt.Match(v => v, _ => null);
		public static Option<TR> Select<T, TR>(this Option<T> opt, Func<T, Option<TR>> selector) => opt.Match(selector, _ => None<TR>());
	}

	public class Option<T> : Union<T, Unit>
	{
		public bool IsSome => IsT1;
		public bool IsNone => IsT2;
		public T Value => ToT1;
		public Option(T value) : base(value, Overload1.Value) { }
		public Option() : base(Unit.Value, Overload2.Value) { }
	}

	public struct Overload1
	{
		public static Overload1 Value => new Overload1();
	}

	public struct Overload2
	{
		public static Overload2 Value => new Overload2();
	}

	public struct Overload3
	{
		public static Overload3 Value => new Overload3();
	}

	public struct Overload4
	{
		public static Overload4 Value => new Overload4();
	}

	public struct Overload5
	{
		public static Overload5 Value => new Overload5();
	}

	public struct Overload6
	{
		public static Overload6 Value => new Overload6();
	}

	public struct Overload7
	{
		public static Overload7 Value => new Overload7();
	}

	public struct Overload8
	{
		public static Overload8 Value => new Overload8();
	}

	public struct Overload9
	{
		public static Overload9 Value => new Overload9();
	}
}
