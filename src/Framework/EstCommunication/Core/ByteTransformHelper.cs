// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.ByteTransformHelper
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using System;

namespace EstCommunication.Core
{
  /// <summary>
  /// 所有数据转换类的静态辅助方法<br />
  /// Static helper method for all data conversion classes
  /// </summary>
  public static class ByteTransformHelper
  {
    /// <summary>结果转换操作的基础方法，需要支持类型，及转换的委托，并捕获转换时的异常方法</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <param name="result">源</param>
    /// <param name="translator">实际转换的委托</param>
    /// <returns>转换结果</returns>
    public static OperateResult<TResult> GetResultFromBytes<TResult>(
      OperateResult<byte[]> result,
      Func<byte[], TResult> translator)
    {
      try
      {
        return result.IsSuccess ? OperateResult.CreateSuccessResult<TResult>(translator(result.Content)) : OperateResult.CreateFailedResult<TResult>((OperateResult) result);
      }
      catch (Exception ex)
      {
        OperateResult<TResult> operateResult = new OperateResult<TResult>();
        operateResult.Message = string.Format("{0} {1} : Length({2}) {3}", (object) StringResources.Language.DataTransformError, (object) SoftBasic.ByteToHexString(result.Content), (object) result.Content.Length, (object) ex.Message);
        return operateResult;
      }
    }

    /// <summary>结果转换操作的基础方法，需要支持类型，及转换的委托</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <param name="result">源结果</param>
    /// <returns>转换结果</returns>
    public static OperateResult<TResult> GetResultFromArray<TResult>(
      OperateResult<TResult[]> result)
    {
      return ByteTransformHelper.GetSuccessResultFromOther<TResult, TResult[]>(result, (Func<TResult[], TResult>) (m => m[0]));
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容，所转换的规则</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn">输入类型</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans">转换方法，从类型TIn转换拿到TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetSuccessResultFromOther<TResult, TIn>(
      OperateResult<TIn> result,
      Func<TIn, TResult> trans)
    {
      return !result.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) result) : OperateResult.CreateSuccessResult<TResult>(trans(result.Content));
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TIn">输入类型</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans">转换方法，从类型TIn转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult GetResultFromOther<TIn>(
      OperateResult<TIn> result,
      Func<TIn, OperateResult> trans)
    {
      return !result.IsSuccess ? (OperateResult) result : trans(result.Content);
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn">输入类型</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans">转换方法，从类型TIn转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetResultFromOther<TResult, TIn>(
      OperateResult<TIn> result,
      Func<TIn, OperateResult<TResult>> trans)
    {
      return !result.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) result) : trans(result.Content);
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn1">输入类型1</typeparam>
    /// <typeparam name="TIn2">输入类型2</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
    /// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2>(
      OperateResult<TIn1> result,
      Func<TIn1, OperateResult<TIn2>> trans1,
      Func<TIn2, OperateResult<TResult>> trans2)
    {
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) result);
      OperateResult<TIn2> operateResult = trans1(result.Content);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult) : trans2(operateResult.Content);
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn1">输入类型1</typeparam>
    /// <typeparam name="TIn2">输入类型2</typeparam>
    /// <typeparam name="TIn3">输入类型3</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
    /// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
    /// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3>(
      OperateResult<TIn1> result,
      Func<TIn1, OperateResult<TIn2>> trans1,
      Func<TIn2, OperateResult<TIn3>> trans2,
      Func<TIn3, OperateResult<TResult>> trans3)
    {
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) result);
      OperateResult<TIn2> operateResult1 = trans1(result.Content);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult1);
      OperateResult<TIn3> operateResult2 = trans2(operateResult1.Content);
      return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult2) : trans3(operateResult2.Content);
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn1">输入类型1</typeparam>
    /// <typeparam name="TIn2">输入类型2</typeparam>
    /// <typeparam name="TIn3">输入类型3</typeparam>
    /// <typeparam name="TIn4">输入类型4</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
    /// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
    /// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
    /// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4>(
      OperateResult<TIn1> result,
      Func<TIn1, OperateResult<TIn2>> trans1,
      Func<TIn2, OperateResult<TIn3>> trans2,
      Func<TIn3, OperateResult<TIn4>> trans3,
      Func<TIn4, OperateResult<TResult>> trans4)
    {
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) result);
      OperateResult<TIn2> operateResult1 = trans1(result.Content);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult1);
      OperateResult<TIn3> operateResult2 = trans2(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult2);
      OperateResult<TIn4> operateResult3 = trans3(operateResult2.Content);
      return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult3) : trans4(operateResult3.Content);
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn1">输入类型1</typeparam>
    /// <typeparam name="TIn2">输入类型2</typeparam>
    /// <typeparam name="TIn3">输入类型3</typeparam>
    /// <typeparam name="TIn4">输入类型4</typeparam>
    /// <typeparam name="TIn5">输入类型5</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
    /// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
    /// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
    /// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
    /// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5>(
      OperateResult<TIn1> result,
      Func<TIn1, OperateResult<TIn2>> trans1,
      Func<TIn2, OperateResult<TIn3>> trans2,
      Func<TIn3, OperateResult<TIn4>> trans3,
      Func<TIn4, OperateResult<TIn5>> trans4,
      Func<TIn5, OperateResult<TResult>> trans5)
    {
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) result);
      OperateResult<TIn2> operateResult1 = trans1(result.Content);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult1);
      OperateResult<TIn3> operateResult2 = trans2(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult2);
      OperateResult<TIn4> operateResult3 = trans3(operateResult2.Content);
      if (!operateResult3.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult3);
      OperateResult<TIn5> operateResult4 = trans4(operateResult3.Content);
      return !operateResult4.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult4) : trans5(operateResult4.Content);
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn1">输入类型1</typeparam>
    /// <typeparam name="TIn2">输入类型2</typeparam>
    /// <typeparam name="TIn3">输入类型3</typeparam>
    /// <typeparam name="TIn4">输入类型4</typeparam>
    /// <typeparam name="TIn5">输入类型5</typeparam>
    /// <typeparam name="TIn6">输入类型6</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
    /// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
    /// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
    /// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
    /// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TIn6的泛型委托</param>
    /// <param name="trans6">转换方法6，从类型TIn6转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6>(
      OperateResult<TIn1> result,
      Func<TIn1, OperateResult<TIn2>> trans1,
      Func<TIn2, OperateResult<TIn3>> trans2,
      Func<TIn3, OperateResult<TIn4>> trans3,
      Func<TIn4, OperateResult<TIn5>> trans4,
      Func<TIn5, OperateResult<TIn6>> trans5,
      Func<TIn6, OperateResult<TResult>> trans6)
    {
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) result);
      OperateResult<TIn2> operateResult1 = trans1(result.Content);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult1);
      OperateResult<TIn3> operateResult2 = trans2(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult2);
      OperateResult<TIn4> operateResult3 = trans3(operateResult2.Content);
      if (!operateResult3.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult3);
      OperateResult<TIn5> operateResult4 = trans4(operateResult3.Content);
      if (!operateResult4.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult4);
      OperateResult<TIn6> operateResult5 = trans5(operateResult4.Content);
      return !operateResult5.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult5) : trans6(operateResult5.Content);
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn1">输入类型1</typeparam>
    /// <typeparam name="TIn2">输入类型2</typeparam>
    /// <typeparam name="TIn3">输入类型3</typeparam>
    /// <typeparam name="TIn4">输入类型4</typeparam>
    /// <typeparam name="TIn5">输入类型5</typeparam>
    /// <typeparam name="TIn6">输入类型6</typeparam>
    /// <typeparam name="TIn7">输入类型7</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
    /// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
    /// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
    /// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
    /// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TIn6的泛型委托</param>
    /// <param name="trans6">转换方法6，从类型TIn6转换拿到OperateResult的TIn7的泛型委托</param>
    /// <param name="trans7">转换方法7，从类型TIn7转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7>(
      OperateResult<TIn1> result,
      Func<TIn1, OperateResult<TIn2>> trans1,
      Func<TIn2, OperateResult<TIn3>> trans2,
      Func<TIn3, OperateResult<TIn4>> trans3,
      Func<TIn4, OperateResult<TIn5>> trans4,
      Func<TIn5, OperateResult<TIn6>> trans5,
      Func<TIn6, OperateResult<TIn7>> trans6,
      Func<TIn7, OperateResult<TResult>> trans7)
    {
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) result);
      OperateResult<TIn2> operateResult1 = trans1(result.Content);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult1);
      OperateResult<TIn3> operateResult2 = trans2(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult2);
      OperateResult<TIn4> operateResult3 = trans3(operateResult2.Content);
      if (!operateResult3.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult3);
      OperateResult<TIn5> operateResult4 = trans4(operateResult3.Content);
      if (!operateResult4.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult4);
      OperateResult<TIn6> operateResult5 = trans5(operateResult4.Content);
      if (!operateResult5.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult5);
      OperateResult<TIn7> operateResult6 = trans6(operateResult5.Content);
      return !operateResult6.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult6) : trans7(operateResult6.Content);
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn1">输入类型1</typeparam>
    /// <typeparam name="TIn2">输入类型2</typeparam>
    /// <typeparam name="TIn3">输入类型3</typeparam>
    /// <typeparam name="TIn4">输入类型4</typeparam>
    /// <typeparam name="TIn5">输入类型5</typeparam>
    /// <typeparam name="TIn6">输入类型6</typeparam>
    /// <typeparam name="TIn7">输入类型7</typeparam>
    /// <typeparam name="TIn8">输入类型8</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
    /// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
    /// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
    /// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
    /// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TIn6的泛型委托</param>
    /// <param name="trans6">转换方法6，从类型TIn6转换拿到OperateResult的TIn7的泛型委托</param>
    /// <param name="trans7">转换方法7，从类型TIn7转换拿到OperateResult的TIn8的泛型委托</param>
    /// <param name="trans8">转换方法8，从类型TIn8转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7, TIn8>(
      OperateResult<TIn1> result,
      Func<TIn1, OperateResult<TIn2>> trans1,
      Func<TIn2, OperateResult<TIn3>> trans2,
      Func<TIn3, OperateResult<TIn4>> trans3,
      Func<TIn4, OperateResult<TIn5>> trans4,
      Func<TIn5, OperateResult<TIn6>> trans5,
      Func<TIn6, OperateResult<TIn7>> trans6,
      Func<TIn7, OperateResult<TIn8>> trans7,
      Func<TIn8, OperateResult<TResult>> trans8)
    {
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) result);
      OperateResult<TIn2> operateResult1 = trans1(result.Content);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult1);
      OperateResult<TIn3> operateResult2 = trans2(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult2);
      OperateResult<TIn4> operateResult3 = trans3(operateResult2.Content);
      if (!operateResult3.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult3);
      OperateResult<TIn5> operateResult4 = trans4(operateResult3.Content);
      if (!operateResult4.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult4);
      OperateResult<TIn6> operateResult5 = trans5(operateResult4.Content);
      if (!operateResult5.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult5);
      OperateResult<TIn7> operateResult6 = trans6(operateResult5.Content);
      if (!operateResult6.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult6);
      OperateResult<TIn8> operateResult7 = trans7(operateResult6.Content);
      return !operateResult7.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult7) : trans8(operateResult7.Content);
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn1">输入类型1</typeparam>
    /// <typeparam name="TIn2">输入类型2</typeparam>
    /// <typeparam name="TIn3">输入类型3</typeparam>
    /// <typeparam name="TIn4">输入类型4</typeparam>
    /// <typeparam name="TIn5">输入类型5</typeparam>
    /// <typeparam name="TIn6">输入类型6</typeparam>
    /// <typeparam name="TIn7">输入类型7</typeparam>
    /// <typeparam name="TIn8">输入类型8</typeparam>
    /// <typeparam name="TIn9">输入类型9</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
    /// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
    /// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
    /// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
    /// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TIn6的泛型委托</param>
    /// <param name="trans6">转换方法6，从类型TIn6转换拿到OperateResult的TIn7的泛型委托</param>
    /// <param name="trans7">转换方法7，从类型TIn7转换拿到OperateResult的TIn8的泛型委托</param>
    /// <param name="trans8">转换方法8，从类型TIn8转换拿到OperateResult的TIn9的泛型委托</param>
    /// <param name="trans9">转换方法9，从类型TIn9转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7, TIn8, TIn9>(
      OperateResult<TIn1> result,
      Func<TIn1, OperateResult<TIn2>> trans1,
      Func<TIn2, OperateResult<TIn3>> trans2,
      Func<TIn3, OperateResult<TIn4>> trans3,
      Func<TIn4, OperateResult<TIn5>> trans4,
      Func<TIn5, OperateResult<TIn6>> trans5,
      Func<TIn6, OperateResult<TIn7>> trans6,
      Func<TIn7, OperateResult<TIn8>> trans7,
      Func<TIn8, OperateResult<TIn9>> trans8,
      Func<TIn9, OperateResult<TResult>> trans9)
    {
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) result);
      OperateResult<TIn2> operateResult1 = trans1(result.Content);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult1);
      OperateResult<TIn3> operateResult2 = trans2(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult2);
      OperateResult<TIn4> operateResult3 = trans3(operateResult2.Content);
      if (!operateResult3.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult3);
      OperateResult<TIn5> operateResult4 = trans4(operateResult3.Content);
      if (!operateResult4.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult4);
      OperateResult<TIn6> operateResult5 = trans5(operateResult4.Content);
      if (!operateResult5.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult5);
      OperateResult<TIn7> operateResult6 = trans6(operateResult5.Content);
      if (!operateResult6.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult6);
      OperateResult<TIn8> operateResult7 = trans7(operateResult6.Content);
      if (!operateResult7.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult7);
      OperateResult<TIn9> operateResult8 = trans8(operateResult7.Content);
      return !operateResult8.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult8) : trans9(operateResult8.Content);
    }

    /// <summary>使用指定的转换方法，来获取到实际的结果对象内容</summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="TIn1">输入类型1</typeparam>
    /// <typeparam name="TIn2">输入类型2</typeparam>
    /// <typeparam name="TIn3">输入类型3</typeparam>
    /// <typeparam name="TIn4">输入类型4</typeparam>
    /// <typeparam name="TIn5">输入类型5</typeparam>
    /// <typeparam name="TIn6">输入类型6</typeparam>
    /// <typeparam name="TIn7">输入类型7</typeparam>
    /// <typeparam name="TIn8">输入类型8</typeparam>
    /// <typeparam name="TIn9">输入类型9</typeparam>
    /// <typeparam name="TIn10">输入类型10</typeparam>
    /// <param name="result">原始的结果对象</param>
    /// <param name="trans1">转换方法1，从类型TIn1转换拿到OperateResult的TIn2的泛型委托</param>
    /// <param name="trans2">转换方法2，从类型TIn2转换拿到OperateResult的TIn3的泛型委托</param>
    /// <param name="trans3">转换方法3，从类型TIn3转换拿到OperateResult的TIn4的泛型委托</param>
    /// <param name="trans4">转换方法4，从类型TIn4转换拿到OperateResult的TIn5的泛型委托</param>
    /// <param name="trans5">转换方法5，从类型TIn5转换拿到OperateResult的TIn6的泛型委托</param>
    /// <param name="trans6">转换方法6，从类型TIn6转换拿到OperateResult的TIn7的泛型委托</param>
    /// <param name="trans7">转换方法7，从类型TIn7转换拿到OperateResult的TIn8的泛型委托</param>
    /// <param name="trans8">转换方法8，从类型TIn8转换拿到OperateResult的TIn9的泛型委托</param>
    /// <param name="trans9">转换方法9，从类型TIn9转换拿到OperateResult的TIn10的泛型委托</param>
    /// <param name="trans10">转换方法10，从类型TIn10转换拿到OperateResult的TResult的泛型委托</param>
    /// <returns>类型为TResult的对象</returns>
    public static OperateResult<TResult> GetResultFromOther<TResult, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TIn7, TIn8, TIn9, TIn10>(
      OperateResult<TIn1> result,
      Func<TIn1, OperateResult<TIn2>> trans1,
      Func<TIn2, OperateResult<TIn3>> trans2,
      Func<TIn3, OperateResult<TIn4>> trans3,
      Func<TIn4, OperateResult<TIn5>> trans4,
      Func<TIn5, OperateResult<TIn6>> trans5,
      Func<TIn6, OperateResult<TIn7>> trans6,
      Func<TIn7, OperateResult<TIn8>> trans7,
      Func<TIn8, OperateResult<TIn9>> trans8,
      Func<TIn9, OperateResult<TIn10>> trans9,
      Func<TIn10, OperateResult<TResult>> trans10)
    {
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) result);
      OperateResult<TIn2> operateResult1 = trans1(result.Content);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult1);
      OperateResult<TIn3> operateResult2 = trans2(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult2);
      OperateResult<TIn4> operateResult3 = trans3(operateResult2.Content);
      if (!operateResult3.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult3);
      OperateResult<TIn5> operateResult4 = trans4(operateResult3.Content);
      if (!operateResult4.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult4);
      OperateResult<TIn6> operateResult5 = trans5(operateResult4.Content);
      if (!operateResult5.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult5);
      OperateResult<TIn7> operateResult6 = trans6(operateResult5.Content);
      if (!operateResult6.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult6);
      OperateResult<TIn8> operateResult7 = trans7(operateResult6.Content);
      if (!operateResult7.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult7);
      OperateResult<TIn9> operateResult8 = trans8(operateResult7.Content);
      if (!operateResult8.IsSuccess)
        return OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult8);
      OperateResult<TIn10> operateResult9 = trans9(operateResult8.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<TResult>((OperateResult) operateResult9) : trans10(operateResult9.Content);
    }
  }
}
