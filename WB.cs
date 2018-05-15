using System;
using SME;

namespace RISCV
{
    [ClockedProcess]
    public class WriteBuffer : SimpleProcess
    {
        [InputBus]
        Rd addrIn;
        [InputBus]
        MemOut dataIn;
        [InputBus]
        RegWrite regwriteIn;

        [OutputBus]
        RegWriteBus regwrite;

        protected override void OnTick()
        {
            regwrite.ena = regwriteIn.flg;
            regwrite.addr = addrIn.addr;
            regwrite.data = dataIn.data;
        }
    }
}
