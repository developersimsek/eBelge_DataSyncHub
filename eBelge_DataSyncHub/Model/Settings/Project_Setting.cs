
using static eBelge_DataSyncHub.Model.Settings.Projects.SettingProjects;

namespace eBelge_DataSyncHub.Model.Settings
{
    public class Project_Setting
    {
        public class Setting
        {
            public Settings Settings { get; set; }
        }
        public class Settings
        {
            public Projets Projets { get; set; }
            public OneSignal oneSignal { get; set; }
            public string fireBaseUri { get; set; }
        }
    }
}
