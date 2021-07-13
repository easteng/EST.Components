/**********************************************************************
*******命名空间： ESTCore.Test.Console
*******类 名 称： TestModule
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/13/2021 10:41:17 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Silky.Lms.Core.Modularity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Test.Console
{
   [DependsOn(typeof(ESTRedisCacheModule))]
    public class TestModule
    {
         
    }
}
