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
        public BaseMessage(byte[] data)
        {
            this.Data = data;
            this.GetTopic();
        }
        public string Topic { get; set; }
        /// <summary>
        /// 元数据
        /// </summary>
        public byte[] Data { get; set; }

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
