// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.LSIS.XGT_Memory_TypeClass
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Profinet.LSIS
{
    public static class XGT_Memory_TypeClass
    {
        /// <summary>입출력(Bit)</summary>
        public const string IO = "P";
        /// <summary>보조릴레이(Bit)</summary>
        public const string SubRelay = "M";
        /// <summary>링크릴레이(Bit)</summary>
        public const string LinkRelay = "L";
        /// <summary>Keep릴레이(Bit)</summary>
        public const string KeepRelay = "K";
        /// <summary>특수릴레이(Bit)</summary>
        public const string EtcRelay = "F";
        /// <summary>타이머(현재값)(Word)</summary>
        public const string Timer = "T";
        /// <summary>카운터(현재값)(Word)</summary>
        public const string Counter = "C";
        /// <summary>데이터레지스터(Word)</summary>
        public const string DataRegister = "D";
        /// <summary>통신 데이터레지스터(Word)</summary>
        public const string ComDataRegister = "N";
        /// <summary>파일 레지스터(Word)</summary>
        public const string FileDataRegister = "R";
        /// <summary>파일 레지스터(Word)</summary>
        public const string StepRelay = "S";
        /// <summary>파일 레지스터(Word)</summary>
        public const string SpecialRegister = "U";
    }
}
