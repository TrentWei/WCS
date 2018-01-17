using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using Ionic.Zip;

namespace Mirle.ASRS
{
    public class Archive
    {
        #region 常數
        public const double cdobCheckInterval = 3600000;
        public const int cintZIPLogDay = 30;
        public const int cintDeleteLogDay = 90;
        #endregion 常數

        #region 變數
        private int intMinZipLogDays;
        private int intMinDeleteLogDays;
        private double dobCheckInterval;
        private string strLogPath;
        private string strArchiveTargetPath;
        #endregion 變數

        private Timer timArchive = new Timer();

        #region 屬性
        public double _CheckInterval
        {
            get { return dobCheckInterval; }
            set
            {
                if(value > cdobCheckInterval)
                    dobCheckInterval = value;
                else
                    dobCheckInterval = cdobCheckInterval;
            }
        }

        public string _LogPath
        {
            get { return strLogPath; }
            set { strLogPath = value; }
        }

        public string _ArchiveTargetPath
        {
            get { return strArchiveTargetPath; }
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    strArchiveTargetPath = strLogPath;
                else
                    strArchiveTargetPath = value;
            }
        }

        public int _MinZipDays
        {
            get
            {
                if(intMinZipLogDays < 0)
                    intMinZipLogDays = cintZIPLogDay;
                return intMinZipLogDays;
            }
            set { intMinZipLogDays = value; }
        }

        public int _MinDeleteDays
        {
            get
            {
                if(intMinDeleteLogDays < 0)
                    intMinDeleteLogDays = cintDeleteLogDay;
                return intMinDeleteLogDays;
            }
            set { intMinDeleteLogDays = value; }
        }
        #endregion 屬性

        #region 建构式
        public Archive()
        {
        }

        public Archive(string logPath, double checkInterval, string archiveTargetPath, int minZipDays, int minDeleteDays)
        {
            _LogPath = logPath;
            _CheckInterval = checkInterval;
            _ArchiveTargetPath = archiveTargetPath;
            _MinZipDays = minZipDays;
            _MinDeleteDays = minDeleteDays;
        }
        #endregion 建构式

        #region 方法
        public void funStart()
        {
            if(timArchive == null)
                timArchive = new Timer();
            timArchive.Interval = _CheckInterval;
            timArchive.Elapsed += this.timArchive_Elapsed;
            timArchive.Start();
        }

        public void funStop()
        {
            timArchive.Stop();
        }

        private void timArchive_Elapsed(object sender, ElapsedEventArgs e)
        {
            timArchive.Enabled = false;
            try
            {
                #region 删除暂存压缩檔
                DirectoryInfo dtiDeleteInfo = new DirectoryInfo(_ArchiveTargetPath);
                foreach(FileInfo fileInfo in dtiDeleteInfo.GetFiles("*.tmp"))
                {
                    if(fileInfo.CreationTime <= DateTime.Now.AddMinutes(-360))
                    {
                        fileInfo.Delete();
                        InitSys.funWriteLog("Archive", "Delete Tmp File Success|" + fileInfo.Name);
                    }
                }
                #endregion 删除暂存压缩檔

                #region 删除压缩檔
                foreach(FileInfo fileInfo in dtiDeleteInfo.GetFiles("*.zip"))
                {
                    if(fileInfo.CreationTime <= DateTime.Now.AddDays(_MinDeleteDays * -1))
                    {
                        fileInfo.Delete();
                        InitSys.funWriteLog("Archive", "Delete ZIP File Success|" + fileInfo.Name);
                    }
                }
                #endregion 删除压缩檔

                #region 檔案压缩
                DirectoryInfo dtiZIPInfo = new DirectoryInfo(_LogPath);
                foreach(DirectoryInfo directoryInfo in dtiZIPInfo.GetDirectories())
                {
                    DateTime objDateTime = DateTime.Now;
                    if(DateTime.TryParse(directoryInfo.Name, out objDateTime))
                    {
                        if(objDateTime <= DateTime.Now.AddDays(_MinZipDays * -1))
                        {
                            bool bolFlag = false;
                            using(ZipFile zipFile = new ZipFile(_ArchiveTargetPath + directoryInfo.Name + @".zip"))
                            {
                                foreach(FileInfo fileInfo in directoryInfo.GetFiles())
                                {
                                    if(!zipFile.EntryFileNames.Contains(fileInfo.Name))
                                        zipFile.AddFile(string.Format(@"{0}\{1}\{2}", _LogPath, directoryInfo.Name, fileInfo.Name), "");
                                }
                                zipFile.Save();
                                InitSys.funWriteLog("Archive", "ZIP Directory Success|" + directoryInfo.Name);
                                bolFlag = true;
                            }
                            if(bolFlag)
                            {
                                directoryInfo.Delete(true);
                                InitSys.funWriteLog("Archive", "Delete Directory Success|" + directoryInfo.Name);
                            }
                        }
                    }
                }
                #endregion 檔案压缩
            }
            catch(Exception ex)
            {
                MethodBase methodBase = MethodBase.GetCurrentMethod();
                InitSys.funWriteLog("Exception", methodBase.DeclaringType.FullName + "|" + methodBase.Name + "|" + ex.Message);
            }
            finally
            {
                timArchive.Enabled = true;
            }
        }
        #endregion 方法
    }
}