// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.OperateResult`7
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common
{
    /// <summary>操作结果的泛型类，允许带七个用户自定义的泛型对象，推荐使用这个类</summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    /// <typeparam name="T3">泛型类</typeparam>
    /// <typeparam name="T4">泛型类</typeparam>
    /// <typeparam name="T5">泛型类</typeparam>
    /// <typeparam name="T6">泛型类</typeparam>
    /// <typeparam name="T7">泛型类</typeparam>
    public class OperateResult<T1, T2, T3, T4, T5, T6, T7> : OperateResult
    {
        /// <summary>实例化一个默认的结果对象</summary>
        public OperateResult()
        {
        }

        /// <summary>使用指定的消息实例化一个默认的结果对象</summary>
        /// <param name="msg">错误消息</param>
        public OperateResult(string msg)
          : base(msg)
        {
        }

        /// <summary>使用错误代码，消息文本来实例化对象</summary>
        /// <param name="err">错误代码</param>
        /// <param name="msg">错误消息</param>
        public OperateResult(int err, string msg)
          : base(err, msg)
        {
        }

        /// <summary>用户自定义的泛型数据1</summary>
        public T1 Content1 { get; set; }

        /// <summary>用户自定义的泛型数据2</summary>
        public T2 Content2 { get; set; }

        /// <summary>用户自定义的泛型数据3</summary>
        public T3 Content3 { get; set; }

        /// <summary>用户自定义的泛型数据4</summary>
        public T4 Content4 { get; set; }

        /// <summary>用户自定义的泛型数据5</summary>
        public T5 Content5 { get; set; }

        /// <summary>用户自定义的泛型数据6</summary>
        public T6 Content6 { get; set; }

        /// <summary>用户自定义的泛型数据7</summary>
        public T7 Content7 { get; set; }

        /// <summary>
        /// 返回一个检查结果对象，可以进行自定义的数据检查。<br />
        /// Returns a check result object that allows you to perform custom data checks.
        /// </summary>
        /// <param name="check">检查的委托方法</param>
        /// <param name="message">检查失败的错误消息</param>
        /// <returns>如果检查成功，则返回对象本身，如果失败，返回错误信息。</returns>
        public OperateResult<T1, T2, T3, T4, T5, T6, T7> Check(
          Func<T1, T2, T3, T4, T5, T6, T7, bool> check,
          string message = "All content data check failed")
        {
            return !this.IsSuccess || check(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7) ? this : new OperateResult<T1, T2, T3, T4, T5, T6, T7>(message);
        }

        /// <summary>
        /// 返回一个检查结果对象，可以进行自定义的数据检查。<br />
        /// Returns a check result object that allows you to perform custom data checks.
        /// </summary>
        /// <param name="check">检查的委托方法</param>
        /// <returns>如果检查成功，则返回对象本身，如果失败，返回错误信息。</returns>
        public OperateResult<T1, T2, T3, T4, T5, T6, T7> Check(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult> check)
        {
            if (!this.IsSuccess)
                return this;
            OperateResult result = check(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7>(result) : this;
        }

        /// <summary>
        /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
        /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
        /// </summary>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult Then(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult> func)
        {
            return !this.IsSuccess ? (OperateResult)this : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }

        /// <summary>指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。</summary>
        /// <typeparam name="TResult">泛型参数</typeparam>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult<TResult> Then<TResult>(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult<TResult>> func)
        {
            return !this.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult)this) : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }

        /// <summary>
        /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
        /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
        /// </summary>
        /// <typeparam name="TResult1">泛型参数一</typeparam>
        /// <typeparam name="TResult2">泛型参数二</typeparam>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult<TResult1, TResult2> Then<TResult1, TResult2>(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult<TResult1, TResult2>> func)
        {
            return !this.IsSuccess ? OperateResult.CreateFailedResult<TResult1, TResult2>((OperateResult)this) : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }

        /// <summary>
        /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
        /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
        /// </summary>
        /// <typeparam name="TResult1">泛型参数一</typeparam>
        /// <typeparam name="TResult2">泛型参数二</typeparam>
        /// <typeparam name="TResult3">泛型参数三</typeparam>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult<TResult1, TResult2, TResult3> Then<TResult1, TResult2, TResult3>(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult<TResult1, TResult2, TResult3>> func)
        {
            return !this.IsSuccess ? OperateResult.CreateFailedResult<TResult1, TResult2, TResult3>((OperateResult)this) : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }

        /// <summary>
        /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
        /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
        /// </summary>
        /// <typeparam name="TResult1">泛型参数一</typeparam>
        /// <typeparam name="TResult2">泛型参数二</typeparam>
        /// <typeparam name="TResult3">泛型参数三</typeparam>
        /// <typeparam name="TResult4">泛型参数四</typeparam>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult<TResult1, TResult2, TResult3, TResult4> Then<TResult1, TResult2, TResult3, TResult4>(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult<TResult1, TResult2, TResult3, TResult4>> func)
        {
            return !this.IsSuccess ? OperateResult.CreateFailedResult<TResult1, TResult2, TResult3, TResult4>((OperateResult)this) : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }

        /// <summary>
        /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
        /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
        /// </summary>
        /// <typeparam name="TResult1">泛型参数一</typeparam>
        /// <typeparam name="TResult2">泛型参数二</typeparam>
        /// <typeparam name="TResult3">泛型参数三</typeparam>
        /// <typeparam name="TResult4">泛型参数四</typeparam>
        /// <typeparam name="TResult5">泛型参数五</typeparam>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5> Then<TResult1, TResult2, TResult3, TResult4, TResult5>(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5>> func)
        {
            return !this.IsSuccess ? OperateResult.CreateFailedResult<TResult1, TResult2, TResult3, TResult4, TResult5>((OperateResult)this) : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }

        /// <summary>
        /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
        /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
        /// </summary>
        /// <typeparam name="TResult1">泛型参数一</typeparam>
        /// <typeparam name="TResult2">泛型参数二</typeparam>
        /// <typeparam name="TResult3">泛型参数三</typeparam>
        /// <typeparam name="TResult4">泛型参数四</typeparam>
        /// <typeparam name="TResult5">泛型参数五</typeparam>
        /// <typeparam name="TResult6">泛型参数六</typeparam>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> Then<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>> func)
        {
            return !this.IsSuccess ? OperateResult.CreateFailedResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>((OperateResult)this) : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }

        /// <summary>
        /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
        /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
        /// </summary>
        /// <typeparam name="TResult1">泛型参数一</typeparam>
        /// <typeparam name="TResult2">泛型参数二</typeparam>
        /// <typeparam name="TResult3">泛型参数三</typeparam>
        /// <typeparam name="TResult4">泛型参数四</typeparam>
        /// <typeparam name="TResult5">泛型参数五</typeparam>
        /// <typeparam name="TResult6">泛型参数六</typeparam>
        /// <typeparam name="TResult7">泛型参数七</typeparam>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> Then<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>> func)
        {
            return !this.IsSuccess ? OperateResult.CreateFailedResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>((OperateResult)this) : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }

        /// <summary>
        /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
        /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
        /// </summary>
        /// <typeparam name="TResult1">泛型参数一</typeparam>
        /// <typeparam name="TResult2">泛型参数二</typeparam>
        /// <typeparam name="TResult3">泛型参数三</typeparam>
        /// <typeparam name="TResult4">泛型参数四</typeparam>
        /// <typeparam name="TResult5">泛型参数五</typeparam>
        /// <typeparam name="TResult6">泛型参数六</typeparam>
        /// <typeparam name="TResult7">泛型参数七</typeparam>
        /// <typeparam name="TResult8">泛型参数八</typeparam>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8> Then<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>> func)
        {
            return !this.IsSuccess ? OperateResult.CreateFailedResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>((OperateResult)this) : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }

        /// <summary>
        /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
        /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
        /// </summary>
        /// <typeparam name="TResult1">泛型参数一</typeparam>
        /// <typeparam name="TResult2">泛型参数二</typeparam>
        /// <typeparam name="TResult3">泛型参数三</typeparam>
        /// <typeparam name="TResult4">泛型参数四</typeparam>
        /// <typeparam name="TResult5">泛型参数五</typeparam>
        /// <typeparam name="TResult6">泛型参数六</typeparam>
        /// <typeparam name="TResult7">泛型参数七</typeparam>
        /// <typeparam name="TResult8">泛型参数八</typeparam>
        /// <typeparam name="TResult9">泛型参数九</typeparam>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8, TResult9> Then<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8, TResult9>(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8, TResult9>> func)
        {
            return !this.IsSuccess ? OperateResult.CreateFailedResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8, TResult9>((OperateResult)this) : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }

        /// <summary>
        /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
        /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
        /// </summary>
        /// <typeparam name="TResult1">泛型参数一</typeparam>
        /// <typeparam name="TResult2">泛型参数二</typeparam>
        /// <typeparam name="TResult3">泛型参数三</typeparam>
        /// <typeparam name="TResult4">泛型参数四</typeparam>
        /// <typeparam name="TResult5">泛型参数五</typeparam>
        /// <typeparam name="TResult6">泛型参数六</typeparam>
        /// <typeparam name="TResult7">泛型参数七</typeparam>
        /// <typeparam name="TResult8">泛型参数八</typeparam>
        /// <typeparam name="TResult9">泛型参数九</typeparam>
        /// <typeparam name="TResult10">泛型参数十</typeparam>
        /// <param name="func">等待当前对象成功后执行的内容</param>
        /// <returns>返回整个方法链最终的成功失败结果</returns>
        public OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8, TResult9, TResult10> Then<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8, TResult9, TResult10>(
          Func<T1, T2, T3, T4, T5, T6, T7, OperateResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8, TResult9, TResult10>> func)
        {
            return !this.IsSuccess ? OperateResult.CreateFailedResult<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8, TResult9, TResult10>((OperateResult)this) : func(this.Content1, this.Content2, this.Content3, this.Content4, this.Content5, this.Content6, this.Content7);
        }
    }
}
