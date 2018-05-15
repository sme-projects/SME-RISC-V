using System;
using SME;

namespace RISCV
{
	[InitializedBus]
	public interface ALUOp : IBus 
	{
		ALUOps opcode { get; set; }
	}

	[InitializedBus]
	public interface ALUResult : IBus
	{
		uint val { get; set; }
	}

	[InitializedBus]
	public interface Source1 : IBus
	{
		uint val { get; set; }
	}

	[InitializedBus]
	public interface Source2 : IBus
	{
		uint val { get; set; }
	}

	[InitializedBus]
	public interface Zero : IBus
	{
		bool flg { get; set; }
	}

	public class ALU : SimpleProcess
	{
		[InputBus]
		Source1 s1;
		[InputBus]
		Source2 s2;
		[InputBus]
		ALUOp opcode;

		[OutputBus]
		ALUResult result;
		[OutputBus]
		Zero zero;

		uint tmp = 0;

		protected override void OnTick()
		{
			switch (opcode.opcode)
			{
				case ALUOps.add: tmp = s1.val + s2.val; break;
			}
			result.val = tmp;
			zero.flg = tmp == 0;
		}
	}

	public class ALUControl : SimpleProcess
	{
		[InputBus]
		Funct3 funct;
		[InputBus]
		Format format;
	
		[OutputBus]
		ALUOp aluop;

		protected override void OnTick()
		{
			switch (format.val) 
			{
				case InstructionFormat.b:
					switch ((Funct3Branch)(uint)funct.val)
					{
						case Funct3Branch.beq:  aluop.opcode = ALUOps.eq;   break;
						case Funct3Branch.bge:  aluop.opcode = ALUOps.sge;  break;
						case Funct3Branch.bgeu: aluop.opcode = ALUOps.sgeu; break;
						case Funct3Branch.blt:  aluop.opcode = ALUOps.slt;  break;
						case Funct3Branch.bltu: aluop.opcode = ALUOps.sltu; break;
						case Funct3Branch.bne:  aluop.opcode = ALUOps.neq;  break;
						default: throw new Exception("What");
					}; break;
				case InstructionFormat.i:
					switch ((Funct3Arithmetic)(uint)funct.val)
					{
						case Funct3Arithmetic.add:  aluop.opcode = ALUOps.add;  break;
						case Funct3Arithmetic.and:  aluop.opcode = ALUOps.and;  break;
						case Funct3Arithmetic.or:   aluop.opcode = ALUOps.or;   break;
						case Funct3Arithmetic.sl:   aluop.opcode = ALUOps.sl;   break;
						case Funct3Arithmetic.slt:  aluop.opcode = ALUOps.slt;  break;
						case Funct3Arithmetic.sltu: aluop.opcode = ALUOps.sltu; break;
						case Funct3Arithmetic.sr:   aluop.opcode = ALUOps.sr;   break;
						case Funct3Arithmetic.xor:  aluop.opcode = ALUOps.xor;  break;
					}; break;
				case InstructionFormat.r:
					switch ((Funct3Arithmetic)(uint)funct.val)
					{
						case Funct3Arithmetic.add:  aluop.opcode = ALUOps.add;  break;
						case Funct3Arithmetic.and:  aluop.opcode = ALUOps.and;  break;
						case Funct3Arithmetic.or:   aluop.opcode = ALUOps.or;   break;
						case Funct3Arithmetic.sl:   aluop.opcode = ALUOps.sl;   break;
						case Funct3Arithmetic.slt:  aluop.opcode = ALUOps.slt;  break;
						case Funct3Arithmetic.sltu: aluop.opcode = ALUOps.sltu; break;
						case Funct3Arithmetic.sr:   aluop.opcode = ALUOps.sr;   break;
						case Funct3Arithmetic.xor:  aluop.opcode = ALUOps.xor;  break;
					}; break;
				case InstructionFormat.s:
					aluop.opcode = ALUOps.add; break;
			}
		}
	}

	public class ImmMux : SimpleProcess
	{
		[InputBus]
		Register2 rs2;
		[InputBus]
		Imm imm;
		[InputBus]
		ImmAsSource2 control;

		[OutputBus]
		Source2 s2;

		protected override void OnTick() 
		{
			s2.val = control.flg ? (uint) imm.val : rs2.val;
		}
	}

	public class PCMux : SimpleProcess
	{
		[InputBus]
		Register1 rs1;
		[InputBus]
		Address pc;
		[InputBus]
		PCAsSource1 control;

		[OutputBus]
		Source1 s1;

		protected override void OnTick()
		{
			s1.val = control.flg ? pc.address : rs1.val;
		}
	}
}

