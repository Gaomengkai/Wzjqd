using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Wzjqd;
using System.Threading.Tasks;

namespace Wzjqd
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        Student stu = new Student("", "");
        Signiner sgn;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            //AppCompatEditText textOpenId = FindViewById<AppCompatEditText>(Resource.Id.openIdInput);
            //textOpenId.KeyPress += OpenidOnFinish;

            Button btnChk = FindViewById<Button>(Resource.Id.btnChkSign);
            btnChk.Click += OnBtnChkClicked;


            Button btnSign = FindViewById<Button>(Resource.Id.btnqd);
            btnSign.Click += OnBtnSignClicked;
            stu.name = "0";
            stu.openid = "";
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private async void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "正在给服务器发送一个请求", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
            AppCompatEditText textOpenId = FindViewById<AppCompatEditText>(Resource.Id.openIdInput);
            string after = Student.OpenidSizer(textOpenId.Text.ToString());
            stu.openid = after;
            sgn = new Signiner(stu);
            string name = await sgn.GetUserName();
            if (name == "")
            {
                Toast.MakeText(this, "设置失败！", ToastLength.Short).Show();
                return;
            }
            stu.name = name;
            string welcome = $"欢迎你，{name}. openid设置成功";
            Toast.MakeText(this, welcome, ToastLength.Short).Show();

            var textview = FindViewById<TextView>(Resource.Id.uname);
            textview.SetText(name, TextView.BufferType.Normal);

            var btnChk = FindViewById<Button>(Resource.Id.btnChkSign);
            btnChk.Enabled = true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async void OpenidOnFinish(object sender, View.KeyEventArgs e)
        {
            AppCompatEditText textOpenId = (AppCompatEditText)sender;
            e.Handled = false;
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                Toast.MakeText(this, "正在给服务器发送一个请求", ToastLength.Short).Show();
                string after = Student.OpenidSizer(textOpenId.Text.ToString());
                stu.openid = after;
                sgn = new Signiner(stu);
                string name = await sgn.GetUserName();
                if (name == "")
                {
                    Toast.MakeText(this, "设置失败！", ToastLength.Short).Show();
                    return;
                }
                stu.name = name;
                string welcome = $"欢迎你，{name}. openid设置成功";
                Toast.MakeText(this, welcome, ToastLength.Short).Show();

                var textview = FindViewById<TextView>(Resource.Id.uname);
                textview.SetText(name, TextView.BufferType.Normal);

                var btnChk = FindViewById<Button>(Resource.Id.btnChkSign);
                btnChk.Enabled = true;
                e.Handled = true;
            }
        }


        private async void OnBtnChkClicked(object sender, EventArgs e)
        {
            TextView qdlist = FindViewById<TextView>(Resource.Id.txtqd);
            qdlist.SetText("正在检测", TextView.BufferType.Normal);
            var lst = await sgn.GetSignList();
            if (lst.Count == 0)
            {
                qdlist.SetText("暂时没有签到捏！", TextView.BufferType.Normal);
                return;
            }
            foreach (var item in lst)
            {
                string fxxk = $"课堂：{item.name},定位：{item.isGPS}";
                qdlist.SetText(fxxk, TextView.BufferType.Normal);
            }
            var btnSign = FindViewById<Button>(Resource.Id.btnqd);
            btnSign.Enabled = true;
        }

        private async void OnBtnSignClicked(object o, EventArgs e)
        {
            var res = await sgn.Sign();
            TextView qdlist = FindViewById<TextView>(Resource.Id.txtqd);
            if (res.Key)
                qdlist.SetText($"签到成功！\n{res.Value}", TextView.BufferType.Normal);
            else
                qdlist.SetText($"签到失败！\n{res.Value}", TextView.BufferType.Normal);
        }
    }
}
