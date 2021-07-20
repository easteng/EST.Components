// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Instrument.DLT.DLTControl
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Instrument.DLT
{
    /// <summary>基本的控制码信息</summary>
    public class DLTControl
    {
        /// <summary>保留</summary>
        public const byte Retain = 0;
        /// <summary>广播</summary>
        public const byte Broadcast = 8;
        /// <summary>读数据</summary>
        public const byte ReadData = 17;
        /// <summary>读后续数据</summary>
        public const byte ReadFollowData = 18;
        /// <summary>读通信地址</summary>
        public const byte ReadAddress = 19;
        /// <summary>写数据</summary>
        public const byte WriteData = 20;
        /// <summary>写通信地址</summary>
        public const byte WriteAddress = 21;
        /// <summary>冻结命令</summary>
        public const byte FreezeCommand = 22;
        /// <summary>更改通信速率</summary>
        public const byte ChangeBaudRate = 23;
        /// <summary>修改密码</summary>
        public const byte ChangePassword = 24;
        /// <summary>最大需求量清零</summary>
        public const byte ClearMaxQuantityDemanded = 25;
        /// <summary>电表清零</summary>
        public const byte ElectricityReset = 26;
        /// <summary>事件清零</summary>
        public const byte EventReset = 27;
        /// <summary>跳合闸、报警、保电</summary>
        public const byte ClosingAlarmPowerpProtection = 28;
        /// <summary>多功能端子输出控制命令</summary>
        public const byte MultiFunctionTerminalOutputControlCommand = 29;
        /// <summary>安全认证命令</summary>
        public const byte SecurityAuthenticationCommand = 3;
    }
}
