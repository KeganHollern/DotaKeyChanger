using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotaKeybinds.KeyChanger;

namespace DotaKeybinds.UI
{
    public partial class Window : Form
    {
        public static void DisplayActions(Changer _changer) 
        {
            Window actionWindow = new Window(_changer);
            actionWindow.ShowDialog();
        }

        private Changer _changer;

        public Window(Changer _changer)
        {
            this._changer = _changer;
            InitializeComponent();
            
        }
        private void UpdateUi()
        {
            if (_changer.CurrentSteamId != "")
            {
                label1.Text = $"Steam Account: {_changer.CurrentSteamId}";
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = FileUtils.AppData.HasBackup(_changer.CurrentSteamId);
                button4.Enabled = FileUtils.AppData.HasKeybinds();
            }
            else
            {
                label1.Text = $"Steam Account: Not Logged In";
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
            }
            
        }

        private void _changer_OnAccountChanged(object? sender, AccountChangedEventArgs e)
        {
            label1.BeginInvoke(new Action(() =>
            {
                UpdateUi();
            }));
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            // remove event
            _changer.OnAccountChanged -= _changer_OnAccountChanged;
        }

        private void Window_Load(object sender, EventArgs e)
        {
            // trigger a fake event so we setup our UI
            _changer_OnAccountChanged(this, new AccountChangedEventArgs()
            {
                SteamID = _changer.CurrentSteamId
            });
            // add event
            _changer.OnAccountChanged += _changer_OnAccountChanged;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //backup
            _changer.CreateBackup();
            UpdateUi();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //restore
            _changer.RestoreBackup();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //set primary
            _changer.CreateKeybinds();
            UpdateUi();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //force transfer
            _changer.WriteKeybinds();
        }
    }
}
