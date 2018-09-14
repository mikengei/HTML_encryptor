using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HTML_encryptor
{
    public partial class MainWindow : Window
    {
        public string filepath_location;
        public Boolean error = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btnopen(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Title = "Choose a File";
            openfile.Filter = "Files (html,css,js)|*.txt;*.html;*.css;*.js";
            //check button clikced
            var result = openfile.ShowDialog();

            //returs a Bool
            if (Convert.ToBoolean(result))
            {
                //true -- show file path on textbox
                filepath_location = openfile.FileName;
                //set file path.
                txtfile.Text = filepath_location;
            }
            else
            {
                //reset
            }
        }

        private bool Precheck(string txtvalue)
        {

            //check if empty
            if (txtvalue != null && !File.Exists(txtvalue))
            {
                MessageBox.Show("The selected item is either empty or doesnt exist", "Error");
            }
            else
            {
                error = false;
            }

            return error;
        }

        private void Btnencrypt(object sender, RoutedEventArgs e)
        {
            Precheck(txtfile.Text);
            //check error 
            if (!error)
            {
                //start encryption
                CreateFile();
            }
            else
            {
                //error dont do anything
            }
        }

        //<content>
        //************************
        // Function to encrypt data
        //***********************
        //<content>
        private static byte[] Encrypt(string myfile, byte[] key, byte[] IV)
        {
            try
            {
                //hold encrypted bytes
                byte[] encrypted;

                using (Aes aesAlg = Aes.Create())
                {
                    //assign params
                    aesAlg.Key = key;
                    aesAlg.IV = IV;

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    //creating streams used for encryption
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(myfile);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }

                }
                //returns byte array
                return encrypted;
            }
            catch (Exception e)
            {
                MessageBox.Show("Sorry you request couldnt be handled", "There was an error");
                return null;
            }
        }

        //<content>
        //************************
        // Function to Write a file
        //***********************
        //</content>
        private void CreateFile()
        {
            //Read byte data and store it
            //StreamReader <params> filestream 
            using (StreamReader readfile = new StreamReader(filepath_location))
            {
                //store read data
                String content = readfile.ReadToEnd();
                //dispose
                readfile.Dispose(); //release resources
                using (Aes myaes = Aes.Create())
                {
                    //<params> file content, key, 
                    byte[] encrpteddata = Encrypt(content, myaes.Key, myaes.IV);

                    //if something broke null is returned
                    if (encrpteddata != null)
                    {
                        string results = Convert.ToBase64String(encrpteddata);
                        //Write data returned
                        //Streamwriter <params> filepath
                        StreamWriter writedata = new StreamWriter(filepath_location);
                        writedata.Write(results);
                        writedata.Dispose();
                    }
                    else { }
                }

            }

        }
    }
}
