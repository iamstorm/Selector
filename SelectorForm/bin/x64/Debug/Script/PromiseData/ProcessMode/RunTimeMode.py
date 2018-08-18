import os
import sqlite3
import datetime
import pandas as pd
from . import Utils

tu = Utils.InitTuShare()

def FetchRunTimeData():
    df = tu.ts.get_today_all()
    return df[['code','trade','open','high','low','volume','amount']]

def Fetch000001RunTimeData():
    df = tu.ts.get_realtime_quotes('sh')
    df.rename(columns={'price':'close', 'volumn':'vol'}, inplace = True)
    df = df[:,['open','close','high','low','vol']]
    Utils.NormlizePrice(df, 'open')
    Utils.NormlizePrice(df, 'high')
    Utils.NormlizePrice(df, 'low')
    Utils.NormlizePrice(df, 'close')
    return df

def UpdateRunTime(bindir):
    datadir = os.path.join(bindir, "data")
    now = datetime.datetime.now();
    curYear = now.date().year
    dbFile = Utils.GetYearDayFilePath(datadir, curYear)
    conn = sqlite3.connect(dbFile)
    runtime_time = int(Utils.GetValFromSys(conn, 'runtime_time', defVal='0'))
    if runtime_time>int(now.date().strftime("%Y%m%d15")):
        return
    Utils.Info("开始下载实时数据...")
    nowtime = int(now.date().strftime("%Y%m%d")+now.time().strftime("%H"))
    df = FetchRunTimeData()
    conn.execute("Delete From runtime");
    conn.commit()
    df.to_sql(name='runtime', con=conn, if_exists='append', index=False)

    df = Fetch000001RunTimeData()
    conn.execute("Delete From [000001runtime]");
    conn.commit()
    df.to_sql(name='000001runtime', con=conn, if_exists='append', index=False)

    Utils.SetValToSys(conn, 'runtime_time', nowtime)
    conn.commit()
    Utils.Info("完成实时数据获取。")

    conn.close()

def Run(bindir):
    UpdateRunTime(bindir)
    Utils.MarkAsSuc(bindir)
