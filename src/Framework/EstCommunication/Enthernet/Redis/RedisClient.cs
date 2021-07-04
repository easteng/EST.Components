// Decompiled with JetBrains decompiler
// Type: EstCommunication.Enthernet.Redis.RedisClient
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Enthernet.Redis
{
  /// <summary>
  /// 这是一个redis的客户端类，支持读取，写入，发布订阅，但是不支持订阅，如果需要订阅，请使用另一个类<see cref="T:EstCommunication.Enthernet.Redis.RedisSubscribe" />
  /// </summary>
  /// <remarks>本类库的API指令的参考及注释来源：http://doc.redisfans.com/index.html</remarks>
  /// <example>
  /// 基本的操作如下所示，举例了几个比较常见的指令，更多的需要参考api接口描述
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Enthernet\RedisSample.cs" region="SampleBasic" title="基本操作代码" />
  /// 如下是基于特性的操作，有必要说明以下：
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Enthernet\RedisSample.cs" region="Sample1" title="基础的使用" />
  /// 总的来说，当读取的数据种类比较多的时候，读取的关键字比较多的时候，处理起来就比较的麻烦，此处推荐一个全新的写法，为了更好的对比，我们假设实现一种需求
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Enthernet\RedisSample.cs" region="Sample2" title="同等代码" />
  /// 为此我们只需要实现一个特性类即可。代码如下：(注意，实际是很灵活的，类型都是自动转换的)
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Enthernet\RedisSample.cs" region="SampleClass" title="数据类" />
  /// </example>
  public class RedisClient : NetworkDoubleBase
  {
    private string password = string.Empty;
    private int dbBlock = 0;
    private Lazy<RedisSubscribe> redisSubscribe;

    /// <summary>实例化一个客户端的对象，用于和服务器通信</summary>
    /// <param name="ipAddress">服务器的ip地址</param>
    /// <param name="port">服务器的端口号</param>
    /// <param name="password">密码，如果服务器没有设置，密码设置为null</param>
    public RedisClient(string ipAddress, int port, string password)
    {
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ReceiveTimeOut = 30000;
      this.password = password;
      this.redisSubscribe = new Lazy<RedisSubscribe>((Func<RedisSubscribe>) (() => this.RedisSubscribeInitialize()));
    }

    /// <summary>实例化一个客户端对象，需要手动指定Ip地址和端口</summary>
    /// <param name="password">密码，如果服务器没有设置，密码设置为null</param>
    public RedisClient(string password)
    {
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.ReceiveTimeOut = 30000;
      this.password = password;
      this.redisSubscribe = new Lazy<RedisSubscribe>((Func<RedisSubscribe>) (() => this.RedisSubscribeInitialize()));
    }

    /// <inheritdoc />
    protected override OperateResult InitializationOnConnect(Socket socket)
    {
      if (!string.IsNullOrEmpty(this.password))
      {
        byte[] send = RedisHelper.PackStringCommand(new string[2]
        {
          "AUTH",
          this.password
        });
        OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, send, true, true);
        if (!operateResult.IsSuccess)
          return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
        string msg = Encoding.UTF8.GetString(operateResult.Content);
        if (!msg.StartsWith("+"))
          return (OperateResult) new OperateResult<string>(msg);
      }
      if (this.dbBlock > 0)
      {
        byte[] send = RedisHelper.PackStringCommand(new string[2]
        {
          "SELECT",
          this.dbBlock.ToString()
        });
        OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, send, true, true);
        if (!operateResult.IsSuccess)
          return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
        string msg = Encoding.UTF8.GetString(operateResult.Content);
        if (!msg.StartsWith("+"))
          return (OperateResult) new OperateResult<string>(msg);
      }
      return base.InitializationOnConnect(socket);
    }

    /// <inheritdoc />
    public override OperateResult<byte[]> ReadFromCoreServer(
      Socket socket,
      byte[] send,
      bool hasResponseData = true,
      bool usePackHeader = true)
    {
      OperateResult result = this.Send(socket, send);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(result);
      return this.ReceiveTimeOut < 0 ? OperateResult.CreateSuccessResult<byte[]>(new byte[0]) : this.ReceiveRedisCommand(socket);
    }

    /// <inheritdoc />
    protected override async Task<OperateResult> InitializationOnConnectAsync(
      Socket socket)
    {
      if (!string.IsNullOrEmpty(this.password))
      {
        byte[] command = RedisHelper.PackStringCommand(new string[2]
        {
          "AUTH",
          this.password
        });
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(socket, command, true, true);
        if (!read.IsSuccess)
          return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) read);
        string msg = Encoding.UTF8.GetString(read.Content);
        if (!msg.StartsWith("+"))
          return (OperateResult) new OperateResult<string>(msg);
        command = (byte[]) null;
        read = (OperateResult<byte[]>) null;
        msg = (string) null;
      }
      if (this.dbBlock > 0)
      {
        byte[] command = RedisHelper.PackStringCommand(new string[2]
        {
          "SELECT",
          this.dbBlock.ToString()
        });
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(socket, command, true, true);
        if (!read.IsSuccess)
          return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) read);
        string msg = Encoding.UTF8.GetString(read.Content);
        if (!msg.StartsWith("+"))
          return (OperateResult) new OperateResult<string>(msg);
        command = (byte[]) null;
        read = (OperateResult<byte[]>) null;
        msg = (string) null;
      }
      return base.InitializationOnConnect(socket);
    }

    /// <inheritdoc />
    public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
      Socket socket,
      byte[] send,
      bool hasResponseData = true,
      bool usePackHeader = true)
    {
      OperateResult sendResult = await this.SendAsync(socket, send);
      if (!sendResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(sendResult);
      if (this.ReceiveTimeOut < 0)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      OperateResult<byte[]> redisCommandAsync = await this.ReceiveRedisCommandAsync(socket);
      return redisCommandAsync;
    }

    /// <summary>
    /// 自定义的指令交互方法，该指令用空格分割，举例：LTRIM AAAAA 0 999 就是收缩列表，GET AAA 就是获取键值，需要对返回的数据进行二次分析
    /// </summary>
    /// <param name="command">举例：LTRIM AAAAA 0 999 就是收缩列表，GET AAA 就是获取键值</param>
    /// <returns>从服务器返回的结果数据对象</returns>
    public OperateResult<string> ReadCustomer(string command)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(RedisHelper.PackStringCommand(command.Split(' ')));
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<string>(Encoding.UTF8.GetString(operateResult.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadCustomer(System.String)" />
    public async Task<OperateResult<string>> ReadCustomerAsync(string command)
    {
      byte[] byteCommand = RedisHelper.PackStringCommand(command.Split(' '));
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(byteCommand);
      OperateResult<string> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<string>(Encoding.UTF8.GetString(read.Content)) : OperateResult.CreateFailedResult<string>((OperateResult) read);
      byteCommand = (byte[]) null;
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <summary>向服务器请求指定，并返回数字的结果对象</summary>
    /// <param name="commands">命令数组</param>
    /// <returns>数字的结果对象</returns>
    public OperateResult<int> OperateNumberFromServer(string[] commands)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(RedisHelper.PackStringCommand(commands));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<int>((OperateResult) operateResult);
      string msg = Encoding.UTF8.GetString(operateResult.Content);
      return !msg.StartsWith(":") ? new OperateResult<int>(msg) : RedisHelper.GetNumberFromCommandLine(operateResult.Content);
    }

    /// <summary>向服务器请求指令，并返回long数字的结果对象</summary>
    /// <param name="commands">命令数组</param>
    /// <returns>long数字的结果对象</returns>
    public OperateResult<long> OperateLongNumberFromServer(string[] commands)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(RedisHelper.PackStringCommand(commands));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<long>((OperateResult) operateResult);
      string msg = Encoding.UTF8.GetString(operateResult.Content);
      return !msg.StartsWith(":") ? new OperateResult<long>(msg) : RedisHelper.GetLongNumberFromCommandLine(operateResult.Content);
    }

    /// <summary>向服务器请求指令，并返回字符串的结果对象</summary>
    /// <param name="commands">命令数组</param>
    /// <returns>字符串的结果对象</returns>
    public OperateResult<string> OperateStringFromServer(string[] commands)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(RedisHelper.PackStringCommand(commands));
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult) operateResult) : RedisHelper.GetStringFromCommandLine(operateResult.Content);
    }

    /// <summary>向服务器请求指令，并返回字符串数组的结果对象</summary>
    /// <param name="commands">命令数组</param>
    /// <returns>字符串数组的结果对象</returns>
    public OperateResult<string[]> OperateStringsFromServer(string[] commands)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(RedisHelper.PackStringCommand(commands));
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<string[]>((OperateResult) operateResult) : RedisHelper.GetStringsFromCommandLine(operateResult.Content);
    }

    /// <summary>向服务器请求指令，并返回状态的结果对象，通常用于写入的判断，或是请求类型的判断</summary>
    /// <param name="commands">命令数组</param>
    /// <returns>是否成功的结果对象</returns>
    public OperateResult<string> OperateStatusFromServer(string[] commands)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(RedisHelper.PackStringCommand(commands));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
      string msg = Encoding.UTF8.GetString(operateResult.Content);
      if (!msg.StartsWith("+"))
        return new OperateResult<string>(msg);
      return OperateResult.CreateSuccessResult<string>(msg.Substring(1).TrimEnd('\r', '\n'));
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.OperateNumberFromServer(System.String[])" />
    public async Task<OperateResult<int>> OperateNumberFromServerAsync(
      string[] commands)
    {
      byte[] command = RedisHelper.PackStringCommand(commands);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<int>((OperateResult) read);
      string msg = Encoding.UTF8.GetString(read.Content);
      return msg.StartsWith(":") ? RedisHelper.GetNumberFromCommandLine(read.Content) : new OperateResult<int>(msg);
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.OperateLongNumberFromServer(System.String[])" />
    public async Task<OperateResult<long>> OperateLongNumberFromServerAsync(
      string[] commands)
    {
      byte[] command = RedisHelper.PackStringCommand(commands);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<long>((OperateResult) read);
      string msg = Encoding.UTF8.GetString(read.Content);
      return msg.StartsWith(":") ? RedisHelper.GetLongNumberFromCommandLine(read.Content) : new OperateResult<long>(msg);
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.OperateStringFromServer(System.String[])" />
    public async Task<OperateResult<string>> OperateStringFromServerAsync(
      string[] commands)
    {
      byte[] command = RedisHelper.PackStringCommand(commands);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
      OperateResult<string> operateResult = read.IsSuccess ? RedisHelper.GetStringFromCommandLine(read.Content) : OperateResult.CreateFailedResult<string>((OperateResult) read);
      command = (byte[]) null;
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.OperateStringsFromServer(System.String[])" />
    public async Task<OperateResult<string[]>> OperateStringsFromServerAsync(
      string[] commands)
    {
      byte[] command = RedisHelper.PackStringCommand(commands);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
      OperateResult<string[]> operateResult = read.IsSuccess ? RedisHelper.GetStringsFromCommandLine(read.Content) : OperateResult.CreateFailedResult<string[]>((OperateResult) read);
      command = (byte[]) null;
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.OperateStatusFromServer(System.String[])" />
    public async Task<OperateResult<string>> OperateStatusFromServerAsync(
      string[] commands)
    {
      byte[] command = RedisHelper.PackStringCommand(commands);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) read);
      string msg = Encoding.UTF8.GetString(read.Content);
      if (!msg.StartsWith("+"))
        return new OperateResult<string>(msg);
      return OperateResult.CreateSuccessResult<string>(msg.Substring(1).TrimEnd('\r', '\n'));
    }

    /// <summary>删除给定的一个或多个 key 。不存在的 key 会被忽略。</summary>
    /// <param name="keys">关键字</param>
    /// <returns>被删除 key 的数量。</returns>
    public OperateResult<int> DeleteKey(string[] keys) => this.OperateNumberFromServer(SoftBasic.SpliceStringArray("DEL", keys));

    /// <summary>删除给定的一个或多个 key 。不存在的 key 会被忽略。</summary>
    /// <param name="key">关键字</param>
    /// <returns>被删除 key 的数量。</returns>
    public OperateResult<int> DeleteKey(string key) => this.DeleteKey(new string[1]
    {
      key
    });

    /// <summary>检查给定 key 是否存在。若 key 存在，返回 1 ，否则返回 0 。</summary>
    /// <param name="key">关键字</param>
    /// <returns>若 key 存在，返回 1 ，否则返回 0 。</returns>
    public OperateResult<int> ExistsKey(string key) => this.OperateNumberFromServer(new string[2]
    {
      "EXISTS",
      key
    });

    /// <summary>
    /// 为给定 key 设置生存时间，当 key 过期时(生存时间为 0 )，它会被自动删除。设置成功返回 1 。当 key 不存在或者不能为 key 设置生存时间时，返回 0 。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="seconds">当前key的生存时间，单位为秒</param>
    /// <returns>设置成功返回 1 。当 key 不存在或者不能为 key 设置生存时间时，返回 0 。</returns>
    /// <remarks>
    /// 在 Redis 中，带有生存时间的 key 被称为『易失的』(volatile)。<br />
    /// 生存时间可以通过使用 DEL 命令来删除整个 key 来移除，或者被 SET 和 GETSET 命令覆写( overwrite)，这意味着，如果一个命令只是修改( alter)一个带生存时间的 key 的值而不是用一个新的 key 值来代替( replace)它的话，那么生存时间不会被改变。<br />
    /// 比如说，对一个 key 执行 INCR 命令，对一个列表进行 LPUSH 命令，或者对一个哈希表执行 HSET 命令，这类操作都不会修改 key 本身的生存时间。<br />
    /// 另一方面，如果使用 RENAME 对一个 key 进行改名，那么改名后的 key 的生存时间和改名前一样。<br />
    /// RENAME 命令的另一种可能是，尝试将一个带生存时间的 key 改名成另一个带生存时间的 another_key ，这时旧的 another_key( 以及它的生存时间)会被删除，然后旧的 key 会改名为 another_key ，因此，新的 another_key 的生存时间也和原本的 key 一样。<br />
    /// 使用 PERSIST 命令可以在不删除 key 的情况下，移除 key 的生存时间，让 key 重新成为一个『持久的』(persistent) key 。<br />
    /// 更新生存时间<br />
    /// 可以对一个已经带有生存时间的 key 执行 EXPIRE 命令，新指定的生存时间会取代旧的生存时间。<br />
    /// 过期时间的精确度<br />
    /// 在 Redis 2.4 版本中，过期时间的延迟在 1 秒钟之内 —— 也即是，就算 key 已经过期，但它还是可能在过期之后一秒钟之内被访问到，而在新的 Redis 2.6 版本中，延迟被降低到 1 毫秒之内。<br />
    /// Redis 2.1.3 之前的不同之处<br />
    /// 在 Redis 2.1.3 之前的版本中，修改一个带有生存时间的 key 会导致整个 key 被删除，这一行为是受当时复制( replication)层的限制而作出的，现在这一限制已经被修复。<br />
    /// </remarks>
    public OperateResult<int> ExpireKey(string key, int seconds) => this.OperateNumberFromServer(new string[3]
    {
      "EXPIRE",
      key,
      seconds.ToString()
    });

    /// <summary>
    /// 查找所有符合给定模式 pattern 的 key 。
    /// * 匹配数据库中所有 key。
    /// h?llo 匹配 hello ， hallo 和 hxllo 等。
    /// h[ae]llo 匹配 hello 和 hallo ，但不匹配 hillo 。
    /// </summary>
    /// <param name="pattern">给定模式</param>
    /// <returns>符合给定模式的 key 列表。</returns>
    public OperateResult<string[]> ReadAllKeys(string pattern) => this.OperateStringsFromServer(new string[2]
    {
      "KEYS",
      pattern
    });

    /// <summary>
    /// 将当前数据库的 key 移动到给定的数据库 db 当中。
    /// 如果当前数据库(源数据库)和给定数据库(目标数据库)有相同名字的给定 key ，或者 key 不存在于当前数据库，那么 MOVE 没有任何效果。
    /// 因此，也可以利用这一特性，将 MOVE 当作锁(locking)原语(primitive)。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="db">数据块</param>
    /// <returns>是否移动成功</returns>
    public OperateResult MoveKey(string key, int db) => (OperateResult) this.OperateStatusFromServer(new string[3]
    {
      "MOVE",
      key,
      db.ToString()
    });

    /// <summary>
    /// 移除给定 key 的生存时间，将这个 key 从『易失的』(带生存时间 key )转换成『持久的』(一个不带生存时间、永不过期的 key )。
    /// 当生存时间移除成功时，返回 1 .
    /// 如果 key 不存在或 key 没有设置生存时间，返回 0 。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <returns>
    /// 当生存时间移除成功时，返回 1 .
    /// 如果 key 不存在或 key 没有设置生存时间，返回 0 。
    /// </returns>
    public OperateResult<int> PersistKey(string key) => this.OperateNumberFromServer(new string[2]
    {
      "PERSIST",
      key
    });

    /// <summary>
    /// 从当前数据库中随机返回(不删除)一个 key 。
    /// 当数据库不为空时，返回一个 key 。
    /// 当数据库为空时，返回 nil 。
    /// </summary>
    /// <returns>
    /// 当数据库不为空时，返回一个 key 。
    /// 当数据库为空时，返回 nil 。
    /// </returns>
    public OperateResult<string> ReadRandomKey() => this.OperateStringFromServer(new string[1]
    {
      "RANDOMKEY"
    });

    /// <summary>
    /// 将 key 改名为 newkey 。
    /// 当 key 和 newkey 相同，或者 key 不存在时，返回一个错误。
    /// 当 newkey 已经存在时， RENAME 命令将覆盖旧值。
    /// </summary>
    /// <param name="key1">旧的key</param>
    /// <param name="key2">新的key</param>
    /// <returns>改名成功时提示 OK ，失败时候返回一个错误。</returns>
    public OperateResult RenameKey(string key1, string key2) => (OperateResult) this.OperateStatusFromServer(new string[3]
    {
      "RENAME",
      key1,
      key2
    });

    /// <summary>
    /// 返回 key 所储存的值的类型。none (key不存在)，string (字符串)，list (列表)，set (集合)，zset (有序集)，hash (哈希表)
    /// </summary>
    /// <param name="key">关键字</param>
    /// <returns>类型</returns>
    public OperateResult<string> ReadKeyType(string key) => this.OperateStatusFromServer(new string[2]
    {
      "TYPE",
      key
    });

    /// <summary>以秒为单位，返回给定 key 的剩余生存时间(TTL, time to live)。</summary>
    /// <param name="key">关键字</param>
    /// <returns>当 key 不存在时，返回 -2 。当 key 存在但没有设置剩余生存时间时，返回 -1 。否则，以秒为单位，返回 key 的剩余生存时间。</returns>
    public OperateResult<int> ReadKeyTTL(string key) => this.OperateNumberFromServer(new string[2]
    {
      "TTL",
      key
    });

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.DeleteKey(System.String[])" />
    public async Task<OperateResult<int>> DeleteKeyAsync(string[] keys)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(SoftBasic.SpliceStringArray("DEL", keys));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.DeleteKey(System.String)" />
    public async Task<OperateResult<int>> DeleteKeyAsync(string key)
    {
      OperateResult<int> operateResult = await this.DeleteKeyAsync(new string[1]
      {
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ExistsKey(System.String)" />
    public async Task<OperateResult<int>> ExistsKeyAsync(string key)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[2]
      {
        "EXISTS",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ExpireKey(System.String,System.Int32)" />
    public async Task<OperateResult<int>> ExpireKeyAsync(string key, int seconds)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[3]
      {
        "EXPIRE",
        key,
        seconds.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadAllKeys(System.String)" />
    public async Task<OperateResult<string[]>> ReadAllKeysAsync(string pattern)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[2]
      {
        "KEYS",
        pattern
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.MoveKey(System.String,System.Int32)" />
    public async Task<OperateResult> MoveKeyAsync(string key, int db)
    {
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(new string[3]
      {
        "MOVE",
        key,
        db.ToString()
      });
      return (OperateResult) operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.PersistKey(System.String)" />
    public async Task<OperateResult<int>> PersistKeyAsync(string key)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[2]
      {
        "PERSIST",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadRandomKey" />
    public async Task<OperateResult<string>> ReadRandomKeyAsync()
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[1]
      {
        "RANDOMKEY"
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.RenameKey(System.String,System.String)" />
    public async Task<OperateResult> RenameKeyAsync(string key1, string key2)
    {
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(new string[3]
      {
        "RENAME",
        key1,
        key2
      });
      return (OperateResult) operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadKeyType(System.String)" />
    public async Task<OperateResult<string>> ReadKeyTypeAsync(string key)
    {
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(new string[2]
      {
        "TYPE",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadKeyTTL(System.String)" />
    public async Task<OperateResult<int>> ReadKeyTTLAsync(string key)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[2]
      {
        "TTL",
        key
      });
      return operateResult;
    }

    /// <summary>
    /// 如果 key 已经存在并且是一个字符串， APPEND 命令将 value 追加到 key 原来的值的末尾。
    /// 如果 key 不存在， APPEND 就简单地将给定 key 设为 value ，就像执行 SET key value 一样。
    /// 返回追加 value 之后， key 中字符串的长度。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">数值</param>
    /// <returns>追加 value 之后， key 中字符串的长度。</returns>
    public OperateResult<int> AppendKey(string key, string value) => this.OperateNumberFromServer(new string[3]
    {
      "APPEND",
      key,
      value
    });

    /// <summary>
    /// 将 key 中储存的数字值减一。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 DECR 操作。
    /// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
    /// 本操作的值限制在 64 位(bit)有符号数字表示之内。
    /// 返回执行 DECR 命令之后 key 的值。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <returns>执行 DECR 命令之后 key 的值。</returns>
    public OperateResult<long> DecrementKey(string key) => this.OperateLongNumberFromServer(new string[2]
    {
      "DECR",
      key
    });

    /// <summary>
    /// 将 key 所储存的值减去减量 decrement 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 DECR 操作。
    /// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
    /// 本操作的值限制在 64 位(bit)有符号数字表示之内。
    /// 返回减去 decrement 之后， key 的值。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">操作的值</param>
    /// <returns>返回减去 decrement 之后， key 的值。</returns>
    public OperateResult<long> DecrementKey(string key, long value) => this.OperateLongNumberFromServer(new string[3]
    {
      "DECRBY",
      key,
      value.ToString()
    });

    /// <summary>
    /// 返回 key 所关联的字符串值。如果 key 不存在那么返回特殊值 nil 。
    /// 假如 key 储存的值不是字符串类型，返回一个错误，因为 GET 只能用于处理字符串值。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <returns>当 key 不存在时，返回 nil ，否则，返回 key 的值。</returns>
    public OperateResult<string> ReadKey(string key) => this.OperateStringFromServer(new string[2]
    {
      "GET",
      key
    });

    /// <summary>
    /// 返回 key 中字符串值的子字符串，字符串的截取范围由 start 和 end 两个偏移量决定(包括 start 和 end 在内)。
    /// 负数偏移量表示从字符串最后开始计数， -1 表示最后一个字符， -2 表示倒数第二个，以此类推。
    /// 返回截取得出的子字符串。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="start">截取开始的位置</param>
    /// <param name="end">截取结束的位置</param>
    /// <returns>返回截取得出的子字符串。</returns>
    public OperateResult<string> ReadKeyRange(string key, int start, int end) => this.OperateStringFromServer(new string[4]
    {
      "GETRANGE",
      key,
      start.ToString(),
      end.ToString()
    });

    /// <summary>
    /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)。当 key 存在但不是字符串类型时，返回一个错误。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">新的值</param>
    /// <returns>返回给定 key 的旧值。当 key 没有旧值时，也即是， key 不存在时，返回 nil 。</returns>
    public OperateResult<string> ReadAndWriteKey(string key, string value) => this.OperateStringFromServer(new string[3]
    {
      "GETSET",
      key,
      value
    });

    /// <summary>
    /// 将 key 中储存的数字值增一。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCR 操作。
    /// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
    /// 返回执行 INCR 命令之后 key 的值。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <returns>返回执行 INCR 命令之后 key 的值。</returns>
    public OperateResult<long> IncrementKey(string key) => this.OperateLongNumberFromServer(new string[2]
    {
      "INCR",
      key
    });

    /// <summary>
    /// 将 key 所储存的值加上增量 increment 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCR 操作。
    /// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">增量数据</param>
    /// <returns>加上 increment 之后， key 的值。</returns>
    public OperateResult<long> IncrementKey(string key, long value) => this.OperateLongNumberFromServer(new string[3]
    {
      "INCRBY",
      key,
      value.ToString()
    });

    /// <summary>
    /// 将 key 所储存的值加上增量 increment 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCRBYFLOAT 操作。
    /// 如果命令执行成功，那么 key 的值会被更新为（执行加法之后的）新值，并且新值会以字符串的形式返回给调用者
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">增量数据</param>
    /// <returns>执行命令之后 key 的值。</returns>
    public OperateResult<string> IncrementKey(string key, float value) => this.OperateStringFromServer(new string[3]
    {
      "INCRBYFLOAT",
      key,
      value.ToString()
    });

    /// <summary>
    /// 返回所有(一个或多个)给定 key 的值。
    /// 如果给定的 key 里面，有某个 key 不存在，那么这个 key 返回特殊值 null 。因此，该命令永不失败。
    /// </summary>
    /// <param name="keys">关键字数组</param>
    /// <returns>一个包含所有给定 key 的值的列表。</returns>
    public OperateResult<string[]> ReadKey(string[] keys) => this.OperateStringsFromServer(SoftBasic.SpliceStringArray("MGET", keys));

    /// <summary>
    /// 同时设置一个或多个 key-value 对。
    /// 如果某个给定 key 已经存在，那么 MSET 会用新值覆盖原来的旧值，如果这不是你所希望的效果，请考虑使用 MSETNX 命令：它只会在所有给定 key 都不存在的情况下进行设置操作。
    /// </summary>
    /// <param name="keys">关键字数组</param>
    /// <param name="values">值数组</param>
    /// <returns>总是返回 OK (因为 MSET 不可能失败)</returns>
    public OperateResult WriteKey(string[] keys, string[] values)
    {
      if (keys == null)
        throw new ArgumentNullException(nameof (keys));
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (keys.Length != values.Length)
        throw new ArgumentException("Two arguement not same length");
      List<string> stringList = new List<string>();
      stringList.Add("MSET");
      for (int index = 0; index < keys.Length; ++index)
      {
        stringList.Add(keys[index]);
        stringList.Add(values[index]);
      }
      return (OperateResult) this.OperateStatusFromServer(stringList.ToArray());
    }

    /// <summary>
    /// 将字符串值 value 关联到 key 。
    /// 如果 key 已经持有其他值， SET 就覆写旧值，无视类型。
    /// 对于某个原本带有生存时间（TTL）的键来说， 当 SET 命令成功在这个键上执行时， 这个键原有的 TTL 将被清除。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">数据值</param>
    /// <returns> SET 在设置操作成功完成时，才返回 OK 。</returns>
    public OperateResult WriteKey(string key, string value) => (OperateResult) this.OperateStatusFromServer(new string[3]
    {
      "SET",
      key,
      value
    });

    /// <summary>将字符串值 value 关联到 key 。并发布一个订阅的频道数据，都成功时，才返回成功</summary>
    /// <param name="key">关键字</param>
    /// <param name="value">数据值</param>
    /// <returns>是否成功的结果对象</returns>
    public OperateResult WriteAndPublishKey(string key, string value)
    {
      OperateResult operateResult = this.WriteKey(key, value);
      return !operateResult.IsSuccess ? operateResult : (OperateResult) this.Publish(key, value);
    }

    /// <summary>
    /// 将值 value 关联到 key ，并将 key 的生存时间设为 seconds (以秒为单位)。如果 key 已经存在， SETEX 命令将覆写旧值。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">数值</param>
    /// <param name="seconds">生存时间，单位秒</param>
    /// <returns>设置成功时返回 OK 。当 seconds 参数不合法时，返回一个错误。</returns>
    public OperateResult WriteExpireKey(string key, string value, long seconds) => (OperateResult) this.OperateStatusFromServer(new string[4]
    {
      "SETEX",
      key,
      seconds.ToString(),
      value
    });

    /// <summary>
    /// 将 key 的值设为 value ，当且仅当 key 不存在。若给定的 key 已经存在，则 SETNX 不做任何动作。设置成功，返回 1 。设置失败，返回 0 。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">数据值</param>
    /// <returns>设置成功，返回 1 。设置失败，返回 0 。</returns>
    public OperateResult<int> WriteKeyIfNotExists(string key, string value) => this.OperateNumberFromServer(new string[3]
    {
      "SETNX",
      key,
      value
    });

    /// <summary>
    /// 用 value 参数覆写(overwrite)给定 key 所储存的字符串值，从偏移量 offset 开始。不存在的 key 当作空白字符串处理。返回被 SETRANGE 修改之后，字符串的长度。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">数值</param>
    /// <param name="offset">起始的偏移量</param>
    /// <returns>被 SETRANGE 修改之后，字符串的长度。</returns>
    public OperateResult<int> WriteKeyRange(string key, string value, int offset) => this.OperateNumberFromServer(new string[4]
    {
      "SETRANGE",
      key,
      offset.ToString(),
      value
    });

    /// <summary>
    /// 返回 key 所储存的字符串值的长度。当 key 储存的不是字符串值时，返回一个错误。返回符串值的长度。当 key 不存在时，返回 0 。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <returns>字符串值的长度。当 key 不存在时，返回 0 。</returns>
    public OperateResult<int> ReadKeyLength(string key) => this.OperateNumberFromServer(new string[2]
    {
      "STRLEN",
      key
    });

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.AppendKey(System.String,System.String)" />
    public async Task<OperateResult<int>> AppendKeyAsync(string key, string value)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[3]
      {
        "APPEND",
        key,
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.DecrementKey(System.String)" />
    public async Task<OperateResult<long>> DecrementKeyAsync(string key)
    {
      OperateResult<long> operateResult = await this.OperateLongNumberFromServerAsync(new string[2]
      {
        "DECR",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.DecrementKey(System.String,System.Int64)" />
    public async Task<OperateResult<long>> DecrementKeyAsync(
      string key,
      long value)
    {
      OperateResult<long> operateResult = await this.OperateLongNumberFromServerAsync(new string[3]
      {
        "DECRBY",
        key,
        value.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadKey(System.String)" />
    public async Task<OperateResult<string>> ReadKeyAsync(string key)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[2]
      {
        "GET",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadKeyRange(System.String,System.Int32,System.Int32)" />
    public async Task<OperateResult<string>> ReadKeyRangeAsync(
      string key,
      int start,
      int end)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[4]
      {
        "GETRANGE",
        key,
        start.ToString(),
        end.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadAndWriteKey(System.String,System.String)" />
    public async Task<OperateResult<string>> ReadAndWriteKeyAsync(
      string key,
      string value)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[3]
      {
        "GETSET",
        key,
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.IncrementKey(System.String)" />
    public async Task<OperateResult<long>> IncrementKeyAsync(string key)
    {
      OperateResult<long> operateResult = await this.OperateLongNumberFromServerAsync(new string[2]
      {
        "INCR",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.IncrementKey(System.String,System.Int64)" />
    public async Task<OperateResult<long>> IncrementKeyAsync(
      string key,
      long value)
    {
      OperateResult<long> operateResult = await this.OperateLongNumberFromServerAsync(new string[3]
      {
        "INCRBY",
        key,
        value.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.IncrementKey(System.String,System.Single)" />
    public async Task<OperateResult<string>> IncrementKeyAsync(
      string key,
      float value)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[3]
      {
        "INCRBYFLOAT",
        key,
        value.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadKey(System.String[])" />
    public async Task<OperateResult<string[]>> ReadKeyAsync(string[] keys)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(SoftBasic.SpliceStringArray("MGET", keys));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.WriteKey(System.String[],System.String[])" />
    public async Task<OperateResult> WriteKeyAsync(string[] keys, string[] values)
    {
      if (keys == null)
        throw new ArgumentNullException(nameof (keys));
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (keys.Length != values.Length)
        throw new ArgumentException("Two arguement not same length");
      List<string> list = new List<string>();
      list.Add("MSET");
      for (int i = 0; i < keys.Length; ++i)
      {
        list.Add(keys[i]);
        list.Add(values[i]);
      }
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(list.ToArray());
      OperateResult operateResult1 = (OperateResult) operateResult;
      list = (List<string>) null;
      return operateResult1;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.WriteKey(System.String,System.String)" />
    public async Task<OperateResult> WriteKeyAsync(string key, string value)
    {
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(new string[3]
      {
        "SET",
        key,
        value
      });
      return (OperateResult) operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.WriteAndPublishKey(System.String,System.String)" />
    public async Task<OperateResult> WriteAndPublishKeyAsync(
      string key,
      string value)
    {
      OperateResult write = await this.WriteKeyAsync(key, value);
      if (!write.IsSuccess)
        return write;
      OperateResult<int> operateResult = await this.PublishAsync(key, value);
      return (OperateResult) operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.WriteExpireKey(System.String,System.String,System.Int64)" />
    public async Task<OperateResult> WriteExpireKeyAsync(
      string key,
      string value,
      long seconds)
    {
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(new string[4]
      {
        "SETEX",
        key,
        seconds.ToString(),
        value
      });
      return (OperateResult) operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.WriteKeyIfNotExists(System.String,System.String)" />
    public async Task<OperateResult<int>> WriteKeyIfNotExistsAsync(
      string key,
      string value)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[3]
      {
        "SETNX",
        key,
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.WriteKeyRange(System.String,System.String,System.Int32)" />
    public async Task<OperateResult<int>> WriteKeyRangeAsync(
      string key,
      string value,
      int offset)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[4]
      {
        "SETRANGE",
        key,
        offset.ToString(),
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadKeyLength(System.String)" />
    public async Task<OperateResult<int>> ReadKeyLengthAsync(string key)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[2]
      {
        "STRLEN",
        key
      });
      return operateResult;
    }

    /// <summary>
    /// 将值 value 插入到列表 key 当中，位于值 pivot 之前。
    /// 当 pivot 不存在于列表 key 时，不执行任何操作。
    /// 当 key 不存在时， key 被视为空列表，不执行任何操作。
    /// 如果 key 不是列表类型，返回一个错误。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">数值</param>
    /// <param name="pivot">原先的值</param>
    /// <returns>
    /// 如果命令执行成功，返回插入操作完成之后，列表的长度。
    /// 如果没有找到 pivot ，返回 -1 。
    /// 如果 key 不存在或为空列表，返回 0 。
    /// </returns>
    public OperateResult<int> ListInsertBefore(string key, string value, string pivot) => this.OperateNumberFromServer(new string[5]
    {
      "LINSERT",
      key,
      "BEFORE",
      pivot,
      value
    });

    /// <summary>
    /// 将值 value 插入到列表 key 当中，位于值 pivot 之后。
    /// 当 pivot 不存在于列表 key 时，不执行任何操作。
    /// 当 key 不存在时， key 被视为空列表，不执行任何操作。
    /// 如果 key 不是列表类型，返回一个错误。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">数值</param>
    /// <param name="pivot">原先的值</param>
    /// <returns>
    /// 如果命令执行成功，返回插入操作完成之后，列表的长度。
    /// 如果没有找到 pivot ，返回 -1 。
    /// 如果 key 不存在或为空列表，返回 0 。
    /// </returns>
    public OperateResult<int> ListInsertAfter(string key, string value, string pivot) => this.OperateNumberFromServer(new string[5]
    {
      "LINSERT",
      key,
      "AFTER",
      pivot,
      value
    });

    /// <summary>
    /// 返回列表 key 的长度。如果 key 不存在，则 key 被解释为一个空列表，返回 0 .如果 key 不是列表类型，返回一个错误。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <returns>列表 key 的长度。</returns>
    public OperateResult<int> GetListLength(string key) => this.OperateNumberFromServer(new string[2]
    {
      "LLEN",
      key
    });

    /// <summary>
    /// 返回列表 key 中，下标为 index 的元素。下标(index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
    /// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。如果 key 不是列表类型，返回一个错误。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="index">索引位置</param>
    /// <returns>列表中下标为 index 的元素。如果 index 参数的值不在列表的区间范围内(out of range)，返回 nil 。</returns>
    public OperateResult<string> ReadListByIndex(string key, long index) => this.OperateStringFromServer(new string[3]
    {
      "LINDEX",
      key,
      index.ToString()
    });

    /// <summary>移除并返回列表 key 的头元素。列表的头元素。当 key 不存在时，返回 nil 。</summary>
    /// <param name="key">关键字信息</param>
    /// <returns>列表的头元素。</returns>
    public OperateResult<string> ListLeftPop(string key) => this.OperateStringFromServer(new string[2]
    {
      "LPOP",
      key
    });

    /// <summary>
    /// 将一个或多个值 value 插入到列表 key 的表头，如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。返回执行 LPUSH 命令后，列表的长度。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">值</param>
    /// <returns>执行 LPUSH 命令后，列表的长度。</returns>
    public OperateResult<int> ListLeftPush(string key, string value) => this.ListLeftPush(key, new string[1]
    {
      value
    });

    /// <summary>
    /// 将一个或多个值 value 插入到列表 key 的表头，如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。返回执行 LPUSH 命令后，列表的长度。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="values">值</param>
    /// <returns>执行 LPUSH 命令后，列表的长度。</returns>
    public OperateResult<int> ListLeftPush(string key, string[] values) => this.OperateNumberFromServer(SoftBasic.SpliceStringArray("LPUSH", key, values));

    /// <summary>
    /// 将值 value 插入到列表 key 的表头，当且仅当 key 存在并且是一个列表。和 LPUSH 命令相反，当 key 不存在时， LPUSHX 命令什么也不做。
    /// 返回LPUSHX 命令执行之后，表的长度。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">值</param>
    /// <returns>是否插入数据成功</returns>
    public OperateResult<int> ListLeftPushX(string key, string value) => this.OperateNumberFromServer(new string[3]
    {
      "LPUSHX",
      key,
      value
    });

    /// <summary>
    /// 返回列表 key 中指定区间内的元素，区间以偏移量 start 和 stop 指定。
    /// 下标(index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
    /// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
    /// 返回一个列表，包含指定区间内的元素。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="start">开始的索引</param>
    /// <param name="stop">结束的索引</param>
    /// <returns>返回一个列表，包含指定区间内的元素。</returns>
    public OperateResult<string[]> ListRange(string key, long start, long stop) => this.OperateStringsFromServer(new string[4]
    {
      "LRANGE",
      key,
      start.ToString(),
      stop.ToString()
    });

    /// <summary>
    /// 根据参数 count 的值，移除列表中与参数 value 相等的元素。count 的值可以是以下几种：
    /// count &gt; 0 : 从表头开始向表尾搜索，移除与 value 相等的元素，数量为 count 。
    /// count &lt; 0 : 从表尾开始向表头搜索，移除与 value 相等的元素，数量为 count 的绝对值。
    /// count = 0 : 移除表中所有与 value 相等的值。
    /// 返回被移除的数量。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="count">移除参数</param>
    /// <param name="value">匹配的值</param>
    /// <returns>被移除元素的数量。因为不存在的 key 被视作空表(empty list)，所以当 key 不存在时， LREM 命令总是返回 0 。</returns>
    public OperateResult<int> ListRemoveElementMatch(
      string key,
      long count,
      string value)
    {
      return this.OperateNumberFromServer(new string[4]
      {
        "LREM",
        key,
        count.ToString(),
        value
      });
    }

    /// <summary>
    /// 设置数组的某一个索引的数据信息，当 index 参数超出范围，或对一个空列表( key 不存在)进行 LSET 时，返回一个错误。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="index">索引位置</param>
    /// <param name="value">值</param>
    /// <returns>操作成功返回 ok ，否则返回错误信息。</returns>
    public OperateResult ListSet(string key, long index, string value) => (OperateResult) this.OperateStatusFromServer(new string[4]
    {
      "LSET",
      key.ToString(),
      index.ToString(),
      value
    });

    /// <summary>
    /// 对一个列表进行修剪(trim)，就是说，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除。
    /// 举个例子，执行命令 LTRIM list 0 2 ，表示只保留列表 list 的前三个元素，其余元素全部删除。
    /// 下标( index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
    /// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
    /// 当 key 不是列表类型时，返回一个错误。
    /// </summary>
    /// <param name="key">关键字信息</param>
    /// <param name="start">起始的索引信息</param>
    /// <param name="end">结束的索引信息</param>
    /// <returns>操作成功返回 ok ，否则返回错误信息。</returns>
    public OperateResult ListTrim(string key, long start, long end) => (OperateResult) this.OperateStatusFromServer(new string[4]
    {
      "LTRIM",
      key,
      start.ToString(),
      end.ToString()
    });

    /// <summary>移除并返回列表 key 的尾元素。当 key 不存在时，返回 nil 。</summary>
    /// <param name="key">关键字信息</param>
    /// <returns>列表的尾元素。</returns>
    public OperateResult<string> ListRightPop(string key) => this.OperateStringFromServer(new string[2]
    {
      "RPOP",
      key
    });

    /// <summary>
    /// 命令 RPOPLPUSH 在一个原子时间内，执行以下两个动作：<br />
    /// 1. 将列表 source 中的最后一个元素( 尾元素)弹出，并返回给客户端。<br />
    /// 2. 将 source 弹出的元素插入到列表 destination ，作为 destination 列表的的头元素。<br /><br />
    /// 举个例子，你有两个列表 source 和 destination ， source 列表有元素 a, b, c ， destination 列表有元素 x, y, z ，执行 RPOPLPUSH source destination 之后， source 列表包含元素 a, b ， destination 列表包含元素 c, x, y, z ，并且元素 c 会被返回给客户端。
    /// 如果 source 不存在，值 nil 被返回，并且不执行其他动作。
    /// 如果 source 和 destination 相同，则列表中的表尾元素被移动到表头，并返回该元素，可以把这种特殊情况视作列表的旋转( rotation)操作。
    /// </summary>
    /// <param name="key1">第一个关键字</param>
    /// <param name="key2">第二个关键字</param>
    /// <returns>返回的移除的对象</returns>
    public OperateResult<string> ListRightPopLeftPush(string key1, string key2) => this.OperateStringFromServer(new string[3]
    {
      "RPOPLPUSH",
      key1,
      key2
    });

    /// <summary>
    /// 将一个或多个值 value 插入到列表 key 的表尾(最右边)。
    /// 如果 key 不存在，一个空列表会被创建并执行 RPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">值</param>
    /// <returns>返回执行 RPUSH 操作后，表的长度。</returns>
    public OperateResult<int> ListRightPush(string key, string value) => this.ListRightPush(key, new string[1]
    {
      value
    });

    /// <summary>
    /// 将一个或多个值 value 插入到列表 key 的表尾(最右边)。
    /// 如果有多个 value 值，那么各个 value 值按从左到右的顺序依次插入到表尾：比如对一个空列表 mylist 执行 RPUSH mylist a b c ，得出的结果列表为 a b c ，
    /// 如果 key 不存在，一个空列表会被创建并执行 RPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。
    /// 返回执行 RPUSH 操作后，表的长度。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="values">值</param>
    /// <returns>返回执行 RPUSH 操作后，表的长度。</returns>
    public OperateResult<int> ListRightPush(string key, string[] values) => this.OperateNumberFromServer(SoftBasic.SpliceStringArray("RPUSH", key, values));

    /// <summary>
    /// 将值 value 插入到列表 key 的表尾，当且仅当 key 存在并且是一个列表。
    /// 和 RPUSH 命令相反，当 key 不存在时， RPUSHX 命令什么也不做。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="value">值</param>
    /// <returns>RPUSHX 命令执行之后，表的长度。</returns>
    public OperateResult<int> ListRightPushX(string key, string value) => this.OperateNumberFromServer(new string[3]
    {
      "RPUSHX",
      key,
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListInsertBefore(System.String,System.String,System.String)" />
    public async Task<OperateResult<int>> ListInsertBeforeAsync(
      string key,
      string value,
      string pivot)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[5]
      {
        "LINSERT",
        key,
        "BEFORE",
        pivot,
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListInsertAfter(System.String,System.String,System.String)" />
    public async Task<OperateResult<int>> ListInsertAfterAsync(
      string key,
      string value,
      string pivot)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[5]
      {
        "LINSERT",
        key,
        "AFTER",
        pivot,
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.GetListLength(System.String)" />
    public async Task<OperateResult<int>> GetListLengthAsync(string key)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[2]
      {
        "LLEN",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadListByIndex(System.String,System.Int64)" />
    public async Task<OperateResult<string>> ReadListByIndexAsync(
      string key,
      long index)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[3]
      {
        "LINDEX",
        key,
        index.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListLeftPop(System.String)" />
    public async Task<OperateResult<string>> ListLeftPopAsync(string key)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[2]
      {
        "LPOP",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListLeftPush(System.String,System.String)" />
    public async Task<OperateResult<int>> ListLeftPushAsync(
      string key,
      string value)
    {
      OperateResult<int> operateResult = await this.ListLeftPushAsync(key, new string[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListLeftPush(System.String,System.String[])" />
    public async Task<OperateResult<int>> ListLeftPushAsync(
      string key,
      string[] values)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(SoftBasic.SpliceStringArray("LPUSH", key, values));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListLeftPushX(System.String,System.String)" />
    public async Task<OperateResult<int>> ListLeftPushXAsync(
      string key,
      string value)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[3]
      {
        "LPUSHX",
        key,
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListRange(System.String,System.Int64,System.Int64)" />
    public async Task<OperateResult<string[]>> ListRangeAsync(
      string key,
      long start,
      long stop)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[4]
      {
        "LRANGE",
        key,
        start.ToString(),
        stop.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListRemoveElementMatch(System.String,System.Int64,System.String)" />
    public async Task<OperateResult<int>> ListRemoveElementMatchAsync(
      string key,
      long count,
      string value)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[4]
      {
        "LREM",
        key,
        count.ToString(),
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListSet(System.String,System.Int64,System.String)" />
    public async Task<OperateResult> ListSetAsync(
      string key,
      long index,
      string value)
    {
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(new string[4]
      {
        "LSET",
        key.ToString(),
        index.ToString(),
        value
      });
      return (OperateResult) operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListTrim(System.String,System.Int64,System.Int64)" />
    public async Task<OperateResult> ListTrimAsync(
      string key,
      long start,
      long end)
    {
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(new string[4]
      {
        "LTRIM",
        key,
        start.ToString(),
        end.ToString()
      });
      return (OperateResult) operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListRightPop(System.String)" />
    public async Task<OperateResult<string>> ListRightPopAsync(string key)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[2]
      {
        "RPOP",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListRightPopLeftPush(System.String,System.String)" />
    public async Task<OperateResult<string>> ListRightPopLeftPushAsync(
      string key1,
      string key2)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[3]
      {
        "RPOPLPUSH",
        key1,
        key2
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListRightPush(System.String,System.String)" />
    public async Task<OperateResult<int>> ListRightPushAsync(
      string key,
      string value)
    {
      OperateResult<int> operateResult = await this.ListRightPushAsync(key, new string[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListRightPush(System.String,System.String[])" />
    public async Task<OperateResult<int>> ListRightPushAsync(
      string key,
      string[] values)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(SoftBasic.SpliceStringArray("RPUSH", key, values));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ListRightPushX(System.String,System.String)" />
    public async Task<OperateResult<int>> ListRightPushXAsync(
      string key,
      string value)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[3]
      {
        "RPUSHX",
        key,
        value
      });
      return operateResult;
    }

    /// <summary>删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。</summary>
    /// <param name="key">关键字</param>
    /// <param name="field">域</param>
    /// <returns>被成功移除的域的数量，不包括被忽略的域。</returns>
    public OperateResult<int> DeleteHashKey(string key, string field) => this.DeleteHashKey(key, new string[1]
    {
      field
    });

    /// <summary>删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。返回被成功移除的域的数量，不包括被忽略的域。</summary>
    /// <param name="key">关键字</param>
    /// <param name="fields">所有的域</param>
    /// <returns>返回被成功移除的域的数量，不包括被忽略的域。</returns>
    public OperateResult<int> DeleteHashKey(string key, string[] fields) => this.OperateNumberFromServer(SoftBasic.SpliceStringArray("HDEL", key, fields));

    /// <summary>
    /// 查看哈希表 key 中，给定域 field 是否存在。如果哈希表含有给定域，返回 1 。
    /// 如果哈希表不含有给定域，或 key 不存在，返回 0 。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="field">域</param>
    /// <returns>如果哈希表含有给定域，返回 1 。如果哈希表不含有给定域，或 key 不存在，返回 0 。</returns>
    public OperateResult<int> ExistsHashKey(string key, string field) => this.OperateNumberFromServer(new string[3]
    {
      "HEXISTS",
      key,
      field
    });

    /// <summary>返回哈希表 key 中给定域 field 的值。当给定域不存在或是给定 key 不存在时，返回 nil</summary>
    /// <param name="key">关键值</param>
    /// <param name="field">域</param>
    /// <returns>
    /// 给定域的值。
    /// 当给定域不存在或是给定 key 不存在时，返回 nil 。
    /// </returns>
    public OperateResult<string> ReadHashKey(string key, string field) => this.OperateStringFromServer(new string[3]
    {
      "HGET",
      key,
      field
    });

    /// <summary>
    /// 返回哈希表 key 中，所有的域和值。在返回值里，紧跟每个域名(field name)之后是域的值(value)，所以返回值的长度是哈希表大小的两倍。
    /// </summary>
    /// <param name="key">关键值</param>
    /// <returns>
    /// 以列表形式返回哈希表的域和域的值。
    /// 若 key 不存在，返回空列表。
    /// </returns>
    public OperateResult<string[]> ReadHashKeyAll(string key) => this.OperateStringsFromServer(new string[2]
    {
      "HGETALL",
      key
    });

    /// <summary>
    /// 为哈希表 key 中的域 field 的值加上增量 increment 。增量也可以为负数，相当于对给定域进行减法操作。
    /// 如果 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="field">域</param>
    /// <param name="value">增量值</param>
    /// <returns>返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。</returns>
    public OperateResult<long> IncrementHashKey(string key, string field, long value) => this.OperateLongNumberFromServer(new string[4]
    {
      "HINCRBY",
      key,
      field,
      value.ToString()
    });

    /// <summary>
    /// 为哈希表 key 中的域 field 的值加上增量 increment 。增量也可以为负数，相当于对给定域进行减法操作。
    /// 如果 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="field">域</param>
    /// <param name="value">增量值</param>
    /// <returns>返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。</returns>
    public OperateResult<string> IncrementHashKey(
      string key,
      string field,
      float value)
    {
      return this.OperateStringFromServer(new string[4]
      {
        "HINCRBYFLOAT",
        key,
        field,
        value.ToString()
      });
    }

    /// <summary>返回哈希表 key 中的所有域。当 key 不存在时，返回一个空表。</summary>
    /// <param name="key">关键值</param>
    /// <returns>
    /// 一个包含哈希表中所有域的表。
    /// 当 key 不存在时，返回一个空表。
    /// </returns>
    public OperateResult<string[]> ReadHashKeys(string key) => this.OperateStringsFromServer(new string[2]
    {
      "HKEYS",
      key
    });

    /// <summary>返回哈希表 key 中域的数量。</summary>
    /// <param name="key">关键字</param>
    /// <returns>哈希表中域的数量。当 key 不存在时，返回 0 。</returns>
    public OperateResult<int> ReadHashKeyLength(string key) => this.OperateNumberFromServer(new string[2]
    {
      "HLEN",
      key
    });

    /// <summary>
    /// 返回哈希表 key 中，一个或多个给定域的值。如果给定的域不存在于哈希表，那么返回一个 nil 值。
    /// 因为不存在的 key 被当作一个空哈希表来处理，所以对一个不存在的 key 进行 HMGET 操作将返回一个只带有 nil 值的表。
    /// </summary>
    /// <param name="key">关键值</param>
    /// <param name="fields">指定的域</param>
    /// <returns>一个包含多个给定域的关联值的表，表值的排列顺序和给定域参数的请求顺序一样。</returns>
    public OperateResult<string[]> ReadHashKey(string key, string[] fields) => this.OperateStringsFromServer(SoftBasic.SpliceStringArray("HMGET", key, fields));

    /// <summary>
    /// 将哈希表 key 中的域 field 的值设为 value 。
    /// 如果 key 不存在，一个新的哈希表被创建并进行 HSET 操作。
    /// 如果域 field 已经存在于哈希表中，旧值将被覆盖。
    /// 如果 field 是哈希表中的一个新建域，并且值设置成功，返回 1 。
    /// 如果哈希表中域 field 已经存在且旧值已被新值覆盖，返回 0 。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="field">域</param>
    /// <param name="value">数据值</param>
    /// <returns>
    /// 如果 field 是哈希表中的一个新建域，并且值设置成功，返回 1 。
    /// 如果哈希表中域 field 已经存在且旧值已被新值覆盖，返回 0 。
    /// </returns>
    public OperateResult<int> WriteHashKey(string key, string field, string value) => this.OperateNumberFromServer(new string[4]
    {
      "HSET",
      key,
      field,
      value
    });

    /// <summary>
    /// 同时将多个 field-value (域-值)对设置到哈希表 key 中。
    /// 此命令会覆盖哈希表中已存在的域。
    /// 如果 key 不存在，一个空哈希表被创建并执行 HMSET 操作。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="fields">域</param>
    /// <param name="values">数据值</param>
    /// <returns>
    /// 如果命令执行成功，返回 OK 。
    /// 当 key 不是哈希表(hash)类型时，返回一个错误
    /// </returns>
    public OperateResult WriteHashKey(string key, string[] fields, string[] values)
    {
      if (fields == null)
        throw new ArgumentNullException(nameof (fields));
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (fields.Length != values.Length)
        throw new ArgumentException("Two arguement not same length");
      List<string> stringList = new List<string>();
      stringList.Add("HMSET");
      stringList.Add(key);
      for (int index = 0; index < fields.Length; ++index)
      {
        stringList.Add(fields[index]);
        stringList.Add(values[index]);
      }
      return (OperateResult) this.OperateStatusFromServer(stringList.ToArray());
    }

    /// <summary>
    /// 将哈希表 key 中的域 field 的值设置为 value ，当且仅当域 field 不存在。若域 field 已经存在，该操作无效。
    /// 设置成功，返回 1 。如果给定域已经存在且没有操作被执行，返回 0 。
    /// </summary>
    /// <param name="key">关键字</param>
    /// <param name="field">域</param>
    /// <param name="value">数据值</param>
    /// <returns>设置成功，返回 1 。如果给定域已经存在且没有操作被执行，返回 0 。</returns>
    public OperateResult<int> WriteHashKeyNx(string key, string field, string value) => this.OperateNumberFromServer(new string[4]
    {
      "HSETNX",
      key,
      field,
      value
    });

    /// <summary>返回哈希表 key 中所有域的值。当 key 不存在时，返回一个空表。</summary>
    /// <param name="key">关键值</param>
    /// <returns>
    /// 返回哈希表 key 中所有域的值。
    /// 当 key 不存在时，返回一个空表。
    /// </returns>
    public OperateResult<string[]> ReadHashValues(string key) => this.OperateStringsFromServer(new string[2]
    {
      "HVALS",
      key
    });

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.DeleteHashKey(System.String,System.String)" />
    public async Task<OperateResult<int>> DeleteHashKeyAsync(
      string key,
      string field)
    {
      OperateResult<int> operateResult = await this.DeleteHashKeyAsync(key, new string[1]
      {
        field
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.DeleteHashKey(System.String,System.String[])" />
    public async Task<OperateResult<int>> DeleteHashKeyAsync(
      string key,
      string[] fields)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(SoftBasic.SpliceStringArray("HDEL", key, fields));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ExistsHashKey(System.String,System.String)" />
    public async Task<OperateResult<int>> ExistsHashKeyAsync(
      string key,
      string field)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[3]
      {
        "HEXISTS",
        key,
        field
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadHashKey(System.String,System.String)" />
    public async Task<OperateResult<string>> ReadHashKeyAsync(
      string key,
      string field)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[3]
      {
        "HGET",
        key,
        field
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadHashKeyAll(System.String)" />
    public async Task<OperateResult<string[]>> ReadHashKeyAllAsync(string key)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[2]
      {
        "HGETALL",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.IncrementHashKey(System.String,System.String,System.Int64)" />
    public async Task<OperateResult<long>> IncrementHashKeyAsync(
      string key,
      string field,
      long value)
    {
      OperateResult<long> operateResult = await this.OperateLongNumberFromServerAsync(new string[4]
      {
        "HINCRBY",
        key,
        field,
        value.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.IncrementHashKey(System.String,System.String,System.Single)" />
    public async Task<OperateResult<string>> IncrementHashKeyAsync(
      string key,
      string field,
      float value)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[4]
      {
        "HINCRBYFLOAT",
        key,
        field,
        value.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadHashKeys(System.String)" />
    public async Task<OperateResult<string[]>> ReadHashKeysAsync(string key)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[2]
      {
        "HKEYS",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadHashKeyLength(System.String)" />
    public async Task<OperateResult<int>> ReadHashKeyLengthAsync(string key)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[2]
      {
        "HLEN",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadHashKey(System.String,System.String[])" />
    public async Task<OperateResult<string[]>> ReadHashKeyAsync(
      string key,
      string[] fields)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(SoftBasic.SpliceStringArray("HMGET", key, fields));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.WriteHashKey(System.String,System.String,System.String)" />
    public async Task<OperateResult<int>> WriteHashKeyAsync(
      string key,
      string field,
      string value)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[4]
      {
        "HSET",
        key,
        field,
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.WriteHashKey(System.String,System.String[],System.String[])" />
    public async Task<OperateResult> WriteHashKeyAsync(
      string key,
      string[] fields,
      string[] values)
    {
      if (fields == null)
        throw new ArgumentNullException(nameof (fields));
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (fields.Length != values.Length)
        throw new ArgumentException("Two arguement not same length");
      List<string> list = new List<string>();
      list.Add("HMSET");
      list.Add(key);
      for (int i = 0; i < fields.Length; ++i)
      {
        list.Add(fields[i]);
        list.Add(values[i]);
      }
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(list.ToArray());
      OperateResult operateResult1 = (OperateResult) operateResult;
      list = (List<string>) null;
      return operateResult1;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.WriteHashKeyNx(System.String,System.String,System.String)" />
    public async Task<OperateResult<int>> WriteHashKeyNxAsync(
      string key,
      string field,
      string value)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[4]
      {
        "HSETNX",
        key,
        field,
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadHashValues(System.String)" />
    public async Task<OperateResult<string[]>> ReadHashValuesAsync(string key)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[2]
      {
        "HVALS",
        key
      });
      return operateResult;
    }

    /// <summary>
    /// 将一个member 元素加入到集合 key 当中，已经存在于集合的 member 元素将被忽略。假如 key 不存在，则创建一个只包含 member 元素作成员的集合。当 key 不是集合类型时，返回一个错误。
    /// </summary>
    /// <param name="key">关键字信息</param>
    /// <param name="member">等待添加的元素</param>
    /// <returns>被添加到集合中的新元素的数量，不包括被忽略的元素。</returns>
    public OperateResult<int> SetAdd(string key, string member) => this.SetAdd(key, new string[1]
    {
      member
    });

    /// <summary>
    /// 将一个或多个 member 元素加入到集合 key 当中，已经存在于集合的 member 元素将被忽略。假如 key 不存在，则创建一个只包含 member 元素作成员的集合。当 key 不是集合类型时，返回一个错误。
    /// </summary>
    /// <param name="key">关键字信息</param>
    /// <param name="members">等待添加的元素</param>
    /// <returns>被添加到集合中的新元素的数量，不包括被忽略的元素。</returns>
    public OperateResult<int> SetAdd(string key, string[] members) => this.OperateNumberFromServer(SoftBasic.SpliceStringArray("SADD", key, members));

    /// <summary>返回集合 key 的基数(集合中元素的数量)。当 key 不存在时，返回 0 。</summary>
    /// <param name="key">集合 key 的名称</param>
    /// <returns>集合的基数。</returns>
    public OperateResult<int> SetCard(string key) => this.OperateNumberFromServer(new string[2]
    {
      "SCARD",
      key
    });

    /// <summary>返回一个集合的全部成员，该集合是所有给定集合之间的差集。</summary>
    /// <param name="key">集合关键字</param>
    /// <param name="diffKey">集合关键字</param>
    /// <returns>交集成员的列表。</returns>
    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetDiff(System.String,System.String[])" />
    public OperateResult<string[]> SetDiff(string key, string diffKey) => this.SetDiff(key, new string[1]
    {
      diffKey
    });

    /// <summary>返回一个集合的全部成员，该集合是所有给定集合之间的差集。</summary>
    /// <param name="key">集合关键字</param>
    /// <param name="diffKeys">集合关键字</param>
    /// <returns>交集成员的列表。</returns>
    public OperateResult<string[]> SetDiff(string key, string[] diffKeys) => this.OperateStringsFromServer(SoftBasic.SpliceStringArray("SDIFF", key, diffKeys));

    /// <summary>
    /// 这个命令的作用和 SDIFF 类似，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
    /// </summary>
    /// <param name="destination">目标集合</param>
    /// <param name="key">等待操作的集合</param>
    /// <param name="diffKey">运算的集合</param>
    /// <returns>结果集中的元素数量。</returns>
    public OperateResult<int> SetDiffStore(
      string destination,
      string key,
      string diffKey)
    {
      return this.SetDiffStore(destination, key, new string[1]
      {
        diffKey
      });
    }

    /// <summary>
    /// 这个命令的作用和 SDIFF 类似，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
    /// </summary>
    /// <param name="destination">目标集合</param>
    /// <param name="key">等待操作的集合</param>
    /// <param name="diffKeys">运算的集合</param>
    /// <returns>结果集中的元素数量。</returns>
    public OperateResult<int> SetDiffStore(
      string destination,
      string key,
      string[] diffKeys)
    {
      return this.OperateNumberFromServer(SoftBasic.SpliceStringArray("SDIFFSTORE", destination, key, diffKeys));
    }

    /// <summary>
    /// 返回一个集合的全部成员，该集合是所有给定集合的交集。不存在的 key 被视为空集。当给定集合当中有一个空集时，结果也为空集(根据集合运算定律)。
    /// </summary>
    /// <param name="key">集合关键字</param>
    /// <param name="interKey">运算的集合</param>
    /// <returns>交集成员的列表。</returns>
    public OperateResult<string[]> SetInter(string key, string interKey) => this.SetInter(key, new string[1]
    {
      interKey
    });

    /// <summary>
    /// 返回一个集合的全部成员，该集合是所有给定集合的交集。不存在的 key 被视为空集。当给定集合当中有一个空集时，结果也为空集(根据集合运算定律)。
    /// </summary>
    /// <param name="key">集合关键字</param>
    /// <param name="interKeys">运算的集合</param>
    /// <returns>交集成员的列表。</returns>
    public OperateResult<string[]> SetInter(string key, string[] interKeys) => this.OperateStringsFromServer(SoftBasic.SpliceStringArray("SINTER", key, interKeys));

    /// <summary>
    /// 这个命令类似于 SINTER 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
    /// </summary>
    /// <param name="destination">目标集合</param>
    /// <param name="key">等待操作的集合</param>
    /// <param name="interKey">运算的集合</param>
    /// <returns>结果集中的成员数量。</returns>
    public OperateResult<int> SetInterStore(
      string destination,
      string key,
      string interKey)
    {
      return this.SetInterStore(destination, key, new string[1]
      {
        interKey
      });
    }

    /// <summary>
    /// 这个命令类似于 SINTER 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
    /// </summary>
    /// <param name="destination">目标集合</param>
    /// <param name="key">等待操作的集合</param>
    /// <param name="interKeys">运算的集合</param>
    /// <returns>结果集中的成员数量。</returns>
    public OperateResult<int> SetInterStore(
      string destination,
      string key,
      string[] interKeys)
    {
      return this.OperateNumberFromServer(SoftBasic.SpliceStringArray("SINTERSTORE", destination, key, interKeys));
    }

    /// <summary>
    /// 判断 member 元素是否集合 key 的成员。如果 member 元素是集合的成员，返回 1 。如果 member 元素不是集合的成员，或 key 不存在，返回 0 。
    /// </summary>
    /// <param name="key">集合key</param>
    /// <param name="member">元素</param>
    /// <returns>如果 member 元素是集合的成员，返回 1 。如果 member 元素不是集合的成员，或 key 不存在，返回 0 。</returns>
    public OperateResult<int> SetIsMember(string key, string member) => this.OperateNumberFromServer(new string[3]
    {
      "SISMEMBER",
      key,
      member
    });

    /// <summary>返回集合 key 中的所有成员。不存在的 key 被视为空集合。</summary>
    /// <param name="key">集合key</param>
    /// <returns>集合中的所有成员。</returns>
    public OperateResult<string[]> SetMembers(string key) => this.OperateStringsFromServer(new string[2]
    {
      "SMEMBERS",
      key
    });

    /// <summary>
    /// 将 member 元素从 source 集合移动到 destination 集合。如果 source 集合不存在或不包含指定的 member 元素，则 SMOVE 命令不执行任何操作，仅返回 0 。
    /// 否则， member 元素从 source 集合中被移除，并添加到 destination 集合中去。当 destination 集合已经包含 member 元素时， SMOVE 命令只是简单地将 source 集合中的 member 元素删除。
    /// 当 source 或 destination 不是集合类型时，返回一个错误。
    /// </summary>
    /// <param name="source">原集合</param>
    /// <param name="destination">目标集合</param>
    /// <param name="member">元素</param>
    /// <returns>如果 member 元素被成功移除，返回 1 。如果 member 元素不是 source 集合的成员，并且没有任何操作对 destination 集合执行，那么返回 0 。</returns>
    public OperateResult<int> SetMove(
      string source,
      string destination,
      string member)
    {
      return this.OperateNumberFromServer(new string[4]
      {
        "SMOVE",
        source,
        destination,
        member
      });
    }

    /// <summary>
    /// 移除并返回集合中的一个随机元素。如果只想获取一个随机元素，但不想该元素从集合中被移除的话，可以使用 SRANDMEMBER 命令。
    /// </summary>
    /// <param name="key">集合关键字</param>
    /// <returns>被移除的随机元素。当 key 不存在或 key 是空集时，返回 nil 。</returns>
    public OperateResult<string> SetPop(string key) => this.OperateStringFromServer(new string[2]
    {
      "SPOP",
      key
    });

    /// <summary>那么返回集合中的一个随机元素。</summary>
    /// <param name="key">集合关键字</param>
    /// <returns>返回一个元素；如果集合为空，返回 nil 。</returns>
    public OperateResult<string> SetRandomMember(string key) => this.OperateStringFromServer(new string[2]
    {
      "SRANDMEMBER",
      key
    });

    /// <summary>
    /// 返回集合中的多个随机元素。<br />
    /// 如果 count 为正数，且小于集合基数，那么命令返回一个包含 count 个元素的数组，数组中的元素各不相同。如果 count 大于等于集合基数，那么返回整个集合。<br />
    /// 如果 count 为负数，那么命令返回一个数组，数组中的元素可能会重复出现多次，而数组的长度为 count 的绝对值。
    /// </summary>
    /// <param name="key">集合关键字</param>
    /// <param name="count">元素个数</param>
    /// <returns>返回一个数组；如果集合为空，返回空数组。</returns>
    public OperateResult<string[]> SetRandomMember(string key, int count) => this.OperateStringsFromServer(new string[3]
    {
      "SRANDMEMBER",
      key,
      count.ToString()
    });

    /// <summary>移除集合 key 中的一个元素，不存在的 member 元素会被忽略。</summary>
    /// <param name="key">集合关键字</param>
    /// <param name="member">等待移除的元素</param>
    /// <returns>被成功移除的元素的数量，不包括被忽略的元素。</returns>
    public OperateResult<int> SetRemove(string key, string member) => this.SetRemove(key, new string[1]
    {
      member
    });

    /// <summary>移除集合 key 中的一个或多个 member 元素，不存在的 member 元素会被忽略。</summary>
    /// <param name="key">集合关键字</param>
    /// <param name="members">等待移除的元素</param>
    /// <returns>被成功移除的元素的数量，不包括被忽略的元素。</returns>
    public OperateResult<int> SetRemove(string key, string[] members) => this.OperateNumberFromServer(SoftBasic.SpliceStringArray("SREM", key, members));

    /// <summary>返回一个集合的全部成员，该集合是所有给定集合的并集。不存在的 key 被视为空集。</summary>
    /// <param name="key">集合关键字</param>
    /// <param name="unionKey">并集的集合</param>
    /// <returns>并集成员的列表。</returns>
    public OperateResult<string[]> SetUnion(string key, string unionKey) => this.SetUnion(key, new string[1]
    {
      unionKey
    });

    /// <summary>返回一个或多个集合的全部成员，该集合是所有给定集合的并集。不存在的 key 被视为空集。</summary>
    /// <param name="key">集合关键字</param>
    /// <param name="unionKeys">并集的集合</param>
    /// <returns>并集成员的列表。</returns>
    public OperateResult<string[]> SetUnion(string key, string[] unionKeys) => this.OperateStringsFromServer(SoftBasic.SpliceStringArray("SUNION", key, unionKeys));

    /// <summary>
    /// 这个命令类似于 SUNION 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 已经存在，则将其覆盖。destination 可以是 key 本身。
    /// </summary>
    /// <param name="destination">存储的目标集合</param>
    /// <param name="key">集合关键字</param>
    /// <param name="unionKey">并集的集合</param>
    /// <returns>结果集中的元素数量。</returns>
    public OperateResult<int> SetUnionStore(
      string destination,
      string key,
      string unionKey)
    {
      return this.SetUnionStore(destination, key, unionKey);
    }

    /// <summary>
    /// 这个命令类似于 SUNION 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 已经存在，则将其覆盖。destination 可以是 key 本身。
    /// </summary>
    /// <param name="destination">存储的目标集合</param>
    /// <param name="key">集合关键字</param>
    /// <param name="unionKeys">并集的集合</param>
    /// <returns>结果集中的元素数量。</returns>
    public OperateResult<int> SetUnionStore(
      string destination,
      string key,
      string[] unionKeys)
    {
      return this.OperateNumberFromServer(SoftBasic.SpliceStringArray("SUNIONSTORE", destination, key, unionKeys));
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetAdd(System.String,System.String)" />
    public async Task<OperateResult<int>> SetAddAsync(string key, string member)
    {
      OperateResult<int> operateResult = await this.SetAddAsync(key, new string[1]
      {
        member
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetAdd(System.String,System.String[])" />
    public async Task<OperateResult<int>> SetAddAsync(string key, string[] members)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(SoftBasic.SpliceStringArray("SADD", key, members));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetCard(System.String)" />
    public async Task<OperateResult<int>> SetCardAsync(string key)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[2]
      {
        "SCARD",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetDiff(System.String,System.String)" />
    public async Task<OperateResult<string[]>> SetDiffAsync(
      string key,
      string diffKey)
    {
      OperateResult<string[]> operateResult = await this.SetDiffAsync(key, new string[1]
      {
        diffKey
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetDiff(System.String,System.String[])" />
    public async Task<OperateResult<string[]>> SetDiffAsync(
      string key,
      string[] diffKeys)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(SoftBasic.SpliceStringArray("SDIFF", key, diffKeys));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetDiffStore(System.String,System.String,System.String)" />
    public async Task<OperateResult<int>> SetDiffStoreAsync(
      string destination,
      string key,
      string diffKey)
    {
      OperateResult<int> operateResult = await this.SetDiffStoreAsync(destination, key, new string[1]
      {
        diffKey
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetDiffStore(System.String,System.String,System.String[])" />
    public async Task<OperateResult<int>> SetDiffStoreAsync(
      string destination,
      string key,
      string[] diffKeys)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(SoftBasic.SpliceStringArray("SDIFFSTORE", destination, key, diffKeys));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetInter(System.String,System.String)" />
    public async Task<OperateResult<string[]>> SetInterAsync(
      string key,
      string interKey)
    {
      OperateResult<string[]> operateResult = await this.SetInterAsync(key, new string[1]
      {
        interKey
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetInter(System.String,System.String[])" />
    public async Task<OperateResult<string[]>> SetInterAsync(
      string key,
      string[] interKeys)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(SoftBasic.SpliceStringArray("SINTER", key, interKeys));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetInterStore(System.String,System.String,System.String)" />
    public async Task<OperateResult<int>> SetInterStoreAsync(
      string destination,
      string key,
      string interKey)
    {
      OperateResult<int> operateResult = await this.SetInterStoreAsync(destination, key, new string[1]
      {
        interKey
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetInterStore(System.String,System.String,System.String[])" />
    public async Task<OperateResult<int>> SetInterStoreAsync(
      string destination,
      string key,
      string[] interKeys)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(SoftBasic.SpliceStringArray("SINTERSTORE", destination, key, interKeys));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetIsMember(System.String,System.String)" />
    public async Task<OperateResult<int>> SetIsMemberAsync(
      string key,
      string member)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[3]
      {
        "SISMEMBER",
        key,
        member
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetMembers(System.String)" />
    public async Task<OperateResult<string[]>> SetMembersAsync(string key)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[2]
      {
        "SMEMBERS",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetMove(System.String,System.String,System.String)" />
    public async Task<OperateResult<int>> SetMoveAsync(
      string source,
      string destination,
      string member)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[4]
      {
        "SMOVE",
        source,
        destination,
        member
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetPop(System.String)" />
    public async Task<OperateResult<string>> SetPopAsync(string key)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[2]
      {
        "SPOP",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetRandomMember(System.String)" />
    public async Task<OperateResult<string>> SetRandomMemberAsync(string key)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[2]
      {
        "SRANDMEMBER",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetRandomMember(System.String,System.Int32)" />
    public async Task<OperateResult<string[]>> SetRandomMemberAsync(
      string key,
      int count)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[3]
      {
        "SRANDMEMBER",
        key,
        count.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetRemove(System.String,System.String)" />
    public async Task<OperateResult<int>> SetRemoveAsync(string key, string member)
    {
      OperateResult<int> operateResult = await this.SetRemoveAsync(key, new string[1]
      {
        member
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetRemove(System.String,System.String[])" />
    public async Task<OperateResult<int>> SetRemoveAsync(
      string key,
      string[] members)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(SoftBasic.SpliceStringArray("SREM", key, members));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetUnion(System.String,System.String)" />
    public async Task<OperateResult<string[]>> SetUnionAsync(
      string key,
      string unionKey)
    {
      OperateResult<string[]> operateResult = await this.SetUnionAsync(key, new string[1]
      {
        unionKey
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetUnion(System.String,System.String[])" />
    public async Task<OperateResult<string[]>> SetUnionAsync(
      string key,
      string[] unionKeys)
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(SoftBasic.SpliceStringArray("SUNION", key, unionKeys));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetUnionStore(System.String,System.String,System.String)" />
    public async Task<OperateResult<int>> SetUnionStoreAsync(
      string destination,
      string key,
      string unionKey)
    {
      OperateResult<int> operateResult = await this.SetUnionStoreAsync(destination, key, unionKey);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SetUnionStore(System.String,System.String,System.String[])" />
    public async Task<OperateResult<int>> SetUnionStoreAsync(
      string destination,
      string key,
      string[] unionKeys)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(SoftBasic.SpliceStringArray("SUNIONSTORE", destination, key, unionKeys));
      return operateResult;
    }

    /// <summary>
    /// 将一个 member 元素及其 score 值加入到有序集 key 当中。如果某个 member 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 member 元素，来保证该 member 在正确的位置上。
    /// score 值可以是整数值或双精度浮点数。<br />
    /// 如果 key 不存在，则创建一个空的有序集并执行 ZADD 操作。当 key 存在但不是有序集类型时，返回一个错误。
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="member">有序集合的元素</param>
    /// <param name="score">每个元素的得分</param>
    /// <returns>被成功添加的新成员的数量，不包括那些被更新的、已经存在的成员。</returns>
    public OperateResult<int> ZSetAdd(string key, string member, double score) => this.ZSetAdd(key, new string[1]
    {
      member
    }, new double[1]{ score });

    /// <summary>
    /// 将一个或多个 member 元素及其 score 值加入到有序集 key 当中。如果某个 member 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 member 元素，来保证该 member 在正确的位置上。
    /// score 值可以是整数值或双精度浮点数。<br />
    /// 如果 key 不存在，则创建一个空的有序集并执行 ZADD 操作。当 key 存在但不是有序集类型时，返回一个错误。
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="members">有序集合的元素</param>
    /// <param name="scores">每个元素的得分</param>
    /// <returns>被成功添加的新成员的数量，不包括那些被更新的、已经存在的成员。</returns>
    public OperateResult<int> ZSetAdd(string key, string[] members, double[] scores)
    {
      if (members.Length != scores.Length)
        throw new Exception(StringResources.Language.TwoParametersLengthIsNotSame);
      List<string> stringList = new List<string>();
      stringList.Add("ZADD");
      stringList.Add(key);
      for (int index = 0; index < members.Length; ++index)
      {
        stringList.Add(scores[index].ToString());
        stringList.Add(members[index]);
      }
      return this.OperateNumberFromServer(stringList.ToArray());
    }

    /// <summary>返回有序集 key 的基数。</summary>
    /// <param name="key">有序集合的关键字</param>
    /// <returns>当 key 存在且是有序集类型时，返回有序集的基数。当 key 不存在时，返回 0 。</returns>
    public OperateResult<int> ZSetCard(string key) => this.OperateNumberFromServer(new string[2]
    {
      "ZCARD",
      key
    });

    /// <summary>
    /// 返回有序集 key 中， score 值在 min 和 max 之间(默认包括 score 值等于 min 或 max )的成员的数量。
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="min">最小值，包含</param>
    /// <param name="max">最大值，包含</param>
    /// <returns>score 值在 min 和 max 之间的成员的数量。</returns>
    public OperateResult<int> ZSetCount(string key, double min, double max) => this.OperateNumberFromServer(new string[4]
    {
      "ZCOUNT",
      key,
      min.ToString(),
      max.ToString()
    });

    /// <summary>
    /// 为有序集 key 的成员 member 的 score 值加上增量 increment 。可以通过传递一个负数值 increment ，让 score 减去相应的值，比如 ZINCRBY key -5 member ，就是让 member 的 score 值减去 5 。
    /// 当 key 不存在，或 member 不是 key 的成员时， ZINCRBY key increment member 等同于 ZADD key increment member 。当 key 不是有序集类型时，返回一个错误。
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="member">成员名称</param>
    /// <param name="increment">增量数据，可以为负数</param>
    /// <returns>member 成员的新 score 值，以字符串形式表示。</returns>
    public OperateResult<string> ZSetIncreaseBy(
      string key,
      string member,
      double increment)
    {
      return this.OperateStringFromServer(new string[4]
      {
        "ZINCRBY",
        key,
        increment.ToString(),
        member
      });
    }

    /// <summary>
    /// 返回有序集 key 中，指定区间内的成员。其中成员的位置按 score 值递增(从小到大)来排序。具有相同 score 值的成员按字典序来排列。
    /// 下标参数 start 和 stop 都以 0 为底，也就是说，以 0 表示有序集第一个成员，以 1 表示有序集第二个成员，以此类推。你也可以使用负数下标，以 -1 表示最后一个成员， -2 表示倒数第二个成员，以此类推。
    /// </summary>
    /// <remarks>
    /// 超出范围的下标并不会引起错误。比如说，当 start 的值比有序集的最大下标还要大，或是 start &gt; stop 时， ZRANGE 命令只是简单地返回一个空列表。另一方面，假如 stop 参数的值比有序集的最大下标还要大，那么 Redis 将 stop 当作最大下标来处理。
    /// 可以通过使用 WITHSCORES 选项，来让成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示。客户端库可能会返回一些更复杂的数据类型，比如数组、元组等。
    /// </remarks>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="start">起始的下标</param>
    /// <param name="stop">结束的下标</param>
    /// <param name="withScore">是否带有 score 返回</param>
    /// <returns>指定区间内，根据参数 withScore 来决定是否带 score 值的有序集成员的列表。</returns>
    public OperateResult<string[]> ZSetRange(
      string key,
      int start,
      int stop,
      bool withScore = false)
    {
      return withScore ? this.OperateStringsFromServer(new string[5]
      {
        "ZRANGE",
        key,
        start.ToString(),
        stop.ToString(),
        "WITHSCORES"
      }) : this.OperateStringsFromServer(new string[4]
      {
        "ZRANGE",
        key,
        start.ToString(),
        stop.ToString()
      });
    }

    /// <summary>
    /// 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。有序集成员按 score 值递增(从小到大)次序排列。
    /// min 和 max 可以是 -inf 和 +inf ，这样一来，你就可以在不知道有序集的最低和最高 score 值的情况下，使用 ZRANGEBYSCORE 这类命令。
    /// 默认情况下，区间的取值使用闭区间 (小于等于或大于等于)，你也可以通过给参数前增加 "(" 符号来使用可选的开区间 (小于或大于)。"(5"代表不包含5
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="min">最小值，可以为-inf，代表最高，如果为5，代表大于等于5，如果是(5，代表大于5</param>
    /// <param name="max">最大值，可以为+inf，代表最低，如果为10，代表小于等于5，如果是(10，代表小于10</param>
    /// <param name="withScore">是否带有 score 返回</param>
    /// <returns>指定区间内，带有 score 值(根据参数 withScore 来决定)的有序集成员的列表。</returns>
    public OperateResult<string[]> ZSetRangeByScore(
      string key,
      string min,
      string max,
      bool withScore = false)
    {
      return withScore ? this.OperateStringsFromServer(new string[5]
      {
        "ZRANGEBYSCORE",
        key,
        min,
        max,
        "WITHSCORES"
      }) : this.OperateStringsFromServer(new string[4]
      {
        "ZRANGEBYSCORE",
        key,
        min,
        max
      });
    }

    /// <summary>
    /// 返回有序集 key 中成员 member 的排名。其中有序集成员按 score 值递增(从小到大)顺序排列。排名以 0 为底，也就是说， score 值最小的成员排名为 0 。
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="member">成员 member 的名称</param>
    /// <returns>如果 member 是有序集 key 的成员，返回 member 的排名。如果 member 不是有序集 key 的成员，返回 nil 。</returns>
    public OperateResult<int> ZSetRank(string key, string member) => this.OperateNumberFromServer(new string[3]
    {
      "ZRANK",
      key,
      member
    });

    /// <summary>移除有序集 key 中的指定成员，不存在的成员将被忽略。当 key 存在但不是有序集类型时，返回一个错误。</summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="member">等待被移除的成员</param>
    /// <returns>被成功移除的成员的数量，不包括被忽略的成员。</returns>
    public OperateResult<int> ZSetRemove(string key, string member) => this.ZSetRemove(key, new string[1]
    {
      member
    });

    /// <summary>
    /// 移除有序集 key 中的一个或多个成员，不存在的成员将被忽略。当 key 存在但不是有序集类型时，返回一个错误。
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="members">等待被移除的成员</param>
    /// <returns>被成功移除的成员的数量，不包括被忽略的成员。</returns>
    public OperateResult<int> ZSetRemove(string key, string[] members) => this.OperateNumberFromServer(SoftBasic.SpliceStringArray("ZREM", key, members));

    /// <summary>
    /// 移除有序集 key 中，指定排名(rank)区间内的所有成员。区间分别以下标参数 start 和 stop 指出，包含 start 和 stop 在内。
    /// 下标参数 start 和 stop 都以 0 为底，也就是说，以 0 表示有序集第一个成员，以 1 表示有序集第二个成员，以此类推。你也可以使用负数下标，以 -1 表示最后一个成员， -2 表示倒数第二个成员，以此类推。
    /// </summary>
    /// <param name="key">有序集合的关键</param>
    /// <param name="start">开始的下标</param>
    /// <param name="stop">结束的下标</param>
    /// <returns>被移除成员的数量。</returns>
    public OperateResult<int> ZSetRemoveRangeByRank(string key, int start, int stop) => this.OperateNumberFromServer(new string[4]
    {
      "ZREMRANGEBYRANK",
      key,
      start.ToString(),
      stop.ToString()
    });

    /// <summary>
    /// 移除有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。
    /// min 和 max 可以是 -inf 和 +inf ，这样一来，你就可以在不知道有序集的最低和最高 score 值的情况下，使用 ZRANGEBYSCORE 这类命令。
    /// 默认情况下，区间的取值使用闭区间 (小于等于或大于等于)，你也可以通过给参数前增加 "(" 符号来使用可选的开区间 (小于或大于)。例如"(5"代表不包括5
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="min">最小值，可以为-inf，代表最低，如果为5，代表大于等于5，如果是(5，代表大于5</param>
    /// <param name="max">最大值，可以为+inf，代表最低，如果为10，代表小于等于5，如果是(10，代表小于10</param>
    /// <returns>被移除成员的数量。</returns>
    public OperateResult<int> ZSetRemoveRangeByScore(
      string key,
      string min,
      string max)
    {
      return this.OperateNumberFromServer(new string[4]
      {
        "ZREMRANGEBYSCORE",
        key,
        min,
        max
      });
    }

    /// <summary>
    /// 返回有序集 key 中，指定区间内的成员。其中成员的位置按 score 值递减(从大到小)来排列。具有相同 score 值的成员按字典序来排列。
    /// 下标参数 start 和 stop 都以 0 为底，也就是说，以 0 表示有序集第一个成员，以 1 表示有序集第二个成员，以此类推。你也可以使用负数下标，以 -1 表示最后一个成员， -2 表示倒数第二个成员，以此类推。
    /// </summary>
    /// <remarks>
    /// 超出范围的下标并不会引起错误。比如说，当 start 的值比有序集的最大下标还要大，或是 start &gt; stop 时， ZRANGE 命令只是简单地返回一个空列表。另一方面，假如 stop 参数的值比有序集的最大下标还要大，那么 Redis 将 stop 当作最大下标来处理。
    /// 可以通过使用 WITHSCORES 选项，来让成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示。客户端库可能会返回一些更复杂的数据类型，比如数组、元组等。
    /// </remarks>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="start">起始的下标</param>
    /// <param name="stop">结束的下标</param>
    /// <param name="withScore">是否带有 score 返回</param>
    /// <returns>指定区间内，根据参数 withScore 来决定是否带 score 值的有序集成员的列表。</returns>
    public OperateResult<string[]> ZSetReverseRange(
      string key,
      int start,
      int stop,
      bool withScore = false)
    {
      return withScore ? this.OperateStringsFromServer(new string[5]
      {
        "ZREVRANGE",
        key,
        start.ToString(),
        stop.ToString(),
        "WITHSCORES"
      }) : this.OperateStringsFromServer(new string[4]
      {
        "ZREVRANGE",
        key,
        start.ToString(),
        stop.ToString()
      });
    }

    /// <summary>
    /// 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。序集成员按 score 值递减(从大到小)的次序排列。
    /// min 和 max 可以是 -inf 和 +inf ，这样一来，你就可以在不知道有序集的最低和最高 score 值的情况下，使用 ZRANGEBYSCORE 这类命令。
    /// 默认情况下，区间的取值使用闭区间 (小于等于或大于等于)，你也可以通过给参数前增加 ( 符号来使用可选的开区间 (小于或大于)。(5代表不包含5
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="max">最大值，可以为+inf，代表最高，如果为10，代表小于等于5，如果是(10，代表小于10</param>
    /// <param name="min">最小值，可以为-inf，代表最低，如果为5，代表大于等于5，如果是(5，代表大于5</param>
    /// <param name="withScore">是否带有 score 返回</param>
    /// <returns>指定区间内，带有 score 值(根据参数 withScore 来决定)的有序集成员的列表。</returns>
    public OperateResult<string[]> ZSetReverseRangeByScore(
      string key,
      string max,
      string min,
      bool withScore = false)
    {
      return withScore ? this.OperateStringsFromServer(new string[5]
      {
        "ZREVRANGEBYSCORE",
        key,
        max,
        min,
        "WITHSCORES"
      }) : this.OperateStringsFromServer(new string[4]
      {
        "ZREVRANGEBYSCORE",
        key,
        max,
        min
      });
    }

    /// <summary>
    /// 返回有序集 key 中成员 member 的排名。其中有序集成员按 score 值递减(从大到小)排序。排名以 0 为底，也就是说，score 值最大的成员排名为 0 。
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="member">成员 member 的名称</param>
    /// <returns>如果 member 是有序集 key 的成员，返回 member 的排名。如果 member 不是有序集 key 的成员，返回 nil 。</returns>
    public OperateResult<int> ZSetReverseRank(string key, string member) => this.OperateNumberFromServer(new string[3]
    {
      "ZREVRANK",
      key,
      member
    });

    /// <summary>
    /// 返回有序集 key 中，成员 member 的 score 值。如果 member 元素不是有序集 key 的成员，或 key 不存在，返回 nil 。
    /// </summary>
    /// <param name="key">有序集合的关键字</param>
    /// <param name="member">成员的名称</param>
    /// <returns>member 成员的 score 值，以字符串形式表示。</returns>
    public OperateResult<string> ZSetScore(string key, string member) => this.OperateStringFromServer(new string[3]
    {
      "ZSCORE",
      key,
      member
    });

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetAdd(System.String,System.String,System.Double)" />
    public async Task<OperateResult<int>> ZSetAddAsync(
      string key,
      string member,
      double score)
    {
      OperateResult<int> operateResult = await this.ZSetAddAsync(key, new string[1]
      {
        member
      }, new double[1]{ score });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetAdd(System.String,System.String[],System.Double[])" />
    public async Task<OperateResult<int>> ZSetAddAsync(
      string key,
      string[] members,
      double[] scores)
    {
      if (members.Length != scores.Length)
        throw new Exception(StringResources.Language.TwoParametersLengthIsNotSame);
      List<string> lists = new List<string>();
      lists.Add("ZADD");
      lists.Add(key);
      for (int i = 0; i < members.Length; ++i)
      {
        lists.Add(scores[i].ToString());
        lists.Add(members[i]);
      }
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(lists.ToArray());
      lists = (List<string>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetCard(System.String)" />
    public async Task<OperateResult<int>> ZSetCardAsync(string key)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[2]
      {
        "ZCARD",
        key
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetCount(System.String,System.Double,System.Double)" />
    public async Task<OperateResult<int>> ZSetCountAsync(
      string key,
      double min,
      double max)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[4]
      {
        "ZCOUNT",
        key,
        min.ToString(),
        max.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetIncreaseBy(System.String,System.String,System.Double)" />
    public async Task<OperateResult<string>> ZSetIncreaseByAsync(
      string key,
      string member,
      double increment)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[4]
      {
        "ZINCRBY",
        key,
        increment.ToString(),
        member
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetRange(System.String,System.Int32,System.Int32,System.Boolean)" />
    public async Task<OperateResult<string[]>> ZSetRangeAsync(
      string key,
      int start,
      int stop,
      bool withScore = false)
    {
      if (withScore)
      {
        OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[5]
        {
          "ZRANGE",
          key,
          start.ToString(),
          stop.ToString(),
          "WITHSCORES"
        });
        return operateResult;
      }
      OperateResult<string[]> operateResult1 = await this.OperateStringsFromServerAsync(new string[4]
      {
        "ZRANGE",
        key,
        start.ToString(),
        stop.ToString()
      });
      return operateResult1;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetRangeByScore(System.String,System.String,System.String,System.Boolean)" />
    public async Task<OperateResult<string[]>> ZSetRangeByScoreAsync(
      string key,
      string min,
      string max,
      bool withScore = false)
    {
      if (withScore)
      {
        OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[5]
        {
          "ZRANGEBYSCORE",
          key,
          min,
          max,
          "WITHSCORES"
        });
        return operateResult;
      }
      OperateResult<string[]> operateResult1 = await this.OperateStringsFromServerAsync(new string[4]
      {
        "ZRANGEBYSCORE",
        key,
        min,
        max
      });
      return operateResult1;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetRank(System.String,System.String)" />
    public async Task<OperateResult<int>> ZSetRankAsync(string key, string member)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[3]
      {
        "ZRANK",
        key,
        member
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetRemove(System.String,System.String)" />
    public async Task<OperateResult<int>> ZSetRemoveAsync(
      string key,
      string member)
    {
      OperateResult<int> operateResult = await this.ZSetRemoveAsync(key, new string[1]
      {
        member
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetRemove(System.String,System.String[])" />
    public async Task<OperateResult<int>> ZSetRemoveAsync(
      string key,
      string[] members)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(SoftBasic.SpliceStringArray("ZREM", key, members));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetRemoveRangeByRank(System.String,System.Int32,System.Int32)" />
    public async Task<OperateResult<int>> ZSetRemoveRangeByRankAsync(
      string key,
      int start,
      int stop)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[4]
      {
        "ZREMRANGEBYRANK",
        key,
        start.ToString(),
        stop.ToString()
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetRemoveRangeByScore(System.String,System.String,System.String)" />
    public async Task<OperateResult<int>> ZSetRemoveRangeByScoreAsync(
      string key,
      string min,
      string max)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[4]
      {
        "ZREMRANGEBYSCORE",
        key,
        min,
        max
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetReverseRange(System.String,System.Int32,System.Int32,System.Boolean)" />
    public async Task<OperateResult<string[]>> ZSetReverseRangeAsync(
      string key,
      int start,
      int stop,
      bool withScore = false)
    {
      if (withScore)
      {
        OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[5]
        {
          "ZREVRANGE",
          key,
          start.ToString(),
          stop.ToString(),
          "WITHSCORES"
        });
        return operateResult;
      }
      OperateResult<string[]> operateResult1 = await this.OperateStringsFromServerAsync(new string[4]
      {
        "ZREVRANGE",
        key,
        start.ToString(),
        stop.ToString()
      });
      return operateResult1;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetReverseRangeByScore(System.String,System.String,System.String,System.Boolean)" />
    public async Task<OperateResult<string[]>> ZSetReverseRangeByScoreAsync(
      string key,
      string max,
      string min,
      bool withScore = false)
    {
      if (withScore)
      {
        OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[5]
        {
          "ZREVRANGEBYSCORE",
          key,
          max,
          min,
          "WITHSCORES"
        });
        return operateResult;
      }
      OperateResult<string[]> operateResult1 = await this.OperateStringsFromServerAsync(new string[4]
      {
        "ZREVRANGEBYSCORE",
        key,
        max,
        min
      });
      return operateResult1;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetReverseRank(System.String,System.String)" />
    public async Task<OperateResult<int>> ZSetReverseRankAsync(
      string key,
      string member)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[3]
      {
        "ZREVRANK",
        key,
        member
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ZSetScore(System.String,System.String)" />
    public async Task<OperateResult<string>> ZSetScoreAsync(
      string key,
      string member)
    {
      OperateResult<string> operateResult = await this.OperateStringFromServerAsync(new string[3]
      {
        "ZSCORE",
        key,
        member
      });
      return operateResult;
    }

    /// <summary>
    /// 从设备里读取支持Est特性的数据内容，
    /// 该特性为<see cref="T:EstCommunication.Reflection.EstRedisKeyAttribute" />，<see cref="T:EstCommunication.Reflection.EstRedisListItemAttribute" />，
    /// <see cref="T:EstCommunication.Reflection.EstRedisListAttribute" />，<see cref="T:EstCommunication.Reflection.EstRedisHashFieldAttribute" />
    /// 详细参考代码示例的操作说明。
    /// </summary>
    /// <typeparam name="T">自定义的数据类型对象</typeparam>
    /// <returns>包含是否成功的结果对象</returns>
    /// <example>
    /// 我们来说明下这个方法到底是怎么用的，当我们需要读取redis好几个数据的时候，我们很可能写如下的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Enthernet\RedisSample.cs" region="Sample1" title="基础的使用" />
    /// 总的来说，当读取的数据种类比较多的时候，读取的关键字比较多的时候，处理起来就比较的麻烦，此处推荐一个全新的写法，为了更好的对比，我们假设实现一种需求
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Enthernet\RedisSample.cs" region="Sample2" title="同等代码" />
    /// 为此我们只需要实现一个特性类即可。代码如下：(注意，实际是很灵活的，类型都是自动转换的)
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Enthernet\RedisSample.cs" region="SampleClass" title="数据类" />
    /// 当然了，异步也是一样的，异步的代码就不重复介绍了。
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Enthernet\RedisSample.cs" region="Sample3" title="异步示例" />
    /// </example>
    public OperateResult<T> Read<T>() where T : class, new() => EstReflectionHelper.Read<T>(this);

    /// <summary>
    /// 从设备里写入支持Est特性的数据内容，
    /// 该特性为<see cref="T:EstCommunication.Reflection.EstRedisKeyAttribute" /> ，<see cref="T:EstCommunication.Reflection.EstRedisHashFieldAttribute" />
    /// 需要注意的是写入并不支持<see cref="T:EstCommunication.Reflection.EstRedisListAttribute" />，<see cref="T:EstCommunication.Reflection.EstRedisListItemAttribute" />特性，详细参考代码示例的操作说明。
    /// </summary>
    /// <typeparam name="T">自定义的数据类型对象</typeparam>
    /// <param name="data">等待写入的数据参数</param>
    /// <returns>包含是否成功的结果对象</returns>
    /// <example>
    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.Read``1" path="example" />
    /// </example>
    public OperateResult Write<T>(T data) where T : class, new() => EstReflectionHelper.Write<T>(data, this);

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.Read``1" />
    public async Task<OperateResult<T>> ReadAsync<T>() where T : class, new()
    {
      OperateResult<T> operateResult = await EstReflectionHelper.ReadAsync<T>(this);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.Write``1(``0)" />
    public async Task<OperateResult> WriteAsync<T>(T data) where T : class, new()
    {
      OperateResult operateResult = await EstReflectionHelper.WriteAsync<T>(data, this);
      return operateResult;
    }

    /// <summary>
    /// SAVE 命令执行一个同步保存操作，将当前 Redis 实例的所有数据快照(snapshot)以 RDB 文件的形式保存到硬盘。
    /// </summary>
    /// <returns>保存成功时返回 OK 。</returns>
    public OperateResult Save() => (OperateResult) this.OperateStatusFromServer(new string[1]
    {
      "SAVE"
    });

    /// <summary>
    /// 在后台异步(Asynchronously)保存当前数据库的数据到磁盘。
    /// BGSAVE 命令执行之后立即返回 OK ，然后 Redis fork 出一个新子进程，原来的 Redis 进程(父进程)继续处理客户端请求，而子进程则负责将数据保存到磁盘，然后退出。
    /// </summary>
    /// <returns>反馈信息。</returns>
    public OperateResult SaveAsync() => (OperateResult) this.OperateStatusFromServer(new string[1]
    {
      "BGSAVE"
    });

    /// <summary>获取服务器的时间戳信息，可用于本地时间的数据同步问题</summary>
    /// <returns>带有服务器时间的结果对象</returns>
    public OperateResult<DateTime> ReadServerTime()
    {
      OperateResult<string[]> operateResult = this.OperateStringsFromServer(new string[1]
      {
        "TIME"
      });
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<DateTime>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<DateTime>(new DateTime(1970, 1, 1, 8, 0, 0).AddSeconds((double) long.Parse(operateResult.Content[0])));
    }

    /// <summary>向服务器进行PING的操作，服务器会返回PONG操作</summary>
    /// <returns>是否成功</returns>
    public OperateResult Ping() => (OperateResult) this.OperateStatusFromServer(new string[1]
    {
      "PING"
    });

    /// <summary>返回当前数据库的 key 的数量。</summary>
    /// <returns>当前数据库的 key 的数量。</returns>
    public OperateResult<long> DBSize() => this.OperateLongNumberFromServer(new string[1]
    {
      "DBSIZE"
    });

    /// <summary>清空当前的数据库的key信息</summary>
    /// <returns>总是返回 OK 。</returns>
    public OperateResult FlushDB() => (OperateResult) this.OperateStatusFromServer(new string[1]
    {
      "FLUSHDB"
    });

    /// <summary>修改Redis的密码信息，如果不需要密码，则传入空字符串即可</summary>
    /// <param name="password">密码信息</param>
    /// <returns>是否更新了密码信息</returns>
    public OperateResult ChangePassword(string password) => (OperateResult) this.OperateStatusFromServer(new string[4]
    {
      "CONFIG",
      "SET",
      "requirepass",
      password
    });

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ReadServerTime" />
    public async Task<OperateResult<DateTime>> ReadServerTimeAsync()
    {
      OperateResult<string[]> times = await this.OperateStringsFromServerAsync(new string[1]
      {
        "TIME"
      });
      if (!times.IsSuccess)
        return OperateResult.CreateFailedResult<DateTime>((OperateResult) times);
      long timeTick = long.Parse(times.Content[0]);
      DateTime dateTime = new DateTime(1970, 1, 1, 8, 0, 0).AddSeconds((double) timeTick);
      return OperateResult.CreateSuccessResult<DateTime>(dateTime);
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.Ping" />
    public async Task<OperateResult> PingAsync()
    {
      OperateResult<string[]> operateResult = await this.OperateStringsFromServerAsync(new string[1]
      {
        "PING"
      });
      return (OperateResult) operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.DBSize" />
    public async Task<OperateResult<long>> DBSizeAsync()
    {
      OperateResult<long> operateResult = await this.OperateLongNumberFromServerAsync(new string[1]
      {
        "DBSIZE"
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.FlushDB" />
    public async Task<OperateResult> FlushDBAsync()
    {
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(new string[1]
      {
        "FLUSHDB"
      });
      return (OperateResult) operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.ChangePassword(System.String)" />
    public async Task<OperateResult> ChangePasswordAsync(string password)
    {
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(new string[4]
      {
        "CONFIG",
        "SET",
        "requirepass",
        password
      });
      return (OperateResult) operateResult;
    }

    /// <summary>将信息 message 发送到指定的频道 channel，返回接收到信息 message 的订阅者数量。</summary>
    /// <param name="channel">频道，和关键字不是一回事</param>
    /// <param name="message">消息</param>
    /// <returns>接收到信息 message 的订阅者数量。</returns>
    public OperateResult<int> Publish(string channel, string message) => this.OperateNumberFromServer(new string[3]
    {
      "PUBLISH",
      channel,
      message
    });

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.Publish(System.String,System.String)" />
    public async Task<OperateResult<int>> PublishAsync(
      string channel,
      string message)
    {
      OperateResult<int> operateResult = await this.OperateNumberFromServerAsync(new string[3]
      {
        "PUBLISH",
        channel,
        message
      });
      return operateResult;
    }

    /// <summary>
    /// 切换到指定的数据库，数据库索引号 index 用数字值指定，以 0 作为起始索引值。默认使用 0 号数据库。
    /// </summary>
    /// <param name="db">索引值</param>
    /// <returns>是否切换成功</returns>
    public OperateResult SelectDB(int db)
    {
      OperateResult operateResult = (OperateResult) this.OperateStatusFromServer(new string[2]
      {
        "SELECT",
        db.ToString()
      });
      if (operateResult.IsSuccess)
        this.dbBlock = db;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SelectDB(System.Int32)" />
    public async Task<OperateResult> SelectDBAsync(int db)
    {
      OperateResult<string> operateResult = await this.OperateStatusFromServerAsync(new string[2]
      {
        "SELECT",
        db.ToString()
      });
      OperateResult select = (OperateResult) operateResult;
      operateResult = (OperateResult<string>) null;
      if (select.IsSuccess)
        this.dbBlock = db;
      OperateResult operateResult1 = select;
      select = (OperateResult) null;
      return operateResult1;
    }

    /// <summary>当接收到Redis订阅的信息的时候触发</summary>
    public event RedisClient.RedisMessageReceiveDelegate OnRedisMessageReceived;

    private RedisSubscribe RedisSubscribeInitialize()
    {
      RedisSubscribe redisSubscribe = new RedisSubscribe(this.IpAddress, this.Port);
      redisSubscribe.Password = this.password;
      redisSubscribe.OnRedisMessageReceived += (RedisSubscribe.RedisMessageReceiveDelegate) ((topic, message) =>
      {
        RedisClient.RedisMessageReceiveDelegate redisMessageReceived = this.OnRedisMessageReceived;
        if (redisMessageReceived == null)
          return;
        redisMessageReceived(topic, message);
      });
      return redisSubscribe;
    }

    /// <summary>
    /// 从Redis服务器订阅一个或多个主题信息<br />
    /// Subscribe to one or more topics from the redis server
    /// </summary>
    /// <param name="topic">主题信息</param>
    /// <returns>订阅结果</returns>
    public OperateResult SubscribeMessage(string topic) => this.SubscribeMessage(new string[1]
    {
      topic
    });

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.SubscribeMessage(System.String)" />
    public OperateResult SubscribeMessage(string[] topics) => this.redisSubscribe.Value.SubscribeMessage(topics);

    /// <summary>
    /// 取消订阅一个或多个主题信息，取消之后，当前的订阅数据就不在接收到。<br />
    /// Unsubscribe from multiple topic information. After cancellation, the current subscription data will not be received.
    /// </summary>
    /// <param name="topic">主题信息</param>
    /// <returns>取消订阅结果</returns>
    public OperateResult UnSubscribeMessage(string topic) => this.UnSubscribeMessage(new string[1]
    {
      topic
    });

    /// <inheritdoc cref="M:EstCommunication.Enthernet.Redis.RedisClient.UnSubscribeMessage(System.String)" />
    public OperateResult UnSubscribeMessage(string[] topics) => this.redisSubscribe.Value.UnSubscribeMessage(topics);

    /// <inheritdoc />
    public override string ToString() => string.Format("RedisClient[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>
    /// 当接收到Redis订阅的信息的时候触发<br />
    /// Triggered when receiving Redis subscription information
    /// </summary>
    /// <param name="topic">主题信息</param>
    /// <param name="message">数据信息</param>
    public delegate void RedisMessageReceiveDelegate(string topic, string message);
  }
}
