using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SelectImpl
{
    public interface IHost
    {
        void uiStartProcessBar();
        void uiSetProcessBar(String msgIn, float percentIn);
        void uiFinishProcessBar();
        void uiSetMsg(string msgIn);
        void uiSetTradeDay();
        void uiReportSelectMsg(String sMsg, bool bImportant);
        bool uiAutoSelectMode();
        DialogResult uiMessageBox(String msgIn, MessageBoxButtons buttons);
    }
}
