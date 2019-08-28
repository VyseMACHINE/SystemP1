using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xorHW
{
  public partial class Form1 : Form
  {
        Thread thCrypt = null;

        public Form1()
        {
            InitializeComponent();
            progressBar.Minimum = 0;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Multiselect = false;
            dlg.Title = "Открытия файла";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtSourceFile.Text = dlg.FileName;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (txtPassword != null)
            {
                try
                {
                    if (radCrypt.Enabled)
                    {
                        thCrypt = new Thread(ForEncrypt);
                        thCrypt.IsBackground = true;
                        thCrypt.Start();
                    }
                    else if (radDecrypt.Enabled)
                    {
                        thCrypt = new Thread(ForDecrypt);
                        thCrypt.IsBackground = true;
                        thCrypt.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Введите пароль!");
            }
        }

        public string Encrypt(string message, int key)
        {
            string result = "";
            for (int i = 0; i < message.Length; i++)
            result += (char)(message[i] ^ key);
            return result;
        }

        public string Decrypt(string message, int key)
        {
            return Encrypt(message, key);
        }

        public void ForEncrypt()
        {
            string encryptedText = "";

            using (FileStream fstream = File.OpenRead(txtSourceFile.Text))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];

                // считываем данные
                fstream.Read(array, 0, array.Length);

                // декодируем байты в строку
                string textFromFile = Encoding.Default.GetString(array);

                encryptedText = Encrypt(textFromFile, Convert.ToInt32(txtPassword.Text));
            }

            using (FileStream fstream = new FileStream(txtSourceFile.Text, FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = Encoding.Default.GetBytes(encryptedText);

                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);


                progressBar.Invoke(new Action<int>(
                    (x) =>
                    {
                        progressBar.Maximum = x;
                        progressBar.Update();
                    }), (int)fstream.Length);

                progressBar.Invoke(new Action<int>(
                    (x) =>
                    {
                        progressBar.Value = x;
                        progressBar.Update();
                    }), (int)fstream.Length);
            }
        }

        public void ForDecrypt()
        {
            string decryptedText = "";

            using (FileStream fstream = File.OpenRead(txtSourceFile.Text))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];

                // считываем данные
                fstream.Read(array, 0, array.Length);

                // декодируем байты в строку
                string textFromFile = Encoding.Default.GetString(array);

                decryptedText = Decrypt(textFromFile, Convert.ToInt32(txtPassword.Text));
            }

            using (FileStream fstream = new FileStream(txtSourceFile.Text, FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = Encoding.Default.GetBytes(decryptedText);

                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);

                progressBar.Invoke(new Action<int>(
                    (x) =>
                    {
                        progressBar.Maximum = x;
                        progressBar.Update();
                    }), (int)fstream.Length);

                progressBar.Invoke(new Action<int>(
                    (x) =>
                    {
                        progressBar.Value = x;
                        progressBar.Update();
                    }), (int)fstream.Length);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
