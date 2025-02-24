﻿namespace Game.Base
{
    using log4net;
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Reflection;

    public class BaseServer
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly int SEND_BUFF_SIZE = 16384;
        protected readonly HybridDictionary _clients = new HybridDictionary();
        protected Socket _linstener;
        protected SocketAsyncEventArgs ac_event;
        public int ClientCount
        {
            get
            {
                return this._clients.Count;
            }
        }
        public BaseServer()
        {
            this.ac_event = new SocketAsyncEventArgs();
            this.ac_event.Completed += new EventHandler<SocketAsyncEventArgs>(this.AcceptAsyncCompleted);
        }
        private void AcceptAsync()
        {
            try
            {
                if (this._linstener != null)
                {
                    SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                    e.Completed += new EventHandler<SocketAsyncEventArgs>(this.AcceptAsyncCompleted);
                    this._linstener.AcceptAsync(e);
                }
            }
            catch (Exception ex)
            {
                BaseServer.log.Error("AcceptAsync is error!", ex);
            }
        }
        private void AcceptAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket sock = null;
            try
            {
                sock = e.AcceptSocket;
                sock.SendBufferSize = BaseServer.SEND_BUFF_SIZE;
                BaseClient client = this.GetNewClient();
                try
                {
                    if (BaseServer.log.IsInfoEnabled)
                    {
                        string ip = sock.Connected ? sock.RemoteEndPoint.ToString() : "socket disconnected";
                        BaseServer.log.Info("Incoming connection from " + ip);
                    }
                    object syncRoot;
                    Monitor.Enter(syncRoot = this._clients.SyncRoot);
                    try
                    {
                        this._clients.Add(client, client);
                        client.Disconnected += new ClientEventHandle(this.client_Disconnected);
                    }
                    finally
                    {
                        Monitor.Exit(syncRoot);
                    }
                    client.Connect(sock);
                    client.ReceiveAsync();
                }
                catch (Exception ex)
                {
                    BaseServer.log.ErrorFormat("create client failed:{0}", ex);
                    client.Disconnect();
                }
            }
            catch
            {
                if (sock != null)
                {
                    try
                    {
                        sock.Close();
                    }
                    catch
                    {
                    }
                }
            }
            finally
            {
                e.Dispose();
                this.AcceptAsync();
            }
        }
        private void client_Disconnected(BaseClient client)
        {
            client.Disconnected -= new ClientEventHandle(this.client_Disconnected);
            this.RemoveClient(client);
        }
        protected virtual BaseClient GetNewClient()
        {
            return new BaseClient(new byte[8192], new byte[8192]);
        }
        public virtual bool InitSocket(IPAddress ip, int port)
        {
            try
            {
                this._linstener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._linstener.Bind(new IPEndPoint(ip, port));
            }
            catch (Exception e)
            {
                BaseServer.log.Error("InitSocket", e);
                return false;
            }
            return true;
        }
        public virtual bool Start()
        {
            if (this._linstener == null)
            {
                return false;
            }
            try
            {
                this._linstener.Listen(100);
                this.AcceptAsync();
                if (BaseServer.log.IsDebugEnabled)
                {
                    BaseServer.log.Debug("Server is now listening to incoming connections!");
                }
            }
            catch (Exception e)
            {
                if (BaseServer.log.IsErrorEnabled)
                {
                    BaseServer.log.Error("Start", e);
                }
                if (this._linstener != null)
                {
                    this._linstener.Close();
                }
                return false;
            }
            return true;
        }
        public virtual void Stop()
        {
            BaseServer.log.Debug("Stopping server! - Entering method");
            try
            {
                if (this._linstener != null)
                {
                    Socket socket = this._linstener;
                    this._linstener = null;
                    socket.Close();
                    BaseServer.log.Debug("Server is no longer listening for incoming connections!");
                }
            }
            catch (Exception e)
            {
                BaseServer.log.Error("Stop", e);
            }
            if (this._clients != null)
            {
                object syncRoot;
                Monitor.Enter(syncRoot = this._clients.SyncRoot);
                try
                {
                    BaseClient[] list = new BaseClient[this._clients.Keys.Count];
                    this._clients.Keys.CopyTo(list, 0);
                    BaseClient[] array = list;
                    for (int i = 0; i < array.Length; i++)
                    {
                        BaseClient client = array[i];
                        client.Disconnect();
                    }
                    BaseServer.log.Debug("Stopping server! - Cleaning up client list!");
                }
                catch (Exception e2)
                {
                    BaseServer.log.Error("Stop", e2);
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
            }
            BaseServer.log.Debug("Stopping server! - End of method!");
        }
        public virtual void RemoveClient(BaseClient client)
        {
            object syncRoot;
            Monitor.Enter(syncRoot = this._clients.SyncRoot);
            try
            {
                this._clients.Remove(client);
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }
        public BaseClient[] GetAllClients()
        {
            object syncRoot;
            Monitor.Enter(syncRoot = this._clients.SyncRoot);
            BaseClient[] result;
            try
            {
                BaseClient[] temp = new BaseClient[this._clients.Count];
                this._clients.Keys.CopyTo(temp, 0);
                result = temp;
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
            return result;
        }
        public void Dispose()
        {
            this.ac_event.Dispose();
        }
    }
}

