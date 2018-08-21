using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
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

        Dictionary<String, String> select(DataStoreHelper dsh, Dictionary<String, String> param);
    }

}
