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
            if (dsh.Ref(Info.LF) < -0.095) {
                return null;
            }
            if (dsh.EveryDown() > 2) {
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

            int iLLIndex = rsh.LL(Info.C, -1, startMinuteCount_);
            int iReverseHHIndex = rsh.ReverseHH(Info.C, -1, iLLIndex);

            var llc = rsh.Ref(Info.C, iLLIndex);
            var reverseHHc = rsh.Ref(Info.C, iReverseHHIndex);

            if (c > llc) {
                return null;
            }
            if (reverseHHc < o) {
                return null;
            }
            var deltaUp = Utils.DeltaF(reverseHHc - llc, lastDayC);
            if (deltaUp < 0.02) {
                return null;
            }
            if ((iLLIndex - rsh.stock_.runtimeDataList_.Count + 1) < 5) {
                if (c > 0.988 * llc) {
                    return null;
                }
            }

            return EmptyRateItemButSel;
        }
    }
}
