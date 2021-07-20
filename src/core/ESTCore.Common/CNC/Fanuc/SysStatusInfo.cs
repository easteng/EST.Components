// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.CNC.Fanuc.SysStatusInfo
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.CNC.Fanuc
{
    /// <summary>系统状态信息</summary>
    public class SysStatusInfo
    {
        /// <summary>dummy</summary>
        public short Dummy { get; set; }

        /// <summary>T/M mode</summary>
        public short TMMode { get; set; }

        /// <summary>selected automatic mode</summary>
        public CNCWorkMode WorkMode { get; set; }

        /// <summary>running status</summary>
        public CNCRunStatus RunStatus { get; set; }

        /// <summary>axis, dwell status</summary>
        public short Motion { get; set; }

        /// <summary>m, s, t, b status</summary>
        public short MSTB { get; set; }

        /// <summary>emergency stop status，为1就是急停，为0就是正常</summary>
        public short Emergency { get; set; }

        /// <summary>alarm status</summary>
        public short Alarm { get; set; }

        /// <summary>editting status</summary>
        public short Edit { get; set; }

        /// <inheritdoc />
        public override string ToString() => string.Format("Dummy: {0}, TMMode:{1}, WorkMode:{2}, RunStatus:{3}, ", (object)this.Dummy, (object)this.TMMode, (object)this.WorkMode, (object)this.RunStatus) + string.Format("Motion:{0}, MSTB:{1}, Emergency:{2}, Alarm:{3}, Edit:{4}", (object)this.Motion, (object)this.MSTB, (object)this.Emergency, (object)this.Alarm, (object)this.Edit);
    }
}
