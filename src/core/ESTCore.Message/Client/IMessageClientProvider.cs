/**********************************************************************
*******命名空间： ESTCore.Message.Client
*******接口名称： IMessageClientProvider
*******接口说明： 消息客户端提供者
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/23/2021 11:31:27 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Message.Message;
using System.Threading.Tasks;

namespace ESTCore.Message.Client
{
    /// <summary>
    /// 消息客户端提供者
    /// </summary>
    public interface IMessageClientProvider
    {
        /// <summary>
        /// 发送消息给服务器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessage<T>(T message) where T : AbstractMessage;
        /// <summary>
        /// 发送命令给服务器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Command"></param>
        /// <returns></returns>
        Task SendCommand<T>(T Command) where T : AbstractCommand;
    }
}
