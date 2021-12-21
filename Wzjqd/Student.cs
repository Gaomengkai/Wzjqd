using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Wzjqd
{
    class Student
    {
        public string name { get; set; }
        public string openid { get; set; }
        public Student(string name, string openid) { this.name = name; this.openid = openid; }
        static public string OpenidSizer(string originalOpenId)
        {
            if (originalOpenId is null)
            {
                throw new ArgumentNullException(nameof(originalOpenId));
            }
            string teachermate = "teachermate";
            Match teachermateMatcher = Regex.Match(originalOpenId, teachermate);
            if (teachermateMatcher.Success)
            {
                string re_ptn = "openid=([^&]*)";
                Match match = Regex.Match(originalOpenId, re_ptn);
                string matcherFinished = match.Value;
                return matcherFinished.Replace("openid=", "");
            }
            return originalOpenId;
        }
    }
}