/**********************************************************************
*******命名空间： ESTCore.Message.Message
*******类 名 称： BaseMessage
*******类 说 明： 基础消息体
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/23/2021 4:20:41 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Message
{
    /// <summary>
    ///  基础消息体
    /// </summary>
    public class BaseMessage
    {
        /// <summary>
        ///  由二进制解析消息体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public static BaseMessage ResolveMessage(byte[] data)
        {
            var  message=new BaseMessage();
            message.Data = data;
            message.GetTopic();
            return message;
        }

        /// <summary>
        /// 通过消息实体创建一条公用的消息体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static BaseMessage CreateMessage<T>(T message) where T : AbstractMessage
        {
            var baseMsg = new BaseMessage();
            var msgStr=JsonConvert.SerializeObject(message);
            var data=Encoding.Default.GetBytes(msgStr);
            baseMsg.Data = data;
            baseMsg.GetTopic(); 
            return baseMsg; 
        }

        /// <summary>
        /// 消息主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 元数据
        /// </summary>
        public byte[] Data { get; set; }

        public override string ToString()=> Encoding.Default.GetString(Data);

        /// <summary>
        /// 获取消息内容
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public TMessage GetMessage<TMessage>() where TMessage:AbstractMessage
        {
            try
            {
                var mstr = Encoding.Default.GetString(this.Data);
                var data = JsonConvert.DeserializeObject<TMessage>(mstr);
                return data;
            }
            catch (Exception ex)
            {
                return default(TMessage);
            }
        }

        private void GetTopic()
        {
            var data = JsonConvert.DeserializeObject<InnerMessage>(GetDataStr());
            this.Topic = data.Topic;
        }

        private string GetDataStr() => Encoding.Default.GetString(this.Data);
        class InnerMessage
        {
            public string Topic { get; set; }
        }
    }
}