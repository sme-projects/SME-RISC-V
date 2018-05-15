using System;

namespace RISCV 
{
    public enum ALUOpcodes
    {
        RFormat,
        sub,
        add,
        addu,
        and,
        or,
        xor,
        slt,
        sltu,
    };
    
    public enum ALUOps
	{
		and,
		or,
		add,
		sl,
		sr,
		sra,
		sub,
		slt,
		addu,
		subu,
		mult,
		multu,
		nor,
		div,
		divu,
		xor,
		mtlo,
		mthi,
		mflo,
		mfhi,
		sltu,
	};

	public enum Funct3Arithmetic
	{
		add,
        sl,
        slt,
        sltu,
        xor,
        sr,
        or,
        and
    };

    public enum Funct3Branch
    {
        beq,
        bne,
        ignore_2,
        ignore_3,
        blt,
        bge,
        bltu,
        bgeu
    };

    public enum Funct3Memory
    {
        b,
        hw,
        w,
        dw,
        bu,
        hu,
        wu
    };

    public enum Funct3Synch
    {
        fence,
        fencei
    };

    public enum InstructionFormat
    {
        r,
        i,
        s,
        b,
        u,
        j
    };

    public enum Opcodes
    {
        load      = 0b0000011,
        store     = 0b0100011,
        immediate = 0b0010011,
        rformat   = 0b0110011,
        branches  = 0b1100011,
        jalr      = 0b1100111,
        jal       = 0b1101111,
        synch     = 0b0001111, // fence
        //imm_word  = 0b0011011, // TODO RV64I
        //r_word    = 0b0111011,
        lui       = 0b0110111,
        auipc     = 0b0010111,
    };
}