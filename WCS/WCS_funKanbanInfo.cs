using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mirle.ASRS
{
    public partial class WCS
    {
        private void funKanbanInfo()
        {
            string strMsg = string.Empty;
            string strSQL = string.Empty;
            string strEM = string.Empty;
            DataTable dtkanbaninfo = new DataTable();
            DataTable dtCmdSno = new DataTable();
            try
            {
                foreach (StationInfo stnDef in lstClearKanbanInfo)
                {
                    int intBufferIndex = stnDef.BufferIndex;
                    string strBufferName = stnDef.BufferName;

                    if (string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._CommandID) &&
                        string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._Destination) &&
                        bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off)
                    {
                        string[] strValues = new string[] { "0" };
                        if (bufferData[intBufferIndex]._Clearnotice)
                        {
                            funClearKanbanInfo(strBufferName);
                            strMsg = bufferData[intBufferIndex]._BufferName + "|";
                            strMsg += "Clear Kanbaninfo Success!";
                            strValues = new string[] { "0" };
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Clearnotice, strValues);
                        }
                        if (!string.IsNullOrWhiteSpace(bufferData[intBufferIndex]._PalletNo))
                        {
                            strValues = new string[] { "0" };
                            InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_PalletNo, strValues);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                if (dtkanbaninfo != null)
                {
                    dtkanbaninfo.Clear();
                    dtkanbaninfo.Dispose();
                    dtkanbaninfo = null;
                }
                if (dtCmdSno != null)
                {
                    dtCmdSno.Clear();
                    dtCmdSno.Dispose();
                    dtCmdSno = null;
                }
            }
        }

        private void funSetKanbanInfoERROR2(string sKanbanModel, string sStnNo, string Message)
        {
            try
            {
                string strMsg = string.Empty;
                string sHtml = "<!DOCTYPE html><html><head><meta http - equiv = \"Content - Type\" content = \"text / html; charset = utf - 8\" /><title></title><meta charset = \"utf - 8\" http-equiv = \"refresh\" content = \"1\" /> <style>  * {font-family:微软雅黑;margin:0;padding:0;background-color:#000000;}html{max-width:100%;min-width:100%;height:100%;display:table;}body{max-width:100%;min-width:100%;display:table-cell;height:100%;}body #frmMain{width:100%;height:100%;}</style></head>";
                sHtml += "<body><form id=\"frmMain\"><table style=\"width:100%; height:100%;\"><tr><td style=\"width:100%; height:100%; border:1px solid #000000;text-align:center;font-size:6em;background-color:black;color:#ff6a00;\">" + Message + "</td></tr></table></form></body></html>";
                InitSys.funWriteHtml(sStnNo, sHtml);

            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);

            }
        }

        private bool funSetKanbanInfoERROR(string sKanbanModel, string sStnNo, string Message, int intBufferIndex)
        {
            try
            {
                string strMsg = string.Empty;
                string sHtml = "<!DOCTYPE html><html><head><meta http - equiv = \"Content - Type\" content = \"text / html; charset = utf - 8\" /><title></title><meta charset = \"utf - 8\" http-equiv = \"refresh\" content = \"1\" /> <style>  * {font-family:微软雅黑;margin:0;padding:0;background-color:#000000;}html{max-width:100%;min-width:100%;height:100%;display:table;}body{max-width:100%;min-width:100%;display:table-cell;height:100%;}body #frmMain{width:100%;height:100%;}</style></head>";
                sHtml += "<body><form id=\"frmMain\"><table style=\"width:100%; height:100%;\"><tr><td style=\"width:100%; height:100%; border:1px solid #000000;text-align:center;font-size:6em;background-color:black;color:#ff6a00;\">" + Message + "</td></tr></table></form></body></html>";
                InitSys.funWriteHtml(sStnNo, sHtml);

                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                strMsg += "Set KanbaninfoERROR Success!";
                funWriteSysTraceLog(strMsg);

                string[] strValues = new string[] { "1" };
                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Clearnotice, strValues);
                return true;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }
        private bool funSetKanbanInfo(string sKanbanModel, string sStnNo, CommandInfo commandInfo, int intBufferIndex)
        {
            string strSQL = string.Empty;
            string strMsg = string.Empty;
            DataTable dtCmdSno = new DataTable();
            string strColor = string.Empty;
            string strCommandID = string.Empty;
            string strPalletNo = string.Empty;
            int intPalletQty = 0;
            int intPalletTaskQty = 0;
            string strPalletQty = string.Empty;
            string strPalletTaskQty = string.Empty;
            KanbanInfoStyle infoStyle = new KanbanInfoStyle();
            string strEM = string.Empty;

            if (!string.IsNullOrEmpty(commandInfo.CommandID)) strCommandID = "&nbsp&nbsp命令序号:" + commandInfo.CommandID;
            if (!string.IsNullOrEmpty(commandInfo.PalletNo)) strPalletNo = "&nbsp&nbsp托盘单号:" + commandInfo.PalletNo;
            if (!string.IsNullOrEmpty(commandInfo.CycleNo)) { strEM = "&nbsp&nbsp盘点单号:" + commandInfo.CycleNo; sKanbanModel = KanbanModel.CYC; }
            if (!string.IsNullOrEmpty(commandInfo.WEIGH) && !string.IsNullOrEmpty(commandInfo.ACTUAL_WEIGHT)) strEM = "&nbsp&nbsp实际重量:" + commandInfo.WEIGH + "计划重量:" + commandInfo.ACTUAL_WEIGHT;
            switch (commandInfo.CommandMode.ToString())
            {
                case CraneMode.StoreIn:
                    if (commandInfo.IOType != IO_TYPE.StoreIn15)
                    {
                        strSQL = " SELECT (SELECT Descr_C FROM ITEM_MST WHERE ITEM_MST.ITEM_NO=B.ITEM_NO) AS Descr_C,(select l.Code_Name from lotacode l  where l.lota_no=B.lotatt08 AND l.LOTA_TYPE='Lotatt08') as Code_Name,B.BOX_ID as USERDEFINE1,B.Lotatt05 as USER_ID,'' AS NOTES,";
                        strSQL += " case B.SNO when null then 2 ELSE 1  end as GAODU ";
                        strSQL += " FROM PLT_MST P JOIN BOX B ON P.SUB_NO=B.SUB_NO";
                        strSQL += " WHERE P.PLT_NO='" + commandInfo.PalletNo + "'";
                    }
                    else
                    {
                        strEM += "&nbsp&nbsp空托盘出库";
                    }
                    break;
                case CraneMode.StoreOut:
                case CraneMode.Picking:

                    strSQL = " SELECT B.SUB_NO FROM CMD_DTL C JOIN BOX B ON C.BOX_ID=B.BOX_ID WHERE C.CMD_SNO='" + commandInfo.CommandID + "'";
                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strMsg))
                    {
                        string Sub_NO = dtCmdSno.Rows[0]["SUB_NO"].ToString();
                        if (commandInfo.IOType == IO_TYPE.StoreOut21)
                        {
                            strSQL = " SELECT (SELECT Descr_C FROM ITEM_MST WHERE ITEM_MST.ITEM_NO=B.ITEM_NO) AS Descr_C,(select l.Code_Name from lotacode l  where l.lota_no=B.lotatt08 AND l.LOTA_TYPE='Lotatt08') as Code_Name,";
                            strSQL += " (SELECT NOTES FROM TKT_OUT_MST TOM WHERE TOM.SNO=TOD.SNO) AS NOTES,";
                            strSQL += " (SELECT USERDEFINE1 FROM TKT_OUT_MST TOM WHERE TOM.SNO=TOD.SNO) AS USERDEFINE1,";
                            strSQL += " (SELECT SOReference5 FROM TKT_OUT_MST TOM WHERE TOM.SNO=TOD.SNO) AS USER_ID,";
                            strSQL += " case B.OUTSNO when 'N' then 2 ELSE 1  end as GAODU ";
                            strSQL += " FROM BOX B LEFT JOIN TKT_OUT_DTL TOD ON B.OUTSNO=TOD.SNO AND B.OutD_EDI_03=TOD.D_EDI_03 ";
                            strSQL += " WHERE TOD.TKT_TYPE='BHD' AND B.sub_no='" + Sub_NO + "'";
                        }
                        if (commandInfo.IOType == IO_TYPE.PickingOut)
                        {
                            strSQL = "SELECT (SELECT Descr_C FROM ITEM_MST WHERE ITEM_MST.ITEM_NO=B.ITEM_NO) AS Descr_C,(select l.Code_Name from lotacode l  where l.lota_no=B.lotatt08 AND l.LOTA_TYPE='Lotatt08') as Code_Name,";
                            strSQL += "";
                        }
                        if (commandInfo.IOType == IO_TYPE.StoreOut20)
                        {

                        }
                    }
                    else
                    {
                        strEM += "&nbsp&nbsp空托盘出库";
                    }
                    break;
                default:
                    break;
            }

            switch (sKanbanModel)
            {
                case KanbanModel.CYC:
                    strColor = HtmlColor.Bule;
                    break;
                case KanbanModel.PICKING:
                    strColor = HtmlColor.Yellow;
                    break;
                case KanbanModel.CYCIN:
                case KanbanModel.IN:
                case KanbanModel.OUT:
                    strColor = HtmlColor.Green;
                    break;
                case KanbanModel.OVERWEIGHT:
                    strColor = HtmlColor.Red;
                    break;
                default:
                    break;
            }


            if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strMsg))
            {
                for (int i = 0; i < dtCmdSno.Rows.Count; i++)
                {

                    if (dtCmdSno.Rows[i]["GAODU"].ToString() == "1")
                    {
                        intPalletQty += 1;
                        intPalletTaskQty += 1;
                        infoStyle.Color = HtmlColor.Antiquewhite;
                    }
                    if (dtCmdSno.Rows[i]["GAODU"].ToString() == "2")
                    {
                        sKanbanModel = KanbanModel.PICKING;
                        intPalletQty += 1;
                        infoStyle.Color = HtmlColor.Gray;
                    }
                    infoStyle.Info += "<tr>";
                    infoStyle.Info += "<td style=\"" + infoStyle.Color + " \">&nbsp" + (i + 1).ToString() + "</td>";
                    infoStyle.Info += "<td style=\"" + infoStyle.Color + " \">&nbsp" + dtCmdSno.Rows[i]["DESCR_C"].ToString() + "</td>";
                    infoStyle.Info += "<td style=\"" + infoStyle.Color + " \">&nbsp" + dtCmdSno.Rows[i]["Code_Name"].ToString() + "</td>";
                    infoStyle.Info += "<td style=\"" + infoStyle.Color + " \">&nbsp" + dtCmdSno.Rows[i]["USER_ID"].ToString() + "</td>";
                    if (dtCmdSno.Rows[i]["NOTES"].ToString().Length > 20)
                    {
                        infoStyle.Info += "<td style=\"" + infoStyle.Color + " \">&nbsp" + dtCmdSno.Rows[i]["USERDEFINE1"].ToString().Substring(0, 19) + "...</td>";
                    }
                    else
                    {
                        infoStyle.Info += "<td style=\"" + infoStyle.Color + " \">&nbsp" + dtCmdSno.Rows[i]["USERDEFINE1"].ToString() + "</td>";
                    }
                    infoStyle.Info += "</tr>";
                }
                infoStyle.Info += "  <tr><td colspan=\"5\" style=\"font-size:2ex;\">" + dtCmdSno.Rows[0]["NOTES"].ToString() + "</td></tr>";
                if (intPalletQty > 0) strPalletQty = intPalletQty.ToString();
                if (intPalletTaskQty > 0) strPalletTaskQty = intPalletTaskQty.ToString();
                if (intPalletQty != intPalletTaskQty) sKanbanModel = KanbanModel.PICKING;

            }

            #region WriteHtml
            try
            {
                //string sHtml = "<!DOCTYPE html><html><head><meta http - equiv = \"Content - Type\" content = \"text / html; charset = utf - 8\" /><title></title><meta charset = \"utf - 8\" http-equiv = \"refresh\" content = \"1\" /><style>* {font-family: 微软雅黑;margin: 0;padding: 0;}";
                //sHtml += "html {max-width: 100%;min-width: 100%;height: 100%;display: table;}body {max-width: 100%;min-width: 100%;display: table-cell;height: 100%;}body #frmMain {width: 100%;height: 100%;}body #frmMain #top_Main {position: relative;width: 100%-2px;height: 40%;margin: 0 auto;}";
                //sHtml += "body #frmMain #top_Main #top_Main_tab { width: 98.2%; height: 95%; position: absolute;top: 0;bottom: 0;left: 0;right: 0; margin: auto;}body #frmMain #top_Main #top_Main_tab tr #td_Left { width: 80%;height: 100%;border: 1px solid #808080;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab {width: 100%;height: 100%;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr1 td {width: 100%;height: 8%;border: 1px solid #808080;text-align: center;background-color: #0026ff;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td {width: 100%;height: 92%;border: 1px solid #808080;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td #td_Left_tr2_tab {width: 100%;height: 100%;}";
                //sHtml += "body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td #td_Left_tr2_tab tr .td_Left_tr2_tab_td1 {width: 40%;height: 25%;border: 1px solid #808080;text-align: center;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td #td_Left_tr2_tab tr .td_Left_tr2_tab_td2 {width: 60%;height: 25%;border: 1px solid #808080;text-align: center;}body #frmMain #follow_Main {position: relative;width: 100%-2px;height: 59.5%;margin: 0 auto;}";
                //sHtml += "body #frmMain #follow_Main #txt_MessageBox {width: 98%; height: 95%;position: absolute;top: 0;bottom: 0; left: 0;right: 0; margin: auto;font-size: 1.1em;border: 1px solid #000000;}#Pel_StnNo {width: 100%;height: 100%;border: 1px solid #808080;}#btn_Open {width: 100%;height: 100%;border: 1px solid #808080;font-size: 2.5em;}#MessageMain {text-align: center;font-size: 6em; }";
                //sHtml += "#CorporateName {text-align: center;color: white;}#txt_Message1, #txt_Message2, #txt_Message3, #txt_Message4, #txt_Message5, #txt_Message6 {width: 98%;height: 90%;font-size: 1.8em;text-align: left;}</style>";
                //sHtml += "</head><body><form id = \"frmMain\" ><div id=\"top_Main\"><table id = \"top_Main_tab\"><tr><td id=\"td_Left\"><table id = \"td_Left_tab\" ><tr id=\"td_Left_tr1\"><td><label id = \"CorporateName\" > 长塑 ASRS站口:" + sStnNo + "</label></td></tr><tr id = \"td_Left_tr2\" ><td><table id=\"td_Left_tr2_tab\"><tr>";
                //sHtml += "<td class=\"td_Left_tr2_tab_td1\" rowspan=\"2\" style=\"align-content: center; " + strColor + ";\"> <label id = \"MessageMain\" Width = \"100%\" > " + sKanbanModel + " </label></td>";
                //sHtml += "<td class=\"td_Left_tr2_tab_td2\"><div id = \"txt_Message1\" > " + strCommandID + " </div></td ></tr ><tr><td class=\"td_Left_tr2_tab_td2\">";
                //sHtml += "<div id = \"txt_Message2\" > " + strPalletNo + " </div>";
                //sHtml += "</td ></tr><tr><td class=\"td_Left_tr2_tab_td1\"><div id = \"txt_Message5\" > " + strPalletQty + "</div></td>";
                //sHtml += "<td class=\"td_Left_tr2_tab_td2\"><div id = \"txt_Message4\" > " + strCycNo + " </div></td></tr><tr>";
                //sHtml += "<td class=\"td_Left_tr2_tab_td1\"><div id = \"txt_Message6\" > " + strPalletTaskQty + "</div></td>";
                //sHtml += "<td class=\"td_Left_tr2_tab_td2\"><div id = \"txt_Message3\" >" + strEM + "</div></td ></tr></table></td></tr></table></td></tr></table></div>";
                //sHtml += "<div id=\"follow_Main\"><div id = \"txt_MessageBox\" >";
                //sHtml += "<table style=\"width:100%; height:100% \">";
                string sHtml = "<!DOCTYPE html><html><head><meta http-equiv=\"Content - Type\" content=\"text / html; charset = utf - 8\" />    <title></title>    <meta charset=\"utf - 8\" http-equiv=\"refresh\" content=\"1\" />    <style>        * {            font-family:微软雅黑;            margin: 0;            padding: 0;            background-color:#ffffff;            color:#000000;        }        html {            max-width: 100%;            min-width: 100%;            height: 100%;            display: table;        }        body {            max-width: 100%;            min-width: 100%;            display: table-cell;            height: 100%;        }        td {            border: 1px solid #808080;        }        body #frmMain {            width: 100%;            height: 100%;        }            body #frmMain #top_Main {                width: 100%-2px;                height: 100%;                margin: 0 auto;            }                body #frmMain #top_Main #td_Left_tr2_tab { width: 100%;    height: 100%;                    border: 1px solid #808080;                }                    body #frmMain #top_Main #td_Left_tr2_tab .td_Left_tr2_tab_td2{                    width: 50%;                    height: 4%;                    border: 1px solid #808080;                    font-size:2.5ex;                }    </style></head><body>    <form id=\"frmMain\">        <div id=\"top_Main\">  ";
                sHtml += " <table id=\"td_Left_tr2_tab\"><tr id=\"td_Left_tr1\">";
                sHtml += "<td colspan=\"3\" style=\"background-color:#4800ff;width:100%;height:3%;text-align:center;\">长塑ASRS站口:" + sStnNo + "</td></tr><tr><td class=\"td_Left_tr2_tab_td1\" rowspan=\"3\" style=\"text-align:center;color:#000000;" + strColor + "width:25%;height:8%;font-size:7.5ex\">";
                sHtml += sKanbanModel + "</td><td class=\"td_Left_tr2_tab_td2\">&nbsp;&nbsp;" + strCommandID + "</td>";
                sHtml += "<td class=\"td_Left_tr2_tab_td1\" rowspan=\"3\" style=\"text-align:center;color:#000000;" + strColor + "width:25%;height:8%;font-size:7.5ex\">" + strPalletQty + "/" + strPalletTaskQty + "</td>";
                sHtml += "</tr><tr><td class=\"td_Left_tr2_tab_td2\">&nbsp;&nbsp;" + strPalletNo + " </td></tr><tr><td class=\"td_Left_tr2_tab_td2\">&nbsp;&nbsp;" + strEM + " </td></tr>";
                sHtml += "<tr><td colspan=\"3\" valign=\"top\" style=\"text-left:center;width:100%;height:60%;font-size:2.5ex;\">";
                sHtml += " <table style=\"width:100%;\"><tr><td style=\"width:5%;\">序号</td><td style=\"width:27%;\">物料名称</td><td style=\"width:18%;\">辅组属性</td><td style=\"width:20%;\">分拣客户/线别</td><td style=\"width:30%;\">备注/箱号</td></tr>";
                sHtml += infoStyle.Info + "</table></td ></tr ></table ></div ></form ></body ></html > ";


                InitSys.funWriteHtml(sStnNo, sHtml);

                strMsg = bufferData[intBufferIndex]._BufferName + "|";
                strMsg += "Set Kanbaninfo Success!";
                funWriteSysTraceLog(strMsg);

                string[] strValues = new string[] { "1" };
                InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Clearnotice, strValues);
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }



        private bool funClearKanbanInfo(string stationNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            try
            {
                string sHtml = "<!DOCTYPE html><html><head><meta http-equiv=\"Content - Type\" content=\"text / html; charset = utf - 8\" />    <title></title>    <meta charset=\"utf - 8\" http-equiv=\"refresh\" content=\"1\" />    <style>        * {            font-family:微软雅黑;            margin: 0;            padding: 0;            background-color:#ffffff;            color:#000000;        }        html {            max-width: 100%;            min-width: 100%;            height: 100%;            display: table;        }        body {            max-width: 100%;            min-width: 100%;            display: table-cell;            height: 100%;        }        td {            border: 1px solid #808080;        }        body #frmMain {            width: 100%;            height: 100%;        }            body #frmMain #top_Main {                width: 100%-2px;                height: 100%;                margin: 0 auto;            }                body #frmMain #top_Main #td_Left_tr2_tab { width: 100%;    height: 100%;                    border: 1px solid #808080;                }                    body #frmMain #top_Main #td_Left_tr2_tab .td_Left_tr2_tab_td2{                    width: 50%;                    height: 4%;                    border: 1px solid #808080;                    font-size:2.5ex;                }    </style></head><body>    <form id=\"frmMain\">        <div id=\"top_Main\">  ";
                sHtml += " <table id=\"td_Left_tr2_tab\"><tr id=\"td_Left_tr1\">";
                sHtml += "<td colspan=\"3\" style=\"background-color:#4800ff;width:100%;height:3%;text-align:center;\">长塑ASRS站口:" + stationNo + "</td></tr><tr><td class=\"td_Left_tr2_tab_td1\" rowspan=\"3\" style=\"text-align:center;color:#000000;background-color:#b6ff00;width:25%;height:8%;font-size:7.5ex\">";
                sHtml += "长塑</td><td class=\"td_Left_tr2_tab_td2\">&nbsp;&nbsp;</td>";
                sHtml += "<td class=\"td_Left_tr2_tab_td1\" rowspan=\"3\" style=\"text-align:center;color:#000000;background-color:#ffffff;width:25%;height:8%;font-size:7.5ex\"></td>";
                sHtml += "</tr><tr><td class=\"td_Left_tr2_tab_td2\">&nbsp;&nbsp; </td></tr><tr><td class=\"td_Left_tr2_tab_td2\">&nbsp;&nbsp; </td></tr>";
                sHtml += "<tr><td colspan=\"3\" valign=\"top\" style=\"text-left:center;width:100%;height:60%;font-size:2.5ex;\">";
                sHtml += " <table style=\"width:100%;\"><tr><td style=\"width:5%;\">序号</td><td style=\"width:30%;\">物料名称</td><td style=\"width:15%;\">辅组属性</td><td style=\"width:20%;\">分拣客户</td><td style=\"width:30%;\">备注</td></tr></table>";
                sHtml += " </td > </tr ></table ></div ></form ></body ></html > ";
                InitSys.funWriteHtml(stationNo, sHtml);
                return true;
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return false;
            }
        }


        private string funGetPStnNo(string Item_No, string Stn_NO, ref string sItemName)
        {
            DataTable dtGetPStnNo = new DataTable();
            string strSQL = string.Empty;
            string strEM = string.Empty;
            try
            {

                strSQL = "SELECT * FROM ITEM_MST ";
                strSQL += " WHERE ITEM_NO='" + Item_No + "' ";
                if (Stn_NO.Substring(0, 1) == "A")
                {
                    strSQL += " and ITEM_TYPE='P'";

                }
                else if (Stn_NO.Substring(0, 1) == "B")
                {
                    strSQL += " and ITEM_TYPE='M'";
                }
                if (InitSys._DB.GetDataTable(strSQL, ref dtGetPStnNo, ref strEM))
                {
                    sItemName = dtGetPStnNo.Rows[0]["Item_Name"].ToString();
                    return dtGetPStnNo.Rows[0]["PStn_No"].ToString();
                }
                else
                {
                    return "目的位置获取失败!";
                }
            }
            catch (Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
                return "目的位置获取失败!";
            }
            finally
            {
                if (dtGetPStnNo != null)
                {
                    dtGetPStnNo.Clear();
                    dtGetPStnNo.Dispose();
                    dtGetPStnNo = null;
                }
            }
        }
    }
}
