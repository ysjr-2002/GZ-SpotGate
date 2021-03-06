﻿using GZ_SpotGate.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Tcp
{
    internal interface ITcpConnection
    {
        TcpClient Tcp { get; }

        bool Running { get; }

        void Start();

        void SetCallback(Action<DataEventArgs> act);

        void Stop();
    }

    internal interface IGateTcpConnection : ITcpConnection
    {
        /// <summary>
        /// 进向开闸
        /// </summary>
        /// <param name="count">值为1时，开闸保持</param>
        void EnterOpen(byte count);
        /// <summary>
        /// 出向开闸
        /// </summary>
        /// <param name="count"></param>
        void ExitOpen(byte count);
        /// <summary>
        /// 进向开闸保持
        /// </summary>
        void EnterHoldOpen();
        /// <summary>
        /// 出向开闸保持
        /// </summary>
        void ExitHoldOpen();
        /// <summary>
        /// 进向关闭
        /// </summary>
        void EnterClose();
        /// <summary>
        /// 出向关闭
        /// </summary>
        void ExitClose();
        /// <summary>
        /// 停止
        /// </summary>
        void StopAsync();
    }
}
