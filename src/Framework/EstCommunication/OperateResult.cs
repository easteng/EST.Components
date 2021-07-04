// Decompiled with JetBrains decompiler
// Type: EstCommunication.OperateResult
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication
{
  /// <summary>
  /// 操作结果的类，只带有成功标志和错误信息<br />
  /// The class that operates the result, with only success flags and error messages
  /// </summary>
  /// <remarks>
  /// 当 <see cref="P:EstCommunication.OperateResult.IsSuccess" /> 为 True 时，忽略 <see cref="P:EstCommunication.OperateResult.Message" /> 及 <see cref="P:EstCommunication.OperateResult.ErrorCode" /> 的值
  /// </remarks>
  public class OperateResult
  {
    /// <summary>实例化一个默认的结果对象</summary>
    public OperateResult()
    {
    }

    /// <summary>使用指定的消息实例化一个默认的结果对象</summary>
    /// <param name="msg">错误消息</param>
    public OperateResult(string msg) => this.Message = msg;

    /// <summary>使用错误代码，消息文本来实例化对象</summary>
    /// <param name="err">错误代码</param>
    /// <param name="msg">错误消息</param>
    public OperateResult(int err, string msg)
    {
      this.ErrorCode = err;
      this.Message = msg;
    }

    /// <summary>
    /// 指示本次操作是否成功。<br />
    /// Indicates whether this operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 具体的错误描述。<br />
    /// Specific error description.
    /// </summary>
    public string Message { get; set; } = StringResources.Language.UnknownError;

    /// <summary>
    /// 具体的错误代码。<br />
    /// The specific error code.
    /// </summary>
    public int ErrorCode { get; set; } = 10000;

    /// <summary>
    /// 获取错误代号及文本描述。<br />
    /// Get the error code and text description.
    /// </summary>
    /// <returns>包含错误码及错误消息</returns>
    public string ToMessageShowString() => string.Format("{0}:{1}{2}{3}:{4}", (object) StringResources.Language.ErrorCode, (object) this.ErrorCode, (object) Environment.NewLine, (object) StringResources.Language.TextDescription, (object) this.Message);

    /// <summary>
    /// 从另一个结果类中拷贝错误信息，主要是针对错误码和错误消息。<br />
    /// Copy error information from another result class, mainly for error codes and error messages.
    /// </summary>
    /// <typeparam name="TResult">支持结果类及派生类</typeparam>
    /// <param name="result">结果类及派生类的对象</param>
    public void CopyErrorFromOther<TResult>(TResult result) where TResult : OperateResult
    {
      if ((object) result == null)
        return;
      this.ErrorCode = result.ErrorCode;
      this.Message = result.Message;
    }

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，如果当前结果为失败，则返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// if the current result is a failure, then return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    /// <param name="content">如果操作成功将赋予的结果内容</param>
    /// <returns>最终的结果类对象</returns>
    public OperateResult<T> Convert<T>(T content) => !this.IsSuccess ? OperateResult.CreateFailedResult<T>(this) : OperateResult.CreateSuccessResult<T>(content);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，直接返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// and directly return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T">结果类型</typeparam>
    /// <returns>最终失败的结果类对象</returns>
    public OperateResult<T> ConvertFailed<T>() => OperateResult.CreateFailedResult<T>(this);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，如果当前结果为失败，则返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// if the current result is a failure, then return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <param name="content1">如果操作成功将赋予的结果内容一</param>
    /// <param name="content2">如果操作成功将赋予的结果内容二</param>
    /// <returns>最终的结果类对象</returns>
    public OperateResult<T1, T2> Convert<T1, T2>(T1 content1, T2 content2) => !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2>(this) : OperateResult.CreateSuccessResult<T1, T2>(content1, content2);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，直接返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// and directly return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <returns>最终失败的结果类对象</returns>
    public OperateResult<T1, T2> ConvertFailed<T1, T2>() => OperateResult.CreateFailedResult<T1, T2>(this);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，如果当前结果为失败，则返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// if the current result is a failure, then return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <param name="content1">如果操作成功将赋予的结果内容一</param>
    /// <param name="content2">如果操作成功将赋予的结果内容二</param>
    /// <param name="content3">如果操作成功将赋予的结果内容三</param>
    /// <returns>最终的结果类对象</returns>
    public OperateResult<T1, T2, T3> Convert<T1, T2, T3>(
      T1 content1,
      T2 content2,
      T3 content3)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3>(this) : OperateResult.CreateSuccessResult<T1, T2, T3>(content1, content2, content3);
    }

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，直接返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// and directly return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <returns>最终失败的结果类对象</returns>
    public OperateResult<T1, T2, T3> ConvertFailed<T1, T2, T3>() => OperateResult.CreateFailedResult<T1, T2, T3>(this);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，如果当前结果为失败，则返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// if the current result is a failure, then return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <param name="content1">如果操作成功将赋予的结果内容一</param>
    /// <param name="content2">如果操作成功将赋予的结果内容二</param>
    /// <param name="content3">如果操作成功将赋予的结果内容三</param>
    /// <param name="content4">如果操作成功将赋予的结果内容四</param>
    /// <returns>最终的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4> Convert<T1, T2, T3, T4>(
      T1 content1,
      T2 content2,
      T3 content3,
      T4 content4)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4>(this) : OperateResult.CreateSuccessResult<T1, T2, T3, T4>(content1, content2, content3, content4);
    }

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，直接返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// and directly return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <returns>最终失败的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4> ConvertFailed<T1, T2, T3, T4>() => OperateResult.CreateFailedResult<T1, T2, T3, T4>(this);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，如果当前结果为失败，则返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// if the current result is a failure, then return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <param name="content1">如果操作成功将赋予的结果内容一</param>
    /// <param name="content2">如果操作成功将赋予的结果内容二</param>
    /// <param name="content3">如果操作成功将赋予的结果内容三</param>
    /// <param name="content4">如果操作成功将赋予的结果内容四</param>
    /// <param name="content5">如果操作成功将赋予的结果内容五</param>
    /// <returns>最终的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5> Convert<T1, T2, T3, T4, T5>(
      T1 content1,
      T2 content2,
      T3 content3,
      T4 content4,
      T5 content5)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5>(this) : OperateResult.CreateSuccessResult<T1, T2, T3, T4, T5>(content1, content2, content3, content4, content5);
    }

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，直接返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// and directly return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <returns>最终失败的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5> ConvertFailed<T1, T2, T3, T4, T5>() => OperateResult.CreateFailedResult<T1, T2, T3, T4, T5>(this);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，如果当前结果为失败，则返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// if the current result is a failure, then return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <param name="content1">如果操作成功将赋予的结果内容一</param>
    /// <param name="content2">如果操作成功将赋予的结果内容二</param>
    /// <param name="content3">如果操作成功将赋予的结果内容三</param>
    /// <param name="content4">如果操作成功将赋予的结果内容四</param>
    /// <param name="content5">如果操作成功将赋予的结果内容五</param>
    /// <param name="content6">如果操作成功将赋予的结果内容六</param>
    /// <returns>最终的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6> Convert<T1, T2, T3, T4, T5, T6>(
      T1 content1,
      T2 content2,
      T3 content3,
      T4 content4,
      T5 content5,
      T6 content6)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6>(this) : OperateResult.CreateSuccessResult<T1, T2, T3, T4, T5, T6>(content1, content2, content3, content4, content5, content6);
    }

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，直接返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// and directly return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <returns>最终失败的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6> ConvertFailed<T1, T2, T3, T4, T5, T6>() => OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6>(this);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，如果当前结果为失败，则返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// if the current result is a failure, then return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <param name="content1">如果操作成功将赋予的结果内容一</param>
    /// <param name="content2">如果操作成功将赋予的结果内容二</param>
    /// <param name="content3">如果操作成功将赋予的结果内容三</param>
    /// <param name="content4">如果操作成功将赋予的结果内容四</param>
    /// <param name="content5">如果操作成功将赋予的结果内容五</param>
    /// <param name="content6">如果操作成功将赋予的结果内容六</param>
    /// <param name="content7">如果操作成功将赋予的结果内容七</param>
    /// <returns>最终的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7> Convert<T1, T2, T3, T4, T5, T6, T7>(
      T1 content1,
      T2 content2,
      T3 content3,
      T4 content4,
      T5 content5,
      T6 content6,
      T7 content7)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7>(this) : OperateResult.CreateSuccessResult<T1, T2, T3, T4, T5, T6, T7>(content1, content2, content3, content4, content5, content6, content7);
    }

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，直接返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// and directly return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <returns>最终失败的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7> ConvertFailed<T1, T2, T3, T4, T5, T6, T7>() => OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7>(this);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，如果当前结果为失败，则返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// if the current result is a failure, then return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <typeparam name="T8">泛型参数八</typeparam>
    /// <param name="content1">如果操作成功将赋予的结果内容一</param>
    /// <param name="content2">如果操作成功将赋予的结果内容二</param>
    /// <param name="content3">如果操作成功将赋予的结果内容三</param>
    /// <param name="content4">如果操作成功将赋予的结果内容四</param>
    /// <param name="content5">如果操作成功将赋予的结果内容五</param>
    /// <param name="content6">如果操作成功将赋予的结果内容六</param>
    /// <param name="content7">如果操作成功将赋予的结果内容七</param>
    /// <param name="content8">如果操作成功将赋予的结果内容八</param>
    /// <returns>最终的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7, T8> Convert<T1, T2, T3, T4, T5, T6, T7, T8>(
      T1 content1,
      T2 content2,
      T3 content3,
      T4 content4,
      T5 content5,
      T6 content6,
      T7 content7,
      T8 content8)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8>(this) : OperateResult.CreateSuccessResult<T1, T2, T3, T4, T5, T6, T7, T8>(content1, content2, content3, content4, content5, content6, content7, content8);
    }

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，直接返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// and directly return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <typeparam name="T8">泛型参数八</typeparam>
    /// <returns>最终失败的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7, T8> ConvertFailed<T1, T2, T3, T4, T5, T6, T7, T8>() => OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8>(this);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，如果当前结果为失败，则返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// if the current result is a failure, then return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <typeparam name="T8">泛型参数八</typeparam>
    /// <typeparam name="T9">泛型参数九</typeparam>
    /// <param name="content1">如果操作成功将赋予的结果内容一</param>
    /// <param name="content2">如果操作成功将赋予的结果内容二</param>
    /// <param name="content3">如果操作成功将赋予的结果内容三</param>
    /// <param name="content4">如果操作成功将赋予的结果内容四</param>
    /// <param name="content5">如果操作成功将赋予的结果内容五</param>
    /// <param name="content6">如果操作成功将赋予的结果内容六</param>
    /// <param name="content7">如果操作成功将赋予的结果内容七</param>
    /// <param name="content8">如果操作成功将赋予的结果内容八</param>
    /// <param name="content9">如果操作成功将赋予的结果内容九</param>
    /// <returns>最终的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9> Convert<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
      T1 content1,
      T2 content2,
      T3 content3,
      T4 content4,
      T5 content5,
      T6 content6,
      T7 content7,
      T8 content8,
      T9 content9)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this) : OperateResult.CreateSuccessResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(content1, content2, content3, content4, content5, content6, content7, content8, content9);
    }

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，直接返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// and directly return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <typeparam name="T8">泛型参数八</typeparam>
    /// <typeparam name="T9">泛型参数九</typeparam>
    /// <returns>最终失败的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9> ConvertFailed<T1, T2, T3, T4, T5, T6, T7, T8, T9>() => OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this);

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，如果当前结果为失败，则返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// if the current result is a failure, then return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <typeparam name="T8">泛型参数八</typeparam>
    /// <typeparam name="T9">泛型参数九</typeparam>
    /// <typeparam name="T10">泛型参数十</typeparam>
    /// <param name="content1">如果操作成功将赋予的结果内容一</param>
    /// <param name="content2">如果操作成功将赋予的结果内容二</param>
    /// <param name="content3">如果操作成功将赋予的结果内容三</param>
    /// <param name="content4">如果操作成功将赋予的结果内容四</param>
    /// <param name="content5">如果操作成功将赋予的结果内容五</param>
    /// <param name="content6">如果操作成功将赋予的结果内容六</param>
    /// <param name="content7">如果操作成功将赋予的结果内容七</param>
    /// <param name="content8">如果操作成功将赋予的结果内容八</param>
    /// <param name="content9">如果操作成功将赋予的结果内容九</param>
    /// <param name="content10">如果操作成功将赋予的结果内容十</param>
    /// <returns>最终的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Convert<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
      T1 content1,
      T2 content2,
      T3 content3,
      T4 content4,
      T5 content5,
      T6 content6,
      T7 content7,
      T8 content8,
      T9 content9,
      T10 content10)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this) : OperateResult.CreateSuccessResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(content1, content2, content3, content4, content5, content6, content7, content8, content9, content10);
    }

    /// <summary>
    /// 将当前的结果对象转换到指定泛型的结果类对象，直接返回指定泛型的失败结果类对象<br />
    /// Convert the current result object to the result class object of the specified generic type,
    /// and directly return the result class object of the specified generic type failure
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <typeparam name="T8">泛型参数八</typeparam>
    /// <typeparam name="T9">泛型参数九</typeparam>
    /// <typeparam name="T10">泛型参数十</typeparam>
    /// <returns>最终失败的结果类对象</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ConvertFailed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>() => OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this);

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult Then(Func<OperateResult> func) => !this.IsSuccess ? this : func();

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <typeparam name="T">泛型参数</typeparam>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult<T> Then<T>(Func<OperateResult<T>> func) => !this.IsSuccess ? OperateResult.CreateFailedResult<T>(this) : func();

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult<T1, T2> Then<T1, T2>(Func<OperateResult<T1, T2>> func) => !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2>(this) : func();

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult<T1, T2, T3> Then<T1, T2, T3>(
      Func<OperateResult<T1, T2, T3>> func)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3>(this) : func();
    }

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult<T1, T2, T3, T4> Then<T1, T2, T3, T4>(
      Func<OperateResult<T1, T2, T3, T4>> func)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4>(this) : func();
    }

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult<T1, T2, T3, T4, T5> Then<T1, T2, T3, T4, T5>(
      Func<OperateResult<T1, T2, T3, T4, T5>> func)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5>(this) : func();
    }

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6> Then<T1, T2, T3, T4, T5, T6>(
      Func<OperateResult<T1, T2, T3, T4, T5, T6>> func)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6>(this) : func();
    }

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7> Then<T1, T2, T3, T4, T5, T6, T7>(
      Func<OperateResult<T1, T2, T3, T4, T5, T6, T7>> func)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7>(this) : func();
    }

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <typeparam name="T8">泛型参数八</typeparam>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7, T8> Then<T1, T2, T3, T4, T5, T6, T7, T8>(
      Func<OperateResult<T1, T2, T3, T4, T5, T6, T7, T8>> func)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8>(this) : func();
    }

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <typeparam name="T8">泛型参数八</typeparam>
    /// <typeparam name="T9">泛型参数九</typeparam>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9> Then<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
      Func<OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>> func)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this) : func();
    }

    /// <summary>
    /// 指定接下来要做的是内容，当前对象如果成功，就返回接下来的执行结果，如果失败，就返回当前对象本身。<br />
    /// Specify what you want to do next, return the result of the execution of the current object if it succeeds, and return the current object itself if it fails.
    /// </summary>
    /// <typeparam name="T1">泛型参数一</typeparam>
    /// <typeparam name="T2">泛型参数二</typeparam>
    /// <typeparam name="T3">泛型参数三</typeparam>
    /// <typeparam name="T4">泛型参数四</typeparam>
    /// <typeparam name="T5">泛型参数五</typeparam>
    /// <typeparam name="T6">泛型参数六</typeparam>
    /// <typeparam name="T7">泛型参数七</typeparam>
    /// <typeparam name="T8">泛型参数八</typeparam>
    /// <typeparam name="T9">泛型参数九</typeparam>
    /// <typeparam name="T10">泛型参数十</typeparam>
    /// <param name="func">等待当前对象成功后执行的内容</param>
    /// <returns>返回整个方法链最终的成功失败结果</returns>
    public OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Then<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
      Func<OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> func)
    {
      return !this.IsSuccess ? OperateResult.CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this) : func();
    }

    /// <summary>创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息</summary>
    /// <typeparam name="T">目标数据类型</typeparam>
    /// <param name="result">之前的结果对象</param>
    /// <returns>带默认泛型对象的失败结果类</returns>
    public static OperateResult<T> CreateFailedResult<T>(OperateResult result)
    {
      OperateResult<T> operateResult = new OperateResult<T>();
      operateResult.ErrorCode = result.ErrorCode;
      operateResult.Message = result.Message;
      return operateResult;
    }

    /// <summary>创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息</summary>
    /// <typeparam name="T1">目标数据类型一</typeparam>
    /// <typeparam name="T2">目标数据类型二</typeparam>
    /// <param name="result">之前的结果对象</param>
    /// <returns>带默认泛型对象的失败结果类</returns>
    public static OperateResult<T1, T2> CreateFailedResult<T1, T2>(OperateResult result)
    {
      OperateResult<T1, T2> operateResult = new OperateResult<T1, T2>();
      operateResult.ErrorCode = result.ErrorCode;
      operateResult.Message = result.Message;
      return operateResult;
    }

    /// <summary>创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息</summary>
    /// <typeparam name="T1">目标数据类型一</typeparam>
    /// <typeparam name="T2">目标数据类型二</typeparam>
    /// <typeparam name="T3">目标数据类型三</typeparam>
    /// <param name="result">之前的结果对象</param>
    /// <returns>带默认泛型对象的失败结果类</returns>
    public static OperateResult<T1, T2, T3> CreateFailedResult<T1, T2, T3>(
      OperateResult result)
    {
      OperateResult<T1, T2, T3> operateResult = new OperateResult<T1, T2, T3>();
      operateResult.ErrorCode = result.ErrorCode;
      operateResult.Message = result.Message;
      return operateResult;
    }

    /// <summary>创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息</summary>
    /// <typeparam name="T1">目标数据类型一</typeparam>
    /// <typeparam name="T2">目标数据类型二</typeparam>
    /// <typeparam name="T3">目标数据类型三</typeparam>
    /// <typeparam name="T4">目标数据类型四</typeparam>
    /// <param name="result">之前的结果对象</param>
    /// <returns>带默认泛型对象的失败结果类</returns>
    public static OperateResult<T1, T2, T3, T4> CreateFailedResult<T1, T2, T3, T4>(
      OperateResult result)
    {
      OperateResult<T1, T2, T3, T4> operateResult = new OperateResult<T1, T2, T3, T4>();
      operateResult.ErrorCode = result.ErrorCode;
      operateResult.Message = result.Message;
      return operateResult;
    }

    /// <summary>创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息</summary>
    /// <typeparam name="T1">目标数据类型一</typeparam>
    /// <typeparam name="T2">目标数据类型二</typeparam>
    /// <typeparam name="T3">目标数据类型三</typeparam>
    /// <typeparam name="T4">目标数据类型四</typeparam>
    /// <typeparam name="T5">目标数据类型五</typeparam>
    /// <param name="result">之前的结果对象</param>
    /// <returns>带默认泛型对象的失败结果类</returns>
    public static OperateResult<T1, T2, T3, T4, T5> CreateFailedResult<T1, T2, T3, T4, T5>(
      OperateResult result)
    {
      OperateResult<T1, T2, T3, T4, T5> operateResult = new OperateResult<T1, T2, T3, T4, T5>();
      operateResult.ErrorCode = result.ErrorCode;
      operateResult.Message = result.Message;
      return operateResult;
    }

    /// <summary>创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息</summary>
    /// <typeparam name="T1">目标数据类型一</typeparam>
    /// <typeparam name="T2">目标数据类型二</typeparam>
    /// <typeparam name="T3">目标数据类型三</typeparam>
    /// <typeparam name="T4">目标数据类型四</typeparam>
    /// <typeparam name="T5">目标数据类型五</typeparam>
    /// <typeparam name="T6">目标数据类型六</typeparam>
    /// <param name="result">之前的结果对象</param>
    /// <returns>带默认泛型对象的失败结果类</returns>
    public static OperateResult<T1, T2, T3, T4, T5, T6> CreateFailedResult<T1, T2, T3, T4, T5, T6>(
      OperateResult result)
    {
      OperateResult<T1, T2, T3, T4, T5, T6> operateResult = new OperateResult<T1, T2, T3, T4, T5, T6>();
      operateResult.ErrorCode = result.ErrorCode;
      operateResult.Message = result.Message;
      return operateResult;
    }

    /// <summary>创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息</summary>
    /// <typeparam name="T1">目标数据类型一</typeparam>
    /// <typeparam name="T2">目标数据类型二</typeparam>
    /// <typeparam name="T3">目标数据类型三</typeparam>
    /// <typeparam name="T4">目标数据类型四</typeparam>
    /// <typeparam name="T5">目标数据类型五</typeparam>
    /// <typeparam name="T6">目标数据类型六</typeparam>
    /// <typeparam name="T7">目标数据类型七</typeparam>
    /// <param name="result">之前的结果对象</param>
    /// <returns>带默认泛型对象的失败结果类</returns>
    public static OperateResult<T1, T2, T3, T4, T5, T6, T7> CreateFailedResult<T1, T2, T3, T4, T5, T6, T7>(
      OperateResult result)
    {
      OperateResult<T1, T2, T3, T4, T5, T6, T7> operateResult = new OperateResult<T1, T2, T3, T4, T5, T6, T7>();
      operateResult.ErrorCode = result.ErrorCode;
      operateResult.Message = result.Message;
      return operateResult;
    }

    /// <summary>创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息</summary>
    /// <typeparam name="T1">目标数据类型一</typeparam>
    /// <typeparam name="T2">目标数据类型二</typeparam>
    /// <typeparam name="T3">目标数据类型三</typeparam>
    /// <typeparam name="T4">目标数据类型四</typeparam>
    /// <typeparam name="T5">目标数据类型五</typeparam>
    /// <typeparam name="T6">目标数据类型六</typeparam>
    /// <typeparam name="T7">目标数据类型七</typeparam>
    /// <typeparam name="T8">目标数据类型八</typeparam>
    /// <param name="result">之前的结果对象</param>
    /// <returns>带默认泛型对象的失败结果类</returns>
    public static OperateResult<T1, T2, T3, T4, T5, T6, T7, T8> CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8>(
      OperateResult result)
    {
      OperateResult<T1, T2, T3, T4, T5, T6, T7, T8> operateResult = new OperateResult<T1, T2, T3, T4, T5, T6, T7, T8>();
      operateResult.ErrorCode = result.ErrorCode;
      operateResult.Message = result.Message;
      return operateResult;
    }

    /// <summary>创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息</summary>
    /// <typeparam name="T1">目标数据类型一</typeparam>
    /// <typeparam name="T2">目标数据类型二</typeparam>
    /// <typeparam name="T3">目标数据类型三</typeparam>
    /// <typeparam name="T4">目标数据类型四</typeparam>
    /// <typeparam name="T5">目标数据类型五</typeparam>
    /// <typeparam name="T6">目标数据类型六</typeparam>
    /// <typeparam name="T7">目标数据类型七</typeparam>
    /// <typeparam name="T8">目标数据类型八</typeparam>
    /// <typeparam name="T9">目标数据类型九</typeparam>
    /// <param name="result">之前的结果对象</param>
    /// <returns>带默认泛型对象的失败结果类</returns>
    public static OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9> CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
      OperateResult result)
    {
      OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9> operateResult = new OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>();
      operateResult.ErrorCode = result.ErrorCode;
      operateResult.Message = result.Message;
      return operateResult;
    }

    /// <summary>创建并返回一个失败的结果对象，该对象复制另一个结果对象的错误信息</summary>
    /// <typeparam name="T1">目标数据类型一</typeparam>
    /// <typeparam name="T2">目标数据类型二</typeparam>
    /// <typeparam name="T3">目标数据类型三</typeparam>
    /// <typeparam name="T4">目标数据类型四</typeparam>
    /// <typeparam name="T5">目标数据类型五</typeparam>
    /// <typeparam name="T6">目标数据类型六</typeparam>
    /// <typeparam name="T7">目标数据类型七</typeparam>
    /// <typeparam name="T8">目标数据类型八</typeparam>
    /// <typeparam name="T9">目标数据类型九</typeparam>
    /// <typeparam name="T10">目标数据类型十</typeparam>
    /// <param name="result">之前的结果对象</param>
    /// <returns>带默认泛型对象的失败结果类</returns>
    public static OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> CreateFailedResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
      OperateResult result)
    {
      OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> operateResult = new OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
      operateResult.ErrorCode = result.ErrorCode;
      operateResult.Message = result.Message;
      return operateResult;
    }

    /// <summary>创建并返回一个成功的结果对象</summary>
    /// <returns>成功的结果对象</returns>
    public static OperateResult CreateSuccessResult() => new OperateResult()
    {
      IsSuccess = true,
      ErrorCode = 0,
      Message = StringResources.Language.SuccessText
    };

    /// <summary>创建并返回一个成功的结果对象，并带有一个参数对象</summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="value">类型的值对象</param>
    /// <returns>成功的结果对象</returns>
    public static OperateResult<T> CreateSuccessResult<T>(T value)
    {
      OperateResult<T> operateResult = new OperateResult<T>();
      operateResult.IsSuccess = true;
      operateResult.ErrorCode = 0;
      operateResult.Message = StringResources.Language.SuccessText;
      operateResult.Content = value;
      return operateResult;
    }

    /// <summary>创建并返回一个成功的结果对象，并带有两个参数对象</summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <param name="value1">类型一对象</param>
    /// <param name="value2">类型二对象</param>
    /// <returns>成的结果对象</returns>
    public static OperateResult<T1, T2> CreateSuccessResult<T1, T2>(
      T1 value1,
      T2 value2)
    {
      OperateResult<T1, T2> operateResult = new OperateResult<T1, T2>();
      operateResult.IsSuccess = true;
      operateResult.ErrorCode = 0;
      operateResult.Message = StringResources.Language.SuccessText;
      operateResult.Content1 = value1;
      operateResult.Content2 = value2;
      return operateResult;
    }

    /// <summary>创建并返回一个成功的结果对象，并带有三个参数对象</summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <param name="value1">类型一对象</param>
    /// <param name="value2">类型二对象</param>
    /// <param name="value3">类型三对象</param>
    /// <returns>成的结果对象</returns>
    public static OperateResult<T1, T2, T3> CreateSuccessResult<T1, T2, T3>(
      T1 value1,
      T2 value2,
      T3 value3)
    {
      OperateResult<T1, T2, T3> operateResult = new OperateResult<T1, T2, T3>();
      operateResult.IsSuccess = true;
      operateResult.ErrorCode = 0;
      operateResult.Message = StringResources.Language.SuccessText;
      operateResult.Content1 = value1;
      operateResult.Content2 = value2;
      operateResult.Content3 = value3;
      return operateResult;
    }

    /// <summary>创建并返回一个成功的结果对象，并带有四个参数对象</summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    /// <param name="value1">类型一对象</param>
    /// <param name="value2">类型二对象</param>
    /// <param name="value3">类型三对象</param>
    /// <param name="value4">类型四对象</param>
    /// <returns>成的结果对象</returns>
    public static OperateResult<T1, T2, T3, T4> CreateSuccessResult<T1, T2, T3, T4>(
      T1 value1,
      T2 value2,
      T3 value3,
      T4 value4)
    {
      OperateResult<T1, T2, T3, T4> operateResult = new OperateResult<T1, T2, T3, T4>();
      operateResult.IsSuccess = true;
      operateResult.ErrorCode = 0;
      operateResult.Message = StringResources.Language.SuccessText;
      operateResult.Content1 = value1;
      operateResult.Content2 = value2;
      operateResult.Content3 = value3;
      operateResult.Content4 = value4;
      return operateResult;
    }

    /// <summary>创建并返回一个成功的结果对象，并带有五个参数对象</summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    /// <typeparam name="T5">第五个参数类型</typeparam>
    /// <param name="value1">类型一对象</param>
    /// <param name="value2">类型二对象</param>
    /// <param name="value3">类型三对象</param>
    /// <param name="value4">类型四对象</param>
    /// <param name="value5">类型五对象</param>
    /// <returns>成的结果对象</returns>
    public static OperateResult<T1, T2, T3, T4, T5> CreateSuccessResult<T1, T2, T3, T4, T5>(
      T1 value1,
      T2 value2,
      T3 value3,
      T4 value4,
      T5 value5)
    {
      OperateResult<T1, T2, T3, T4, T5> operateResult = new OperateResult<T1, T2, T3, T4, T5>();
      operateResult.IsSuccess = true;
      operateResult.ErrorCode = 0;
      operateResult.Message = StringResources.Language.SuccessText;
      operateResult.Content1 = value1;
      operateResult.Content2 = value2;
      operateResult.Content3 = value3;
      operateResult.Content4 = value4;
      operateResult.Content5 = value5;
      return operateResult;
    }

    /// <summary>创建并返回一个成功的结果对象，并带有六个参数对象</summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    /// <typeparam name="T5">第五个参数类型</typeparam>
    /// <typeparam name="T6">第六个参数类型</typeparam>
    /// <param name="value1">类型一对象</param>
    /// <param name="value2">类型二对象</param>
    /// <param name="value3">类型三对象</param>
    /// <param name="value4">类型四对象</param>
    /// <param name="value5">类型五对象</param>
    /// <param name="value6">类型六对象</param>
    /// <returns>成的结果对象</returns>
    public static OperateResult<T1, T2, T3, T4, T5, T6> CreateSuccessResult<T1, T2, T3, T4, T5, T6>(
      T1 value1,
      T2 value2,
      T3 value3,
      T4 value4,
      T5 value5,
      T6 value6)
    {
      OperateResult<T1, T2, T3, T4, T5, T6> operateResult = new OperateResult<T1, T2, T3, T4, T5, T6>();
      operateResult.IsSuccess = true;
      operateResult.ErrorCode = 0;
      operateResult.Message = StringResources.Language.SuccessText;
      operateResult.Content1 = value1;
      operateResult.Content2 = value2;
      operateResult.Content3 = value3;
      operateResult.Content4 = value4;
      operateResult.Content5 = value5;
      operateResult.Content6 = value6;
      return operateResult;
    }

    /// <summary>创建并返回一个成功的结果对象，并带有七个参数对象</summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    /// <typeparam name="T5">第五个参数类型</typeparam>
    /// <typeparam name="T6">第六个参数类型</typeparam>
    /// <typeparam name="T7">第七个参数类型</typeparam>
    /// <param name="value1">类型一对象</param>
    /// <param name="value2">类型二对象</param>
    /// <param name="value3">类型三对象</param>
    /// <param name="value4">类型四对象</param>
    /// <param name="value5">类型五对象</param>
    /// <param name="value6">类型六对象</param>
    /// <param name="value7">类型七对象</param>
    /// <returns>成的结果对象</returns>
    public static OperateResult<T1, T2, T3, T4, T5, T6, T7> CreateSuccessResult<T1, T2, T3, T4, T5, T6, T7>(
      T1 value1,
      T2 value2,
      T3 value3,
      T4 value4,
      T5 value5,
      T6 value6,
      T7 value7)
    {
      OperateResult<T1, T2, T3, T4, T5, T6, T7> operateResult = new OperateResult<T1, T2, T3, T4, T5, T6, T7>();
      operateResult.IsSuccess = true;
      operateResult.ErrorCode = 0;
      operateResult.Message = StringResources.Language.SuccessText;
      operateResult.Content1 = value1;
      operateResult.Content2 = value2;
      operateResult.Content3 = value3;
      operateResult.Content4 = value4;
      operateResult.Content5 = value5;
      operateResult.Content6 = value6;
      operateResult.Content7 = value7;
      return operateResult;
    }

    /// <summary>创建并返回一个成功的结果对象，并带有八个参数对象</summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    /// <typeparam name="T5">第五个参数类型</typeparam>
    /// <typeparam name="T6">第六个参数类型</typeparam>
    /// <typeparam name="T7">第七个参数类型</typeparam>
    /// <typeparam name="T8">第八个参数类型</typeparam>
    /// <param name="value1">类型一对象</param>
    /// <param name="value2">类型二对象</param>
    /// <param name="value3">类型三对象</param>
    /// <param name="value4">类型四对象</param>
    /// <param name="value5">类型五对象</param>
    /// <param name="value6">类型六对象</param>
    /// <param name="value7">类型七对象</param>
    /// <param name="value8">类型八对象</param>
    /// <returns>成的结果对象</returns>
    public static OperateResult<T1, T2, T3, T4, T5, T6, T7, T8> CreateSuccessResult<T1, T2, T3, T4, T5, T6, T7, T8>(
      T1 value1,
      T2 value2,
      T3 value3,
      T4 value4,
      T5 value5,
      T6 value6,
      T7 value7,
      T8 value8)
    {
      OperateResult<T1, T2, T3, T4, T5, T6, T7, T8> operateResult = new OperateResult<T1, T2, T3, T4, T5, T6, T7, T8>();
      operateResult.IsSuccess = true;
      operateResult.ErrorCode = 0;
      operateResult.Message = StringResources.Language.SuccessText;
      operateResult.Content1 = value1;
      operateResult.Content2 = value2;
      operateResult.Content3 = value3;
      operateResult.Content4 = value4;
      operateResult.Content5 = value5;
      operateResult.Content6 = value6;
      operateResult.Content7 = value7;
      operateResult.Content8 = value8;
      return operateResult;
    }

    /// <summary>创建并返回一个成功的结果对象，并带有九个参数对象</summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    /// <typeparam name="T5">第五个参数类型</typeparam>
    /// <typeparam name="T6">第六个参数类型</typeparam>
    /// <typeparam name="T7">第七个参数类型</typeparam>
    /// <typeparam name="T8">第八个参数类型</typeparam>
    /// <typeparam name="T9">第九个参数类型</typeparam>
    /// <param name="value1">类型一对象</param>
    /// <param name="value2">类型二对象</param>
    /// <param name="value3">类型三对象</param>
    /// <param name="value4">类型四对象</param>
    /// <param name="value5">类型五对象</param>
    /// <param name="value6">类型六对象</param>
    /// <param name="value7">类型七对象</param>
    /// <param name="value8">类型八对象</param>
    /// <param name="value9">类型九对象</param>
    /// <returns>成的结果对象</returns>
    public static OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9> CreateSuccessResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
      T1 value1,
      T2 value2,
      T3 value3,
      T4 value4,
      T5 value5,
      T6 value6,
      T7 value7,
      T8 value8,
      T9 value9)
    {
      OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9> operateResult = new OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>();
      operateResult.IsSuccess = true;
      operateResult.ErrorCode = 0;
      operateResult.Message = StringResources.Language.SuccessText;
      operateResult.Content1 = value1;
      operateResult.Content2 = value2;
      operateResult.Content3 = value3;
      operateResult.Content4 = value4;
      operateResult.Content5 = value5;
      operateResult.Content6 = value6;
      operateResult.Content7 = value7;
      operateResult.Content8 = value8;
      operateResult.Content9 = value9;
      return operateResult;
    }

    /// <summary>创建并返回一个成功的结果对象，并带有十个参数对象</summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    /// <typeparam name="T5">第五个参数类型</typeparam>
    /// <typeparam name="T6">第六个参数类型</typeparam>
    /// <typeparam name="T7">第七个参数类型</typeparam>
    /// <typeparam name="T8">第八个参数类型</typeparam>
    /// <typeparam name="T9">第九个参数类型</typeparam>
    /// <typeparam name="T10">第十个参数类型</typeparam>
    /// <param name="value1">类型一对象</param>
    /// <param name="value2">类型二对象</param>
    /// <param name="value3">类型三对象</param>
    /// <param name="value4">类型四对象</param>
    /// <param name="value5">类型五对象</param>
    /// <param name="value6">类型六对象</param>
    /// <param name="value7">类型七对象</param>
    /// <param name="value8">类型八对象</param>
    /// <param name="value9">类型九对象</param>
    /// <param name="value10">类型十对象</param>
    /// <returns>成的结果对象</returns>
    public static OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> CreateSuccessResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
      T1 value1,
      T2 value2,
      T3 value3,
      T4 value4,
      T5 value5,
      T6 value6,
      T7 value7,
      T8 value8,
      T9 value9,
      T10 value10)
    {
      OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> operateResult = new OperateResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
      operateResult.IsSuccess = true;
      operateResult.ErrorCode = 0;
      operateResult.Message = StringResources.Language.SuccessText;
      operateResult.Content1 = value1;
      operateResult.Content2 = value2;
      operateResult.Content3 = value3;
      operateResult.Content4 = value4;
      operateResult.Content5 = value5;
      operateResult.Content6 = value6;
      operateResult.Content7 = value7;
      operateResult.Content8 = value8;
      operateResult.Content9 = value9;
      operateResult.Content10 = value10;
      return operateResult;
    }
  }
}
