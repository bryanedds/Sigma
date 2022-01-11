using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sprache;

namespace Sigma.Union2
{
	public class Union<T1>
	{
		public int Tag => tag;
		protected bool IsT1 => tag == 0;
		protected T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();

		public Union(T1 value1, Overload1 _ = default)
		{
			tag = 0;
			this.value1 = value1;
		}

		public void Switch(Action<T1> case1)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				default: throw new InvalidOperationException();
			}
		}

		public TR Match<TR>(Func<T1, TR> case1)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				default: throw new InvalidOperationException();
			}
		}

		public void PartialSwitch(Action<T1> case1 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				default: throw new InvalidOperationException();
			}
		}

		public TR PartialMatch<TR>(Func<T1, TR> case1 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				default: throw new InvalidOperationException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
	}

	public class Union<T1, T2>
	{
		public int Tag => tag;

		protected bool IsT1 => tag == 0;
		protected bool IsT2 => tag == 1;

		protected T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();

		protected Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();

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

		public void Switch(
			Action<T1> case1,
			Action<T2> case2)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				case 1: case2(value2); break;
				default: throw new InvalidOperationException();
			}
		}

		public TR Match<TR>(
			Func<T1, TR> case1,
			Func<T2, TR> case2)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				case 1: return case2(value2);
				default: throw new InvalidOperationException();
			}
		}

		public void PartialSwitch(
			Action<T1> case1 = null,
			Action<T2> case2 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				case 1: case2?.Invoke(value2); break;
				default: throw new InvalidOperationException();
			}
		}

		public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null,
			Func<T2, TR> case2 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				case 1: return (case2 ?? (_ => default)).Invoke(value2);
				default: throw new InvalidOperationException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
	}

	public class Union<T1, T2, T3>
	{
		public int Tag => tag;

		protected bool IsT1 => tag == 0;
		protected bool IsT2 => tag == 1;
		protected bool IsT3 => tag == 2;

		protected T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();

		protected Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();

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

		public void Switch(
			Action<T1> case1,
			Action<T2> case2,
			Action<T3> case3)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				case 1: case2(value2); break;
				case 2: case3(value3); break;
				default: throw new InvalidOperationException();
			}
		}

		public TR Match<TR>(
			Func<T1, TR> case1,
			Func<T2, TR> case2,
			Func<T3, TR> case3)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				case 1: return case2(value2);
				case 2: return case3(value3);
				default: throw new InvalidOperationException();
			}
		}

		public void PartialSwitch(
			Action<T1> case1 = null,
			Action<T2> case2 = null,
			Action<T3> case3 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				case 1: case2?.Invoke(value2); break;
				case 2: case3?.Invoke(value3); break;
				default: throw new InvalidOperationException();
			}
		}

		public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null,
			Func<T2, TR> case2 = null,
			Func<T3, TR> case3 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				case 1: return (case2 ?? (_ => default)).Invoke(value2);
				case 2: return (case3 ?? (_ => default)).Invoke(value3);
				default: throw new InvalidOperationException();
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
		private readonly T3 value3;
	}

	public class Union<T1, T2, T3, T4>
	{
		public int Tag => tag;

		protected bool IsT1 => tag == 0;
		protected bool IsT2 => tag == 1;
		protected bool IsT3 => tag == 2;
		protected bool IsT4 => tag == 3;

		protected T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();

		protected Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();

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

		public void Switch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR Match<TR>(
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
				default: throw new InvalidOperationException();
			}
		}

		public void PartialSwitch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR PartialMatch<TR>(
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
				default: throw new InvalidOperationException();
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
		public int Tag => tag;

		protected bool IsT1 => tag == 0;
		protected bool IsT2 => tag == 1;
		protected bool IsT3 => tag == 2;
		protected bool IsT4 => tag == 3;
		protected bool IsT5 => tag == 4;

		protected T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();
		protected T5 ToT5 => tag == 4 ? value5 : throw new InvalidOperationException();

		protected Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		protected Option<T5> TryT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();

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

		public void Switch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR Match<TR>(
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
				default: throw new InvalidOperationException();
			}
		}

		public void PartialSwitch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR PartialMatch<TR>(
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
				default: throw new InvalidOperationException();
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
		public int Tag => tag;

		protected bool IsT1 => tag == 0;
		protected bool IsT2 => tag == 1;
		protected bool IsT3 => tag == 2;
		protected bool IsT4 => tag == 3;
		protected bool IsT5 => tag == 4;
		protected bool IsT6 => tag == 5;

		protected T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();
		protected T5 ToT5 => tag == 4 ? value5 : throw new InvalidOperationException();
		protected T6 ToT6 => tag == 5 ? value6 : throw new InvalidOperationException();

		protected Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		protected Option<T5> TryT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();
		protected Option<T6> TryT6 => tag == 5 ? Option.Some(value6) : Option.None<T6>();

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

		public void Switch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR Match<TR>(
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
				default: throw new InvalidOperationException();
			}
		}

		public void PartialSwitch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR PartialMatch<TR>(
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
				default: throw new InvalidOperationException();
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
		public int Tag => tag;

		protected bool IsT1 => tag == 0;
		protected bool IsT2 => tag == 1;
		protected bool IsT3 => tag == 2;
		protected bool IsT4 => tag == 3;
		protected bool IsT5 => tag == 4;
		protected bool IsT6 => tag == 5;
		protected bool IsT7 => tag == 6;

		protected T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();
		protected T5 ToT5 => tag == 4 ? value5 : throw new InvalidOperationException();
		protected T6 ToT6 => tag == 5 ? value6 : throw new InvalidOperationException();
		protected T7 ToT7 => tag == 6 ? value7 : throw new InvalidOperationException();

		protected Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		protected Option<T5> TryT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();
		protected Option<T6> TryT6 => tag == 5 ? Option.Some(value6) : Option.None<T6>();
		protected Option<T7> TryT7 => tag == 6 ? Option.Some(value7) : Option.None<T7>();

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

		public void Switch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR Match<TR>(
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
				default: throw new InvalidOperationException();
			}
		}

		public void PartialSwitch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR PartialMatch<TR>(
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
				default: throw new InvalidOperationException();
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
		public int Tag => tag;

		protected bool IsT1 => tag == 0;
		protected bool IsT2 => tag == 1;
		protected bool IsT3 => tag == 2;
		protected bool IsT4 => tag == 3;
		protected bool IsT5 => tag == 4;
		protected bool IsT6 => tag == 5;
		protected bool IsT7 => tag == 6;
		protected bool IsT8 => tag == 7;

		protected T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();
		protected T5 ToT5 => tag == 4 ? value5 : throw new InvalidOperationException();
		protected T6 ToT6 => tag == 5 ? value6 : throw new InvalidOperationException();
		protected T7 ToT7 => tag == 6 ? value7 : throw new InvalidOperationException();
		protected T8 ToT8 => tag == 7 ? value8 : throw new InvalidOperationException();

		protected Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		protected Option<T5> TryT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();
		protected Option<T6> TryT6 => tag == 5 ? Option.Some(value6) : Option.None<T6>();
		protected Option<T7> TryT7 => tag == 6 ? Option.Some(value7) : Option.None<T7>();
		protected Option<T8> TryT8 => tag == 7 ? Option.Some(value8) : Option.None<T8>();

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

		public void Switch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR Match<TR>(
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
				default: throw new InvalidOperationException();
			}
		}

		public void PartialSwitch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR PartialMatch<TR>(
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
				default: throw new InvalidOperationException();
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
		public int Tag => tag;

		protected bool IsT1 => tag == 0;
		protected bool IsT2 => tag == 1;
		protected bool IsT3 => tag == 2;
		protected bool IsT4 => tag == 3;
		protected bool IsT5 => tag == 4;
		protected bool IsT6 => tag == 5;
		protected bool IsT7 => tag == 6;
		protected bool IsT8 => tag == 7;
		protected bool IsT9 => tag == 8;

		protected T1 ToT1 => tag == 0 ? value1 : throw new InvalidOperationException();
		protected T2 ToT2 => tag == 1 ? value2 : throw new InvalidOperationException();
		protected T3 ToT3 => tag == 2 ? value3 : throw new InvalidOperationException();
		protected T4 ToT4 => tag == 3 ? value4 : throw new InvalidOperationException();
		protected T5 ToT5 => tag == 4 ? value5 : throw new InvalidOperationException();
		protected T6 ToT6 => tag == 5 ? value6 : throw new InvalidOperationException();
		protected T7 ToT7 => tag == 6 ? value7 : throw new InvalidOperationException();
		protected T8 ToT8 => tag == 7 ? value8 : throw new InvalidOperationException();
		protected T9 ToT9 => tag == 8 ? value9 : throw new InvalidOperationException();

		protected Option<T1> TryT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		protected Option<T2> TryT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		protected Option<T3> TryT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		protected Option<T4> TryT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		protected Option<T5> TryT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();
		protected Option<T6> TryT6 => tag == 5 ? Option.Some(value6) : Option.None<T6>();
		protected Option<T7> TryT7 => tag == 6 ? Option.Some(value7) : Option.None<T7>();
		protected Option<T8> TryT8 => tag == 7 ? Option.Some(value8) : Option.None<T8>();
		protected Option<T9> TryT9 => tag == 8 ? Option.Some(value9) : Option.None<T9>();

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

		public void Switch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR Match<TR>(
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
				default: throw new InvalidOperationException();
			}
		}

		public void PartialSwitch(
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
				default: throw new InvalidOperationException();
			}
		}

		public TR PartialMatch<TR>(
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
				default: throw new InvalidOperationException();
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

	/// <summary>
	/// Convert strings and symbols, with the following parses:
	/// 
	/// (* Atom values *)
	/// None
	/// CharacterAnimationFacing
	/// 
	/// (* Number values *)
	/// 0.0f
	/// -5
	///
	/// (* String value *)
	/// "String with quoted spaces."
	///
	/// (* Quoted value *)
	/// `[Some 1]'
	/// 
	/// (* Symbols values *)
	/// []
	/// [Some 0]
	/// [Left 0]
	/// [[0 1] [2 4]]
	/// [AnimationData 4 8]
	/// [Gem `[Some 1]']
	///
	/// ...and so on.
	/// </summary>
	public static class SymbolParser
	{
		public static char OpenSymbolsChar = '[';
		public static char CloseSymbolsChar = ']';
		public static char OpenStringChar = '\"';
		public static char CloseStringChar = '\"';
		public static char OpenQuoteChar = '`';
		public static char CloseQuoteChar = '\'';
		public static string NewlineChars = "\n\r";
		public static string WhitespaceChars = " \t" + NewlineChars;
		public static string StructureCharsNoStr = "[]`\'";
		public static string StructureChars = "\"" + StructureCharsNoStr;

		public static Parser<string> ReadWhitespaces =
			Parse.Chars(WhitespaceChars).Many().Text();

		public static Parser<string> ReadAtomChars =
			Parse.CharExcept(StructureChars + WhitespaceChars).AtLeastOnce().Text();

		public static Parser<string> ReadStringChars =
			Parse.CharExcept(CloseStringChar).Many().Text();

		public static Parser<string> ReadQuoteChars =
			Parse.CharExcept(CloseQuoteChar).Many().Text();

		public static Parser<Symbol> ReadAtom =
			from chars in ReadAtomChars
			from _ in ReadWhitespaces
			select Symbol.Atom(chars);

		public static Parser<Symbol> ReadNumber =
			from chars in Parse.Number
			from _ in ReadWhitespaces
			select Symbol.Number(chars);

		public static Parser<Symbol> ReadString =
			from _ in Parse.Char(OpenStringChar)
			from _2 in ReadWhitespaces
			from chars in ReadStringChars
			from _3 in Parse.Char(CloseStringChar)
			from _4 in ReadWhitespaces
			select Symbol.Text(chars);

		public static Parser<Symbol> ReadQuote =
			from _ in Parse.Char(OpenQuoteChar)
			from _2 in ReadWhitespaces
			from chars in ReadQuoteChars
			from _3 in Parse.Char(CloseQuoteChar)
			from _4 in ReadWhitespaces
			select Symbol.Quote(chars);

		public static Parser<Symbol> ReadSymbols =
			from _ in Parse.Char(OpenSymbolsChar)
			from _2 in ReadWhitespaces
			from symbols in ReadSymbol.Many()
			from _3 in Parse.Char(CloseSymbolsChar)
			from _4 in ReadWhitespaces
			select Symbol.Symbols(symbols.ToImmutableList());

		public static Parser<Symbol> ReadSymbol =
			ReadQuote.Or(
			ReadString.Or(
			ReadNumber.Or(
			ReadAtom.Or(
			ReadSymbols))));

		public static Symbol ParseSymbol(string sexpr)
		{
			return
				(from _ in ReadWhitespaces
				 from symbol in ReadSymbol
				 select symbol)
				.Parse(sexpr);
		}

		public static string Distillate(string str)
		{
			return str
				.Replace(OpenStringChar.ToString(), string.Empty)
				.Replace(CloseStringChar.ToString(), string.Empty);
		}

		public static bool IsNumber(string content)
		{
			return
				(from chars in Parse.Number
				 select chars)
				 .TryParse(content)
				 .WasSuccessful;
		}

		public static bool IsExplicit(string content)
		{
			return content.StartsWith(OpenStringChar.ToString()) && content.EndsWith(CloseStringChar.ToString());
		}

		public static bool ShouldBeExplicit(string content)
		{
			return content.Any(chr => char.IsWhiteSpace(chr) || StructureCharsNoStr.Contains(chr));
		}

		public static string Implicitize(string str)
		{
			var distilled = Distillate(str);
			return distilled.Substring(1, distilled.Length - 2);
		}

		public static string Explicitize(string str)
		{
			var distilled = Distillate(str);
			return $"{OpenStringChar}{distilled}{CloseStringChar}";
		}

		private static string WriteAtom(string content)
		{
			var distilled = Distillate(content);
			if (distilled.Empty()) return $"{OpenStringChar}{CloseStringChar}";
			if (!IsExplicit(distilled) && ShouldBeExplicit(distilled)) return $"{OpenStringChar}{distilled}{CloseStringChar}";
			if (IsExplicit(distilled) && !ShouldBeExplicit(distilled)) return distilled.Substring(1, distilled.Length - 2);
			return distilled;
		}

		private static string WriteNumber(string content)
		{
			return Distillate(content);
		}

		private static string WriteString(string content)
		{
			return $"{OpenStringChar}{Distillate(content)}{CloseStringChar}";
		}

		private static string WriteQuote(string content)
		{
			return $"{OpenQuoteChar}{Distillate(content)}{CloseQuoteChar}";
		}

		private static string WriteSymbols(ImmutableList<Symbol> symbols)
		{
			return $"{OpenSymbolsChar}{string.Join(" ", symbols.Select(WriteSymbol))}{CloseSymbolsChar}";
		}

		public static string WriteSymbol(Symbol symbol)
		{
			return symbol.Match(WriteAtom, WriteNumber, WriteString, WriteQuote, WriteSymbols);
		}

		public static string UnparseSymbol(Symbol symbol)
		{
			return WriteSymbol(symbol);
		}
	}

	public class Symbol : Union<string, string, string, string, ImmutableList<Symbol>>
	{
		public static Symbol Atom(string atom) => new Symbol(atom, Overload1.Value);
		public static Symbol Number(string number) => new Symbol(number, Overload2.Value);
		public static Symbol Text(string text) => new Symbol(text, Overload3.Value);
		public static Symbol Quote(string quote) => new Symbol(quote, Overload4.Value);
		public static Symbol Symbols(IEnumerable<Symbol> symbols) => new Symbol(symbols, Overload5.Value);
		public string TryAtom => TryT1.ToReference();
		public string TryNumber => TryT2.ToReference();
		public string TryText => TryT3.ToReference();
		public string TryQuote => TryT4.ToReference();
		public ImmutableList<Symbol> TrySymbols => TryT5.ToReference();
		public Symbol(string atom, Overload1 o = default) : base(atom, o) { }
		public Symbol(string number, Overload2 o = default) : base(number, o) { }
		public Symbol(string text, Overload3 o = default) : base(text, o) { }
		public Symbol(string quote, Overload4 o = default) : base(quote, o) { }
		public Symbol(IEnumerable<Symbol> symbols, Overload5 o = default) : base(symbols.ToImmutableList(), o) { }
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

	[Serializable]
	public struct Unit : IEquatable<Unit>
	{
		public static readonly Unit Value = new Unit();
		public static bool operator ==(Unit first, Unit second) => true;
		public static bool operator !=(Unit first, Unit second) => false;
		public bool Equals(Unit other) => true;
		public override bool Equals(object obj) => obj is Unit;
		public override int GetHashCode() => 0;
		public override string ToString() => "()";
	}
}
