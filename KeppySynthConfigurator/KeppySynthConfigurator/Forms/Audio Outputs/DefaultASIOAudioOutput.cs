﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// For device info
using Un4seen.Bass;
using Un4seen.BassAsio;

namespace KeppySynthConfigurator
{
    public partial class DefaultASIOAudioOutput : Form
    {
        public DefaultASIOAudioOutput()
        {
            InitializeComponent();
        }

        private void DefaultASIOAudioOutput_Load(object sender, EventArgs e)
        {
            try
            {
                int n;
                int selecteddeviceprev = (int)KeppySynthConfiguratorMain.SynthSettings.GetValue("defaultAdev", 0);
                BASS_ASIO_DEVICEINFO info = new BASS_ASIO_DEVICEINFO();
                BassAsio.BASS_ASIO_GetDeviceInfo(selecteddeviceprev, info);
                DefOut.Text = String.Format("Def. ASIO output: {0}", info.ToString());
                for (n = 0; BassAsio.BASS_ASIO_GetDeviceInfo(n, info); n++)
                {
                    DevicesList.Items.Add(info.ToString());
                }

                if (n < 1)
                {
                    Functions.ShowErrorDialog(1, System.Media.SystemSounds.Asterisk, "Error", "No ASIO devices installed!\n\nClick OK to close this window.", false, null);
                    Close();
                    return;
                }

                DevicesList.SelectedIndex = selecteddeviceprev;
                MaxThreads.Text = String.Format("ASIO is allowed to use a maximum of {0} threads.", Environment.ProcessorCount);
                BassAsio.BASS_ASIO_Init(DevicesList.SelectedIndex, 0);

                DeviceTrigger();
                DevicesList.SelectedIndexChanged += new System.EventHandler(this.DevicesList_SelectedIndexChanged);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load the dialog.\nBASS is probably unable to start, or it's missing.\n\nError:\n" + ex.Message.ToString(), "Oh no! Keppy's Synthesizer encountered an error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                Dispose();
            }
        }

        private void DevicesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Functions.SetDefaultDevice(1, DevicesList.SelectedIndex);
            BassAsio.BASS_ASIO_Free();
            BassAsio.BASS_ASIO_Init(DevicesList.SelectedIndex, 0);
            int value = DeviceTrigger();

            if (value == DeviceStatus.DEVICE_UNSTABLE) MessageBox.Show("This device might crash the app, while in use.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (value == DeviceStatus.DEVICE_UNSUPPORTED) MessageBox.Show("This device is unsupported.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            BassAsio.BASS_ASIO_Free();
            Close();
            Dispose();
        }

        private void DeviceCP_Click(object sender, EventArgs e)
        {
            BassAsio.BASS_ASIO_ControlPanel();
        }

        private void ASIODevicesSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/KaleidonKep99/Keppy-s-Synthesizer#asio-support-details");
        }

        private int DeviceTrigger()
        {
            if (DevicesList.Text.Contains("ASIO2WASAPI")) return UnsupportedDevice();
            else if (DevicesList.Text.Contains("AKIYAMA ASIO")) return UnstableDevice();
            else if (DevicesList.Text.Contains("ASIO ADSP24(WDM)")) return UnsupportedDevice();
            else if (DevicesList.Text.Contains("ASIO4ALL")) return UnstableDevice();
            else if (DevicesList.Text.Contains("ASUS Xonar D2 ASIO")) return SupportedDevice();
            else if (DevicesList.Text.Contains("BEHRINGER USB AUDIO")) return SupportedDevice();
            else if (DevicesList.Text.Contains("FL Studio ASIO")) return SupportedDevice();
            else if (DevicesList.Text.Contains("FlexASIO")) return UnstableDevice();
            else if (DevicesList.Text.Contains("Focusrite USB ASIO")) return SupportedDevice();
            else if (DevicesList.Text.Contains("JackRouter")) return SupportedDevice();
            else if (DevicesList.Text.Contains("ReaRoute ASIO")) return SupportedDevice();
            else if (DevicesList.Text.Contains("Realtek ASIO")) return UnsupportedDevice();
            else if (DevicesList.Text.Contains("USB Audio ASIO Driver")) return SupportedDevice();
            else if (DevicesList.Text.Contains("USBPre 2.0 ASIO")) return SupportedDevice();
            else if (DevicesList.Text.Contains("Voicemeeter AUX Virtual ASIO")) return UnsupportedDevice();
            else if (DevicesList.Text.Contains("Voicemeeter Insert Virtual ASIO")) return UnsupportedDevice();
            else if (DevicesList.Text.Contains("Voicemeeter Virtual ASIO")) return UnsupportedDevice();
            else if (DevicesList.Text.Contains("ZOOM R8 ASIO Driver")) return SupportedDevice();
            else return UnknownDevice();
        }

        private int SupportedDevice()
        {
            Status.Font = new Font(Status.Font, FontStyle.Regular);
            Status.ForeColor = Color.DarkOliveGreen;
            Status.Text = "Supported device";
            return DeviceStatus.DEVICE_SUPPORTED;
        }

        private int UnstableDevice()
        {
            Status.Font = new Font(Status.Font, FontStyle.Bold);
            Status.ForeColor = Color.DarkOrange;
            Status.Text = "Supported device, might be unstable";
            return DeviceStatus.DEVICE_UNSTABLE;
        }

        private int UnsupportedDevice()
        {
            Status.Font = new Font(Status.Font, FontStyle.Bold);
            Status.ForeColor = Color.Red;
            Status.Text = "Unsupported device";
            return DeviceStatus.DEVICE_UNSUPPORTED;
        }

        private int UnknownDevice()
        {
            Status.Font = new Font(Status.Font, FontStyle.Bold);
            Status.ForeColor = Color.DarkGray;
            Status.Text = "Unknown device";
            return DeviceStatus.DEVICE_UNKNOWN;
        }
    }

    static class DeviceStatus
    {
        public const int DEVICE_SUPPORTED = 0;
        public const int DEVICE_UNSTABLE = 1;
        public const int DEVICE_UNSUPPORTED = 2;
        public const int DEVICE_UNKNOWN = 3;
    }
}
