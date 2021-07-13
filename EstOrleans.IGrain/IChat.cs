/**********************************************************************
*******命名空间： EstOrleans.IGrain
*******接口名称： IChat
*******接口说明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/10/2021 3:36:25 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using Orleans;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstOrleans.IGrain
{
    public interface IChat: IGrainObserver
    {
        void ReceiveMessage(string message);
    }
}
