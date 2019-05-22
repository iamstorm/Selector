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
            if (dsh.Ref(Info.HF) > 0.095) {
                return null;
            }
            if (dsh.Ref(Info.LF) < -0.095) {
                return null;
            }
            
            var lastDayC = dsh.Ref(Info.C, 1);
            var rsh = dsh.newRuntimeDsh();

            int globalLLIndex = rsh.LL(Info.C, -1);
            int globalHHIndex = rsh.HH(Info.C, -1);
            int iLLIndex = rsh.LL(Info.C, -1, startMinuteCount_);
            int iReverseHHIndex = rsh.ReverseHH(Info.C, -1, iLLIndex);

            var c = rsh.Ref(Info.C);
            var o = rsh.Ref(Info.O);
            var llc = rsh.Ref(Info.C, iLLIndex);
            var gllc = rsh.Ref(Info.C, globalLLIndex);

            if (c >= lastDayC) {
                return null;
            }

            if (c > gllc) {
                return null;
            }
            
            var reverseHHc = rsh.Ref(Info.C, iReverseHHIndex);
            if (reverseHHc < o) {
                return null;
            }
            var deltaUp = Utils.DeltaF(reverseHHc - llc, lastDayC);
            if (deltaUp < 0.02) {
                return null;
            }
            if ((iLLIndex - rsh.stock_.runtimeDataList_.Count + 1) < 5) {
                if (c > 0.988*gllc) {
                    return null;
                }
            }

            return EmptyRateItemButSel;
        }
    }
}
