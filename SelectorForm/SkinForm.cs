using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SelectImpl;

namespace SelectorForm
{
    public partial class SkinForm : Form
    {
        public bool bWantedSkinEngineIsActive = false;
        public SkinForm()
        {
            InitializeComponent();

            if (MainForm.Me.skinEngine_ == null)
            {
                MainForm.Me.skinEngine_ = new Sunisoft.IrisSkin.SkinEngine();
            }
            else
            {
                bWantedSkinEngineIsActive = MainForm.Me.skinEngine_.Active;
            }
            FileInfo[] infos = new DirectoryInfo(Dist.binPath_ + "Skins").GetFiles();
            skinList_.DataSource = infos;
            skinList_.DisplayMember = "Name";


            var skinName  = Utils.GetSysInfo(DB.Global(), "SkinName", "");
            if (skinName != "")
            {
                for (int i = 0; i < infos.Length; i++)
			    {
			        if (String.Equals(skinName, infos[i].Name, StringComparison.CurrentCultureIgnoreCase))
	                {
		                skinList_.SelectedIndex = i;
                        break;
	                }
			    }
            }
            MainForm.Me.skinEngine_.Active = true;
        }

        private void skinList__SelectedIndexChanged(object sender, EventArgs e)
        {
            if (skinList_.SelectedItem != null)
                MainForm.Me.skinEngine_.SkinFile = (skinList_.SelectedItem as FileInfo).FullName;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            bWantedSkinEngineIsActive = true;
            Utils.SetSysInfo(DB.Global(), "SkinName", skinList_.SelectedItem.ToString());
            Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            MainForm.Me.skinEngine_.Active = false;
            bWantedSkinEngineIsActive = false;
            Utils.SetSysInfo(DB.Global(), "SkinName", "");
            Close();
        }

        private void SkinForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.Me.skinEngine_.Active = bWantedSkinEngineIsActive;
        }
    }
}
