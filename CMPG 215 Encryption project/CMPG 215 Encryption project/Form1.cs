using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CMPG_215_Encryption_project
{
    public partial class Form1 : Form
    {
        byte[] abc;
        byte[,] table;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Multiselect = false;
            if(od.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = od.FileName;
            }
        }

        private void rbEncrypt_CheckedChanged(object sender, EventArgs e)
        {
            if(rbEncrypt.Checked)
            {
                rbDecrypt.Checked = false;
            }
        }

        private void rbDecrypt_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDecrypt.Checked)
            {
                rbEncrypt.Checked = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rbEncrypt.Checked = true;

            abc = new byte[256];
            for(int i = 0; i < 256; i++)
            {
                abc[i] = Convert.ToByte(i);
            }

            table = new byte[256, 256];
            for(int i = 0; i < 256; i++)
            {
                for(int j = 0; j < 256; j++)
                {
                    table[i, j] = abc[(i + j) % 256];
                }
            }


        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if(!File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("File does not exist");
                return;
            }

            if(String.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Please enter a password");
                return;
            }


            try
            {
                byte[] fileContent = File.ReadAllBytes(txtFilePath.Text);
                byte[] passwordTemp = Encoding.ASCII.GetBytes(txtPassword.Text);
                byte[] keys = new byte[fileContent.Length];
                for(int i = 0; i < fileContent.Length; i++)
                {
                    keys[i] = passwordTemp[i % passwordTemp.Length];
                }

                //Encryption
                byte[] result = new byte[fileContent.Length];

                if (rbEncrypt.Checked)
                {
                    
                    for(int i = 0; i < fileContent.Length; i++)
                    {
                        byte value = fileContent[i];
                        byte key = keys[i];
                        int valueIndex = -1, keyIndex = -1;
                        for (int j = 0; i < 256; j++)
                        {
                            if(abc[j] == value)
                            {
                                valueIndex = j;
                                break;
                            }
                        }
                        for (int j = 0; i < 256; j++)
                        {
                            if (abc[j] == key)
                            {
                                keyIndex = j;
                                break;
                            }
                        }

                        result[i] = table[keyIndex, valueIndex];
                    }
                }
                //Decryption
                else
                {
                    for (int i = 0; i < fileContent.Length; i++)
                    {
                        byte value = fileContent[i];
                        byte key = keys[i];
                        int valueIndex = -1, keyIndex = -1;
                        
                        for (int j = 0; i < 256; j++)
                        {
                            if (abc[j] == key)
                            {
                                keyIndex = j;
                                break;
                            }
                        }
                        for (int j = 0; i < 256; j++)
                        {
                            if (table[keyIndex, j] == value)
                            {
                                valueIndex = j;
                                break;
                            }
                        }

                        result[i] = abc[valueIndex];
                    }
                    
                }

                String fileExt = Path.GetExtension(txtFilePath.Text);
                SaveFileDialog sd = new SaveFileDialog();
                sd.Filter = "Files (*" + fileExt + ") | *" + fileExt;
                if(sd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sd.FileName, result);
                }
                
            }
            catch
            {
                MessageBox.Show("File is open. Please close the file");
                return;
            }


        }
    }
}
