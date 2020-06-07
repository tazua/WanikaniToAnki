using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WanikaniToAnki
{
    public partial class Form2 : Form
    {
        public string ReturnValue1 { get; set; }
        public Form2()
        {
            InitializeComponent();
            this.Shown += new System.EventHandler(this.Form2_Shown);
            button1.DialogResult = DialogResult.OK;
            AcceptButton = button1;
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            
            listBox1.BeginUpdate();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (Directory.Exists(d.Name + "Users\\"))
                {
                    foreach (string b in Directory.GetDirectories(d.Name + "Users\\"))
                    {
                        if (!(b.EndsWith("Public") || b.EndsWith("Default") || b.EndsWith("Default User") || b.EndsWith("All Users")))
                        {
                            if (Directory.Exists(b + "\\AppData\\Roaming\\Anki2"))
                            {
                                foreach (string c in Directory.GetDirectories(b + "\\AppData\\Roaming\\Anki2"))
                                {
                                    if (!(c.EndsWith("addons") || c.EndsWith("addons21")))
                                    {
                                        Console.WriteLine(c);
                                        listBox1.Items.Add(c);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            listBox1.EndUpdate();
            if (listBox1.Items.Count==1)
            {
                this.ReturnValue1 = listBox1.Items[0].ToString() + "\\collection.media";
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(listBox1.SelectedItem.ToString());
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.ReturnValue1 = listBox1.SelectedItem.ToString()+ "\\collection.media";
        }
    }
}
