using System;


namespace ZPC.Phone
{
    public struct Bitrate : IEquatable<Bitrate>
    {
        public int Bps { get; }
        public float Kbps => ((float)Bps) / 1000;

        public Bitrate(int bitsPerSecond)
        {
            Bps = bitsPerSecond;
        }

        public Bitrate(float kilobitsPerSecond)
        {
            Bps = (int)(kilobitsPerSecond * 1000);
        }

        public bool Equals(Bitrate other)
        {
            return Bps == other.Bps;
        }

        /*
        explicit 和 implicit 属于转换运算符，如用这两者可以让我们自定义的类型支持相互交换

        explicti 表示显式转换，如从 A -> B 必须进行强制类型转换（B = (B)A）

        implicit 表示隐式转换，如从 B -> A 只需直接赋值（A = B）
        */
        public static implicit operator Bitrate(int bitsPerSecond) => new Bitrate(bitsPerSecond);
        public static implicit operator Bitrate(float kilobitsPerSecond) => new Bitrate(kilobitsPerSecond);

        public static bool operator ==(Bitrate b1, Bitrate b2)
        {
            return b1.Bps == b2.Bps;
        }

        public static bool operator !=(Bitrate b1, Bitrate b2)
        {
            return b1.Bps != b2.Bps;
        }

        public static Bitrate operator +(Bitrate b1, Bitrate b2)
        {
            return new Bitrate(b1.Bps + b2.Bps);
        }

        public static Bitrate operator -(Bitrate b1, Bitrate b2)
        {
            return new Bitrate(b1.Bps - b2.Bps);
        }
    }
}