// Decompiled with JetBrains decompiler
// Type: EstCommunication.Authorization
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Enthernet;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace EstCommunication
{
  /// <summary>系统的基本授权类</summary>
  public class Authorization
  {
    private static DateTime niahdiahduasdbubfas = DateTime.Now;
    internal static long naihsdadaasdasdiwasdaid = 0;
    internal static long moashdawidaisaosdas = 0;
    internal static double nuasgdawydbishcgas = 8.0;
    internal static int nuasgdaaydbishdgas = 0;
    internal static int nuasgdawydbishdgas = 8;
    internal static double nuasgdawydaishdgas = 24.0;
    internal static int nasidhadguawdbasd = 1000;
    internal static int niasdhasdguawdwdad = 12345;
    internal static int hidahwdauushduasdhu = 23456;
    internal static long iahsduiwikaskfhishfdi = 0;
    internal static int zxnkasdhiashifshfsofh = 0;

    static Authorization()
    {
      Authorization.niahdiahduasdbubfas = Authorization.iashdagsdawbdawda();
      if ((ulong) Authorization.naihsdadaasdasdiwasdaid > 0UL)
        Authorization.naihsdadaasdasdiwasdaid = 0L;
      if (Authorization.nuasgdawydaishdgas != 24.0)
        Authorization.nuasgdawydaishdgas = 24.0;
      if ((uint) Authorization.nuasgdaaydbishdgas > 0U)
        Authorization.nuasgdaaydbishdgas = 0;
      if (Authorization.nuasgdawydbishdgas == 24)
        return;
      Authorization.nuasgdawydbishdgas = 24;
    }

    private static void asidhiahfaoaksdnasoif(object obj)
    {
      for (int index = 0; index < 3600; ++index)
      {
        Thread.Sleep(1000);
        if (Authorization.naihsdadaasdasdiwasdaid == (long) Authorization.niasdhasdguawdwdad && Authorization.nuasgdaaydbishdgas > 0)
          return;
      }
     // new NetSimplifyClient("118.24.36.220", 18467).ReadCustomerFromServer((NetHandle) 500, SoftBasic.FrameworkVersion.ToString());
    }

    internal static bool nzugaydgwadawdibbas()
    {
      ++Authorization.moashdawidaisaosdas;
            //return Authorization.naihsdadaasdasdiwasdaid == (long) Authorization.niasdhasdguawdwdad && Authorization.nuasgdaaydbishdgas > 0 || (Authorization.iashdagsdawbdawda() - Authorization.niahdiahduasdbubfas).TotalHours < Authorization.nuasgdawydaishdgas ? Authorization.nuasduagsdwydbasudasd() : Authorization.asdhuasdgawydaduasdgu();
            return true;
    }

    /// <summary>商业授权则返回true，否则返回false</summary>
    /// <returns>是否成功进行商业授权</returns>
    internal static bool asdniasnfaksndiqwhawfskhfaiw() => Authorization.naihsdadaasdasdiwasdaid == (long) Authorization.niasdhasdguawdwdad && Authorization.nuasgdaaydbishdgas > 0 || (Authorization.iashdagsdawbdawda() - Authorization.niahdiahduasdbubfas).TotalHours < (double) Authorization.nuasgdawydbishdgas ? Authorization.nuasduagsdwydbasudasd() : Authorization.asdhuasdgawydaduasdgu();

    internal static bool nuasduagsdwydbasudasd() => true;

    internal static bool asdhuasdgawydaduasdgu() => true;

    internal static bool ashdadgawdaihdadsidas() => Authorization.niasdhasdguawdwdad == 12345;

    internal static DateTime iashdagsdawbdawda() => DateTime.Now;

    internal static DateTime iashdagsaawbdawda() => DateTime.Now.AddDays(1.0);

    internal static DateTime iashdagsaawadawda() => DateTime.Now.AddDays(2.0);

    internal static void oasjodaiwfsodopsdjpasjpf() => Interlocked.Increment(ref Authorization.iahsduiwikaskfhishfdi);

    internal static string nasduabwduadawdb(string miawdiawduasdhasd)
    {
      StringBuilder stringBuilder = new StringBuilder();
      MD5 md5 = MD5.Create();
      byte[] hash = md5.ComputeHash(Encoding.Unicode.GetBytes(miawdiawduasdhasd));
      md5.Clear();
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(((int) byte.MaxValue - (int) hash[index]).ToString("X2"));
      return stringBuilder.ToString();
    }

    /// <summary>
    /// 设置本组件系统的授权信息，如果激活失败，只能使用24小时，24小时后所有的网络通信不会成功<br />
    /// Set the authorization information of this component system. If the activation fails, it can only be used for 8 hours. All network communication will not succeed after 8 hours
    /// </summary>
    /// <param name="code">授权码</param>
    public static bool SetAuthorizationCode(string code)
    {
      if (Authorization.nasduabwduadawdb(code) == "E7A0FE0168770AAB09BFE8CA603CFAE6")
      {
        Authorization.nuasgdaaydbishdgas = 1;
        Authorization.nuasgdawydbishcgas = 286512937.0;
        Authorization.nuasgdawydaishdgas = 87600.0;
        return Authorization.nuasduagsdwydbasudasd();
      }
      if (Authorization.nasduabwduadawdb(code) == "B7D40F4D8B229E02AC463A096BCD5707")
      {
        Authorization.nuasgdaaydbishdgas = 1;
        Authorization.nuasgdawydbishcgas = 286512937.0;
        Authorization.nuasgdawydaishdgas = 2160.0;
        return Authorization.nuasduagsdwydbasudasd();
      }
      if (!(Authorization.nasduabwduadawdb(code) == "2765FFFDDE2A8465A9522442F5A15593"))
        return Authorization.asdhuasdgawydaduasdgu();
      Authorization.nuasgdaaydbishdgas = 10000;
      Authorization.nuasgdawydbishcgas = (double) Authorization.nuasgdawydbishdgas;
      Authorization.naihsdadaasdasdiwasdaid = (long) Authorization.niasdhasdguawdwdad;
      return Authorization.nuasduagsdwydbasudasd();
    }
  }
}
