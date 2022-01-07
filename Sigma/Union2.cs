using System;

namespace Sigma.Union2
{
	public class Union<T1>
	{
		public int Tag => tag;

		public bool IsT1 => tag == 0;

		public T1 ToT1 => tag == 0 ? value1 : throw new Exception("Unexpected match failure.");

		public Option<T1> AsT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();

		public Union(T1 value1)
		{
			tag = 0;
			this.value1 = value1;
		}

		public void Switch(
			Action<T1> case1)
		{
			switch (tag)
			{
				case 0: case1(value1); break;
				default: throw new Exception("Unexpected match failure.");
			}
		}

		public TR Match<TR>(
			Func<T1, TR> case1)
		{
			switch (tag)
			{
				case 0: return case1(value1);
				default: throw new Exception("Unexpected match failure.");
			}
		}

		public void PartialSwitch(
			Action<T1> case1 = null)
		{
			switch (tag)
			{
				case 0: case1?.Invoke(value1); break;
				default: throw new Exception("Unexpected match failure.");
			}
		}

		public TR PartialMatch<TR>(
			Func<T1, TR> case1 = null)
		{
			switch (tag)
			{
				case 0: return (case1 ?? (_ => default)).Invoke(value1);
				default: throw new Exception("Unexpected match failure.");
			}
		}

		private readonly int tag;
		private readonly T1 value1;
	}

	public class Union<T1, T2>
	{
		public int Tag => tag;

		public bool IsT1 => tag == 0;
		public bool IsT2 => tag == 1;

		public T1 ToT1 => tag == 0 ? value1 : throw new Exception("Unexpected match failure.");
		public T2 ToT2 => tag == 1 ? value2 : throw new Exception("Unexpected match failure.");

		public Option<T1> AsT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		public Option<T2> AsT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();

		public Union(T1 value1)
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
			}
		}

		private readonly int tag;
		private readonly T1 value1;
		private readonly T2 value2;
	}

	public class Union<T1, T2, T3>
	{
		public int Tag => tag;

		public bool IsT1 => tag == 0;
		public bool IsT2 => tag == 1;
		public bool IsT3 => tag == 2;

		public T1 ToT1 => tag == 0 ? value1 : throw new Exception("Unexpected match failure.");
		public T2 ToT2 => tag == 1 ? value2 : throw new Exception("Unexpected match failure.");
		public T3 ToT3 => tag == 2 ? value3 : throw new Exception("Unexpected match failure.");

		public Option<T1> AsT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		public Option<T2> AsT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		public Option<T3> AsT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();

		public Union(T1 value1)
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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

		public bool IsT1 => tag == 0;
		public bool IsT2 => tag == 1;
		public bool IsT3 => tag == 2;
		public bool IsT4 => tag == 3;

		public T1 ToT1 => tag == 0 ? value1 : throw new Exception("Unexpected match failure.");
		public T2 ToT2 => tag == 1 ? value2 : throw new Exception("Unexpected match failure.");
		public T3 ToT3 => tag == 2 ? value3 : throw new Exception("Unexpected match failure.");
		public T4 ToT4 => tag == 3 ? value4 : throw new Exception("Unexpected match failure.");

		public Option<T1> AsT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		public Option<T2> AsT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		public Option<T3> AsT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		public Option<T4> AsT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();

		public Union(T1 value1)
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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

		public bool IsT1 => tag == 0;
		public bool IsT2 => tag == 1;
		public bool IsT3 => tag == 2;
		public bool IsT4 => tag == 3;
		public bool IsT5 => tag == 4;

		public T1 ToT1 => tag == 0 ? value1 : throw new Exception("Unexpected match failure.");
		public T2 ToT2 => tag == 1 ? value2 : throw new Exception("Unexpected match failure.");
		public T3 ToT3 => tag == 2 ? value3 : throw new Exception("Unexpected match failure.");
		public T4 ToT4 => tag == 3 ? value4 : throw new Exception("Unexpected match failure.");
		public T5 ToT5 => tag == 4 ? value5 : throw new Exception("Unexpected match failure.");

		public Option<T1> AsT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		public Option<T2> AsT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		public Option<T3> AsT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		public Option<T4> AsT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		public Option<T5> AsT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();

		public Union(T1 value1)
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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

		public bool IsT1 => tag == 0;
		public bool IsT2 => tag == 1;
		public bool IsT3 => tag == 2;
		public bool IsT4 => tag == 3;
		public bool IsT5 => tag == 4;
		public bool IsT6 => tag == 5;

		public T1 ToT1 => tag == 0 ? value1 : throw new Exception("Unexpected match failure.");
		public T2 ToT2 => tag == 1 ? value2 : throw new Exception("Unexpected match failure.");
		public T3 ToT3 => tag == 2 ? value3 : throw new Exception("Unexpected match failure.");
		public T4 ToT4 => tag == 3 ? value4 : throw new Exception("Unexpected match failure.");
		public T5 ToT5 => tag == 4 ? value5 : throw new Exception("Unexpected match failure.");
		public T6 ToT6 => tag == 5 ? value6 : throw new Exception("Unexpected match failure.");

		public Option<T1> AsT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		public Option<T2> AsT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		public Option<T3> AsT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		public Option<T4> AsT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		public Option<T5> AsT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();
		public Option<T6> AsT6 => tag == 5 ? Option.Some(value6) : Option.None<T6>();

		public Union(T1 value1)
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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

		public bool IsT1 => tag == 0;
		public bool IsT2 => tag == 1;
		public bool IsT3 => tag == 2;
		public bool IsT4 => tag == 3;
		public bool IsT5 => tag == 4;
		public bool IsT6 => tag == 5;
		public bool IsT7 => tag == 6;

		public T1 ToT1 => tag == 0 ? value1 : throw new Exception("Unexpected match failure.");
		public T2 ToT2 => tag == 1 ? value2 : throw new Exception("Unexpected match failure.");
		public T3 ToT3 => tag == 2 ? value3 : throw new Exception("Unexpected match failure.");
		public T4 ToT4 => tag == 3 ? value4 : throw new Exception("Unexpected match failure.");
		public T5 ToT5 => tag == 4 ? value5 : throw new Exception("Unexpected match failure.");
		public T6 ToT6 => tag == 5 ? value6 : throw new Exception("Unexpected match failure.");
		public T7 ToT7 => tag == 6 ? value7 : throw new Exception("Unexpected match failure.");

		public Option<T1> AsT1 => tag == 0 ? Option.Some(value1) : Option.None<T1>();
		public Option<T2> AsT2 => tag == 1 ? Option.Some(value2) : Option.None<T2>();
		public Option<T3> AsT3 => tag == 2 ? Option.Some(value3) : Option.None<T3>();
		public Option<T4> AsT4 => tag == 3 ? Option.Some(value4) : Option.None<T4>();
		public Option<T5> AsT5 => tag == 4 ? Option.Some(value5) : Option.None<T5>();
		public Option<T6> AsT6 => tag == 5 ? Option.Some(value6) : Option.None<T6>();
		public Option<T7> AsT7 => tag == 6 ? Option.Some(value7) : Option.None<T7>();

		public Union(T1 value1)
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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
				default: throw new Exception("Unexpected match failure.");
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

	public class Option
	{
		public static Option<T> Some<T>(T value) => new Option<T>(value);
		public static Option<T> None<T>() => new Option<T>();
		public static Option<T> FromNullable<T>(T? nullable) where T : struct => nullable.HasValue ? Some(nullable.Value) : None<T>();
		public static Option<T> FromReference<T>(T nullable) where T : class => nullable != null ? Some(nullable) : None<T>();
		public static T? ToNullable<T>(Option<T> opt) where T : struct => opt.Match(v => (T?)v, _ => null);
		public static T ToReference<T>(Option<T> opt) where T : class => opt.Match(v => v, _ => null);
	}

	public class Option<T> : Union<T, Unit>
	{
		public Option() : base(Unit.Value, Overload2.Value) { }
		public Option(T value) : base(value) { }
	}

	public struct Unit
	{
		public static Unit Value => new Unit(); // NOTE: BGE: assuming that construction costs are zero.
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
}
