using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UI.Models {
    public class VersionModel : DependencyObject {


        public static readonly DependencyProperty UpgradeVersionProperty =
            DependencyProperty.Register("UpgradeVersion", typeof(string), typeof(VersionModel));
        public string UpgradeVersion {
            get { return (string)GetValue(UpgradeVersionProperty); }
            set { SetValue(UpgradeVersionProperty, value); }
        }

        public static readonly DependencyProperty CurrentVersionProperty =
            DependencyProperty.Register("CurrentVersion", typeof(string), typeof(VersionModel));
        public string CurrentVersion {
            get { return (string)GetValue(CurrentVersionProperty); }
        }

        public static readonly DependencyProperty CanUpgradeProperty =
            DependencyProperty.Register("CanUpgrade", typeof(bool), typeof(VersionModel), new PropertyMetadata(false));
        public bool CanUpgrade {
            get { return (bool)GetValue(CanUpgradeProperty); }
            set { SetValue(CanUpgradeProperty, value); }
        }

        public static readonly DependencyProperty VersionUrlProperty =
            DependencyProperty.Register("VersionUrl", typeof(string), typeof(VersionModel));
        public string VersionUrl {
            get { return (string)GetValue(VersionUrlProperty); }
            set { SetValue(VersionUrlProperty, value); }
        }

        public static readonly DependencyProperty BrowseUrlProperty =
            DependencyProperty.Register("BrowseUrl", typeof(string), typeof(VersionModel));
        public string BrowseUrl {
            get { return (string)GetValue(BrowseUrlProperty); }
            set { SetValue(BrowseUrlProperty, value); }
        }

        public VersionModel() {
            var assembly = Assembly.GetExecutingAssembly();
            var fileinfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            SetValue(CurrentVersionProperty, fileinfo.FileVersion);
        }

        public void Check() {
            var url = VersionUrl;
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, e) => {
                var client = new WebClient();
                var result = client.DownloadString(url);
                e.Result = Version.Parse(result).ToString();
            };
            worker.RunWorkerCompleted += (sender, e) => {
                if (e.Error == null) {
                    CanUpgrade = (UpgradeVersion = (string)e.Result) != CurrentVersion;
                }
            };
            worker.RunWorkerAsync();
        }
    }
}
