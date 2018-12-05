using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectImpl
{
    public abstract class CloseBuyStrategy : BaseStrategyImpl
    {
        public virtual int buySpan()
        {
            return 1;
        }
        public virtual float firstDayBonusLimit()
        {
            return 0.095f;
        }
        public override float bonusLimit()
        {
            return 0.095f;
        }
        public virtual String computeBonus(SelectItem item, Stock stock, int buyDate, out BuySellInfo info)
        {
            IStrategy stra = (IStrategy)this;
            float firstDayBonusL = firstDayBonusLimit();
            float bonusL = bonusLimit();
            DataStoreHelper dsh = new DataStoreHelper();
            dsh.setStock(stock);
            dsh.iIndex_ = App.ds_.index(stock, buyDate);
            float buyC = dsh.Ref(Info.C);
            --dsh.iIndex_;
            info = new BuySellInfo();
            if (dsh.iIndex_ == -1)
            {
                return "";// 还未遇到交易日，不知道盈亏
            }
            int nBuySpan = 0;
            int nBuySpanLimit = buySpan();
            float sellC = 0;
            for (; dsh.iIndex_ >= 0; --dsh.iIndex_, ++nBuySpan)
            {
                float o = dsh.Ref(Info.O);
                float h = dsh.Ref(Info.H);
                float c = dsh.Ref(Info.C);

                float of = dsh.Ref(Info.OF);
                float hf = dsh.Ref(Info.HF);
                float lf = dsh.Ref(Info.LF);
                float zf = dsh.Ref(Info.ZF);

                //           dsh.iSZIndex_ = App.ds_.index(App.ds_.szListData_, dsh.Date());

                var limit = nBuySpan == 0 ? firstDayBonusL : bonusL;
                float wantedC = buyC * (1 + limit);
                float wantedMinC = buyC * (1 + bonusL);
                // 
                //                 if (lf < -0.111 || hf > 0.111)
                //                 {//除权了
                //                     return "";
                //                 }
                if (Utils.IsDownStop(hf))//一直跌停
                {
                    info.bSellWhenMeetMyBounusLimit_ = false;
                    continue;
                }
                //今天没一直跌停，大概率能卖出
                if (info.bSellWhenMeetMyBounusLimit_)
                {
                    if (o > wantedC)//开盘超出盈利顶额
                    {
                        sellC = o;
                        ++nBuySpan;
                        break;
                    }
                    if (h > wantedC)//达到盈利顶额
                    {
                        sellC = wantedC;
                        ++nBuySpan;
                        break;
                    }
                    if (Utils.IsDownStop(zf))//尾盘跌停，卖不了
                    {
                        info.bSellWhenMeetMyBounusLimit_ = false;
                        continue;
                    }
                    sellC = c;

                    if (zf >= 0.01 || h >= wantedMinC || nBuySpan >= nBuySpanLimit - 1)
                    {
                        ++nBuySpan;
                        break;
                    }
                }
                else
                {//逃命模式
                    if (!Utils.IsDownStop(of))//开盘没跌停
                    {//跑了
                        sellC = o;
                        ++nBuySpan;
                        break;
                    }
                    //开盘跌停但因为没一直跌停，所以肯定可以以跌停价卖出
                    sellC = dsh.Ref(Info.C, 1) * (1 - 0.095f);
                    ++nBuySpan;
                    break;
                }
            }
            if (dsh.iIndex_ == -1)
            {
                return "";
            }
            info.sellDate_ = dsh.Date();
            info.tradeSpan_ = nBuySpan;
            float bonus = (sellC - buyC) / buyC;
            info.allBonus_ = Utils.ToBonus(bonus - 0.002f);
            return info.allBonus_;
        }
    }
}
