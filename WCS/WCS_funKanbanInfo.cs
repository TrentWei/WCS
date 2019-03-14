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
                   bufferData[intBufferIndex]._Clearnotice &&
                   bufferData[intBufferIndex]._EQUStatus.Load == Buffer.Signal.Off &&
                   bufferData[intBufferIndex]._EQUStatus.AutoMode == Buffer.Signal.On)
                    {
                        funClearKanbanInfo(strBufferName);
                        string[] strValues = new string[] { "0" };
                        InitSys._MPLC.funWriteMPLC(bufferData[intBufferIndex]._W_Clearnotice, strValues);
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

        private bool funSetKanbanInfo(string sKanbanModel, string sStnNo, string sCmdSno, string SubNo, string Message, int intBufferIndex)
        {
            string strSQL = string.Empty;
            DataTable dtCmdSno = new DataTable();
            string strColor = string.Empty;
            string strCommandID = string.Empty;
            string strPalletNo = string.Empty;
            int intPalletQty = 0;
            int intPalletTaskQty = 0;
            string strPalletQty = string.Empty;
            string strPalletTaskQty = string.Empty;
            string strCycNo = string.Empty;
            List<KanbanInfoStyle> listInfo = new List<KanbanInfoStyle>();
            string strEM = string.Empty;
            if (sKanbanModel == KanbanModel.ERROR)
            {
                strColor = HtmlColor.Red;
                strEM = "异常:" + Message;
            }
            else
            {
                strSQL = "select * from CMD_MST ";
                strSQL += " where CMD_SNO='" + sCmdSno.PadLeft(5, '0') + "'";

                if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                {
                    CommandInfo commandInfo = new CommandInfo();
                    commandInfo.CommandID = dtCmdSno.Rows[0]["Cmd_Sno"].ToString();
                    commandInfo.CycleNo = dtCmdSno.Rows[0]["Cyc_No"].ToString();
                    commandInfo.PalletNo = dtCmdSno.Rows[0]["Plt_No"].ToString();
                    commandInfo.CommandMode = int.Parse(dtCmdSno.Rows[0]["Cmd_Mode"].ToString());
                    commandInfo.IOType = dtCmdSno.Rows[0]["Io_Type"].ToString();
                    commandInfo.WEIGH = dtCmdSno.Rows[0]["WEIGH"].ToString();
                    commandInfo.ACTUAL_WEIGHT = dtCmdSno.Rows[0]["ACTUAL_WEIGHT"].ToString();

                    if (!string.IsNullOrEmpty(commandInfo.CommandID)) strCommandID = "命令序号:" + commandInfo.CommandID;
                    if (!string.IsNullOrEmpty(commandInfo.PalletNo)) strPalletNo = "托盘单号:" + commandInfo.PalletNo;
                    if (!string.IsNullOrEmpty(commandInfo.CycleNo)) { strCycNo = "盘点单号:" + commandInfo.CycleNo; sKanbanModel = KanbanModel.CYC; }
                    if (!string.IsNullOrEmpty(commandInfo.WEIGH) && !string.IsNullOrEmpty(commandInfo.ACTUAL_WEIGHT)) strEM = "计划重量:" + commandInfo.WEIGH + "实际重量:" + commandInfo.ACTUAL_WEIGHT;
                    strSQL = "select case TKT_NO when null then 2 ELSE 1  end as GAODU,BOX_ID,ITEM_NO  from Box ";
                    strSQL += " where Sub_NO='" + SubNo + "' order by GAODU ";
                    if (InitSys._DB.GetDataTable(strSQL, ref dtCmdSno, ref strEM))
                    {
                        for (int i = 0; i < dtCmdSno.Rows.Count; i++)
                        {
                            KanbanInfoStyle infoStyle = new KanbanInfoStyle();
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
                            infoStyle.Info = "物料编号:" + dtCmdSno.Rows[i]["ITEM_NO"].ToString() + "  箱号:" + dtCmdSno.Rows[i]["BOX_ID"].ToString();
                            listInfo.Add(infoStyle);
                        }
                        switch (sKanbanModel)
                        {
                            case KanbanModel.CYC:
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

                    }
                }
            }

            if (listInfo.Count < 10)
            {
                int Count = listInfo.Count;
                for (int i = 0; i < 10 - Count; i++)
                {
                    KanbanInfoStyle infoStyle = new KanbanInfoStyle();
                    infoStyle.Color =" ";
                    infoStyle.Info = " ";
                    listInfo.Add(infoStyle);
                }
            }
            if (intPalletQty > 0) strPalletQty = "托盘数量:" + intPalletQty.ToString();
            if (intPalletTaskQty > 0) strPalletTaskQty = "托盘数量:" + intPalletTaskQty.ToString();
            try
            {
                string sHtml = "<!DOCTYPE html><html><head><meta http - equiv = \"Content - Type\" content = \"text / html; charset = utf - 8\" /><title></title><meta charset = \"utf - 8\" http-equiv = \"refresh\" content = \"1\" /><style>* {font-family: 微软雅黑;margin: 0;padding: 0;}";
                sHtml += "html {max-width: 100%;min-width: 100%;height: 100%;display: table;}body {max-width: 100%;min-width: 100%;display: table-cell;height: 100%;}body #frmMain {width: 100%;height: 100%;}body #frmMain #top_Main {position: relative;width: 100%-2px;height: 55%;margin: 0 auto;}";
                sHtml += "body #frmMain #top_Main #top_Main_tab { width: 98.2%; height: 95%; position: absolute;top: 0;bottom: 0;left: 0;right: 0; margin: auto;}body #frmMain #top_Main #top_Main_tab tr #td_Left { width: 80%;height: 100%;border: 1px solid #808080;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab {width: 100%;height: 100%;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr1 td {width: 100%;height: 8%;border: 1px solid #808080;text-align: center;background-color: #0026ff;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td {width: 100%;height: 92%;border: 1px solid #808080;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td #td_Left_tr2_tab {width: 100%;height: 100%;}";
                sHtml += "body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td #td_Left_tr2_tab tr .td_Left_tr2_tab_td1 {width: 40%;height: 25%;border: 1px solid #808080;text-align: center;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td #td_Left_tr2_tab tr .td_Left_tr2_tab_td2 {width: 60%;height: 25%;border: 1px solid #808080;text-align: center;}body #frmMain #follow_Main {position: relative;width: 100%-2px;height: 44.5%;margin: 0 auto;}";
                sHtml += "body #frmMain #follow_Main #txt_MessageBox {width: 98%; height: 95%;position: absolute;top: 0;bottom: 0; left: 0;right: 0; margin: auto;font-size: 1.3em;border: 1px solid #000000;}#Pel_StnNo {width: 100%;height: 100%;border: 1px solid #808080;}#btn_Open {width: 100%;height: 100%;border: 1px solid #808080;font-size: 2.5em;}#MessageMain {text-align: center;font-size: 6em; }";
                sHtml += "#CorporateName {text-align: center;color: white;}#txt_Message1, #txt_Message2, #txt_Message3, #txt_Message4, #txt_Message5, #txt_Message6 {width: 98%;height: 90%;font-size: 2.5em;text-align: left;}</style>";
                sHtml += "</head><body><form id = \"frmMain\" ><div id=\"top_Main\"><table id = \"top_Main_tab\"><tr><td id=\"td_Left\"><table id = \"td_Left_tab\" ><tr id=\"td_Left_tr1\"><td><label id = \"CorporateName\" > 长塑 ASRS站口:" + sStnNo + "</label></td></tr><tr id = \"td_Left_tr2\" ><td><table id=\"td_Left_tr2_tab\"><tr>";
                sHtml += "<td class=\"td_Left_tr2_tab_td1\" rowspan=\"2\" style=\"align-content: center; " + strColor + ";\"> <label id = \"MessageMain\" Width = \"100%\" > " + sKanbanModel + " </label></td>";
                sHtml += "<td class=\"td_Left_tr2_tab_td2\"><div id = \"txt_Message1\" > " + strCommandID + " </div></td ></tr ><tr><td class=\"td_Left_tr2_tab_td2\">";
                sHtml += "<div id = \"txt_Message2\" > " + strPalletNo + " </div>";
                sHtml += "</td ></tr><tr><td class=\"td_Left_tr2_tab_td1\"><div id = \"txt_Message5\" > " + strPalletQty + "</div></td>";
                sHtml += "<td class=\"td_Left_tr2_tab_td2\"><div id = \"txt_Message4\" > " + strCycNo + " </div></td></tr><tr>";
                sHtml += "<td class=\"td_Left_tr2_tab_td1\"><div id = \"txt_Message6\" > " + strPalletTaskQty + "</div></td>";
                sHtml += "<td class=\"td_Left_tr2_tab_td2\"><div id = \"txt_Message3\" >" + strEM + "</div></ td ></tr></table></td></tr></table></td></tr></table></div>";
                sHtml += "<div id=\"follow_Main\"><div id = \"txt_MessageBox\" >";
                sHtml += "<table style=\"width:100%; height:100% \">";
                sHtml += "<tr>";
                sHtml += "<td style=\"border: 1px solid #808080;width:50%;height:20%; " + listInfo[0].Color + " \">" + listInfo[0].Info + "</td>";
                sHtml += "<td style=\"border: 1px solid #808080;width:50%;height:20%; " + listInfo[1].Color + " \">" + listInfo[1].Info + "</td>";
                sHtml += "</tr>";
                sHtml += "<tr>";
                sHtml += "<td style=\"border: 1px solid #808080;width:50%;height:20%; " + listInfo[2].Color + " \">" + listInfo[2].Info + "</td>";
                sHtml += "<td style=\"border: 1px solid #808080;width:50%;height:20%; " + listInfo[3].Color + " \">" + listInfo[3].Info + "</td>";
                sHtml += "</tr>";
                sHtml += "<tr>";
                sHtml += "<td style=\"border: 1px solid #808080;width:50%;height:20%; " + listInfo[4].Color + " \">" + listInfo[4].Info + "</td>";
                sHtml += "<td style=\"border: 1px solid #808080;width:50%;height:20%; " + listInfo[5].Color + " \">" + listInfo[5].Info + "</td>";
                sHtml += "</tr>";
                sHtml += "<tr>";
                sHtml += "<td style=\"border: 1px solid #808080;width:50%;height:20%; " + listInfo[6].Color + " \">" + listInfo[6].Info + "</td>";
                sHtml += "<td style=\"border: 1px solid #808080;width:50%;height:20%; " + listInfo[7].Color + " \">" + listInfo[7].Info + "</td>";
                sHtml += "</tr>";
                sHtml += "<tr>";
                sHtml += "<td style=\"border: 1px solid #808080;width:50%;height:20%; " + listInfo[8].Color + " \">" + listInfo[8].Info + "</td>";
                sHtml += "<td style=\"border: 1px solid #808080;width:50%;height:20%; " + listInfo[9].Color + " \">" + listInfo[9].Info + "</td>";
                sHtml += "</tr>";
                sHtml += "</table>";
                sHtml += "</div></div></form></body></html>";

                InitSys.funWriteHtml(sStnNo, sHtml);

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



        private bool funClearKanbanInfo(string stationNo)
        {
            string strSQL = string.Empty;
            string strEM = string.Empty;
            if (stationNo == "A13" || stationNo == "A09")
            {
                stationNo = "A01";
            }
            if (stationNo == "A03" || stationNo == "A08")
            {
                stationNo = "A02";
            }
            if (stationNo == "B06") stationNo = "B04";
            if (stationNo == "B09") stationNo = "B07";
            try
            {
                string sHtml = "<!DOCTYPE html><html><head><meta http - equiv = \"Content - Type\" content = \"text / html; charset = utf - 8\" /><title></title><meta charset = \"utf - 8\" http-equiv = \"refresh\" content = \"1\" /><style>* {font-family: 微软雅黑;margin: 0;padding: 0;}";
                sHtml += "html {max-width: 100%;min-width: 100%;height: 100%;display: table;}body {max-width: 100%;min-width: 100%;display: table-cell;height: 100%;}body #frmMain {width: 100%;height: 100%;}body #frmMain #top_Main {position: relative;width: 100%-2px;height: 55%;margin: 0 auto;}";
                sHtml += "body #frmMain #top_Main #top_Main_tab { width: 98.2%; height: 95%; position: absolute;top: 0;bottom: 0;left: 0;right: 0;margin: auto;}body #frmMain #top_Main #top_Main_tab tr #td_Left { width: 80%;height: 100%;border: 1px solid #808080;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab {width: 100%;height: 100%;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr1 td {width: 100%;height: 8%;border: 1px solid #808080;text-align: center;background-color: #0026ff;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td {width: 100%;height: 92%;border: 1px solid #808080;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td #td_Left_tr2_tab {width: 100%;height: 100%;}";
                sHtml += "body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td #td_Left_tr2_tab tr .td_Left_tr2_tab_td1 {width: 40%;height: 25%;border: 1px solid #808080;text-align: center;}body #frmMain #top_Main #top_Main_tab tr #td_Left #td_Left_tab #td_Left_tr2 td #td_Left_tr2_tab tr .td_Left_tr2_tab_td2 {width: 60%;height: 25%;border: 1px solid #808080;text-align: center;}body #frmMain #follow_Main {position: relative;width: 100%-2px;height: 44.5%;margin: 0 auto;}";
                sHtml += "body #frmMain #follow_Main #txt_MessageBox {width: 98%; height: 95%;position: absolute;top: 0;bottom: 0; left: 0;right: 0;margin: auto;font-size: 1.3em;border: 1px solid #000000;}#Pel_StnNo {width: 100%;height: 100%;border: 1px solid #808080;}#btn_Open {width: 100%;height: 100%;border: 1px solid #808080;font-size: 2.5em;}#MessageMain {text-align: center;font-size: 6em; }";
                sHtml += "#CorporateName {text-align: center;color: white;}#txt_Message1, #txt_Message2, #txt_Message3, #txt_Message4, #txt_Message5, #txt_Message6 {width: 98%;height: 90%;font-size: 2.5em;text-align: left;}</style>";
                sHtml += "</head><body><form id = \"frmMain\" ><div id=\"top_Main\"><table id = \"top_Main_tab\"><tr><td id=\"td_Left\"><table id = \"td_Left_tab\" ><tr id=\"td_Left_tr1\"><td><label id = \"CorporateName\" > 鲁达:ASRS</label></td></tr><tr id = \"td_Left_tr2\" ><td><table id=\"td_Left_tr2_tab\"><tr>";
                sHtml += "<td class=\"td_Left_tr2_tab_td1\" rowspan=\"2\" style=\"align-content: center; background-color:#ff0000;\"><label id = \"MessageMain\"  Width=\"100%\">鲁达</label></td>";
                sHtml += "<td class=\"td_Left_tr2_tab_td2\"><div id = \"txt_Message1\" > 噪声有害: </div></td ></tr ><tr><td class=\"td_Left_tr2_tab_td2\">";
                sHtml += "<div id = \"txt_Message2\" >必须戴护耳器！ </div>";
                sHtml += "</td ></tr><tr><td class=\"td_Left_tr2_tab_td1\"><div id = \"txt_Message5\" > </div></td>";
                sHtml += "<td class=\"td_Left_tr2_tab_td2\"><div id = \"txt_Message4\" > 注意防尘: </div></td></tr><tr>";
                sHtml += "<td class=\"td_Left_tr2_tab_td1\"><div id = \"txt_Message6\" > </div></td>";
                sHtml += "<td class=\"td_Left_tr2_tab_td2\"><div id = \"txt_Message3\" > 须配戴防尘口罩，注意通风 </div></ td ></tr></table></td></tr></table></td></tr></table></div>";
                sHtml += "<div id=\"follow_Main\"><div id = \"txt_MessageBox\" ><p></p></div></div></form></body></html>";
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
