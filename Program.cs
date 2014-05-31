using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX.Windows;
using System.Diagnostics;
using DirectX_01;

namespace WindowsFormsApplication2
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Form1 form = new Form1();
            MessagePump.Run(form,form.Render);
        }
    }
}
