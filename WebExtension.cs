using Newtonsoft.Json;
using Plugable.io;
using Plugable.io.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using uhttpsharp;
using WebProt.WebHttp.Provider.Extensions;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebProt.Provider.Plugin.Ping
{
    public class WebExtension : WebSocketBehavior, IPlugable, IProtocolPlugin
    {
        #region Variables
        internal PluginsManager server;
        public const string WebProtNotifierProvider = "WebProt.WebSocket.Provider";
        #endregion

        public void Initialize(string[] args, dynamic parent)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
            this.server = parent;
        }

        public void Initialize(string[] args, PluginsManager parent, dynamic server)
        {
            if (server != null)
            {
                Type type = ((object)server).GetType();
                if (type != null)
                {
                    if (type.Name == "Router")
                    {

                    }
                    else
                    {
                        AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
                        server.AddWebSocketService<WebExtension>("/ping");
                    }
                }
            }
        }

        public Dictionary<string, Func<IHttpContext, Dictionary<string, string>, Func<Task>, Task>> GetRoutes()
        {
            var routes = new Dictionary<string, Func<IHttpContext, Dictionary<string, string>, Func<Task>, Task>>();

            routes.Add(@"/ping", (ctx, data, handler) =>
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
