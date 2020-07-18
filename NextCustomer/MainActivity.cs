using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Button = Android.Widget.Button;
using ListView = Android.Widget.ListView;
using LahoreSocketAsync;
using Environment = System.Environment;

namespace NextCustomer
{
    public class Cust
    {
        public string Name { get; set; }
        public string phone { get; set; }
        public string payOption { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public LahoreSocketClient client = null;
        ObservableCollection<Cust> cust = new ObservableCollection<Cust>();
        public ObservableCollection<Cust> Customer { get { return cust; } }
        public EditText ip = null;
        public EditText nameC = null;
        public EditText phoneC = null;
        public static Button connectIt = null;
        public Button disconnectIt = null;
        public static Context self = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.alternative_main);

            Button sendIt = FindViewById<Button>(Resource.Id.button1);
            phoneC = FindViewById<EditText>(Resource.Id.editText1);
            nameC = FindViewById<EditText>(Resource.Id.editText2);
            ip = FindViewById<EditText>(Resource.Id.editText3);
            connectIt = FindViewById<Button>(Resource.Id.connect);
            disconnectIt = FindViewById<Button>(Resource.Id.disconnect);
            int counter = 0;
            ToggleButton buttonCompany = FindViewById<ToggleButton>(Resource.Id.toggleButton2);
            self = this;
            sendIt.Click += (sender, e) =>
            {
                string toadd = phoneC.Text + "|" + nameC.Text + "|"+ buttonCompany.Text;
                //Toast.MakeText(ApplicationContext, toadd, ToastLength.Long).Show();
                Toast.MakeText(this, toadd, ToastLength.Short).Show();
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Νέος πελάτης : " + counter);
                alert.SetMessage(toadd);
                Dialog dialog = alert.Create();
                dialog.Show();
                counter += 1;
                Console.WriteLine(toadd);
                client.SendToServer(toadd);
                phoneC.Text = "";
                nameC.Text = "";
                cust.Add(new Cust
                {
                    Name = nameC.Text,
                    phone = phoneC.Text,
                });
                //BindingContext = this;
            };
            connectIt.Click += connectToServerBtn;
            disconnectIt.Click += DisconnectBtn;
        }
        private static void HandleTextReceived(object sender, TextReceivedEventArgs trea)
        {
            string msg = string.Format(
                "{0} - Received: {1}{2}",
                DateTime.Now,
                trea.TextReceived,
                Environment.NewLine);
            //Console.WriteLine(msg);
            
            if (trea.TextReceived.Contains("PONG"))
            {
                Toast.MakeText(self, "still connected...", ToastLength.Short).Show();
            }
            else { Toast.MakeText(self, msg, ToastLength.Short).Show(); }
        }

        private static void HandleServerDisconnected(object sender, ConnectionDisconnectedEventArgs cdea)
        {
            string msg = string.Format(
                    "{0} - Disconnected from server: {1}{2}",
                    DateTime.Now,
                    cdea.DisconnectedPeer,
                    Environment.NewLine);
            //System.Console.ReadLine();
            //Environment.Exit(1);
            //Console.WriteLine(msg);
            Toast.MakeText(self, msg, ToastLength.Short).Show();
            connectIt.Text = "Connect";
        }

        private static void HandleServerConnected(object sender, ConnectionDisconnectedEventArgs cdea)
        {

            string msg = string.Format(
                "{0} - Connected to server: {1}{2}",
                DateTime.Now,
                cdea.DisconnectedPeer,
                Environment.NewLine);
            //Console.WriteLine(msg);
            Toast.MakeText(self, msg, ToastLength.Short).Show();
            connectIt.Text = "CONNECTED!!!";

        }
        private void connectToServerBtn(object sender, EventArgs e)
        {
            client = new LahoreSocketClient();
            client.RaiseTextReceivedEvent += HandleTextReceived;
            client.RaiseServerDisconnected += HandleServerDisconnected;
            client.RaiseServerConnected += HandleServerConnected;
            string strIPAddress = ip.Text;
            string strPortInput = "23000";

            if (!client.SetServerIPAddress(strIPAddress) ||
                !client.SetPortNumber(strPortInput))
            {
               /* MessageBox.Show(
                    string.Format(
                        "Wrong IP Address or port number supplied - {0} - {1} - Press a key to exit",
                        strIPAddress,
                        strPortInput));

                return;*/
            }

            client.ConnectToServer();
        }
        private void DisconnectBtn(object sender, EventArgs e)
        {
            client.CloseAndDisconnect();
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}