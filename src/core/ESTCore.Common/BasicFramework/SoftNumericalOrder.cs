// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.SoftNumericalOrder
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>一个用于自动流水号生成的类，必须指定保存的文件，实时保存来确认安全</summary>
    /// <remarks>
    /// <note type="important">
    /// 序号生成器软件，当获取序列号，清空序列号操作后，会自动的将ID号存储到本地的文件中，存储方式采用乐观并发模型实现。
    /// </note>
    /// </remarks>
    /// <example>
    /// 此处举个例子，也是Demo程序的源代码，包含了2个按钮的示例和瞬间调用100万次的性能示例。
    /// <note type="tip">百万次调用的实际耗时取决于计算机的性能，不同的计算机的表现存在差异，比如作者的：i5-4590cpu,内存ddr3-8G表示差不多在800毫秒左右</note>
    /// <code lang="cs" source="TestProject\ESTCore.CommonDemo\FormSeqCreate.cs" region="FormSeqCreate" title="示例代码" />
    /// </example>
    public sealed class SoftNumericalOrder : SoftFileSaveBase
    {
        /// <summary>当前的生成序列号</summary>
        private long CurrentIndex = 0;
        /// <summary>流水号的文本头</summary>
        private string TextHead = string.Empty;
        /// <summary>时间格式默认年月日</summary>
        private string TimeFormate = "yyyyMMdd";
        /// <summary>流水号数字应该显示的长度</summary>
        private int NumberLength = 5;
        /// <summary>高性能存储块</summary>
        private EstAsyncCoordinator AsyncCoordinator = (EstAsyncCoordinator)null;

        /// <summary>实例化一个流水号生成的对象</summary>
        /// <param name="textHead">流水号的头文本</param>
        /// <param name="timeFormate">流水号带的时间信息</param>
        /// <param name="numberLength">流水号数字的标准长度，不够补0</param>
        /// <param name="fileSavePath">流水号存储的文本位置</param>
        public SoftNumericalOrder(
          string textHead,
          string timeFormate,
          int numberLength,
          string fileSavePath)
        {
            this.LogHeaderText = nameof(SoftNumericalOrder);
            this.TextHead = textHead;
            this.TimeFormate = timeFormate;
            this.NumberLength = numberLength;
            this.FileSavePath = fileSavePath;
            this.LoadByFile();
            this.AsyncCoordinator = new EstAsyncCoordinator((Action)(() =>
           {
               if (string.IsNullOrEmpty(this.FileSavePath))
                   return;
               using (StreamWriter streamWriter = new StreamWriter(this.FileSavePath, false, Encoding.Default))
                   streamWriter.Write(this.CurrentIndex);
           }));
        }

        /// <summary>获取流水号的值</summary>
        /// <returns>字符串信息</returns>
        public override string ToSaveString() => this.CurrentIndex.ToString();

        /// <summary>加载流水号</summary>
        /// <param name="content">源字符串信息</param>
        public override void LoadByString(string content) => this.CurrentIndex = Convert.ToInt64(content);

        /// <summary>清除流水号计数，进行重新计数</summary>
        public void ClearNumericalOrder()
        {
            Interlocked.Exchange(ref this.CurrentIndex, 0L);
            this.AsyncCoordinator.StartOperaterInfomation();
        }

        /// <summary>获取流水号数据</summary>
        /// <returns>新增计数后的信息</returns>
        public string GetNumericalOrder()
        {
            long num = Interlocked.Increment(ref this.CurrentIndex);
            this.AsyncCoordinator.StartOperaterInfomation();
            return string.IsNullOrEmpty(this.TimeFormate) ? this.TextHead + num.ToString().PadLeft(this.NumberLength, '0') : this.TextHead + DateTime.Now.ToString(this.TimeFormate) + num.ToString().PadLeft(this.NumberLength, '0');
        }

        /// <summary>获取流水号数据</summary>
        /// <param name="textHead">指定一个新的文本头</param>
        /// <returns>带头信息的计数后的信息</returns>
        public string GetNumericalOrder(string textHead)
        {
            long num = Interlocked.Increment(ref this.CurrentIndex);
            this.AsyncCoordinator.StartOperaterInfomation();
            return string.IsNullOrEmpty(this.TimeFormate) ? textHead + num.ToString().PadLeft(this.NumberLength, '0') : textHead + DateTime.Now.ToString(this.TimeFormate) + num.ToString().PadLeft(this.NumberLength, '0');
        }

        /// <summary>单纯的获取数字形式的流水号</summary>
        /// <returns>新增计数后的信息</returns>
        public long GetLongOrder()
        {
            long num = Interlocked.Increment(ref this.CurrentIndex);
            this.AsyncCoordinator.StartOperaterInfomation();
            return num;
        }
    }
}
