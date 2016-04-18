using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDT
{
    class Program
    {
        static void Main(string[] args)
        {
            var dap = new Packets.DataAckPacket(123, new List<int> { 1, 23, 12, 321, 3213, 213, 12312, 312, 12 });
            var dap2 = new Packets.DataAckPacket(dap.Dgram);
            Console.ReadKey();
        }
    }
}
