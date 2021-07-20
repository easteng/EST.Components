// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.LSIS.XGTAddressData
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Text;

namespace ESTCore.Common.Profinet.LSIS
{
    public class XGTAddressData
    {
        public string Address { get; set; }

        public string Data { get; set; }

        public byte[] DataByteArray { get; set; }

        /// <summary>주소 문자열 표현, EX) %DW1100</summary>
        public string AddressString { get; set; }

        /// <summary>AddressString 을 바이트 배열로 변환</summary>
        public byte[] AddressByteArray => Encoding.ASCII.GetBytes(this.AddressString);

        /// <summary>AddressByteArray 바이트 배열의 수(2byte)</summary>
        public byte[] LengthByteArray => BitConverter.GetBytes((short)this.AddressByteArray.Length);
    }
}
