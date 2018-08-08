using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OS_module_cs.nvidia
{
    // Handle error loading nvGpuLoad_x86.dll (e.g. missing Microsoft Visual C++ 2015 Redistributable  (x86) or missing dll)
    class NvGpuLoad
    {
        // dll with code from http://eliang.blogspot.de/2011/05/getting-nvidia-gpu-usage-in-c.html
        [DllImport("nvidia/nvGpuLoad_x86.dll")]
        public static extern int getGpuLoad();
        internal static int GetGpuLoad()
        {
            int a = new int();
            a = getGpuLoad();
            return a;
        }
    }
}
