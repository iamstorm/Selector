using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectorForm
{
    public interface IStrategy
    {
#region meta data
        int version();
        String name();
        Dictionary<String, String> paramters();
        String[] rateItemNames();
        bool focusOnNew();
#endregion
        Dictionary<String, String> setup();

        String[] select(DataStore store, Stock stock, int iIndex, Dictionary<String, String> param);
        int rate(String[] rateItems);
    }

}
