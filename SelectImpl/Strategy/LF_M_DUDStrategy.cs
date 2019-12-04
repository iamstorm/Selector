using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class LF_M_DUDStrategy : LFMBuyStrategy, IStrategy_LF_M
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "LF_M_DUD";
        }
        #endregion

        const int startMinuteCount_ = 30;

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            IStrategy_LF_M stra = (IStrategy_LF_M)this;
            int nowMiniute = Utils.NowMinute();
            if (nowMiniute < 60) {
                return null;
            }
            if (dsh.stock_.name_.ToUpper().IndexOf("ST") != -1) {
                return null;
            }
            if (dsh.Ref(Info.ZF, dsh.HH(Info.ZF, 10)) > 0.095) {
                return null;
            }
            if (dsh.Ref(Info.ZF, dsh.LL(Info.ZF, 10)) < -0.095) {
                return null;
            }
            var hf = dsh.Ref(Info.HF);
            if (hf > 0.05 || hf < 0.01) {
                return null;
            }
            if (dsh.Ref(Info.LF) < -0.06) {
                return null;
            }

            var lastDayC = dsh.Ref(Info.C, 1);
            var gllc = dsh.Ref(Info.L);
            var ghhc = dsh.Ref(Info.H);
            var c = dsh.Ref(Info.C);
            var o = dsh.Ref(Info.O);

            if (c >= lastDayC) {
                return null;
            }

            if (ghhc <= lastDayC) {
                return null;
            }
            var rsh = dsh.newRuntimeDsh();

            var c5 = rsh.Ref(Info.C, 5);
            if ((c - c5)/lastDayC > -0.01) {
                return null;
            }
            var c10 = rsh.Ref(Info.C, 10);
            if ((c - c10)/lastDayC > -0.015) {
                return null;
            }
            var c15 = rsh.Ref(Info.C, 15);
            if ((c - c15)/lastDayC > -0.02) {
                return null;
            }
            var c20 = rsh.Ref(Info.C, 20);
            if ((c - c20)/lastDayC < -0.025) {
                return null;
            }
            var c25 = rsh.Ref(Info.C, 25);
            if ((c - c25)/lastDayC < -0.03) {
                return null;
            }
            var c30 = rsh.Ref(Info.C, 30);
            if ((c - c30)/lastDayC > -0.035) {
                return null;
            }
       
            return EmptyRateItemButSel;
        }
    }
}
