﻿using SME;
using SME.VHDL;

namespace RISCV
{
    [InitializedBus]
    public interface Format : IBus 
    {
        InstructionFormat val { get; set; }
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
    public interface Imm
    {
        int val { get; set; }
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
    public interface ImmAsSource2 : IBus 
    {
        bool flg { get; set; }
    }

    [InitializedBus]
    public interface Opcode : IBus 
    {
        Opcodes val { get; set; }
    }

    [InitializedBus]
    public interface MemRead : IBus 
    {
        bool flg { get; set; }
    }

    [InitializedBus]
    public interface MemWrite : IBus 
    {
        bool flg { get; set; }
    }

    [InitializedBus]
    public interface PCAsSource1 : IBus 
    {
        bool flg { get; set; }
    }

    [InitializedBus]
    public interface Rd : IBus 
    {
        UInt5 addr { get; set; }
    }

    [InitializedBus]
    public interface Register1 
    {
        uint val { get; set; }
    }

    [InitializedBus]
    public interface Register2
    {
        uint val { get; set; }
    }

    [InitializedBus] // From WB to Register file
    public interface RegWriteBus
    {
        bool ena { get; set; }
        UInt5 addr { get; set; }
        uint data { get; set; }
    }

    [InitializedBus]
    public interface RegWrite : IBus
    {
        bool flg { get; set; }
    }

    [InitializedBus]
    public interface Rs1 : IBus 
    {
        UInt5 addr { get; set; }
    }

    [InitializedBus]
    public interface Rs2 : IBus 
    {
        UInt5 addr { get; set; }
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
            rs2.addr   = (UInt5)  ((tmp >> 20) & 0b11111);
            rs1.addr   = (UInt5)  ((tmp >> 15) & 0b11111);
            funct3.val = (UInt3)  ((tmp >> 12) & 0b111);
            rd.addr    = (UInt5)  ((tmp >> 7)  & 0b11111);
            opcode.val = (Opcodes) (tmp        & 0b1111111);
            imm12.val  = (UInt12) ((tmp >> 20) & 0b111111111111);
            imm7.val   = (UInt7)  ((tmp >> 25) & 0b1111111);
            imm5.val   = (UInt5)  ((tmp >> 7)  & 0b11111);
            imm20.val  = (UInt20) ((tmp >> 12) & 0b11111111111111111111);
        }
    }

    public class SignExtend : SimpleProcess
    {
        [InputBus]
        Imm5 imm5;
        [InputBus]
        Imm7 imm7;
        [InputBus]
        Imm12 imm12;
        [InputBus]
        Imm20 imm20;
        [InputBus]
        Format format;

        [OutputBus]
        Imm imm;

        protected override void OnTick() // TODO mere manuel
        {
            switch (format.val)
            {
                case InstructionFormat.i: imm.val = (int) imm12.val; break;
                case InstructionFormat.s: imm.val = (int) ((imm7.val << 5) | (imm5.val)); break;
                case InstructionFormat.b: {
                    int eleven = imm5.val & 1;
                    UInt5 fourone = (UInt5)(imm5.val & 0b11110);
                    int twelve = imm7.val >> 6 & 1;
                    UInt6 tenfive = (UInt6)(imm7.val & 0b0111111);
                    imm.val = (int) ((twelve == 1 ? 0 : unchecked((int)0xFFFFF000)) |
                        (eleven << 11) |
                        (tenfive << 5) |
                        fourone);
                }; break;
                case InstructionFormat.u: imm.val = (int) (imm20.val << 12); break;
                case InstructionFormat.j: {
                    int twenty = (int)((imm20.val >> 19) & 1);
                    int tenone = (int)((imm20.val >> 9) & 0x1FF);
                    int eleven = (int)((imm20.val >> 8) & 1);
                    int nineteentwelve = (int)((imm20.val) & 0b11111111);
                    imm.val = (int) ((twenty == 1 ? 0 : unchecked((int)0xFFF00000)) |
                        (nineteentwelve << 12) |
                        (eleven << 11) |
                        (tenone << 1)
                    );
                }; break;
                default: imm.val = 0; break;
            }
        }
    }

    public class Control : SimpleProcess
    {
        [InputBus]
        Opcode opcode;

        [OutputBus]
        Format format;
        [OutputBus]
        PCAsSource1 pcas1;
        [OutputBus]
        ImmAsSource2 ias2;
        [OutputBus]
        MemRead mr;
        [OutputBus]
        MemWrite mw;
        [OutputBus]
        RegWrite rw;

        protected override void OnTick()
        {
            byte flags = 0;
            ALUOps alu = ALUOps.add;
            switch (opcode.val)
            {
                case Opcodes.auipc:     flags = 0b10011; format.val = InstructionFormat.u; break;
                case Opcodes.branches:  flags = 0b00000; format.val = InstructionFormat.b; break;
                case Opcodes.immediate: flags = 0b10001; format.val = InstructionFormat.i; break;
                case Opcodes.jal:       flags = 0b10001; format.val = InstructionFormat.j; break;
                case Opcodes.jalr:      flags = 0b10000; format.val = InstructionFormat.j; break;
                case Opcodes.load:      flags = 0b11000; format.val = InstructionFormat.i; break;
                case Opcodes.lui:       flags = 0b10001; format.val = InstructionFormat.u; break;
                case Opcodes.rformat:   flags = 0b10000; format.val = InstructionFormat.r; break;
                case Opcodes.store:     flags = 0b00101; format.val = InstructionFormat.s; break;
                case Opcodes.synch:     flags = 0b00001; format.val = InstructionFormat.i; break;
            }

            ias2.flg  = ((flags >> 0) & 1) == 1;
            pcas1.flg = ((flags >> 1) & 1) == 1;
            mr.flg    = ((flags >> 2) & 1) == 1;
            mw.flg    = ((flags >> 3) & 1) == 1;
            rw.flg    = ((flags >> 4) & 1) == 1;
        }
    }

    public class Register : SimpleProcess
    {
        [InputBus]
        Rs1 rs1;
        [InputBus]
        Rs2 rs2;
        [InputBus]
        RegWriteBus regwrite;

        [OutputBus]
        Register1 register1;
        [OutputBus]
        Register2 register2;

        uint[] data = new uint[32];

        protected override void OnTick()
        {
            if (regwrite.ena && regwrite.addr > 0)
            {
                data[regwrite.addr] = regwrite.data;
            }
            register1.val = data[rs1.addr];
            register2.val = data[rs2.addr];
        }
    }
}
