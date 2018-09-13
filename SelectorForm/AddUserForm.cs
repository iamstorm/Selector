using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NFine.Code;
using SelectImpl;

namespace SelectorForm
{
    public partial class AddUserForm : Form
    {
        public AddUserForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            String sName = textName_.Text.Trim();
            String sPsw = textPsw_.Text.Trim();
            if (sName == "")
            {
                MessageBox.Show("Must provide name!", "Selector");
                return;
            }
            if (sPsw == "")
            {
                MessageBox.Show("Must provide password!", "Selector");
                return;
            }
            if (DB.Global().ExecuteScalar<int>(String.Format("Select Count(1) From User Where name = '{0}'", sName)) > 1)
            {
                MessageBox.Show("Name already exist!", "Selector");
                return;
            }
            String sKey = Md5.md5(Md5.CreateNo(), 16).ToLower();
            String sMd5Psw = Md5.md5(DESEncrypt.Encrypt(Md5.md5(sPsw, 32).ToLower(), sKey).ToLower(), 32).ToLower();
            Dictionary<String, Object> rowDict = new Dictionary<String, Object>();
            rowDict["name"] = sName;
            rowDict["psw"] = sMd5Psw;
            rowDict["key"] = sKey;
            DB.Global().Insert("User", rowDict);
            MessageBox.Show("Add Success.", "Selector");
            Close();
        }
    }
}
