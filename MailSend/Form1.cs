using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace MailSend
{
    
    public partial class Form1 : Form
    {
        public static List<string> Simple;
        public static string textFromFile;
        public static string UnicKey;

        public static string styleSMPT;
        public static string TitleMes;

        #region getMail

        public static Int64 countSend = 0;
        public static string[] emailRow;


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog BaseMail = new OpenFileDialog();

            BaseMail.Filter = "Text Files|*.txt";
            BaseMail.Title = "Выберете файл с базой почт";
            //BaseMail.InitialDirectory = "c:\\";
            BaseMail.RestoreDirectory = true;

            if (BaseMail.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = BaseMail.FileName;

                    using (FileStream fStream = File.OpenRead(@path))
                    {
                        byte[] array = new byte[fStream.Length];
                        fStream.Read(array, 0, array.Length);
                        string textFromFile = System.Text.Encoding.Default.GetString(array);
                        textBox1.Text = textFromFile;
                        countSend = File.ReadAllLines(@path).Length;
                        emailRow = File.ReadAllLines(@path);
                    }

                }
                catch (Exception error)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + error.Message);
                }
            }
        }
        #endregion

        #region getText
        
        
        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog BaseMail = new OpenFileDialog();
            BaseMail.Filter = "Text Files|*.txt";
            BaseMail.Title = "Выберете файл с текстом для рассылки";
            BaseMail.RestoreDirectory = true;

            if (BaseMail.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = BaseMail.FileName;
                    using (FileStream fStream = File.OpenRead(@path))
                    {
                        byte[] array = new byte[fStream.Length];
                        fStream.Read(array, 0, array.Length);
                        textFromFile = System.Text.Encoding.UTF8.GetString(array);
                        textBox2.Text = textFromFile;
                    }

                }
                catch (Exception error)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + error.Message);
                }
            }
        }
        #endregion

        #region SendMail
        private void button3_Click(object sender, EventArgs e)
        {

            
            for (int i = 0; i < countSend; i++)
            {
                SendMail(emailRow[i], textFromFile);
            }
        }
        #endregion

        #region funcSendMail
        static public void SendMail(string toMail, string text)
        {
            string myDoc = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            string Login = File.ReadLines(myDoc + @"/MailSend/AccountData.txt").Skip(0).First();
            string Pass = File.ReadLines(myDoc + @"/MailSend/AccountData.txt").Skip(1).First();
            string StudioName = File.ReadLines(myDoc + @"/MailSend/AccountData.txt").Skip(2).First();

            SmtpClient smtp;

            MailAddress from = new MailAddress(Login, StudioName);
            MailAddress to = new MailAddress(toMail);
            MailMessage m = new MailMessage(from, to);
            
            

            
            m.Subject = TitleMes;
            m.Body = text;
            m.IsBodyHtml = true;

            if(styleSMPT == "Отправить через Gmail")
            {
                smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(Login, Pass);
                smtp.EnableSsl = true;
                smptSend();
            }
            else if(styleSMPT == "Отправить через Mail")
            {
                smtp = new SmtpClient("smtp.mail.ru", 25);
                smtp.Port = 25;
                smtp.Credentials = new NetworkCredential(Login, Pass);
                smtp.EnableSsl = true;
                smptSend();
            }

            void smptSend()
            {
                try
                {
                    smtp.Send(m);
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                        if (status == SmtpStatusCode.MailboxUnavailable)
                        {
                            MessageBox.Show("TEST");
                            System.Threading.Thread.Sleep(5000);
                            smtp.Send(m);
                        }
                        else
                        {
                            MessageBox.Show("Failed to deliver message to {0}",
                                ex.InnerExceptions[i].FailedRecipient);
                        }
                    }
                }
            }


            
            
        }
        #endregion

        #region otherTrash  

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string myDoc = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            Directory.CreateDirectory(myDoc + @"/MailSend");
            using (var stream = new FileStream(myDoc + @"/MailSend/AccountData.txt", FileMode.Create));
            using (var stream = new FileStream(myDoc + @"/MailSend/Email.txt", FileMode.Create));
            using (var stream = new FileStream(myDoc + @"/MailSend/Text.txt", FileMode.Create));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            styleSMPT = comboBox1.Text;
            if (styleSMPT == "Отправить через Gmail")
            {
                MessageBox.Show("Ваша рассылка будет через Gmail");
            }
            else if (styleSMPT == "Отправить через Mail")
            {
                MessageBox.Show("Ваша рассылка будет через Mail");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            TitleMes = textBox3.Text;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
