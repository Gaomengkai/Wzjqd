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

namespace Wzjqd
{
    class SignConst
    {
        public const string URL_CHECKIN = "https://v18.teachermate.cn/wechat-api/v1/class-attendance/student-sign-in";
        public const string URL_COURSES = "https://v18.teachermate.cn/wechat-api/v1/students/courses";
        public const string URL_ACTIVESIGNS = "https://v18.teachermate.cn/wechat-api/v1/class-attendance/student/active_signs";
        public const string URL_REFERER_T = "https://v18.teachermate.cn/wechat-pro-ssr/student/sign?openid={0}";
        public const string URL_GETNAME_T = "https://v18.teachermate.cn/wechat-pro-ssr/?openid={0}&from=wzj";
        public const string URL_CHECKINREFER_T = "https://v18.teachermate.cn/wechat-pro-ssr/student/sign/list/{0}";
        public const string CODE_REPEATSIGNIN = "305";
    }
}