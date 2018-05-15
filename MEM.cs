using System;
using SME;
using System.Threading.Tasks;

namespace RISCV
{
    [InitializedBus]
    public interface ReadData : IBus
    {
        uint data { get; set; }
    }

    [InitializedBus]
    public interface MemOut : IBus
    {
        uint data { get; set; }
    }

    public class Memory : SimpleProcess
    {
        [InputBus]
        MemWrite memwrite;
        [InputBus]
        MemRead memread;
        [InputBus]
        ALUResult addr;
        [InputBus]
        Register2 write;

        [OutputBus]
        ReadData output;

        uint[] mem = new uint[64];

        protected override void OnTick()
        {
            uint add = addr.val >> 2;
            if (memread.flg)
            {
                output.data = mem[add];
            }
            else if (memwrite.flg)
            {
                mem[add] = write.val;
                output.data = 0;
            }
            else
            {
                output.data = 0;
            }
        }
    }

    public class MemMux : SimpleProcess
    {
        [InputBus]
        ReadData mem;
        [InputBus]
        ALUResult aluresult;
        [InputBus]
        MemRead memtoreg;

        [OutputBus]
        MemOut output;

        protected override void OnTick()
        {
            output.data = memtoreg.flg ? mem.data : aluresult.val;
        }
    }
}

