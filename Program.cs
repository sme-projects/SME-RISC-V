using System;
using SME;

namespace RISCV
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            /*new Simulation()
                //.BuildCSVFile()
                //.BuildGraph()
                //.BuildVHDL()
                .Run(typeof(MainClass).Assembly);*/
            new Simulation().Run(Loader.StartProcesses(typeof(MainClass).Assembly, true));
        }
    }
}
