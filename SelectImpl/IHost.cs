using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectImpl
{
    public interface IHost
    {
        void uiStartProcessBar();
        void uiSetProcessBar(String msgIn, float percentIn);
        void uiFinishProcessBar();
        void uiSetMsg(string msgIn);
        void uiSetTradeDay();
    }
}
