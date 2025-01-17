﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace CryptoPad
{
    public partial class frmMain : Form
    {
        private string FileName;
        private string BaseContent;
        private Dictionary<CryptoMode, object> FileParams;
        private EncryptedData CurrentFile;
        private string StringToPrint;
        private AppSettings Settings;

        private bool HasChange
        {
            get
            {
                if (string.IsNullOrEmpty(BaseContent))
                {
                    return !string.IsNullOrEmpty(tbEditor.Text);
                }
                return tbEditor.Text != BaseContent;
            }
        }

        public frmMain(string[] args)
        {
            InitializeComponent();

            Settings = AppSettings.GetSettings();

            WindowState = Settings.WindowStartupState;
            if (WindowState == FormWindowState.Normal)
            {
                Size = Settings.WindowSize;
            }

            tbEditor.Font = Settings.GetFont();
            try
            {
                tbEditor.ForeColor = Settings.EditorForegroundColor.GetColor();
            }
            catch
            {
                Settings.EditorForegroundColor = new ColorCode(tbEditor.ForeColor);
            }
            try
            {
                tbEditor.BackColor = Settings.EditorBackgroundColor.GetColor();
            }
            catch
            {
                Settings.EditorBackgroundColor = new ColorCode(tbEditor.BackColor);
            }

            FileParams = new Dictionary<CryptoMode, object>();

            //Copy icons over into the context menu
            cutToolStripMenuItem.Image = cutToolStripButton.Image;
            copyToolStripMenuItem.Image = copyToolStripButton.Image;
            pasteToolStripMenuItem.Image = pasteToolStripButton.Image;

            UpdateStatus();

            dlgFont.Apply += delegate
            {
                tbEditor.Font = dlgFont.Font;
            };


            if(args != null && args.Length > 0)
            {
                var fileName = args[0];
                OpenFile(fileName);
            }
        }

        public void UpdateStatus()
        {
            var name = string.IsNullOrEmpty(FileName) ? "New File" : Path.GetFileName(FileName);
            var encStatus = "Not encrypted";
            if (HasChange)
            {
                name += "*";
            }
            if (CurrentFile != null)
            {
                encStatus = string.Join(", ", CurrentFile.Providers.Select(m => m.Mode).Distinct());
            }
            Text = $"CryptoPad: [{name}]";
            tsStatusLabel.Text = name;
            tsEncryptionLabel.Text = encStatus;
            tsSizeLabel.Text = $"{tbEditor.Text.Length} UTF-8 characters";
            tsSizeLabel.ToolTipText = $"{Encoding.UTF8.GetByteCount(tbEditor.Text)} bytes";
        }

        private void NewText()
        {
            if (!HasChange || SaveText(false, HasChange))
            {
                tbEditor.Text = FileName = BaseContent = string.Empty;
                FileParams.Clear();
                CurrentFile = null;
                UpdateStatus();
            }
        }

        private bool SaveText(bool SaveAs, bool AskToSave)
        {
            if (AskToSave)
            {
                var dlgresult = Program.AlertMsg("Save changes to the current file?", true, true);
                if (dlgresult == DialogResult.No)
                {
                    return true;
                }
                if (dlgresult == DialogResult.Cancel)
                {
                    return false;
                }
            }
            if ((!string.IsNullOrEmpty(FileName) && !SaveAs) || dlgSave.ShowDialog() == DialogResult.OK)
            {
                if (CurrentFile == null)
                {
                    var GS = AppSettings.GlobalSettings();
                    if (GS == null)
                    {
                        GS = new AppSettings()
                        {
                            Restrictions = new Restrictions()
                        };
                    }
                    using (var dlgCrypt = new frmCryptoModeSelect(Settings, GS.Restrictions.AllowedModes))
                    {
                        if (dlgCrypt.ShowDialog() == DialogResult.OK)
                        {
                        //    if (dlgCrypt.Modes == 0 && GS.Restrictions.AutoRsaKeys.Length == 0)
                        //    {
                        //        Program.ErrorMsg("Please select at least one mode of encryption");
                        //        return false;
                        //    }
                            var Params = new Dictionary<CryptoMode, object>();
                            //if (dlgCrypt.Modes.HasFlag(CryptoMode.Password))
                            //{
                                Params[CryptoMode.Password] = dlgCrypt.Password;
                            //}
                            //if (dlgCrypt.Modes.HasFlag(CryptoMode.Keyfile))
                            //{
                            //    Params[CryptoMode.Keyfile] = dlgCrypt.Keyfile;
                            //}
                            //if (dlgCrypt.Modes.HasFlag(CryptoMode.RSA) || GS.Restrictions.AutoRsaKeys.Length > 0)
                            //{
                            //    var RsaList = new List<RSAParameters>(GS.Restrictions.AutoRsaKeys.Select(m => m.Key));
                            //    if (dlgCrypt.Modes.HasFlag(CryptoMode.RSA))
                            //    {
                            //        RsaList.Add(dlgCrypt.RsaKey.Key);
                            //    }
                            //    Params[CryptoMode.RSA] = RsaList;
                            //}

                            try
                            {
                                CurrentFile = Encryption.Encrypt(dlgCrypt.Modes, Encoding.UTF8.GetBytes(tbEditor.Text), Params);
                                FileParams = Params;
                            }
                            catch (Exception ex)
                            {
                                Program.ErrorMsg($"Unable to encrypt your file.\r\n{ex.Message}");
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    Encryption.ReEncrypt(CurrentFile, Encoding.UTF8.GetBytes(tbEditor.Text));
                }
                if (SaveAs || string.IsNullOrEmpty(FileName))
                {
                    FileName = dlgSave.FileName;
                }
                try
                {
                    File.WriteAllText(FileName, CurrentFile.ToXML());
                }
                catch (Exception ex)
                {
                    Program.ErrorMsg($"Unable to save your file.\r\n{ex.Message}");
                    return false;
                }
                //Saved the current file
                BaseContent = tbEditor.Text;
                UpdateStatus();
                return true;
            }
            return false;
        }

        private void OpenFile(string fileName)
        {
            byte[] Data = null;
            EncryptedData TempFile = null;
            try
            {
                TempFile = Tools.FromXML<EncryptedData>(File.ReadAllText(fileName));
                try
                {
                    Data = Encryption.Decrypt(TempFile);
                    Debug.WriteLine("Decrypted using parameterless provider");
                }
                catch
                {
                    Debug.WriteLine("Parameterless provider could not decrypt the file");
                    if (TempFile.HasProvider(CryptoMode.RSA))
                    {
                        //Try all RSA keys until one succeeds
                        foreach (var K in Settings.LoadRSAKeys().Where(m => RSAEncryption.HasPrivateKey(m.Key)))
                        {
                            FileParams[CryptoMode.RSA] = K.Key;
                            try
                            {
                                Data = Encryption.Decrypt(TempFile, FileParams);
                                Debug.WriteLine($"Decrypted using RSA provider and key: {K.Name}");
                                break;
                            }
                            catch
                            {
                                Debug.WriteLine($"Key failed: {K.Name}");
                                //Try next key
                            }
                        }
                        if (Data == null)
                        {
                            Debug.WriteLine($"No RSA key could decrypt the content");
                            FileParams.Remove(CryptoMode.RSA);
                        }
                    }
                    if (Data == null)
                    {
                        if (TempFile.HasProvider(CryptoMode.Keyfile) || TempFile.HasProvider(CryptoMode.Password))
                        {
                            using (var pwd = new frmCryptoInput(TempFile.AllModes, null))
                            {
                                if (pwd.ShowDialog() == DialogResult.OK)
                                {
                                    if (pwd.ValidInput)
                                    {
                                        if (!string.IsNullOrEmpty(pwd.Password))
                                        {
                                            FileParams[CryptoMode.Password] = pwd.Password;
                                        }
                                        if (!string.IsNullOrEmpty(pwd.Keyfile))
                                        {
                                            if (File.Exists(pwd.Keyfile))
                                            {
                                                FileParams[CryptoMode.Password] = pwd.Keyfile;
                                            }
                                            else
                                            {
                                                Program.ErrorMsg("Invalid key file selected");
                                            }
                                        }
                                        if (FileParams.Count > 0)
                                        {
                                            try
                                            {
                                                Data = Encryption.Decrypt(TempFile, FileParams);
                                            }
                                            catch (Exception ex)
                                            {
                                                Program.ErrorMsg($"Unable to decrypt the file using the supplied data. Invalid key file or password?\r\n{ex.Message}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Program.ErrorMsg("You need to provide at least one of the offered options to decrypt the file.");
                                    }
                                }
                            }
                        }
                        else if (TempFile.HasProvider(CryptoMode.RSA))
                        {
                            Program.AlertMsg(
                                "The file is encrypted using RSA but none of your keys can decrypt it.\r\n" +
                                "Please add the matching RSA private key to the key store using the \"Tools >> Options\" Menu");
                        }
                        else
                        {
                            Program.ErrorMsg("Failed to decrypt the data.");
                        }
                    }
                }
            }
            catch
            {
                Program.ErrorMsg("Unable to open the specified file. It's not a valid encrypted text document");
            }
            //Open the selected file, provided it could be decrypted
            if (Data != null)
            {
                FileName = fileName;
                CurrentFile = TempFile;
                BaseContent = tbEditor.Text = Encoding.UTF8.GetString(Data);
                UpdateStatus();
            }
        }

        private void OpenText()
        {
            if (!HasChange || SaveText(false, HasChange))
            {
                if (dlgOpen.ShowDialog() == DialogResult.OK)
                {
                    OpenFile(dlgOpen.FileName);
                }
            }

            ////Open the selected file, provided it could be decrypted
            //if (Data != null)
            //{
            //    FileName = dlgOpen.FileName;
            //    CurrentFile = TempFile;
            //    BaseContent = tbEditor.Text = Encoding.UTF8.GetString(Data);
            //    UpdateStatus();
            //}
        }

        private void tbEditor_TextChanged(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        tbEditor.SelectAll();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.C:
                        tbEditor.Copy();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.V:
                        tbEditor.Paste();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.X:
                        tbEditor.Cut();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.S:
                        SaveText(false, false);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.N:
                        NewText();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.O:
                        OpenText();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                }
            }
        }

        private void PrintTextFileHandler(object sender, PrintPageEventArgs e)
        {
            int charactersOnPage = 0;
            int linesPerPage = 0;

            if (string.IsNullOrEmpty(StringToPrint))
            {
                StringToPrint = tbEditor.Text.TrimEnd();
            }

            //Sets the value of charactersOnPage to the number of characters 
            //of StringToPrint that will fit within the bounds of the page.
            e.Graphics.MeasureString(StringToPrint, tbEditor.Font,
                e.MarginBounds.Size, StringFormat.GenericTypographic,
                out charactersOnPage, out linesPerPage);

            //Draws the string within the bounds of the page
            e.Graphics.DrawString(StringToPrint, tbEditor.Font, Brushes.Black,
                e.MarginBounds, StringFormat.GenericTypographic);

            //Remove the portion of the string that has been printed.
            StringToPrint = StringToPrint.Substring(charactersOnPage);

            //Check to see if more pages are to be printed.
            e.HasMorePages = (StringToPrint.Length > 0);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            var canCancel =
                //e.CloseReason != CloseReason.TaskManagerClosing &&
                e.CloseReason != CloseReason.WindowsShutDown;
            var msg = "You have unsaved changed. Save before exit?";
            if (!canCancel)
            {
                msg += "\r\nCAUTION! Application exit was requested by the system. You can't cancel it.";
            }
            if (HasChange)
            {
                switch (Program.AlertMsg(msg, true, canCancel))
                {
                    case DialogResult.Yes:
                        if (!SaveText(false, false))
                        {
                            frmMain_FormClosing(sender, e);
                        }
                        break;
                    case DialogResult.No:
                        //No action
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void tsEncryptionLabel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                if (Program.InfoMsg("To set the encryption mode for a new file you have to save it first. Save now?", true) == DialogResult.Yes)
                {
                    SaveText(false, false);
                }
            }
            else
            {
                var GS = AppSettings.GlobalSettings();
                if (GS == null)
                {
                    GS = new AppSettings()
                    {
                        Restrictions = new Restrictions()
                    };
                }
                using (var dlgModes = new frmCryptoModeSelect(Settings, GS.Restrictions.AllowedModes, CurrentFile.AllModes))
                {
                    if (dlgModes.ShowDialog() == DialogResult.OK)
                    {
                        //if (dlgModes.Modes == 0)
                        //{
                        //    Program.ErrorMsg("Please select at least one mode of encryption");
                        //}
                        //else
                        //{
                              var Params = new Dictionary<CryptoMode, object>();
                        //    if (dlgModes.Modes.HasFlag(CryptoMode.Password))
                        //    {
                                Params[CryptoMode.Password] = dlgModes.Password;
                            //}
                            //if (dlgModes.Modes.HasFlag(CryptoMode.Keyfile))
                            //{
                            //    Params[CryptoMode.Keyfile] = dlgModes.Keyfile;
                            //}
                            //if (dlgModes.Modes.HasFlag(CryptoMode.RSA))
                            //{
                            //    Params[CryptoMode.RSA] = dlgModes.RsaKey.Key;
                            //}
                            try
                            {
                                CurrentFile = Encryption.Encrypt(dlgModes.Modes, Encoding.UTF8.GetBytes(tbEditor.Text), Params);
                                if (Program.InfoMsg("The password waas changed. Save the file now?", true) == DialogResult.Yes)
                                {
                                    SaveText(false, false);
                                }
                            }
                            catch (Exception ex)
                            {
                                Program.ErrorMsg($"Unable to encrypt your file.\r\n{ex.Message}");
                            }
                        //}
                    }
                }
            }
        }

        #region Menu

        private void ExitAction_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NewAction_Click(object sender, EventArgs e)
        {
            NewText();
        }

        private void OpenAction_Click(object sender, EventArgs e)
        {
            OpenText();
        }

        private void SaveAction_Click(object sender, EventArgs e)
        {
            SaveText(false, false);
        }

        private void SaveAsAction_Click(object sender, EventArgs e)
        {
            SaveText(true, false);
        }

        private void CopyAction_Click(object sender, EventArgs e)
        {
            tbEditor.Copy();
        }

        private void CutAction_Click(object sender, EventArgs e)
        {
            tbEditor.Cut();
        }

        private void PasteAction_Click(object sender, EventArgs e)
        {
            tbEditor.Paste();
        }

        private void SelectAllAction_Click(object sender, EventArgs e)
        {
            tbEditor.SelectAll();
        }

        private void CustomizeAction_Click(object sender, EventArgs e)
        {
            dlgFont.Font = tbEditor.Font;
            if (dlgFont.ShowDialog() == DialogResult.OK)
            {
                tbEditor.Font = dlgFont.Font;
            }
        }

        private void PrintAction_Click(object sender, EventArgs e)
        {
            using (var pd = new PrintDocument())
            {
                pd.PrintPage += PrintTextFileHandler;
                pd.DocumentName = Path.GetFileName(FileName);
                printDialog.Document = pd;
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    pd.Print();
                }
            }
        }

        private void PrintPreviewAction_Click(object sender, EventArgs e)
        {
            using (var pd = new PrintDocument())
            {
                pd.PrintPage += PrintTextFileHandler;
                pd.DocumentName = Path.GetFileName(FileName);
                printPreview.Document = pd;
                printPreview.ShowDialog();
            }
        }

        private void HelpAction_Click(object sender, EventArgs e)
        {
            Program.InfoMsg(@"A simple, encrypted text editor

Forked from https://github.com/AyrA/CryptoPad and customized to fit my needs.");
        }

        #endregion

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.WindowStartupState = WindowState;
            if (WindowState == FormWindowState.Normal)
            {
                Settings.WindowSize = Size;
            }

            Settings.SetFont(tbEditor.Font);
            Settings.EditorForegroundColor = new ColorCode(tbEditor.ForeColor);
            Settings.EditorBackgroundColor = new ColorCode(tbEditor.BackColor);

            try
            {
                Settings.SaveSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to save settings on exit: {ex.Message}");
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new frmSettings(Settings))
            {
                frm.ShowDialog();
            }
        }

        private void tbEditor_DragDrop(object sender, DragEventArgs e)
        {
            var dropped = ((string[])e.Data.GetData(DataFormats.FileDrop));
            var files = dropped.ToList();

            if (!files.Any())
                return;

            OpenFile(files.First());
        }

        private void tbEditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
    }
}
