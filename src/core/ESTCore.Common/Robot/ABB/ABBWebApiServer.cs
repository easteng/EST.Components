// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Robot.ABB.ABBWebApiServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Enthernet;
using ESTCore.Common.Reflection;

using System;
using System.Net;
using System.Text;

namespace ESTCore.Common.Robot.ABB
{
    /// <summary>
    /// <b>[商业授权]</b> ABB机器人的虚拟服务器，基于WebApi协议构建，可用于读取一些数据信息<br />
    /// <b>[Authorization]</b>The virtual server of ABB robot, built based on the WebApi protocol, can be used to read some data information
    /// </summary>
    /// <remarks>
    /// 本虚拟服务器实例化之后，就可以启动了，需要注意的是，程序需要管理员模式运行，否则启动服务的时候会报错，显示拒绝当前的操作。
    /// 支持和<see cref="T:ESTCore.Common.Robot.ABB.ABBWebApiClient" />进行测试通信。本服务器的运行需要商业授权支持，否则只能运行24小时。
    /// </remarks>
    public class ABBWebApiServer : HttpServer
    {
        private string userName = "Default User";
        private string password = "robotics";

        /// <summary>
        /// 设置用户的登录信息，用户名和密码信息<br />
        /// Set user login information, user name and password information
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="password">密码</param>
        [EstMqttApi]
        public void SetLoginAccount(string name, string password)
        {
            this.userName = name;
            this.password = password;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.HttpServer.HandleRequest(System.Net.HttpListenerRequest,System.Net.HttpListenerResponse,System.String)" />
        protected override string HandleRequest(
          HttpListenerRequest request,
          HttpListenerResponse response,
          string data)
        {
            if (!ESTCore.Common.Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return StringResources.Language.InsufficientPrivileges;
            string[] values = request.Headers.GetValues("Authorization");
            if (values == null || values.Length < 1 || string.IsNullOrEmpty(values[0]))
            {
                response.StatusCode = 401;
                response.AddHeader("WWW-Authenticate", "Basic realm=\"Secure Area\"");
                return "";
            }
            string[] strArray = Encoding.UTF8.GetString(Convert.FromBase64String(values[0].Split(new char[1]
            {
        ' '
            }, StringSplitOptions.RemoveEmptyEntries)[1])).Split(new char[1]
            {
        ':'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length < 2)
            {
                response.StatusCode = 401;
                response.AddHeader("WWW-Authenticate", "Basic realm=\"Secure Area\"");
                return "";
            }
            if (this.userName != strArray[0] || this.password != strArray[1])
            {
                response.StatusCode = 401;
                response.AddHeader("WWW-Authenticate", "Basic realm=\"Secure Area\"");
                this.LogNet?.WriteDebug("Account Check Failed:" + strArray[0] + ":" + strArray[1]);
                return "";
            }
            if (request.RawUrl == "/rw/motionsystem/mechunits/ROB_1/jointtarget")
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<html>\r\n<head>\r\n<title>motionsystem</title>\r\n<base href= \"http://localhost/rw/motionsystem/mechunits/ROB_1/jointtarget/\" />\r\n</head>\r\n<body>\r\n<div class=\"state\">\r\n<ul>\r\n<li class=\"ms-jointtarget\" title=\"ROB_1\">\r\n<span class=\"rax_1\">0</span>\r\n<span class=\"rax_2\">0</span>\r\n<span class=\"rax_3\">0</span>\r\n<span class=\"rax_4\">0</span>\r\n<span class=\"rax_5\">0</span>\r\n<span class=\"rax_6\">0</span>\r\n<span class=\"eax_a\">0</span>\r\n<span class=\"eax_b\">0</span>\r\n<span class=\"eax_c\">0</span>\r\n<span class=\"eax_d\">0</span>\r\n<span class=\"eax_e\">0</span>\r\n<span class=\"eax_f\">0</span>\r\n</li>\r\n</ul>\r\n</div>\r\n</body>\r\n</html>";
            if (request.RawUrl == "/rw/motionsystem/errorstate")
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<html>\r\n\t<head>\r\n\t\t<title>motionsystem</title>\r\n\t\t<base href= \"http://localhost/rw/motionsystem/\" />\r\n\t</head>\r\n\t<body>\r\n\t\t<div class=\"state\">\r\n\t\t\t<a href= \"errorstate\" rel=\"self\"/>\r\n\t\t\t<ul>\r\n\t\t\t\t<li class=\"ms-errorstate\" title=\"errorstate\">\r\n\t\t\t\t\t<span class=\"err-state\">HPJ_OK</span>\r\n\t\t\t\t\t<span class=\"err-count\">0</span>\r\n\t\t\t\t</li>\r\n\t\t\t</ul>\r\n\t\t</div>\r\n\t</body>\r\n</html>";
            if (request.RawUrl == "/rw/motionsystem/mechunits/ROB_1/robtarget")
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<html>\r\n<head>\r\n<title>motionsystem</title>\r\n<base href= \"http://localhost/rw/motionsystem/mechunits/ROB_1/robtarget/\" />\r\n</head>\r\n<body>\r\n<div class=\"state\">\r\n<ul>\r\n<li class=\"ms-robtargets\" title=\"ROB_1\">\r\n<span class=\"x\">515</span>\r\n<span class=\"y\">0</span>\r\n<span class=\"z\">712</span>\r\n<span class=\"q1\">0.7071068</span>\r\n<span class=\"q2\">0</span>\r\n<span class=\"q3\">0.7071068</span>\r\n<span class=\"q4\">0</span>\r\n<span class=\"cf1\">0</span>\r\n<span class=\"cf4\">0</span>\r\n<span class=\"cf6\">0</span>\r\n<span class=\"cfx\">0</span>\r\n<span class=\"eax_a\">8.999999e+009</span>\r\n<span class=\"eax_b\">8.999999e+009</span>\r\n<span class=\"eax_c\">8.999999e+009</span>\r\n<span class=\"eax_d\">8.999999e+009</span>\r\n<span class=\"eax_e\">8.999999e+009</span>\r\n<span class=\"eax_f\">8.999999e+009</span>\r\n</li>\r\n</ul>\r\n</div>\r\n</body>\r\n</html>";
            if (request.RawUrl == "/rw/system")
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n<title>system</title>\r\n<base href=\"http://localhost/rw/system/\"/>\r\n</head>\r\n<body>\r\n<div class=\"state\">\r\n<a href=\"\" rel=\"self\"/>\r\n<ul>\r\n<li class=\"sys-system-li\" title=\"system\">\r\n<span class=\"major\">6</span>\r\n<span class=\"minor\">05</span>\r\n<span class=\"build\">0047</span>\r\n<span class=\"revision\">00</span>\r\n<span class=\"sub-revision\">00</span>\r\n<span class=\"buildtag\">Internal build 0047</span>\r\n<span class=\"robapi-compatibility-revision\">0</span>\r\n<span class=\"title\">RobotWare</span>\r\n<span class=\"type\">RobotWare</span>\r\n<span class=\"description\">Controller Software</span>\r\n<span class=\"date\">2016-11-25</span>\r\n<span class=\"mctimestamp\">#20:14:43/nov 24 2016#</span>\r\n<span class=\"name\">6.05.0047</span>\r\n<span class=\"rwversion\">6.05.0047</span>\r\n<span class=\"sysid\">{14C798C8-3C47-4E4C-8CD4-040EC9483A10}</span>\r\n<span class=\"starttm\">2016-12-09 T 17:47:42</span>\r\n<span class=\"rwversionname\">6.05.00.00 Internal build 0047</span>\r\n</li>\r\n<li class=\"sys-options-li\" title=\"options\">\r\n<a href=\"options\" rel=\"self\" />\r\n<ul>\r\n<li class=\"sys-option-li\" title=\"0\">\r\n<span class=\"option\">RobotWare Base</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"1\">\r\n<span class=\"option\">English</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"2\">\r\n<span class=\"option\">614-1 FTP and NFS client</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"3\">\r\n<span class=\"option\">616-1 PC Interface</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"4\">\r\n<span class=\"option\">617-1 FlexPendant Interface</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"5\">\r\n<span class=\"option\">623-1 Multitasking</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"6\">\r\n<span class=\"option\">608-1 World Zones</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"7\">\r\n<span class=\"option\">Motor Commutation</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"8\">\r\n<span class=\"option\">Service Info System</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"9\">\r\n<span class=\"option\">Calib. Pendelum RAPID</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"10\">\r\n<span class=\"option\">Drive System 120/140/260/360/1200/1400/1520/1600</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"11\">\r\n<span class=\"option\">IRB 140-5/0.8 Type A</span>\r\n</li>\r\n<li class=\"sys-option-li\" title=\"12\">\r\n<span class=\"option\">810-2 SafeMove</span>\r\n</li>\r\n<li class=\"sys-energy-li\" title=\"energy\">\r\n<a href=\"energy\" rel=\"self\"/>\r\n</li>\r\n<li class=\"sys-license-li\" title=\"license\">\r\n<a href=\"license\" rel=\"self\"/>\r\n</li>\r\n<li class=\"sys-products-li\" title=\"products\">\r\n<a href=\"products\" rel=\"self\"/>\r\n</li>\r\n</ul>\r\n</li>\r\n</ul>\r\n</div>\r\n</body>\r\n</html>";
            if (request.RawUrl == "/rw/panel/speedratio")
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n<title>panel</title>\r\n<base href= \"http://localhost/rw/panel/speedratio/\" />\r\n</head>\r\n<body>\r\n<div class=\"state\">\r\n<a href= \"\" rel=\"self\"/>\r\n<a href= \"?action=show\" rel=\"action\"/>\r\n<ul>\r\n<li class=\"pnl-speedratio\" title=\"speedratio\">\r\n<span class=\"speedratio\">100</span>\r\n</li>\r\n</ul>\r\n</div>\r\n</body>\r\n</html>";
            if (request.RawUrl == "/rw/panel/opmode")
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n<title>panel</title>\r\n<base href= \"http://localhost/rw/panel/opmode/\" />\r\n</head>\r\n<body>\r\n<div class=\"state\">\r\n<a href= \"\" rel=\"self\"/>\r\n<a href= \"?action=show\" rel=\"action\"/>\r\n<ul>\r\n<li class=\"pnl-opmode\" title=\"opmode\">\r\n<span class=\"opmode\">MANR</span>\r\n</li>\r\n</ul>\r\n</div>\r\n</body>\r\n</html>";
            if (request.RawUrl == "/rw/panel/ctrlstate")
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n<title>panel</title>\r\n<base href= \"http://localhost/rw/panel/ctrlstate/\" />\r\n</head>\r\n<body>\r\n<div class=\"state\">\r\n<a href= \"\" rel=\"self\"/>\r\n<a href= \"?action=show\" rel=\"action\"/>\r\n<ul>\r\n<li class=\"pnl-ctrlstate\" title=\"ctrlstate\">\r\n<span class=\"ctrlstate\">motoron</span>\r\n</li>\r\n</ul>\r\n</div>\r\n</body>\r\n</html>";
            if (request.RawUrl == "/rw/iosystem/devices/D652_10" || request.RawUrl == "/rw/iosystem/devices/BK5250")
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">\r\n\t<head><title>io</title>\r\n\t<base href=\"http://localhost/rw/iosystem/\"/>\r\n</head>\r\n<body>\r\n<div class=\"state\">\r\n\t<a href=\"devices/Local/PANEL\" rel=\"self\"></a>\r\n\t<a href= \"devices/Local/PANEL?action=show\" rel=\"action\"></a>\r\n\t<ul>\r\n\t<li class=\"ios-device\" title=\"Local/PANEL\">\r\n\t\t<a href=\"networks/Local\"  rel=\"network\"/>\r\n\t\t<span class=\"name\">PANEL</span>\r\n\t\t<span class=\"lstate\">enabled</span>\r\n\t\t<span class=\"pstate\">running</span>\r\n\t\t<span class=\"address\">-</span>\r\n\t\t<span class=\"indata\">1FFFE063</span>\r\n\t\t<span class=\"inmask\">FFFFFFFF</span>\r\n\t\t<span class=\"outdata\">0000000E</span>\r\n\t\t<span class=\"outmask\">FFFFFFFF</span>\r\n\t</li>\r\n\t</ul>\r\n</div>\r\n</body>\r\n</html>";
            if (request.RawUrl == "/rw/elog/0?lang=zh&amp;resource=title")
                return "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n  <head>\r\n\t<title>Elog</title>\r\n\t<base href=\"http://localhost/rw/elog/\"/>\r\n  </head>\r\n  <body>\r\n\r\n\t<div class=\"state\">      \r\n\t  <a href=\"0\" rel=\"self\"></a>    \r\n\t  <a href= \"0?action=show\" rel=\"action\"/>\r\n\t  <ul>                        \r\n\t\t<li class=\"elog-message-li\" title=\"/rw/elog/0/5\">\r\n\t\t\t<a href=\"0/5\" rel=\"self\"></a>          \r\n\t\t\t<span class=\"msgtype\">1</span>\r\n\t\t\t<span class=\"code\">10015</span>\r\n\t\t\t<span class=\"src-name\">MC0</span>\r\n\t\t\t<span class=\"tstamp\">2013-09-08 T 11:22:09</span>\r\n\t\t\t<span class=\"argc\">0</span> \r\n\t\t</li>\r\n\t\t<li class=\"elog-message-li\" title=\"/rw/elog/0/4\">\r\n\t\t\t<a href=\"0/4\" rel=\"self\"></a>          \r\n\t\t\t<span class=\"msgtype\">1</span>\r\n\t\t\t<span class=\"code\">10013</span>\r\n\t\t\t<span class=\"src-name\">MC0</span>\r\n\t\t\t<span class=\"tstamp\">2013-09-08 T 11:22:09</span>\r\n\t\t\t<span class=\"argc\">0</span> \r\n\t\t</li>\r\n\t\t<li class=\"elog-message-li\" title=\"/rw/elog/0/3\">\r\n\t\t\t<a href=\"0/3\" rel=\"self\"></a>          \r\n\t\t\t<span class=\"msg-type\">1</span>\r\n\t\t\t<span class=\"code\">10002</span>\r\n\t\t\t<span class=\"src-name\">MC0</span>\r\n\t\t\t<span class=\"tstamp\">2013-09-08 T 11:22:07</span>\r\n\t\t\t<span class=\"argc\">2</span> \r\n\t\t\t<span class=\"arg1\" type=\"string\">TRAFO</span>\r\n\t\t\t<span class=\"arg2\" type=\"string\">trafo_dm1</span>\r\n\t\t</li>\r\n\t  </ul>       \r\n\t</div>\r\n  </body>\r\n</html>";
            if (request.RawUrl == "/rw/iosystem/signals/Local/DRV_1/DRV1K1")
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<html\r\n  xmlns=\"http://www.w3.org/1999/xhtml\">\r\n  <head>\r\n\t<title>io</title>\r\n\t<base href=\"http://localhost/rw/iosystem/\"/>\r\n  </head>\r\n  <body>\r\n\t<div class=\"state\">\r\n\t  <a href=\"signals/Local/DRV_1/DRV1K1\" rel=\"self\"></a>\r\n\t  <a href=\"signals/Local/DRV_1/DRV1K1?action=show\" rel=\"action\"></a>\r\n\t  <ul>\r\n\t\t<li class=\"ios-signal\" title=\"Local/DRV_1/DRV1K1\">\r\n\t\t  <a href=\"devices/DRV_1\" rel=\"device\"></a>\r\n\t\t  <span class=\"name\">DRV1K1</span>\r\n\t\t  <span class=\"type\">DO</span>\r\n\t\t  <span class=\"category\">safety</span>\r\n\t\t  <span class=\"lvalue\">0</span>\r\n\t\t  <span class=\"lstate\">not simulated</span>\r\n\t\t  <span class=\"unitnm\">DRV_1</span>\r\n\t\t  <span class=\"phstate\">valid</span>\r\n\t\t  <span class=\"pvalue\">0</span>\r\n\t\t  <span class=\"ltime-sec\">0</span>\r\n\t\t  <span class=\"ltime-microsec\">0</span>\r\n\t\t  <span class=\"ptime-sec\">0</span>\r\n\t\t  <span class=\"ptime-microsec\">0</span>\r\n\t\t  <span class=\"quality\">1</span>\r\n\t\t</li>\r\n\t  </ul>\r\n\t</div>\r\n  </body>\r\n</html>";
            if (request.RawUrl == "/rw/rapid/execution")
                return "    <?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n\t<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n\t<head>\r\n\t\t<title>rapid</title>\r\n\t\t<base href=\"http://localhost/rw/rapid/\"/>\r\n\t</head>\r\n\t<body>\r\n\t\t<div class=\"state\">\r\n\t\t\t<a href=\"execution\" rel=\"self\"></a>\r\n\t\t\t<a href=\"execution?action=show\" rel=\"action\"></a>\r\n\t\t\t<ul>\r\n\t\t\t\t<li class=\"rap-execution\" title=\"execution\">\r\n\t\t\t\t\t<span class=\"ctrlexecstate\">stopped</span>\r\n\t\t\t\t\t<span class=\"cycle\">forever</span>\r\n\t\t\t\t</li>\r\n\t\t\t</ul>\r\n\t\t</div>\r\n\t</body>\r\n\t</html>";
            return request.RawUrl == "/rw/rapid/tasks" ? "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n<title>rapid</title>\r\n<base href=\"http://127.0.0.1/rw/rapid/\"/>\r\n</head>\r\n<body>\r\n<div class=\"state\">\r\n<a href=\"tasks\" rel=\"self\"/>\r\n<a href= \"tasks?action=show\" rel=\"action\"/>\r\n<ul>\r\n<li class=\"rap-task-li\" title=\"T_ROB1\">\r\n<a href= \"tasks/T_ROB1\" rel=\"self\"/>\r\n<span class=\"name\">T_ROB1</span>\r\n<span class=\"type\">norm</span>\r\n<span class=\"taskstate\">link</span>\r\n<span class=\"excstate\">read</span>\r\n<span class=\"active\">On</span>\r\n<span class=\"motiontask\">TRUE</span>\r\n</li>\r\n<li class=\"rap-task-li\" title=\"T_ROB2\">\r\n<a href= \"tasks/T_ROB2\" rel=\"self\"/>\r\n<span class=\"name\">T_ROB2</span>\r\n<span class=\"type\">norm</span>\r\n<span class=\"taskstate\">link</span>\r\n<span class=\"excstate\">read</span>\r\n<span class=\"active\">On</span>\r\n<span class=\"motiontask\">TRUE</span>\r\n</li>\r\n</ul>\r\n</div>\r\n</body>\r\n</html>" : base.HandleRequest(request, response, data);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("ABBWebApiServer[{0}]", (object)this.Port);
    }
}
