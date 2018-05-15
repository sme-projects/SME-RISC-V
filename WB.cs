using System;
using SME;

namespace RISCV
{
    [InitializedBus]
    public interface MemToReg : IBus
    {
        bool flg { get; set; }
    }

    [InitializedBus]
    public interface BufIn : IBus
    {
        uint data { get; set; }
    }

    [ClockedProcess]
    public class WriteBuffer : SimpleProcess
    {
        [InputBus]
        RegWriteAddr addrIn;
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
