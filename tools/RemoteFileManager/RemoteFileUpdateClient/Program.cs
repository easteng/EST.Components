using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteFileUpdateClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var frm = new RemoteFileUpdate.UpdateForm(
                "测温客户端自动更新程序",
                "yinao",
                "client2",
                "D3A3690E-3D1A-4EC2-9F7C-1496CF5811FD",
                "1.117.85.80",
                5188
                );
            frm.ShowDialog();
        }
    }
}
