import os
import sqlite3
import datetime
import pandas as pd
import sys
from . import Utils

tu = Utils.InitTuShare()

def FetchRunTimeData():
    df = tu.ts.get_today_all()
    df = df.loc[:, ['code','trade','open','high','low','volume','amount']]
    dfValid = df[df['open']>0]
    dfValid = Utils.NormlizePrice(dfValid, ['open', 'high', 'low', 'trade'])
    return dfValid

def Fetch000001RunTimeData():
    df = tu.ts.get_realtime_quotes('sh')
    df.rename(columns={'price':'close', 'volume':'vol'}, inplace = True)
    df = df.loc[:,['open','close','high','low','vol']]
    return df

def UpdateRunTime(bindir):
    datadir = os.path.join(bindir, "data")
    now = datetime.datetime.now();
    dbFile = Utils.GetGlobalFilePath(bindir)
    conn = sqlite3.connect(dbFile)
    runtime_time = int(Utils.GetValFromSys(conn, 'runtime_time', defVal='0'))
    if runtime_time>int(now.date().strftime("%Y%m%d15")):
        return
    Utils.Info("开始下载实时数据...")
    nowtime = int(now.date().strftime("%Y%m%d")+now.time().strftime("%H"))
    df = FetchRunTimeData()
    conn.execute("Delete From runtime");
    conn.commit()
    df.drop_duplicates(['code'], inplace=True)
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
    try:
        UpdateRunTime(bindir)
    except:
        print(sys.exc_info())
        traceback.print_exc(file=sys.stdout)
        raise    
    Utils.MarkAsSuc(bindir)
