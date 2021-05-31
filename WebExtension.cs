using Newtonsoft.Json;
using Plugable.io;
using Plugable.io.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using uhttpsharp.Helpers;
using uhttpsharp.Interfaces;
using WebProt.WebHttp.Provider.Extensions;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebProt.Provider.Plugin.Ping
{
    public class WebExtension : WebSocketBehavior, IPlugable, IProtocolPlugin, IRoutable
    {
        #region Variables
        internal PluginsManager server;
        public const string WebProtNotifierProvider = "WebProt.WebSocket.Provider";
        #endregion

        public void Initialize(string[] args, dynamic parent, Router router)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
            this.server = parent;
        }

        public void Initialize(string[] args, PluginsManager parent, dynamic server)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
            server.AddWebSocketService<WebExtension>("/ping");
        }

        public Dictionary<string, RouteAction> GetRoutes()
        {
            var routes = new Dictionary<string, RouteAction>();

            routes.Add(@"/ping", (router, ctx, data, handler) =>
            {
                return ctx.OutputUtf8(JsonConvert.SerializeObject(new
                {
                    Status = "Successful",
                    Data = DateTime.Now.ToString(),
                    Request = "/ping"
                }), "application/json");
            });


            return routes;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                Send(JsonConvert.SerializeObject(new
                {
                    Status = "Successful",
                    Data = DateTime.Now.ToString(),
                    Request = "/ping"
                }));
            }
            else if (e.IsBinary)
            {

            }
        }

        #region getVersion
        public string getVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }
        #endregion

        #region getName
        public string getName()
        {
            return GetType().Assembly.GetName().Name;
        }
        #endregion

        #region ResolveAssembly
        public Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var assembly = (args.Name.Contains(","))
                    ? args.Name.Substring(0, args.Name.IndexOf(','))
                    : args.Name;

            var directory = Path.Combine(Environment.CurrentDirectory, "extensions");
            var plugin = getName() + "_" + getVersion() + ".zip";

            return Path.Combine(directory, plugin).GetAssemblyFromPlugin(assembly);
        }
        #endregion
    }
}
