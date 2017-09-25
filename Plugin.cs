using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using YamlDotNet.Serialization;
//reference: YamlDotNet.Serialization;

[assembly: AssemblyTitle("Buffet")]
[assembly: AssemblyDescription("A simple buff tracking display")]
[assembly: AssemblyCompany("captainbahab")]
[assembly: AssemblyVersion("1.0.0.1")]

namespace Buffet
{
    public class Plugin : UserControl, IActPluginV1
    {
        private StreamWriter logWriter;
        private PowerDisplay display;
        private BuffetVM VM;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        public List<string> AssemblyResolveDirectories { get; private set; } = new List<string>();
        public Regex[] logRegex = new Regex[]
        {
            new Regex("Vulnerability Up"),
            new Regex("Chain Strategem"),
            new Regex("Brotherhood"),
            new Regex("Dragon Sight"),
            new Regex("Left Eye"),
            new Regex("Contagion"),
            new Regex("Foe('s)? Requiem"),
            new Regex("Devotion"),
            new Regex("Balance"),
            new Regex("Spear"),
            new Regex("Trick Attack"),
            new Regex("Hypercharge"),
            new Regex("Embolden"),
            new Regex("Critical Up"),
            new Regex("Battle Voice"),
            new Regex("Battle Litany"),
        };

        #region Designer Created Code (Avoid editing)
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(686, 384);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            // 
            // BuffetPlugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.elementHost1);
            this.Name = "BuffetPlugin";
            this.Size = new System.Drawing.Size(686, 384);
            this.ResumeLayout(false);

        }

        #endregion

        #endregion
        public Plugin()
        {
            InitializeComponent();
        }

        Label lblStatus;    // The status label that appears in ACT's Plugin tab
        string settingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\Buffet.config.yaml");
        string logfile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "buffet.log");


        #region IActPluginV1 Members
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            AppDomain.CurrentDomain.AssemblyResolve += AppDomain_AssemblyResolve;

            lblStatus = pluginStatusText;   // Hand the status label's reference to our local var
            pluginScreenSpace.Controls.Add(this);   // Add this UserControl to the tab ACT provides
            this.Dock = DockStyle.Fill; // Expand the UserControl to fill the tab's client space

            logWriter = new StreamWriter(logfile, true);

            LoadSettings();
            VM.InitializeWindowManager(new BuffetViewModelWindowManager());
            VM.PowerDisplay.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(PowerDisplayVM.EnableClickThrough))
                {
                    display.Close();
                    display = new PowerDisplay(VM.PowerDisplay);
                    display.Show();
                }
            };
            
            elementHost1.Child = new SettingsControl() { DataContext = VM };

            display = new PowerDisplay(VM.PowerDisplay);
            display.Show();

            // Create some sort of parsing event handler.  After the "+=" hit TAB twice and the code will be generated for you.
            //ActGlobals.oFormActMain.AfterCombatAction += new CombatActionDelegate(oFormActMain_AfterCombatAction);
            ActGlobals.oFormActMain.OnLogLineRead += oFormActMain_OnLogLineRead;

            lblStatus.Text = "Plugin Started";
        }

        public void DeInitPlugin()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= AppDomain_AssemblyResolve;
            // Unsubscribe from any events you listen to when exiting!
            //ActGlobals.oFormActMain.AfterCombatAction -= oFormActMain_AfterCombatAction;
            ActGlobals.oFormActMain.OnLogLineRead -= oFormActMain_OnLogLineRead;

            logWriter.Flush();
            logWriter.Close();

            Properties.Settings.Default.Save();
            SaveSettings();

            VM = null;

            display.Close();
            display = null;

            lblStatus.Text = "Plugin Exited";
        }
        #endregion

        private void oFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            VM.HandleLog(logInfo.logLine, DateTime.Now);
            foreach(var r in logRegex)
                if (r.IsMatch(logInfo.logLine))
                {
                    logWriter.WriteLineAsync(logInfo.logLine);
                    break;
                }
        }

        void LoadSettings()
        {
            if (File.Exists(settingsFile))
            {
                using (var sr = new StreamReader(settingsFile))
                {
                    var s = new Deserializer();
                    VM = s.Deserialize<BuffetVM>(sr);
                }
            }
            else
            { 
                VM = new BuffetVM();
                foreach (var effect in Effects.AllEffects())
                    VM.SoundSettings.Add(new SoundSettingsVM() { BuffName = effect.name, PlayBuffEndedSound = true, PlayBuffGrantedSound = true });
            }
        }
        void SaveSettings()
        {
            using (var sw = new StreamWriter(settingsFile))
            {
                var s = new Serializer();
                s.Serialize(sw, VM);
            }
        }

        private Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs e)
        {
            var mainDir = ActGlobals.oFormActMain?.PluginGetSelfData(this)?.pluginFile.DirectoryName;
            if (!string.IsNullOrEmpty(mainDir))
            {
                if (!this.AssemblyResolveDirectories.Where(dir => dir == mainDir).Any())this.AssemblyResolveDirectories.Add(mainDir);
            }
            
            foreach (var dir in this.AssemblyResolveDirectories)
            {
                var asm = TryLoadAssembly(e.Name, dir, ".dll");
                if (asm != null)
                {
                    return asm;
                }
            }

            return null;
        }

        private Assembly TryLoadAssembly(string assemblyName, string directory, string extension)
        {
            var asm = new AssemblyName(assemblyName);

            var asmPath = Path.Combine(directory, asm.Name + extension);
            if (File.Exists(asmPath))
            {
                return Assembly.LoadFrom(asmPath);
            }

            return null;
        }
    }
}
