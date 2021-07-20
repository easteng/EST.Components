// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Siemens.SiemensDateTime
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Collections.Generic;

namespace ESTCore.Common.Profinet.Siemens
{
    /// <summary>
    /// Contains the methods to convert between <see cref="T:System.DateTime" /> and S7 representation of datetime values.
    /// </summary>
    /// <remarks>
    /// 这部分的代码参考了另一个s7的库，感谢原作者，此处贴出出处，遵循 MIT 协议
    /// 
    /// https://github.com/S7NetPlus/s7netplus
    /// </remarks>
    public class SiemensDateTime
    {
        /// <summary>
        /// The minimum <see cref="T:System.DateTime" /> value supported by the specification.
        /// </summary>
        public static readonly DateTime SpecMinimumDateTime = new DateTime(1990, 1, 1);
        /// <summary>
        /// The maximum <see cref="T:System.DateTime" /> value supported by the specification.
        /// </summary>
        public static readonly DateTime SpecMaximumDateTime = new DateTime(2089, 12, 31, 23, 59, 59, 999);

        /// <summary>
        /// Parses a <see cref="T:System.DateTime" /> value from bytes.
        /// </summary>
        /// <param name="bytes">Input bytes read from PLC.</param>
        /// <returns>A <see cref="T:System.DateTime" /> object representing the value read from PLC.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when the length of
        /// <paramref name="bytes" /> is not 8 or any value in <paramref name="bytes" />
        /// is outside the valid range of values.</exception>
        public static DateTime FromByteArray(byte[] bytes) => SiemensDateTime.FromByteArrayImpl((IList<byte>)bytes);

        /// <summary>
        /// Parses an array of <see cref="T:System.DateTime" /> values from bytes.
        /// </summary>
        /// <param name="bytes">Input bytes read from PLC.</param>
        /// <returns>An array of <see cref="T:System.DateTime" /> objects representing the values read from PLC.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when the length of
        /// <paramref name="bytes" /> is not a multiple of 8 or any value in
        /// <paramref name="bytes" /> is outside the valid range of values.</exception>
        public static DateTime[] ToArray(byte[] bytes)
        {
            if ((uint)(bytes.Length % 8) > 0U)
                throw new ArgumentOutOfRangeException(nameof(bytes), (object)bytes.Length, string.Format("Parsing an array of DateTime requires a multiple of 8 bytes of input data, input data is '{0}' long.", (object)bytes.Length));
            int num = bytes.Length / 8;
            DateTime[] dateTimeArray = new DateTime[bytes.Length / 8];
            for (int index = 0; index < num; ++index)
                dateTimeArray[index] = SiemensDateTime.FromByteArrayImpl((IList<byte>)new ArraySegment<byte>(bytes, index * 8, 8).Array);
            return dateTimeArray;
        }

        private static DateTime FromByteArrayImpl(IList<byte> bytes)
        {
            int year = bytes.Count == 8 ? ByteToYear(bytes[0]) : throw new ArgumentOutOfRangeException(nameof(bytes), (object)bytes.Count, string.Format("Parsing a DateTime requires exactly 8 bytes of input data, input data is {0} bytes long.", (object)bytes.Count));
            int month = AssertRangeInclusive(DecodeBcd(bytes[1]), (byte)1, (byte)12, "month");
            int day = AssertRangeInclusive(DecodeBcd(bytes[2]), (byte)1, (byte)31, "day of month");
            int hour = AssertRangeInclusive(DecodeBcd(bytes[3]), (byte)0, (byte)23, "hour");
            int minute = AssertRangeInclusive(DecodeBcd(bytes[4]), (byte)0, (byte)59, "minute");
            int second = AssertRangeInclusive(DecodeBcd(bytes[5]), (byte)0, (byte)59, "second");
            int num1 = AssertRangeInclusive(DecodeBcd(bytes[6]), (byte)0, (byte)99, "first two millisecond digits");
            int num2 = AssertRangeInclusive((int)bytes[7] >> 4, (byte)0, (byte)9, "third millisecond digit");
            AssertRangeInclusive((int)bytes[7] & 15, (byte)1, (byte)7, "day of week");
            return new DateTime(year, month, day, hour, minute, second, num1 * 10 + num2);


        }
        private static int ByteToYear(byte bcdYear)
        {
            int num = DecodeBcd(bcdYear);
            if (num < 90)
                return num + 2000;
            if (num < 100)
                return num + 1900;
            throw new ArgumentOutOfRangeException(nameof(bcdYear), (object)bcdYear, string.Format("Value '{0}' is higher than the maximum '99' of S7 date and time representation.", (object)num));
        }
        private static int AssertRangeInclusive(int input, byte min, byte max, string field)
        {
            if (input < (int)min)
                throw new ArgumentOutOfRangeException(nameof(input), (object)input, string.Format("Value '{0}' is lower than the minimum '{1}' allowed for {2}.", (object)input, (object)min, (object)field));
            return input <= (int)max ? input : throw new ArgumentOutOfRangeException(nameof(input), (object)input, string.Format("Value '{0}' is higher than the maximum '{1}' allowed for {2}.", (object)input, (object)max, (object)field));
        }
        private static int DecodeBcd(byte input) => 10 * ((int)input >> 4) + ((int)input & 15);

        /// <summary>
        /// Converts a <see cref="T:System.DateTime" /> value to a byte array.
        /// </summary>
        /// <param name="dateTime">The DateTime value to convert.</param>
        /// <returns>A byte array containing the S7 date time representation of <paramref name="dateTime" />.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when the value of
        /// <paramref name="dateTime" /> is before <see cref="P:SpecMinimumDateTime" />
        /// or after <see cref="P:SpecMaximumDateTime" />.</exception>
        public static byte[] ToByteArray(DateTime dateTime)
        {
            if (dateTime < SiemensDateTime.SpecMinimumDateTime)
                throw new ArgumentOutOfRangeException(nameof(dateTime), (object)dateTime, string.Format("Date time '{0}' is before the minimum '{1}' supported in S7 date time representation.", (object)dateTime, (object)SiemensDateTime.SpecMinimumDateTime));
            if (dateTime > SiemensDateTime.SpecMaximumDateTime)
                throw new ArgumentOutOfRangeException(nameof(dateTime), (object)dateTime, string.Format("Date time '{0}' is after the maximum '{1}' supported in S7 date time representation.", (object)dateTime, (object)SiemensDateTime.SpecMaximumDateTime));
            return new byte[8]
            {
                 EncodeBcd((int) MapYear(dateTime.Year)),
                 EncodeBcd(dateTime.Month),
                 EncodeBcd(dateTime.Day),
                 EncodeBcd(dateTime.Hour),
                 EncodeBcd(dateTime.Minute),
                 EncodeBcd(dateTime.Second),
                 EncodeBcd(dateTime.Millisecond / 10),
        (byte) (dateTime.Millisecond % 10 << 4 | DayOfWeekToInt(dateTime.DayOfWeek))
            };

        }
        private static byte EncodeBcd(int value) => (byte)(value / 10 << 4 | value % 10);
        private static byte MapYear(int year) => year < 2000 ? (byte)(year - 1900) : (byte)(year - 2000);
        private static int DayOfWeekToInt(DayOfWeek dayOfWeek) => (int)(dayOfWeek + 1);
        /// <summary>
        /// Converts an array of <see cref="T:System.DateTime" /> values to a byte array.
        /// </summary>
        /// <param name="dateTimes">The DateTime values to convert.</param>
        /// <returns>A byte array containing the S7 date time representations of <paramref name="dateTimes" />.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when any value of
        /// <paramref name="dateTimes" /> is before <see cref="P:SpecMinimumDateTime" />
        /// or after <see cref="P:SpecMaximumDateTime" />.</exception>
        public static byte[] ToByteArray(DateTime[] dateTimes)
        {
            List<byte> byteList = new List<byte>(dateTimes.Length * 8);
            foreach (DateTime dateTime in dateTimes)
                byteList.AddRange((IEnumerable<byte>)SiemensDateTime.ToByteArray(dateTime));
            return byteList.ToArray();
        }
    }
}
