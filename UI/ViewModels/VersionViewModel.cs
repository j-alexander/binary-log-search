using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models;

namespace UI.ViewModels {

    public class VersionViewModel {

        public VersionModel Model { get; set; }

        public VersionViewModel() {
            Model = new VersionModel();
            Model.VersionUrl = @"https://raw.githubusercontent.com/j-alexander/binary-log-search/master/Setup.version";
            Model.BrowseUrl = @"https://github.com/j-alexander/binary-log-search";
            Model.Check();
        }

        public void OnBrowseRequest() {
            Process.Start(Model.BrowseUrl);
        }
    }
}
