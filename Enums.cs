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
    }
    
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

	public enum Funcs
	{
		sll,
        ignore_1,
		srl = 2,
		sra,
		sllv,
        ignore_5,
		srlv = 6,
		srav,
		jr,
		jalr,
		movz,
		movn,
		syscall,
		bbreak, // break is a keyword
        ignore_14,
		sync = 15,
		mfhi,
		mthi,
		mflo,
		mtlo,
        ignore_20,
        ignore_21,
        ignore_22,
        ignore_23,
        mult = 24,
		multu,
		div,
		divu,
        ignore_28,
        ignore_29,
        ignore_30,
        ignore_31,
        add = 32,
		addu,
		sub,
		subu,
		and,
		or,
		xor,
		nor,
        ignore_40,
        ignore_41,
        slt = 42,
		sltu,
        ignore_44,
        ignore_45,
        ignore_46,
        ignore_47,
        tge = 48,
		tgeu,
		tlt,
		tltu,
		teq,
        ignore_53,
        tne = 54,
        ignore_55,
        ignore_56,
        ignore_57,
        ignore_58,
        ignore_59,
        ignore_60,
        ignore_61,
        ignore_62,
        ignore_63,
    }

    public enum Opcodes : int
    {
        Rformat,
        ignore_1,
        j = 2,
        jal,
        beq,
        bne,
        blez,
        bgtz,
        addi = 8,
        addiu,
        slti,
        sltiu,
        andi,
        ori,
        xori,
        lui,
        ignore_16,
        floating = 17,
        ignore_18,
        ignore_19,
        ignore_20,
        ignore_21,
        ignore_22,
        ignore_23,
        ignore_24,
        ignore_25,
        ignore_26,
        ignore_27,
        ignore_28,
        ignore_29,
        ignore_30,
        ignore_31,
        lb = 32,
        lh,
        lwl,
        lw = 35,
        lbu,
        lhu,
        lwr,
        ignore_39,
        sb = 40,
        sh,
        swl,
        sw = 43,
        ignore_44,
        ignore_45,
        swr = 46,
        cache,
        ll,
        lwc1,
        lwc2,
        pref,
        ignore_52,
        ldc1 = 53,
        ldc2,
        ignore_55,
        sc = 56,
        swc1,
        swc2,
        ignore_59,
        ignore_60,
        sdc1 = 61,
        sdc2 = 62,
        ignore_63,
    }
}