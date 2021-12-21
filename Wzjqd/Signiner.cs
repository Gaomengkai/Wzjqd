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
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;

namespace Wzjqd
{
    class Signiner
    {
        private Student stu;
        public Signiner(Student stu)
        {
            this.stu = stu;
        }
        [DataContract]
        public class SignEvent
        {
            [DataMember]
            public string courseId { get; set; }
            [DataMember]
            public string signId { get; set; }
            [DataMember]
            public bool isGPS { get; set; }
            [DataMember]
            public bool isQR { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public string code { get; set; }
            [DataMember]
            public int startYear { get; set; }
            [DataMember]
            public string term { get; set; }
            [DataMember]
            public string cover { get; set; }
        }
        [DataContract]
        public class SignSuccessEvent
        {
            [DataMember]
            public string signRank { get; set; }
            [DataMember]
            public string studentRank { get; set; }
        }
        private List<SignEvent> signs { get; set; }
        public async Task<string> GetUserName()
        {
            var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Get, String.Format(SignConst.URL_GETNAME_T, stu.openid));

            req.Headers.Add("User-Agent", "Mozilla/5.0 (Linux; Android 11; Mi 10 Build/RKQ1.200826.002; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/86.0.4240.99 XWEB/3165 MMWEBSDK/20210902 Mobile Safari/537.36 MMWEBID/3949");
            req.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            req.Headers.Add("Host", "v18.teachermate.cn");
            req.Headers.Add("Referer", String.Format(SignConst.URL_REFERER_T, stu.openid));
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            Console.WriteLine(req.Headers.ToString());

            // Use Regex to find UserName in the HTML file
            var res = await client.SendAsync(req);
            var resS = await res.Content.ReadAsStringAsync();
            var pattern = "name\":\"([^\"]+)";
            var match = Regex.Match(resS, pattern);
            if (!match.Success) return "";
            return match.Value.Replace("name\":\"", "");
        }
        public async Task<List<SignEvent>> GetSignList()
        {
            var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Get, SignConst.URL_ACTIVESIGNS);

            req.Headers.Add("User-Agent", "Mozilla/5.0 (Linux; Android 11; Mi 10 Build/RKQ1.200826.002; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/86.0.4240.99 XWEB/3165 MMWEBSDK/20210902 Mobile Safari/537.36 MMWEBID/3949");
            req.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            req.Headers.Add("Host", "v18.teachermate.cn");
            req.Headers.Add("Referer", String.Format(SignConst.URL_REFERER_T, stu.openid));
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            req.Headers.Add("openid", this.stu.openid);

            var res = await client.SendAsync(req);
            var serializer = new DataContractJsonSerializer(typeof(List<SignEvent>));
            var jsonstr = await res.Content.ReadAsStringAsync();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonstr));
            signs = (List<SignEvent>)serializer.ReadObject(ms);
            return signs;
        }
        public async Task<KeyValuePair<bool,string>> Sign()
        {
            var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Post, SignConst.URL_CHECKIN);
            var body = new Dictionary<string, string>();

            req.Headers.Add("User-Agent", "Mozilla/5.0 (Linux; Android 11; Mi 10 Build/RKQ1.200826.002; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/86.0.4240.99 XWEB/3165 MMWEBSDK/20210902 Mobile Safari/537.36 MMWEBID/3949");
            req.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            req.Headers.Add("Host", "v18.teachermate.cn");
            req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            req.Headers.Add("openid", this.stu.openid);

            if (signs.Count == 0) return new KeyValuePair<bool, string>(false,"暂无签到");
            foreach (var sign in signs)
            {
                Console.WriteLine($"课堂：{sign.name}");
                if (sign.isQR)
                {
                    Console.WriteLine("不支持二维码签到");
                    return new KeyValuePair<bool, string>(false, "没法进行二维码签到");
                }

                body.Add("courseId", sign.courseId);
                body.Add("signId", sign.signId);
                req.Headers.Add("Referer", String.Format(SignConst.URL_CHECKINREFER_T, sign.courseId));
                req.Content = new FormUrlEncodedContent(body);
                var res = await client.SendAsync(req);
                var jsonstr = await res.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(SignSuccessEvent));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonstr));
                var signResult = (SignSuccessEvent)serializer.ReadObject(ms);
                if (signResult.signRank == null)
                {
                    if (jsonstr.Contains(SignConst.CODE_REPEATSIGNIN))
                    {
                        Console.WriteLine("您已经签到成功！");
                        return new KeyValuePair<bool, string>(true, "已经签过到了捏");
                    }
                    else
                    {
                        return new KeyValuePair<bool, string>(false, "未知签到异常");
                    }
                }
                Console.WriteLine(signResult);
                Console.WriteLine(await res.Content.ReadAsStringAsync());
                Console.WriteLine(res.StatusCode);
                return new KeyValuePair<bool, string>(true, "签到成功");
            }
            return new KeyValuePair<bool, string>(true, "签到成功");
        }
    }
}