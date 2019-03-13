using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirle.ASRS
{
    public partial class WCS
    {
        private class StationInfo
        {
            public string BufferName;
            public int BufferIndex;
            public string StationName;
            public int StationIndex;

            public StationInfo()
            {
                BufferName = string.Empty;
                BufferIndex = 0;
                StationName = string.Empty;
                StationIndex = 0;
            }
        }

        private class Trace
        {
            /// <summary>
            /// 命令产生
            /// </summary>
            public const string Inital = "0";


            /// <summary>
            /// 分配储位
            /// </summary>
            public const string StoreIn_GetStoreInCommandAndSetLoc = "10";
            /// <summary>
            /// 命令写入PLC
            /// </summary>
            public const string StoreIn_GetStoreInCommandAndWritePLC = "11";

            /// <summary>
            /// 产生主机命令
            /// </summary>
            public const string StoreIn_CrateCraneCommand = "12";
            /// <summary>
            /// 命令结束
            /// </summary>
            public const string StoreIn_CraneCommandFinish = "13";

            /// <summary>
            /// 命令写入PLC
            /// </summary>
            public const string StoreOut_GetStoreOutCommandAndWritePLC = "21";
            /// <summary>
            /// 产生主机命令
            /// </summary>
            public const string StoreOut_CrateCraneCommand = "22";
            /// <summary>
            /// 命令结束
            /// </summary>
            public const string StoreOut_CraneCommandFinish = "23";

            public const string LoactionToLoaction_CrateCraneCommand = "51";
            public const string LoactionToLoaction_CraneCommandFinish = "52";

            private Trace()
            {
            }
        }

        private class KanbanInfoStyle
        {
            public string Info;
            public string Color;
        }
        private class KanbanModel
        {
            public const string OUT = "出库";
            public const string IN = "入库";
            public const string CYC = "盘点";
            public const string ERROR = "异常";
            public const string CYCIN = "回库";
            public const string OVERWEIGHT = "超重";
            public const string PICKING = "捡料";
        }
        private class HtmlColor
        {
            public const string Red = "background-color:#ff0000;";
            public const string Bule = "background-color:#00ffff;";
            public const string Green = "background-color:#b6ff00;";
            public const string Gray = "background-color:#808080;";
            public const string Yellow = "background-color:#ffd800;";
        }
        private class CommandInfo
        {
            public string CommandID;
            public int CommandMode;
            public string Cmd_Sts;
            public string IOType;
            public string Loaction;
            public string NewLoaction;
            public string StationNo;
            public string Priority;
            public string CycleNo;
            public string PalletNo;
            public string WEIGH;
            public string ACTUAL_WEIGHT;
            public string LOC_SIZE;

            public CommandInfo()
            {

                CommandID = string.Empty;
                CommandMode = 0;
                IOType = string.Empty;
                Loaction = string.Empty;
                NewLoaction = string.Empty;
                StationNo = string.Empty;
                Priority = string.Empty;
                CycleNo = string.Empty;
                PalletNo = string.Empty;
                Cmd_Sts = string.Empty;
                WEIGH = string.Empty;
                ACTUAL_WEIGHT = string.Empty;
                LOC_SIZE = string.Empty;
            }
        }

        private class CommandState
        {
            public const string Inital = "0";
            public const string Start = "1";

            public const string OnTransit = "2";
            /// <summary>
            /// 完成待过账
            /// </summary>
            public const string CompletedWaitPost = "3";
            /// <summary>
            /// 完成
            /// </summary>
            public const string Completed = "8";
            public const string EQUCompleted = "9";
            /// <summary>
            /// 取消待过账
            /// </summary>
            public const string CancelWaitPost = "4";
            public const string EQUCancelWaitPost = "8";
            /// <summary>
            /// 强制完成待过账
            /// </summary>
            public const string CompletedConstraint = "5";
            /// <summary>
            /// 强制取消待过账
            /// </summary>
            public const string CancelConstraint = "6";
            public const string Cancel = "9";
            public const string PostFail = "F";

            private CommandState()
            {
            }
        }

        private class LoactionState
        {
            public const string IN = "I";
            public const string OUT = "O";
            public const string S = "S";

        }

        private class STN_NO
        {
            public const string StoreOutA01 = "A01";
            public const string StoreOutA10 = "A10";
            public const string StoreOutA18 = "A18";
            public const string StoreOutA26 = "A26";
            public const string StoreOutA34 = "A34";
            public const string StoreInA53 = "A53";
            public const string StoreInA90 = "A90";
            public const string StoreInA41 = "A41";
            public const string StoreInA117 = "A117";
            public const string StoreInA83 = "A83";
            public const string StoreInA08 = "A08";
            public const string StoreInA93 = "A93";
            public const string StoreInA98 = "A98";
            public const string StoreInA103= "A103";
            public const string StoreInA108 = "A108";
            public const string StoreInA113 = "A113";
        }
        private class CRANE_NO
        {
            public const int Crane1 = 1;
            public const int Crane2 = 2;
            public const int Crane3 = 3;
            public const int Crane4 = 4;
            public const int Crane5 = 5;
        }
        private class CraneMode
        {
            public const string StoreIn = "1";
            public const string StoreOut = "2";
            public const string Picking = "3";
            public const string StationToStation = "4";
            public const string LoactionToLoaction = "5";
            public const string Move = "6";
            public const string Scan = "7";
            public const string PickUp = "8";
            public const string Deposite = "9";
            public const string ReturnHP = "10";

            private CraneMode()
            {
            }
        }
        private class IO_TYPE
        {
            public const string StoreIn11 = "11";
            public const string StoreIn15 = "25";
            public const string StoreOut21 = "21";
            public const string StoreOut25 = "25";
            public const string PickingIn = "30";
            public const string PickingOut = "31";
            public const string LoactionToLoaction = "51";
            public const string LoactionToLoaction2 = "52";
            private IO_TYPE()
            {
            }
        }
        private class CMDMode
        {
            public const string StoreIn = "1";
            public const string StoreOut = "2";
            public const string Picking = "3";
            public const string LoactionToLoaction = "5";

            private CMDMode()
            {
            }
        }

        private class SMPLCData_1
        {
            public ushort BCR1_1 { get; set; }
            public ushort BCR1_2 { get; set; }
            public ushort BCR1_3 { get; set; }
            public ushort BCR1_4 { get; set; }
            public ushort BCR1_5 { get; set; }
            public ushort BCR2_1 { get; set; }
            public ushort BCR2_2 { get; set; }
            public ushort BCR2_3 { get; set; }
            public ushort BCR2_4 { get; set; }
            public ushort BCR2_5 { get; set; }
            public ushort Tmp { get; set; }
            public ushort Load { get; set; }
        }

        private class SMPLCData_2
        {
            public ushort StoreIn { get; set; }
            public ushort StoreOut { get; set; }
        }

        private struct Cmd_Mst
        {
            public string Cmd_Date;
            public string Cmd_Sno;
            public string Cmd_Sts;
            public string Prty;
            public string Stn_No;
            public string Cmd_Mode;
            public string Io_Type;
            public string Loc;
            public string New_Loc;
            public string Trace;
            public string Plt_No;
            public string Weigh;
            public string Actual_Weight;
            public string Crt_Dte;
            public string Exp_Dte;
            public string End_Dte;
            public string Fin_Dte;
            public string User_Id;
            public string Host_Name;
            public string CompleteCode;
            public string Cyc_No;
            public string Remark;
            public string Loc_Size;
        }

        private struct Prodecu
        {
            public string Prodecu_No;
            public string Item_No;
            public string PStn_No;
            public int Prodecu_Qty;
            public string Cmd_Sno;
            public string Item_Type;
        }
    }
}
