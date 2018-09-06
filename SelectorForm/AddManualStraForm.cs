using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SelectImpl;
using System.Data.SQLite;

namespace SelectorForm
{
    public partial class AddManualStraForm : Form
    {
        public String sName_;
        public String sRateItemKeys_;
        public AddManualStraForm()
        {
            InitializeComponent();
            DataTable rateItemDt = new DataTable();
            rateItemDt.Columns.Add("Key");
            dataGridRateItemKey_.DataSource = rateItemDt;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string sName = textName_.Text.Trim();
            if (String.IsNullOrEmpty(sName))
            {
                MessageBox.Show("Must provide Name!", "Selector");
                return;
            }
            String sSQL = String.Format("Select Count(1) From manual_stra Where straname = '{0}'", sName);
            if (DB.Global().ExecuteScalar<int>(sSQL) > 0)
            {
                MessageBox.Show("Name already exist!", "Selector");
                return;
            }
            List<String> rateItemKeyList = new List<string>();
            DataTable dt = (DataTable)dataGridRateItemKey_.DataSource;
            foreach (DataRow row in dt.Rows)
            {
                rateItemKeyList.Add(row["Key"].ToString().Trim());
            }
            String sRateItemKeys = String.Join(",", rateItemKeyList);
            sSQL = String.Format("Insert Into manual_stra (straname, rateItemKeys) Values ('{0}', '{1}')", sName, sRateItemKeys);
            DB.Global().Execute(sSQL);
            SQLiteHelper sh = DB.RegConn(Dist.manualStraPath_+sName, sName);
            App.asset_.createStraTable(sh);
            sh.Execute(@"CREATE TABLE selectitems (
                                id           INTEGER          PRIMARY KEY AUTOINCREMENT,
                                code       VARCHAR( 10 )              NOT NULL,
                                name       VARCHAR( 20 )              NOT NULL,
                                date              INT              NOT NULL,
                                rateItems  VARCHAR( 4000 )  
                            );
                    ");
            sName_ = sName;
            sRateItemKeys_ = sRateItemKeys;
            MessageBox.Show("Add Success.", "Selector");
            this.DialogResult = DialogResult.OK;
            
        }
    }
}
