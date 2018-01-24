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
            public string BCRName;

            public StationInfo()
            {
                BufferName = string.Empty;
                BufferIndex = 0;
                StationName = string.Empty;
                StationIndex = 0;
                BCRName = string.Empty;
            }
        }

        private class Trace
        {
            public const string Inital = "0";

            public const string StoreIn_GetStoreInCommandAndWritePLC = "11";
            public const string StoreIn_CrateCraneCommand = "12";
            public const string StoreIn_CraneCommandFinish = "13";

            public const string StoreOut_GetStoreOutCommandAndWritePLC = "21";
            public const string StoreOut_CrateCraneCommand = "22";
            public const string StoreOut_CraneCommandFinish = "23";

            public const string LoactionToLoaction_CrateCraneCommand = "51";
            public const string LoactionToLoaction_CraneCommandFinish = "52";

            private Trace()
            {
            }
        }

        private class CommandInfo
        {
            public string CommandID;
            public int CommandMode;
            public string IO_Type;
            public string Loaction;
            public string NewLoaction;
            public string StationNo;
            public string Priority;

            public CommandInfo()
            {
                CommandID = string.Empty;
                CommandMode = 0;
                IO_Type = string.Empty;
                Loaction = string.Empty;
                NewLoaction = string.Empty;
                StationNo = string.Empty;
                Priority = string.Empty;
            }
        }

        private class CommandState
        {
            public const string Inital = "0";
            public const string Start = "1";
            public const string CompletedWaitPost = "7";
            public const string CancelWaitPost = "8";
            public const string Completed = "9";
            public const string Cancel = "D";
            public const string PostFail = "E";

            private CommandState()
            {
            }
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

        private class DB2
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
            public ushort StroreInToStation { get; set; }
            public ushort StroreOutToStation { get; set; }
        }

        private struct Cmd_Mst
        {
            public string Cmd_Sno;
            public string Cmd_Sts;
            public string Cmd_Mode;
            public string Io_Type;
            public string Stn_No;
            public string Loc;
            public string New_Loc;
            public string Height;
            public string Prty;
            public string Crt_Dte;
            public string Exp_Dte;
            public string End_Dte;
            public string User_Id;
            public string Cyc_No;
            public string Plt_No;
            public string Trace;
            public string Remark;
        }
    }
}
