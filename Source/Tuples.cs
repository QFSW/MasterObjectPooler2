using System;
using System.Collections.Generic;

namespace QFSW.MOP2.Internal
{
    internal struct Tuple2<T1, T2> : IEquatable<Tuple2<T1, T2>>
    {
        public readonly T1 Value1;
        public readonly T2 Value2;

        public Tuple2(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public override bool Equals(object obj)
        {
            return obj is Tuple2<T1, T2> tuple && Equals(tuple);
        }

        public bool Equals(Tuple2<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(Value1, other.Value1) &&
                   EqualityComparer<T2>.Default.Equals(Value2, other.Value2);
        }

        public override int GetHashCode()
        {
            var hashCode = -1959444751;
            hashCode = hashCode * -1521134295 + EqualityComparer<T1>.Default.GetHashCode(Value1);
            hashCode = hashCode * -1521134295 + EqualityComparer<T2>.Default.GetHashCode(Value2);
            return hashCode;
        }

        public static bool operator==(Tuple2<T1, T2> a, Tuple2<T1, T2> b)
        {
            return Equals(a.Value1, b.Value1) && Equals(a.Value2, b.Value2);
        }

        public static bool operator !=(Tuple2<T1, T2> a, Tuple2<T1, T2> b)
        {
            return !(a == b);
        }
    }
}
