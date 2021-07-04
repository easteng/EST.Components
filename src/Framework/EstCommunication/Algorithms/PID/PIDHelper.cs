// Decompiled with JetBrains decompiler
// Type: EstCommunication.Algorithms.PID.PIDHelper
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Algorithms.PID
{
  /// <summary>一个PID的辅助类，可以设置 P,I,D 三者的值，用来模拟信号波动的时候，信号的收敛情况</summary>
  public class PIDHelper
  {
    private double prakp;
    private double praki;
    private double prakd;
    private double prvalue;
    private double err;
    private double err_last;
    private double err_next;
    private double setValue;
    private double deadband;
    private double MAXLIM;
    private double MINLIM;
    private int index;
    private int UMAX;
    private int UMIN;

    /// <summary>实例化一个默认的对象</summary>
    public PIDHelper() => this.PidInit();

    /// <summary>初始化PID的数据信息</summary>
    private void PidInit()
    {
      this.prakp = 0.0;
      this.praki = 0.0;
      this.prakd = 0.0;
      this.prvalue = 0.0;
      this.err = 0.0;
      this.err_last = 0.0;
      this.err_next = 0.0;
      this.MAXLIM = double.MaxValue;
      this.MINLIM = double.MinValue;
      this.UMAX = 310;
      this.UMIN = -100;
      this.deadband = 2.0;
    }

    /// <summary>
    /// -rando
    /// 比例的参数信息
    /// </summary>
    public double Kp
    {
      set => this.prakp = value;
      get => this.prakp;
    }

    /// <summary>积分的参数信息</summary>
    public double Ki
    {
      set => this.praki = value;
      get => this.praki;
    }

    /// <summary>微分的参数信息</summary>
    public double Kd
    {
      set => this.prakd = value;
      get => this.prakd;
    }

    /// <summary>获取或设置死区的值</summary>
    public double DeadBand
    {
      set => this.deadband = value;
      get => this.deadband;
    }

    /// <summary>获取或设置输出的上限，默认为没有设置</summary>
    public double MaxLimit
    {
      set => this.MAXLIM = value;
      get => this.MAXLIM;
    }

    /// <summary>获取或设置输出的下限，默认为没有设置</summary>
    public double MinLimit
    {
      set => this.MINLIM = value;
      get => this.MINLIM;
    }

    /// <summary>获取或设置当前设置的值</summary>
    public double SetValue
    {
      set => this.setValue = value;
      get => this.setValue;
    }

    /// <summary>计算Pid数据的值</summary>
    /// <returns>计算值</returns>
    public double PidCalculate()
    {
      this.err_next = this.err_last;
      this.err_last = this.err;
      this.err = this.SetValue - this.prvalue;
      this.prvalue += this.prakp * (this.err - this.err_last + this.praki * this.err + this.prakd * (this.err - 2.0 * this.err_last + this.err_next));
      if (this.prvalue > this.MAXLIM)
        this.prvalue = this.MAXLIM;
      if (this.prvalue < this.MINLIM)
        this.prvalue = this.MINLIM;
      return this.prvalue;
    }
  }
}
