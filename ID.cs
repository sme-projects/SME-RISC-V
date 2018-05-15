using SME;
using SME.VHDL;

namespace RISCV
{
    [InitializedBus]
    public interface RegDst : IBus
    {
        bool flg { get; set; }
    }

    [InitializedBus]
    public interface RegWrite : IBus
    {
        bool flg { get; set; }
    }

    [InitializedBus]
    public interface LogicalImmediate : IBus
    {
        bool flg { get; set; }
    }

    [InitializedBus]
    public interface ReadA : IBus
    {
        byte addr { get; set; }
    }

    [InitializedBus]
    public interface ReadB : IBus
    {
        byte addr { get; set; }
    }

    [InitializedBus]
    public interface WriteAddr : IBus
    {
        byte val { get; set; }
    }

    [InitializedBus]
    public interface WriteData : IBus
    {
        uint data { get; set; }
    }

    [InitializedBus]
    public interface SignExtIn : IBus
    {
        short data { get; set; }
    }

    [InitializedBus]
    public interface SignExtOut : IBus
    {
        uint data { get; set; }
    }

    [InitializedBus]
    public interface OutputA : IBus
    {
        uint data { get; set; }
    }

    [InitializedBus]
    public interface OutputB : IBus
    {
        uint data { get; set; }
    }

    [InitializedBus]
    public interface ControlIn : IBus
    {
        Opcodes opcode { get; set; }
    }

    [InitializedBus]
    public interface MuxInput : IBus
    {
        byte rt { get; set; }
        byte rd { get; set; }
    }

    [InitializedBus]
    public interface MuxOutput : IBus
    {
        byte addr { get; set; }
    }

    [InitializedBus]
    public interface WriteEnabled : IBus
    {
        bool flg { get; set; }
    }

    [InitializedBus]
    public interface Shamt : IBus
    {
        byte amount { get; set; }
    }

    [InitializedBus]
    public interface Funct3 : IBus 
    {
        UInt3 val { get; set; }
    }

    [InitializedBus]
    public interface Funct7 : IBus 
    {
        UInt7 val { get; set; }
    }

    [InitializedBus]
    public interface Imm5
    {
        UInt5 val { get; set; }
    }

    [InitializedBus]
    public interface Imm7
    {
        UInt7 val { get; set; }
    }
    
    [InitializedBus]
    public interface Imm12
    {
        UInt12 val { get; set; }
    }
    
    [InitializedBus]
    public interface Imm20
    {
        UInt20 val { get; set; }
    }

    [InitializedBus]
    public interface Opcode : IBus 
    {
        Opcodes val { get; set; }
    }

    [InitializedBus]
    public interface Rd : IBus 
    {
        UInt5 index { get; set; }
    }

    [InitializedBus]
    public interface Rs1 : IBus 
    {
        UInt5 index { get; set; }
    }

    [InitializedBus]
    public interface Rs2 : IBus 
    {
        UInt5 index { get; set; }
    }

    public class Splitter : SimpleProcess
    {
        [InputBus]
        Instruction instr;

        [OutputBus]
        Funct7 funct7;
        [OutputBus]
        Rs2 rs2;
        [OutputBus]
        Rs1 rs1;
        [OutputBus]
        Funct3 funct3;
        [OutputBus]
        Rd rd;
        [OutputBus]
        Opcode opcode;
        [OutputBus]
        Imm12 imm12;
        [OutputBus]
        Imm7 imm7;
        [OutputBus]
        Imm5 imm5;
        [OutputBus]
        Imm20 imm20;

        protected override void OnTick()
        {
            uint tmp   = instr.instruction;
            funct7.val = (UInt7)  ((tmp >> 25) & 0b1111111);
            rs2.index  = (UInt5)  ((tmp >> 20) & 0b11111);
            rs1.index  = (UInt5)  ((tmp >> 15) & 0b11111);
            funct3.val = (UInt3)  ((tmp >> 12) & 0b111);
            rd.index   = (UInt5)  ((tmp >> 7)  & 0b11111);
            opcode.val = (Opcodes) (tmp        & 0b1111111);
            imm12.val  = (UInt12) ((tmp >> 20) & 0b111111111111);
            imm7.val   = (UInt7)  ((tmp >> 25) & 0b1111111);
            imm5.val   = (UInt5)  ((tmp >> 7)  & 0b11111);
            imm20.val  = (UInt20) ((tmp >> 12) & 0b11111111111111111111);
        }
    }

    public class Mux : SimpleProcess
    {
        [InputBus]
        RegDst regdst;
        [InputBus]
        MuxInput input;

        [OutputBus]
        MuxOutput write;

        protected override void OnTick()
        {
            write.addr = regdst.flg ? input.rd : input.rt;
        }
    }

    public class SignExtend : SimpleProcess
    {
        [InputBus]
        SignExtIn input;
        [InputBus]
        LogicalImmediate logIm;

        [OutputBus]
        SignExtOut output;

        protected override void OnTick()
        {
            if (logIm.flg)
                output.data = (uint)(0 | input.data);
            else
                output.data = (uint)input.data;
        }
    }

    public class Control : SimpleProcess
    {
        [InputBus]
        ControlIn input;

        [OutputBus]
        RegDst regdst;
        [OutputBus]
        Branch branch;
        [OutputBus]
        MemRead memread;
        [OutputBus]
        MemToReg memtoreg;
        [OutputBus]
        ALUOp aluop;
        [OutputBus]
        MemWrite memwrite;
        [OutputBus]
        ALUSrc alusrc;
        [OutputBus]
        RegWrite regwrite;
        [OutputBus]
        Jump jump;
        [OutputBus]
        JAL jal;
        [OutputBus]
        LogicalImmediate logIm;
        [OutputBus]
        BranchNot bne;

        protected override void OnTick()
        {
            // flag format = [BranchNot, Jump reg, Logical immediate, JAL, Jump, RegDst, ALUSrc, MemToReg, RegWrite, MemRead, MemWrite, Branch]
            short flags = 0; // nop
            ALUOpcodes alu = 0; // nop
            Opcodes code = input.opcode;

            if (code == Opcodes.Rformat) { flags = 0x048; alu = ALUOpcodes.RFormat; }
            else if (code == Opcodes.lw) { flags = 0x03C; alu = ALUOpcodes.add; }
            else if (code == Opcodes.sw) { flags = 0x022; alu = ALUOpcodes.add; }
            else if (code == Opcodes.beq) { flags = 0x001; alu = ALUOpcodes.sub; }
            else if (code == Opcodes.addi) { flags = 0x028; alu = ALUOpcodes.add; }
            else if (code == Opcodes.addiu) { flags = 0x028; alu = ALUOpcodes.addu; }
            else if (code == Opcodes.j) { flags = 0x080; alu = ALUOpcodes.or; }
            else if (code == Opcodes.andi) { flags = 0x228; alu = ALUOpcodes.and; }
            else if (code == Opcodes.ori) { flags = 0x228; alu = ALUOpcodes.or; }
            else if (code == Opcodes.slti) { flags = 0x228; alu = ALUOpcodes.slt; }
            else if (code == Opcodes.xori) { flags = 0x228; alu = ALUOpcodes.xor; }
            else if (code == Opcodes.sltiu) { flags = 0x228; alu = ALUOpcodes.sltu; }
            else if (code == Opcodes.jal) { flags = 0x188; alu = ALUOpcodes.or; }
            else if (code == Opcodes.bne) { flags = 0x401; alu = ALUOpcodes.sub; }
            else { flags = 0; alu = (ALUOpcodes)0; }

            /*switch (input.opcode)
            { // The comments are the flags, X is dont care
                case Opcodes.Rformat: flags = 0x048; alu = ALUOpcodes.rFormat; break; // 000 0100 1000
                case Opcodes.lw:      flags = 0x03C; alu = ALUOpcodes.add;  break; // 000 0011 1100
                case Opcodes.sw:      flags = 0x022; alu = ALUOpcodes.add;  break; // 000 001X 0010
                case Opcodes.beq:     flags = 0x001; alu = ALUOpcodes.sub;  break; // 000 0X0X 0001
                case Opcodes.addi:    flags = 0x028; alu = ALUOpcodes.add;  break; // 000 0010 1000
                case Opcodes.addiu:   flags = 0x028; alu = ALUOpcodes.addu; break; // 000 0010 1000    
                case Opcodes.j:       flags = 0x080; alu = ALUOpcodes.or;   break; // 0X0 1XXX 000X
                case Opcodes.andi:    flags = 0x228; alu = ALUOpcodes.and;  break; // 010 0010 1000    
                case Opcodes.ori:     flags = 0x228; alu = ALUOpcodes.or;   break; // 010 0010 1000
                case Opcodes.xori:    flags = 0x228; alu = ALUOpcodes.xor;  break; // 010 0010 1000
                case Opcodes.slti:    flags = 0x228; alu = ALUOpcodes.slt;  break; // 010 0010 1000    
                case Opcodes.sltiu:   flags = 0x228; alu = ALUOpcodes.sltu; break; // 010 0010 1000
                case Opcodes.jal:     flags = 0x188; alu = ALUOpcodes.or;   break; // 0X1 1XXX 100X
                case Opcodes.bne:     flags = 0x401; alu = ALUOpcodes.sub;  break; // 100 0X0X 0001
                default: flags = 0; alu = 0; break;
            }*/

            bne.flg = ((flags >> 10) & 1) == 1;
            logIm.flg = ((flags >> 9) & 1) == 1;
            jal.flg = ((flags >> 8) & 1) == 1;
            jump.flg = ((flags >> 7) & 1) == 1;
            regdst.flg = ((flags >> 6) & 1) == 1;
            alusrc.flg = ((flags >> 5) & 1) == 1;
            memtoreg.flg = ((flags >> 4) & 1) == 1;
            regwrite.flg = ((flags >> 3) & 1) == 1;
            memread.flg = ((flags >> 2) & 1) == 1;
            memwrite.flg = ((flags >> 1) & 1) == 1;
            branch.flg = (flags & 1) == 1;
            aluop.code = alu;
        }
    }

    public class Register : SimpleProcess
    {
        [InputBus]
        ReadA readA;
        [InputBus]
        ReadB readB;
        [InputBus]
        WriteEnabled regWrite;
        [InputBus]
        WriteAddr writeAddr;
        [InputBus]
        WriteData writeData;

        [OutputBus]
        OutputA outputA;
        [OutputBus]
        OutputB outputB;

        //uint[] data = Enumerable.Repeat((uint) 0, 32).ToArray();
        uint[] data = new uint[32];

        protected override void OnTick()
        {
            if (regWrite.flg && writeAddr.val > 0)
            {
                data[writeAddr.val] = writeData.data;
            }
            outputA.data = data[readA.addr];
            outputB.data = data[readB.addr];
        }
        /* Print the register file */
        /*
        Console.Write("[");
        for (int i = 0; i < 4; i++)
        {
            Console.Write("\t");
            for (int j = 0; j < 7; j++)
            {
                Console.Write(data[i * 8 + j] + ",\t");
            }
            Console.WriteLine(data[i * 8 + 7] + (i == 3 ? "\t]" : ","));
        }
        */
    }
}
