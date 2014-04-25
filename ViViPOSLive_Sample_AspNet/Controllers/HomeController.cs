using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViViPOSLive_Sample_AspNet.Messaging;
using ViViPOSLive_Sample_AspNet.Models;

namespace ViViPOSLive_Sample_AspNet.Controllers
{
    public class HomeController : Controller
    {

        AutoResetEvent waitHandle = new AutoResetEvent(false);
        public ActionResult Index()
        {
            Product[] products = null;
            var connection = new HubConnection(ConfigurationManager.AppSettings["viviposliveURL"],
                "app_id=" + ConfigurationManager.AppSettings["app_id"] +
                "&app_key=" + ConfigurationManager.AppSettings["app_key"] +
                "&cid=" + ConfigurationManager.AppSettings["cid"] +
                "&s=" + ConfigurationManager.AppSettings["s"]);

            var proxy = connection.CreateHubProxy("LiveHub");
            bool connected = false;
            connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                }
                else
                {
                    connected = true;
                    Console.WriteLine("Connected");
                }
            }).Wait();

            if (connected)
            {
                var request = new Request()
                {
                    meta = new Meta() { target = "aabbccddeeff" },
                    data = null
                };

                proxy.On<string>("onError", (errorMessage) =>
                {
                    ViewBag.ErrorMessage = errorMessage;
                    waitHandle.Set();
                }
                );

                proxy.On<Response>("onProductsReceived", (response) =>
                {
                    products = JsonConvert.DeserializeObject<Product[]>(response.data.ToString());

                    waitHandle.Set();
                });


                proxy.Invoke<string>("GetProducts", request).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("There was an error calling send: {0}", task.Exception.GetBaseException());
                    }
                    else
                    {
                        Console.WriteLine(task.Result);
                    }
                });


                waitHandle.WaitOne();

                return View(products);
            }
            else
            {
                ViewBag.ErrorMessage = "Error Connecting to ViViPOS Live";
                return View(products);
            }

        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "ViViPOS Australia";

            return View();
        }
    }
}