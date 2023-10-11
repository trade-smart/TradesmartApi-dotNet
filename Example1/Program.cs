using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NorenRestApiWrapper;

namespace NorenRestSample
{    
    public static class Program
    {
        #region dev  credentials
        //for UAT
        public const string endPoint = "https://kambala-uat.tradesmartonline.in/NorenWClientTP/";
        public const string wsendpoint = "wss://kambala-uat.tradesmartonline.in/NorenWSTP/";
        //for live
        //public const string endPoint = "https://kambala-uat.tradesmartonline.in/NorenWClientTP/";
        //public const string wsendpoint = "wss://kambala-uat.tradesmartonline.in/NorenWSTP/";
        public const string uid = "";
        public const string actid = "";
        public const string pwd = "";
        public const string factor2 = dob;
        public const string pan = "";
        public const string dob = "";
        public const string imei = "";
        public const string vc = "";
        public const string appkey = "";
        public const string newpwd = "";
        #endregion


        public static bool loggedin = false;

        
        public static void OnStreamConnect(NorenStreamMessage msg)
        {
            Program.loggedin = true;
            nApi.SubscribeOrders(Handlers.OnOrderUpdate, uid);
            //nApi.SubscribeToken("NSE", "22");
            nApi.SubscribeTokenDepth("NSE", "22");
            
        }
        public static NorenRestApi nApi = new NorenRestApi();
        
        static void Main(string[] args)
        {
            LoginMessage loginMessage = new LoginMessage();
            loginMessage.apkversion = "1.0.0";
            loginMessage.uid = uid;
            loginMessage.pwd = pwd;
            loginMessage.factor2 = factor2;
            loginMessage.imei = imei;
            loginMessage.vc = vc;
            loginMessage.source = "API";
            loginMessage.appkey = appkey;
            nApi.SendLogin(Handlers.OnAppLoginResponse, endPoint, loginMessage);

            nApi.SessionCloseCallback = Handlers.OnAppLogout;
            nApi.onStreamConnectCallback = Program.OnStreamConnect;

            while (Program.loggedin == false)
            {
                //dont do anything till we get a login response         
                Thread.Sleep(5);
            }          
            
            bool dontexit = true;
            while(dontexit)
            {                
                var input = Console.ReadLine();
                var opts = input.Split(' ');
                foreach (string opt in opts)
                {
                    switch (opt.ToUpper())
                    {
                        case "B":
                            ActionPlaceBuyorder();
                            break;
                        case "C":
                            // process argument...
                            ActionPlaceCOorder();
                            break;
                        case "D":
                            ActionGetOptionChain();
                            break;
                        case "G":
                            nApi.SendGetHoldings(Handlers.OnHoldingsResponse, actid, "C");
                            break;
                        case "H":
                            //check order
                            Console.WriteLine("Enter OrderNo:");
                            var orderno = Console.ReadLine();
                            nApi.SendGetOrderHistory(Handlers.OnOrderHistoryResponse, orderno);
                            break;

                        case "L":
                            nApi.SendGetLimits(Handlers.OnResponseNOP, actid);
                            break;
                        case "O":
                            nApi.SendGetOrderBook(Handlers.OnOrderBookResponse, "");
                            break;

                        case "R":
                            ActionPlaceBOorder();
                            break;
                        case "S":
                            string exch;
                            string token;
                            Console.WriteLine("Enter exch:");
                            exch = Console.ReadLine();
                            Console.WriteLine("Enter Token:");
                            token = Console.ReadLine();
                            nApi.SendGetSecurityInfo(Handlers.OnResponseNOP, exch, token);
                            break;
                        case "T":
                            nApi.SendGetTradeBook(Handlers.OnTradeBookResponse, actid);
                            break;
                        case "Q":
                            nApi.SendLogout(Handlers.OnAppLogout);
                            dontexit = false;
                            return;
                        case "V":
                            DateTime today = DateTime.Now.Date;
                            double start = ConvertToUnixTimestamp(today);

                            //start and end time are optional
                            //here we are getting one day's data
                            nApi.SendGetTPSeries(Handlers.OnResponseNOP, "NSE", "22", start.ToString(), null , "5" );
                            break;
                        case "W":
                            Console.WriteLine("Enter exch:");
                            exch = Console.ReadLine();
                            Console.WriteLine("Enter Token:");
                            token = Console.ReadLine();
                            nApi.SendSearchScrip(Handlers.OnResponseNOP, exch, token);
                            break;
                        case "Y":
                            Console.WriteLine("Enter exch:");
                            exch = Console.ReadLine();
                            Console.WriteLine("Enter Token:");
                            token = Console.ReadLine();
                            nApi.SendGetQuote(Handlers.OnResponseNOP, exch, token);
                            break;

                        case "WU":
                            nApi.UnSubscribeToken("NSE", "22");
                            break;
                        case "WL":
                            Quote quote = new Quote();
                            quote.exch = "NSE";
                            quote.token = "22";

                            List<Quote> l = new List<Quote>();
                            l.Add(quote);

                            nApi.UnSubscribe(l);
                            break;
                        case "ST":
                            NorenRestApi nApi_2 = new NorenRestApi();
                            nApi_2.SendGetHoldings(Handlers.OnHoldingsResponse, actid, "C");
                            nApi_2.SendGetQuote(Handlers.OnResponseNOP, "NSE", "22");
                            break;
                        default:
                            // do other stuff...
                            ActionOptions();
                            break;
                    }
                }
                

                //var kp = Console.ReadKey();
                //if (kp.Key == ConsoleKey.Q)
                //    dontexit = false;
                //Console.WriteLine("Press q to exit.");
            }            
        }
        

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        #region actions
        public static void ActionPlaceCOorder()
        {
            //sample cover order
            PlaceOrder order = new PlaceOrder();
            order.uid = uid;
            order.actid = actid;
            order.exch = "CDS";
            order.tsym = "USDINR27JAN21F";
            order.qty = "10";
            order.dscqty = "0";
            order.prc = "76.0025";
            order.blprc = "74.0025";
            order.prd = "H";
            order.trantype = "B";
            order.prctyp = "LMT";
            order.ret = "DAY";
            order.ordersource = "API";

            nApi.SendPlaceOrder(Handlers.OnResponseNOP, order);
        }

        public static void ActionPlaceBOorder()
        {
            //sample cover order
            PlaceOrder order = new PlaceOrder();
            order.uid = uid;
            order.actid = actid;
            order.exch = "NSE";
            order.tsym = "INFY-EQ";
            order.qty = "10";
            order.dscqty = "0";
            order.prc = "2800";
            order.blprc = "2780";
            order.bpprc = "2820";
            order.prd = "B";
            order.trantype = "B";
            order.prctyp = "LMT";
            order.ret = "DAY";
            order.ordersource = "API";

            nApi.SendPlaceOrder(Handlers.OnResponseNOP, order);
        }

        public static void ActionPlaceBuyorder()
        {
            //sample cover order
            PlaceOrder order = new PlaceOrder();
            order.uid = uid;
            order.actid = actid;
            order.exch = "NSE";
            order.tsym = "M&M-EQ";
            order.qty = "10";
            order.dscqty = "0";
            order.prc = "100.5";
            
            order.prd = "I";
            order.trantype = "B";
            order.prctyp = "LMT";
            order.ret = "DAY";
            order.ordersource = "API";

            nApi.SendPlaceOrder(Handlers.OnResponseNOP, order);
        }


        public static void ActionGetOptionChain()
        {
            string exch;
            string tsym;
            string strike;
            Console.WriteLine("Enter exch:");
            exch = Console.ReadLine();
            Console.WriteLine("Enter TradingSymbol:");
            tsym = Console.ReadLine();
            Console.WriteLine("Enter Strike:");
            strike = Console.ReadLine();

            nApi.SendGetOptionChain(Handlers.OnResponseNOP, exch, tsym, strike, 1);

        }

        public static void ActionOptions()
        {
            Console.WriteLine("Q: logout.");
            Console.WriteLine("O: get OrderBook");
            Console.WriteLine("T: get TradeBook");
            Console.WriteLine("B: place a buy order");
            Console.WriteLine("C: place a cover order");
            Console.WriteLine("R: place a bracket order");
            Console.WriteLine("Y: get quote");
            Console.WriteLine("S: get security info");
            Console.WriteLine("H: get order history");
            Console.WriteLine("G: get holdings");
            Console.WriteLine("L: get limits");
            Console.WriteLine("M: get singleorder margin");
            Console.WriteLine("W: search for scrips (min 3 chars)");
            Console.WriteLine("V: get intraday 1 min price data");
            Console.WriteLine("D: get Option Chain");
        }
        #endregion
    }

}
