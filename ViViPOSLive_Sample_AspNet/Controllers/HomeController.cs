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
        //Wait Handle
        AutoResetEvent waitHandle = new AutoResetEvent(false);

        public ActionResult Index()
        {
            Product[] products = null;

            var connection = new HubConnection(ConfigurationManager.AppSettings["viviposliveURL"],
                "app_id=" + ConfigurationManager.AppSettings["app_id"] +
                "&app_key=" + ConfigurationManager.AppSettings["app_key"] +
                "&cid=" + ConfigurationManager.AppSettings["cid"] +
                "&s=" + ConfigurationManager.AppSettings["s"]);

            var proxy = connection.CreateHubProxy("LiveHub");   //Create Proxy for ViViPOS Live Hub

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

                //Setup onError handler
                proxy.On<string>("onError", (errorMessage) =>
                {
                    ViewBag.ErrorMessage = errorMessage;    //Set error Message to display if there is an error
                    waitHandle.Set();
                });


                //Set up call back handler when product json is received from ViViPOS terminal
                proxy.On<Response>("onProductsReceived", (response) =>
                {
                    products = JsonConvert.DeserializeObject<Product[]>(response.data.ToString()); 
                    waitHandle.Set();
                });


                var request = new Request()
                {
                    //Replace Target with the MacAddress of the targetted ViViPOS 
                    meta = new Meta() { target = "aabbccddeeff" },
                    data = null
                };


                //Try to get products from ViViPOS terminal, pass in request object
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


                waitHandle.WaitOne();   //Wait until we receive messages from the ViViPOS

                return View(products);  //return results to the View
            }
            else
            {
                //If there is a connection error, display error
                ViewBag.ErrorMessage = "Error Connecting to ViViPOS Live";
                return View(products);
            }

        }

        /// <summary>
        /// Dummy About Page
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        /// <summary>
        /// Dummy Contact Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "ViViPOS Australia";
            return View();
        }
    }
}