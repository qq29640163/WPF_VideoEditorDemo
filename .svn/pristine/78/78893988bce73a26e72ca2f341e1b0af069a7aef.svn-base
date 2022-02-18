using System;
using System.Collections;
using System.Collections.Generic;


namespace ZPC.Phone
{
    public class TimeInterval
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan Duration => End - Start;

        public TimeInterval(TimeSpan start, TimeSpan end)
        {
            if (end >= start)
            {
                Start = start;
                End = end;
            }
            else
            {
                throw new Exception("Arguments not valid");
            }
        }

        /// <summary>
        /// 返回一个值，该值确定TimeSpan t是否在时间间隔内
        /// </summary>
        public bool Contains(TimeSpan t)
        {
            return t >= Start && t <= End;
        }

        /// <summary>
        /// 返回一个值，该值确定一个时间间隔是否完全在此时间 timeInterval 间隔内
        /// </summary>
        public bool Contains(TimeInterval timeInterval)
        {
            return timeInterval.Start >= Start && timeInterval.End <= End;
        }

        /// <summary>
        /// 返回一个值，该值确定两个间隔是否至少共享一个点
        /// </summary>
        public bool Intersect(TimeInterval other)
        {
            // 交叉如果存在，所以 Start <= t <= End && other.Start <= t <= other.End
            // 所以 Start <= other.End and other.Start <= End
            return Start <= other.End && other.Start <= End; 
        }

        /// <summary>
        /// 连接两个相邻的间隔
        /// </summary>
        public static TimeInterval operator +(TimeInterval t1, TimeInterval t2)
        {
            return t1.Add(t2);
        }

        /// <summary>
        /// 连接两个相邻的间隔
        /// </summary>
        public TimeInterval Add(TimeInterval other)
        {
            if (Intersect(other))
                return new TimeInterval(Start < other.Start ? Start : other.Start, End > other.End ? End : other.End);
            else
                throw new Exception("Intervals do not intersect each other");
        }

        /// <summary>
        /// 如果间隔相邻，则从另一个间隔中删除间隔
        /// </summary>
        public static TimeInterval operator -(TimeInterval t1, TimeInterval t2)
        {
            return t1.Subtract(t2);
        }

        /// <summary>
        /// 从另一个间隔中删除一个间隔，前提是该间隔不完全在该间隔内
        /// </summary>
        public TimeInterval Subtract(TimeInterval other)
        {
            if (!Contains(other) && this != other)
            {
                if (!Intersect(other))
                {
                    return this;
                }
                if (this > other)
                {
                    return new TimeInterval(other.Start, End);
                }
                if (this < other)
                {
                    return new TimeInterval(Start, other.End);
                }
                return this; //这一行永远不会运行，但它是编译器所必需的
            }
            else
            {
                throw new Exception("Interval can't be inside");
            }
        }

        public static bool operator <(TimeInterval t1, TimeInterval t2)
        {
            return t1.Start < t2.Start;
        }

        public static bool operator >(TimeInterval t1, TimeInterval t2)
        {
            return t1.Start > t2.Start;
        }
    }


    public class TimeIntervalCollection : IEnumerable<TimeInterval>
    {
        public TimeSpan TotalDuration
        {
            get
            {
                if (intervalList.Count == 0) return ActualEnd - ActualStart;

                TimeSpan totalDuration = TimeSpan.Zero;
                foreach (var item in intervalList)
                {
                    totalDuration += item.Duration;
                }
                return totalDuration;
            }
        }

        public int Count 
        {
            get 
            { 
                return intervalList.Count; 
            } 
        }

        /// <summary>
        /// 返回第一个时间间隔的开始，如果没有时间间隔元素，则返回开始
        /// </summary>
        public TimeSpan ActualStart
        {
            get
            {
                if (intervalList.Count > 0)
                    return intervalList[0].Start;
                return Start;
            }
        }

        /// <summary>
        /// 返回上一个时间间隔的结束，如果没有时间间隔元素，则返回结束
        /// </summary>
        public TimeSpan ActualEnd
        {
            get
            {
                if (intervalList.Count > 0)
                    return intervalList[Count - 1].End;
                return End;
            }
        }

        public TimeSpan Start { get; private set; }
        public TimeSpan End { get; private set; }

        public TimeInterval this[int index]
        {
            get
            {
                if (index >= 0 && index < intervalList.Count)
                {
                    return intervalList[index];
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        private readonly List<TimeInterval> intervalList;


        public TimeIntervalCollection(TimeSpan end) : this(TimeSpan.Zero, end)
        {
        }

        public TimeIntervalCollection(TimeSpan start, TimeSpan end)
        {
            intervalList = new List<TimeInterval>();
            Start = start;
            End = end;
        }

        /// <summary>
        /// 将时间间隔添加到此集合，忽略Start-End范围之外的时间间隔的最终部分
        /// </summary>
        public void Add(TimeSpan start, TimeSpan end)
        {
            Add(new TimeInterval(start, end));
        }

        /// <summary>
        /// 将时间间隔添加到此集合，忽略Start-End范围之外的时间间隔的最终部分
        /// </summary>
        public void Add(TimeInterval timeInterval)
        {
            TimeInterval collectionInterval = new TimeInterval(Start, End);
            if (collectionInterval.Intersect(timeInterval))
            {
                // Discards the part of the interval that's outisde of the collection range
                if (timeInterval.Start < Start)
                    timeInterval.Start = Start;
                if (timeInterval.End > End)
                    timeInterval.End = End;

                for (int i = intervalList.Count - 1; i >= 0; i--) // 循环被反转以允许从列表中删除项
                {
                    if (intervalList[i].Intersect(timeInterval)) // 将现有相交间隔吸收到新间隔中
                    {
                        timeInterval += intervalList[i];
                        intervalList.RemoveAt(i);
                    }
                }
                intervalList.Add(timeInterval);
                intervalList.Sort((t1, t2) => t1.Start <= t2.Start ? -1 : 1);
            }
        }

        public void Remove(TimeSpan start, TimeSpan end)
        {
            Remove(new TimeInterval(start, end));
        }

        public void Remove(TimeInterval timeInterval)
        {
            for (int i = intervalList.Count - 1; i >= 0; i--) // 循环被反转以允许从列表中删除项
            {
                TimeInterval currentInterval = intervalList[i];
                if (timeInterval.Contains(currentInterval))
                {
                    TimeInterval before, after;
                    before = new TimeInterval(currentInterval.Start, timeInterval.Start);
                    after = new TimeInterval(timeInterval.End, currentInterval.End);
                    intervalList.RemoveAt(i);
                    intervalList.Add(before);
                    intervalList.Add(after);
                }
                else if (timeInterval.Intersect(currentInterval))
                {
                    intervalList.RemoveAt(i);
                    intervalList.Add(currentInterval - timeInterval);
                }
            }
        }

        public TimeIntervalCollection Reverse()
        {
            TimeIntervalCollection complementary = new TimeIntervalCollection(Start, End);

            if (Count == 0)
            {
                complementary.Add(Start, End);
            }
            else
            {
                if (intervalList[0].Start > Start)
                    complementary.Add(Start, intervalList[0].Start);
                for (int i = 1; i < intervalList.Count; i++)
                {
                    complementary.Add(intervalList[i - 1].End, intervalList[i].Start);
                }
                if (intervalList[Count - 1].End != End)
                    complementary.Add(intervalList[Count - 1].End, End);

            }

            return complementary;
        }

        public bool Contains(TimeSpan timeSpan)
        {
            foreach (var item in intervalList)
            {
                if (item.Contains(timeSpan)) return true;
            }

            return false;
        }

        /// <summary>
        /// 如果此集合中不包含TimeSpan，则返回TimeSpan之前最近间隔的结尾，否则返回参数
        /// </summary>
        public TimeSpan GetClosestTimeSpanBefore(TimeSpan timeSpan)
        {
            if (Contains(timeSpan)) return timeSpan;

            // 由于intervaList已排序，因此timeSpan之前的第一个intervalList是正确的
            for (int i = intervalList.Count; i >= 0 ; i--)
            {
                if (timeSpan > intervalList[i].End) return intervalList[i].End;
            }

            //timeSpan不在集合中
            if (timeSpan < Start) return ActualStart;
            return End;
        }

        /// <summary>
        /// 如果此集合中不包含TimeSpan，则返回TimeSpan之后最近间隔的开始，否则返回参数
        /// </summary>
        public TimeSpan GetClosestTimeSpanAfter(TimeSpan timeSpan)
        {
            if (Contains(timeSpan)) return timeSpan;

            //由于intervaList已排序，因此timeSpan之后的第一个intervalList是正确的
            foreach (var timeInterval in intervalList)
            {
                if (timeSpan < timeInterval.Start) return timeInterval.Start;
            }

            //timeSpan不在集合中
            if (timeSpan < Start) return ActualStart;
            return End;
        }

        public IEnumerator<TimeInterval> GetEnumerator()
        {
            return intervalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}