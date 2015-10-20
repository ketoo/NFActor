//-----------------------------------------------------------------------
// <copyright file="NFTest.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/NFActor>
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace NFrame
{
    
    public class NFTest
    {
        class TestHandler
        {
            public void Handler(NFIDENTID address, NFIDENTID from, NFIActorMessage xMessage)
            {
                Console.WriteLine("Sleep: 5000, ThreadID: " + Thread.CurrentThread.ManagedThreadId );

                Thread.Sleep(5000);

                Console.WriteLine("handler ThreadID: " +  Thread.CurrentThread.ManagedThreadId + " " +  xMessage.data);
            }
        }

        static void Main()
        {
            //Start();

            TestHandler xTestHandler = new TestHandler();

            Console.WriteLine("start run... ThreadID: " + Thread.CurrentThread.ManagedThreadId);

            NFIDENTID xID = NFCActorMng.Intance().CreateActor();
            NFCActorMng.Intance().RegisterHandler(xID, xTestHandler.Handler);

            NFIActorMessage xMsgData = new NFIActorMessage();
            xMsgData.data = "test1";
            //xMsgData.bAsync = false;

            NFCActorMng.Intance().SendMsg(xID, null, xMsgData);

            Console.WriteLine("start loop... ThreadID: " + Thread.CurrentThread.ManagedThreadId);

            while(true)
            {

                Thread.Sleep(1);

                NFCActorMng.Intance().Execute();
            }

           }
    }
}
