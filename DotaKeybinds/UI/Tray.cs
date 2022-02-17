using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotaKeybinds.Properties;
using DotaKeybinds.KeyChanger;

namespace DotaKeybinds.UI
{
    public class Tray : ApplicationContext
    {
        private NotifyIcon _icon;
        private Changer _changer;
        private ToolStripMenuItem _titleItem;
        public Tray(bool userStart)
        {
            RegistryUtils.Startup.Add();

            
            _icon = new NotifyIcon();
            _icon.Icon = Resources.DotaIcon;
            _icon.ContextMenuStrip = new ContextMenuStrip();
            _titleItem = new ToolStripMenuItem("Steam ID: Not Logged In");
            _icon.ContextMenuStrip.Items.Add(_titleItem);
            _icon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            _icon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Show Window", null, BtnShowWindow));
            _icon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Run at Startup", null, BtnStartup)
            {
                Checked = RegistryUtils.Startup.IsEnabled()
            });
            _icon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            _icon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, BtnExit));

            _icon.Visible = true;


            _changer = new Changer();
            _changer.OnAccountChanged += _changer_OnAccountChanged;

            // show window because user launched the app (not startup)
            if (userStart)
            {
                Window.DisplayActions(_changer);
            }
        }


        private void _changer_OnAccountChanged(object? sender, AccountChangedEventArgs e)
        {
            if (_icon.ContextMenuStrip.InvokeRequired)
            {
                _icon.ContextMenuStrip.BeginInvoke(new Action(() =>
                {
                    _titleItem.Text = $"Steam ID: {(_changer.CurrentSteamId == "" ? "Not Logged In" : _changer.CurrentSteamId)}";
                }));
            } 
            else
            {
                _titleItem.Text = $"Steam ID: {(_changer.CurrentSteamId == "" ? "Not Logged In" : _changer.CurrentSteamId)}";
            }
        }

        private void BtnShowWindow(object? sender, EventArgs e)
        {
            Window.DisplayActions(_changer);
        }
        private void BtnStartup(object? sender, EventArgs e)
        {
            if (sender != null && sender is ToolStripMenuItem menuItem)
            {
                menuItem.Checked = !menuItem.Checked;
                RegistryUtils.Startup.Enable(menuItem.Checked);
            }
        }
        private void BtnExit(object? sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
