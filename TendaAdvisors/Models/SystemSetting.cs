using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class SystemSetting
    {
        public int SystemSettingId { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}