using BizHawk.Common.NumberExtensions;
using System;

namespace BizHawk.Emulation.Common.Components.MC6809
{
	public partial class MC6809
	{
		// this contains the vectors of instrcution operations
		// NOTE: This list is NOT confirmed accurate for each individual cycle

		private void NOP_()
		{
			PopulateCURINSTR(IDLE);
		}

		private void ILLEGAL()
		{

		}

		private void SYNC_()
		{

		}

		private void DIRECT_MEM(ushort oper)
		{
			PopulateCURINSTR(RD_INC, ALU, PC,
							SET_ADDR, ADDR, DP, ALU,
							RD, ALU, ADDR,
							oper, ALU,
							WR, ADDR, ALU);
		}

		private void EXT_MEM(ushort oper)
		{
			PopulateCURINSTR(RD_INC, ALU, PC,
							RD_INC, ALU2, PC,
							SET_ADDR, ADDR, ALU, ALU2,
							RD, ALU, ADDR,
							oper, ALU,
							WR, ADDR, ALU);
		}

		private void REG_OP_IMD_CC(ushort oper)
		{
			Regs[ALU2] = Regs[CC];
			PopulateCURINSTR(RD_INC_OP, ALU, PC, oper, ALU2, ALU,
							TR, CC, ALU2);
		}

		private void EXG_()
		{
			PopulateCURINSTR(RD_INC, ALU,
							EXG, ALU,
							IDLE,
							IDLE,
							IDLE,
							IDLE,
							IDLE);
		}

		private void TFR_()
		{
			PopulateCURINSTR(RD_INC, ALU,
							TFR, ALU,
							IDLE,
							IDLE,
							IDLE);
		}

		private void REG_OP(ushort oper, ushort src)
		{
			PopulateCURINSTR(oper, src);
		}

		private void JMP_DIR_()
		{
			PopulateCURINSTR(RD_INC, ALU, PC,
							SET_ADDR, PC, DP, ALU);
		}

		private void JMP_EXT_()
		{
			PopulateCURINSTR(RD_INC, ALU, PC,
							RD_INC, ALU2, PC,
							SET_ADDR, PC, ALU, ALU2);
		}

		private void LBR_(bool cond)
		{
			if (cond)
			{
				PopulateCURINSTR(RD_INC, ALU, PC,
								RD_INC, ALU2, PC,
								SET_ADDR, ADDR, ALU, ALU2,
								ADD16BR, PC, ADDR);
			}
			else
			{
				PopulateCURINSTR(RD_INC, ALU, PC,
								RD_INC, ALU2, PC,
								SET_ADDR, PC, ALU, ALU2);
			}			
		}

		private void BR_(bool cond)
		{
			if (cond)
			{
				PopulateCURINSTR(RD_INC, ALU, PC,
								ADD8BR, PC, ALU);
			}
			else
			{
				PopulateCURINSTR(RD_INC, ALU, PC,
								IDLE);
			}
		}

		private void LBSR_()
		{
				PopulateCURINSTR(RD_INC, ALU, PC,
								RD_INC, ALU2, PC,
								SET_ADDR, ADDR, ALU, ALU2,
								ADD16BR, PC, ADDR,
								TR, ADDR, PC,
								DEC16, SP,
								WR_DEC_LO, SP, ADDR,
								WR_HI, SP, ADDR);
		}

		private void PAGE_2()
		{

			PopulateCURINSTR(OP_PG_2);
		}

		private void PAGE_3()
		{

			PopulateCURINSTR(OP_PG_3);
		}

		private void ABX_()
		{
			PopulateCURINSTR(ABX,
							IDLE);
		}

		private void MUL_()
		{
			PopulateCURINSTR(MUL,
							IDLE,
							IDLE,
							IDLE,
							IDLE,
							IDLE,
							IDLE,
							IDLE,
							IDLE,
							IDLE);
		}

		private void RTS()
		{
			PopulateCURINSTR(IDLE,
							RD_INC, ALU, SP,
							RD_INC, ALU2, SP,
							SET_ADDR, PC, ALU, ALU2);
		}

		private void RTI()
		{
			PopulateCURINSTR(IDLE,
							RD_INC_OP, CC, SP, JPE,
							RD_INC, A, SP,
							RD_INC, B, SP,
							RD_INC, DP, SP,
							RD_INC, ALU, SP,
							RD_INC_OP, ALU2, SP, SET_ADDR, X, ALU, ALU2,
							RD_INC, ALU, SP,
							RD_INC_OP, ALU2, SP, SET_ADDR, Y, ALU, ALU2,
							RD_INC, ALU, SP,
							RD_INC_OP, ALU2, SP, SET_ADDR, US, ALU, ALU2,
							SET_ADDR, PC, ALU2, ALU);
		}

		private void PSH(ushort src)
		{
			PopulateCURINSTR(RD_INC, ALU, PC,
							IDLE,
							DEC16, SP,
							PSH_n, src);
		}

		// Post byte info is in ALU
		// mask out bits until the end
		private void PSH_n_BLD(ushort src)
		{
			if (Regs[ALU].Bit(7))
			{
				PopulateCURINSTR(WR_DEC_LO, src, PC,
								WR_DEC_HI_OP, src, PC, PSH_n, src);

				Regs[ALU] &= 0x7F;
			}
			else if (Regs[ALU].Bit(6))
			{
				if (src == US)
				{
					PopulateCURINSTR(WR_DEC_LO, src, SP,
									WR_DEC_HI_OP, src, SP, PSH_n, src);
				}
				else
				{
					PopulateCURINSTR(WR_DEC_LO, src, US,
									WR_DEC_HI_OP, src, US, PSH_n, src);
				}
				
				Regs[ALU] &= 0x3F;
			}
			else if (Regs[ALU].Bit(5))
			{
				PopulateCURINSTR(WR_DEC_LO, src, Y,
								WR_DEC_HI_OP, src, Y, PSH_n, src);

				Regs[ALU] &= 0x1F;
			}
			else if (Regs[ALU].Bit(4))
			{
				PopulateCURINSTR(WR_DEC_LO, src, X,
								WR_DEC_HI_OP, src, X, PSH_n, src);

				Regs[ALU] &= 0xF;
			}
			else if (Regs[ALU].Bit(3))
			{
				PopulateCURINSTR(WR_DEC_LO_OP, src, DP, PSH_n, src);

				Regs[ALU] &= 0x7;
			}
			else if (Regs[ALU].Bit(2))
			{
				PopulateCURINSTR(WR_DEC_LO_OP, src, B, PSH_n, src);

				Regs[ALU] &= 0x3;
			}
			else if (Regs[ALU].Bit(1))
			{
				PopulateCURINSTR(WR_DEC_LO_OP, src, A, PSH_n, src);

				Regs[ALU] &= 0x1;
			}
			else if (Regs[ALU].Bit(0))
			{
				PopulateCURINSTR(WR_DEC_LO_OP, src, CC, PSH_n, src);
			}
			else
			{
				Regs[src] += 1; // we decremented an extra time overall, regardless of what was run
			}
		}

		private void DEC_16(ushort src_l, ushort src_h)
		{

		}

		private void ADD_16(ushort dest_l, ushort dest_h, ushort src_l, ushort src_h)
		{

		}

		private void STOP_()
		{

		}

		private void HALT_()
		{

		}

		private void JR_COND(bool cond)
		{

		}

		private void JP_COND(bool cond)
		{

		}

		private void RET_()
		{

		}

		private void RETI_()
		{

		}


		private void RET_COND(bool cond)
		{

		}

		private void CALL_COND(bool cond)
		{

		}

		private void INT_OP(ushort operation, ushort src)
		{

		}

		private void BIT_OP(ushort operation, ushort bit, ushort src)
		{

		}

		private void RST_(ushort n)
		{

		}

		private void DI_()
		{

		}

		private void EI_()
		{

		}

		private void JP_HL()
		{

		}

		private void ADD_SP()
		{

		}

		private void LD_SP_HL()
		{

		}

		private void LD_HL_SPn()
		{

		}

		private void JAM_()
		{

		}
	}
}
